using sb2cssa.CSS.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.CSS.Tools
{
    public static class Optimzer
    {
        public static void AnimationOptimze(ProgressiveKeyFrames key_frames,Selector ref_animation_selector)
        {
            if (key_frames.Timeline.Count==1)
            {
                var props = key_frames.Timeline.First();

            }
        }
    }
}
