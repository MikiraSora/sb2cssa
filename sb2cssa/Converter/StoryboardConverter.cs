using ReOsuStoryboardPlayer.Core.Base;
using ReOsuStoryboardPlayer.Core.Commands;
using sb2cssa.Converter.CommandValueConverters;
using sb2cssa.CSS;
using sb2cssa.CSS.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.Converter
{
    public static class StoryboardConverter
    {
        public static (KeyFrames frames,int start_time,int duration) ConverterTimelineToKeyFrames(CommandTimeline storyboard_timeline)
        {
            var command_value_converter = GetValueConverter(storyboard_timeline.Event);

            var keyframe_timeline=storyboard_timeline.Select(x => new[] {
                (x.StartTime,command_value_converter.Convert(x,x.StartTime)),
                (x.EndTime,command_value_converter.Convert(x,x.EndTime)),
            }).SelectMany(l=>l).Distinct().OrderBy(x=>x.Item1);

            int start_time = keyframe_timeline.Min(x=>x.Item1);
            int end_time = keyframe_timeline.Max(x => x.Item1);

            var duration = end_time - start_time;

            ProgressiveKeyFrames kf = new ProgressiveKeyFrames("");

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

        private static Dictionary<Event, ICommandValueConvertable> converters = new Dictionary<Event, ICommandValueConvertable>()
        {
            { Event.Move,new MoveCommandConverter() }
        };

        public static ICommandValueConvertable GetValueConverter(Event e)
        {
            if (converters.TryGetValue(e,out var converter))
                return converter;

            throw new Exception($"Converter {e} is not found.");
        }
    }
}
