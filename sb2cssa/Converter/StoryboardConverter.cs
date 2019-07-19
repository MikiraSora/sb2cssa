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

namespace sb2cssa.Converter
{
    public static class StoryboardConverter
    {
        private static Property position_fix_prop = new Property("position", "fixed");

        public static (KeyFrames frames,int start_time,int duration) ConverterTimelineToKeyFrames(CommandTimeline storyboard_timeline,string name)
        {
            var command_value_converter = GetValueConverter(storyboard_timeline.Event);

            var keyframe_timeline=storyboard_timeline.Select(x => new[] {
                (/*x.RelativeLine,*/x.StartTime,command_value_converter.Convert(x,x.StartTime)),
                (/*x.RelativeLine,*/x.EndTime,command_value_converter.Convert(x,x.EndTime)),
            }).SelectMany(l=>l).Distinct().OrderBy(x=>x.Item1);

            int start_time = keyframe_timeline.Min(x=>x.Item1);
            int end_time = keyframe_timeline.Max(x => x.Item1);

            var duration = end_time - start_time;

            Log.User($"Build Frame {storyboard_timeline} -> {name} ");

            ProgressiveKeyFrames kf = new ProgressiveKeyFrames(name);

            foreach (var frame in keyframe_timeline)
            {
                var progress = CalculateInterploter(frame.Item1);

                kf.Timeline.Add((progress, frame.Item2.ToList()));
            }

            return (kf,start_time,duration);//todo

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

        public static (KeyFrames[] keyframes, Selector selector) ConvertStoryboardObject(StoryboardObject obj)
        {
            var obj_name = Utils.GetStoryboardIdentityName(obj);

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

            var animation_key_frames = obj.CommandMap.Values
                .Where(x => CanConvert(x))
                .Select(x =>(ConverterTimelineToKeyFrames(x, $"k{(CREATED_ID++).ToString()}"),x)).ToArray();

            var animation_prop = new Property("animation", string.Join(",", animation_key_frames.Select(x => BuildAnimationValues(x.Item1))));


            selector.Properties.Add(animation_prop);
            selector.Properties.Add(new Property("background-image", $"url(\"{System.Text.RegularExpressions.Regex.Escape(obj.ImageFilePath)}\")"));

            selector.Properties.Add(new Property("background-blend-mode", "multiply"));
            selector.Properties.Add(position_fix_prop);

            return (animation_key_frames.Select(x => x.Item1.frames).ToArray(),selector);
        }

        private static void SetupBaseTransform(Selector selector, StoryboardObject obj)
        {
            obj.ResetTransform();

            //position
            selector.Properties.Add(new Property("left", $"{obj.Postion.X}px"));
            selector.Properties.Add(new Property("top", $"{obj.Postion.Y}px"));

            //rotate
            selector.Properties.Add(new Property("transform", $"rotate({obj.Postion.Y}rad)"));

            //fade
            selector.Properties.Add(new Property("opacity", $"{obj.Color/255.0f:F2}"));

            //scale
            selector.Properties.Add(new Property("opacity", $"{obj.Color / 255.0f:F2}"));

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
