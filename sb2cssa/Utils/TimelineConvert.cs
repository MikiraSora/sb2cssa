using ReOsuStoryboardPlayer.Core.Base;
using ReOsuStoryboardPlayer.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.Utils
{
    public static class TimelineConvert
    {
        private static Event[] SUPPORT_CONVERT_EVENTS = new[] { Event.Fade,Event.Color,Event.Move,Event.MoveX,Event.MoveY,Event.Rotate,Event.Scale,Event.VectorScale };

        public static IEnumerable<(int time, Command cmd)> ConvertToKeyFrame(CommandTimeline timeline)
        {
            //support events
            if (!SUPPORT_CONVERT_EVENTS.Contains(timeline.Event))
                yield break;

            if (timeline.Overlay)
                throw new Exception("OVERLAY!");

            for (int i = 0; i < timeline.Count; i++)
            {
                var cmd = timeline[i];
                var next_cmd = i < timeline.Count - 1 ? timeline[i + 1] : null;

                //return relative code
                yield return (cmd.StartTime, cmd);
                yield return (cmd.EndTime, cmd);

                if (next_cmd != null)
                    yield return (next_cmd.StartTime, cmd);
            }
        }
    }
}
