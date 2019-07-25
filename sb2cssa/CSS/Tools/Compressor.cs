using ReOsuStoryboardPlayer.Core.Utils;
using sb2cssa.CSS.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.CSS.Tools
{
    public static class Compressor
    {
        public class ProgressiveFrameValueComparer : IEqualityComparer<ProgressiveFrame>
        {
            public bool Equals(ProgressiveFrame x, ProgressiveFrame y)
            {
                if (x.NormalizeTime != y.NormalizeTime)
                    return false;

                 return x.ChangedProperties.All(z => y.ChangedProperties.Any(w => z == w));
            }

            public int GetHashCode(ProgressiveFrame obj)
            {
                //我写的是什么几把
                return obj.NormalizeTime.GetHashCode();
            }

            public static ProgressiveFrameValueComparer Default { get; } = new ProgressiveFrameValueComparer();
        }

        public static void Compress(ProgressiveKeyFrames key_frames)
        {
            key_frames.Timeline = key_frames.Timeline.Distinct(ProgressiveFrameValueComparer.Default).ToList();
        }
    }
}
