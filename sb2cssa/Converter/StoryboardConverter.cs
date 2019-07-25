using ReOsuStoryboardPlayer.Core.Base;
using ReOsuStoryboardPlayer.Core.Commands;
using sb2cssa.Converter.CommandValueConverters;
using ReOsuStoryboardPlayer.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sb2cssa.CSS;
using sb2cssa.CSS.Animation;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using sb2cssa.Utils;
using sb2cssa.CSS.Tools;

namespace sb2cssa.Converter
{
    public static class StoryboardConverter
    {
        private static Property position_fix_prop = new Property("position", "fixed");

        public static (KeyFrames frames, int start_time, int duration) ConverterTimelineToKeyFrames(CommandTimeline storyboard_timeline, string name)
        {
            var command_value_converter = GetValueConverter(storyboard_timeline.Event);

            var extra_imm_timeline = new List<(long,int, IEnumerable<Property>)>();

            var keyframe_timeline = TimelineConvert.ConvertToKeyFrame(storyboard_timeline).Select(x=>(x.time,command_value_converter.Convert(x.cmd,x.time)));

            int start_time = keyframe_timeline.Min(x => x.Item1);
            int end_time = keyframe_timeline.Max(x => x.Item1);

            var duration = end_time - start_time;

            Log.User($"Build Frame {storyboard_timeline} -> {name} ");

            ProgressiveKeyFrames kf = new ProgressiveKeyFrames(name);

            foreach (var frame in keyframe_timeline)
            {
                var progress = CalculateInterploter(frame.Item1);

                kf.Timeline.Add(new ProgressiveFrame() { NormalizeTime = progress, ChangedProperties = frame.Item2.ToList() });
            }

            Compressor.Compress(kf);
            ProgressiveFrameSeparater.Separate(kf);

            return (kf, start_time, duration);//todo

            float CalculateInterploter(float time)
            {
                float t;

                if (time <= start_time)
                    t = 0;
                else if (time >= end_time)
                    t = 1;
                else
                    t = ((time - start_time) / (end_time - start_time));

                return t;
            }
        }

        public static bool CanConvert(CommandTimeline commands) => converters.ContainsKey(commands.Event);

        private static ulong CREATED_ID=0;

        public static string GetStoryboardIdentityName(StoryboardObject obj) => $"_{obj.FileLine}_" + string.Join("", obj.ImageFilePath.ToLower().Replace('/', '_').Replace('\\', '_').Replace('.', '_').Where(x => (x >= 'a' && x <= 'z') || (x >= '0' && x <= '9') || x == '_'));

        public static (KeyFrames[] keyframes, Selector selector) ConvertStoryboardObject(StoryboardObject obj,string dir_path)
        {
            var obj_name = GetStoryboardIdentityName(obj);

            Selector selector = new Selector($".{obj_name}");

            //temp solve

            obj.AddCommand(new FadeCommand() {
                EndTime=obj.FrameStartTime,
                StartTime=0,

                StartValue=0,
                EndValue=0
            });
            obj.AddCommand(new FadeCommand()
            {
                EndTime = obj.FrameEndTime+1,
                StartTime = obj.FrameEndTime,

                StartValue = 0,
                EndValue = 0
            });

            SetupBaseTransform(selector, obj);

            var size=SetupWidthHeightProperties(obj, selector, dir_path);

            var animation_key_frames = obj.CommandMap.Values
                .Where(x => CanConvert(x))
                .Select(x =>(ConverterTimelineToKeyFrames(x, $"k{(CREATED_ID++).ToString()}"),x)).ToArray();

            var animation_prop = new Property("animation", string.Join(",", animation_key_frames.Select(x => BuildAnimationValues(x.Item1))));

            selector.Properties.Add(animation_prop);
            selector.Properties.Add(new Property("background-image", $"url(\"{System.Text.RegularExpressions.Regex.Escape(obj.ImageFilePath)}\")"));

            selector.Properties.Add(new Property("background-blend-mode", "multiply"));
            selector.Properties.Add(position_fix_prop);

            var key_frames = animation_key_frames.Select(x => x.Item1.frames).ToArray();

            ApplyOriginOffset(obj, key_frames,size,selector);

            return (key_frames, selector);
        }

