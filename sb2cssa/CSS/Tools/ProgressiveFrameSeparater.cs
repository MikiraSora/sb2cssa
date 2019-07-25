using sb2cssa.CSS.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.CSS.Tools
{
    public static class ProgressiveFrameSeparater
    {
        public static void Separate(ProgressiveKeyFrames key_frames)
        {
            float current_progress = -2857;
            int repeat = 0;

            foreach (var frame in key_frames.Timeline.OrderBy(x=>x.NormalizeTime))
            {
                if (frame.NormalizeTime==current_progress)
                {
                    repeat++;
                    frame.NormalizeTime = 0.00001f * repeat + current_progress;
                }
                else
                {
                    repeat = 0;
                    current_progress = frame.NormalizeTime;
                }
            }
        }
    }
}