        private static Property ApplyOriginOffset(StoryboardObject obj, KeyFrames[] key_frames,(int width,int height) size, Selector selector)
        {
            var anchor = AnchorConvert.Convert(obj.OriginOffset)??Anchor.Centre;
            var origin = TransformOriginConvert.Convert(anchor);

            int offset_width = (int)(origin.w_offset * size.width) - 107;
            int offset_height = (int)(origin.h_offset * size.height);

            #region Apply Offset for translate()

            foreach (var left in key_frames
                .OfType<ProgressiveKeyFrames>()
                .SelectMany(l=>l.Timeline)
                .Select(l=>l.ChangedProperties)
                .SelectMany(l=>l).Concat(selector.Properties)
                .Where(l=>l.Name.StartsWith("left")))
            {
                left.Value =new StringValue($"{(double.Parse((left.Value as StringValue).Value.Trim('p','x')) - offset_width)}px");
            }

            foreach (var top in key_frames
                .OfType<ProgressiveKeyFrames>()
                .SelectMany(l => l.Timeline)
                .Select(l => l.ChangedProperties)
                .SelectMany(l => l).Concat(selector.Properties)
                .Where(l => l.Name.StartsWith("top")))
            {
                top.Value = new StringValue($"{(double.Parse((top.Value as StringValue).Value.Trim('p', 'x')) - offset_height)}px");
            }

            #endregion

            return new Property("transform-origin", $"{(int)(origin.w_offset*100)}% {(int)(origin.h_offset * 100)}%");
        }

        private static Dictionary<string, (int width, int height)> cache_pic_width = new Dictionary<string, (int width, int height)>();

        private static (int width,int height) SetupWidthHeightProperties(StoryboardObject obj, Selector selector, string dir_path)
        {
            int width = 0, height = 0;

            var pic_file = Path.Combine(dir_path, obj.ImageFilePath);

            if (cache_pic_width.TryGetValue(pic_file, out var p))
            {
                width = p.width;
                height = p.height;
            }
            else
            {
                using (Image<Rgba32> f = Image.Load<Rgba32>(pic_file))
                {
                    height = f.Height;
                    width = f.Width;

                    cache_pic_width[obj.ImageFilePath] = (width, height);
                }
            }

            selector.Properties.Add(new Property("height", $"{height}px"));
            selector.Properties.Add(new Property("width", $"{width}px"));

            return (width, height);
        }

        private static void SetupBaseTransform(Selector selector, StoryboardObject obj)
        {
            obj.ResetTransform();

            //position
            selector.Properties.Add(new Property("left", $"{obj.Postion.X}px"));
            selector.Properties.Add(new Property("top", $"{obj.Postion.Y}px"));

            //rotate & scale
            selector.Properties.Add(new Property("transform", $"rotate({obj.Postion.Y}rad) sacleX({obj.Scale.X}) sacleY({obj.Scale.Y})"));

            //fade
            selector.Properties.Add(new Property("opacity", $"{obj.Color.Z /255.0f:F2}"));

            //color
            //todo : color
        }

        private static string BuildAnimationValues((KeyFrames frames, int start_time, int duration) info)
        {
            Console.WriteLine($"Link Frame {info.frames.Name}");

            return $"{info.frames.Name} {info.duration}ms linear {info.start_time}ms forwards";
        }

        private static Dictionary<Event, ICommandValueConvertable> converters = new Dictionary<Event, ICommandValueConvertable>()
        {
            { Event.Move,new MoveCommandConverter() },
            { Event.MoveX,new MoveXCommandConverter() },
            { Event.MoveY,new MoveYCommandConverter() },
            { Event.Scale,new ScaleCommandConverter() },
            { Event.VectorScale,new VectorScaleCommandConverter() },
            { Event.Fade,new FadeCommandConverter() },
            //{ Event.Color,new ColorCommandConverter() }, 有丶问题，找不到css对应屙屎颜色混合的实现，先咕着
            { Event.Rotate,new RotateCommandConverter() },
        };

        public static ICommandValueConvertable GetValueConverter(Event e)
        {
            if (converters.TryGetValue(e,out var converter))
                return converter;

            throw new Exception($"Converter {e} is not found.");
        }
    }
}
