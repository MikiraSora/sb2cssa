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
        public class ProgressiveFrameValueComparer : IEqualityComparer<(float time, List<Property> props)>
        {
            public bool Equals((float time, List<Property> props) x, (float time, List<Property> props) y)
            {
                if (x.time != y.time)
                    return false;

                 return x.props.All(z => y.props.Any(w => z == w));
            }

            public int GetHashCode((float time, List<Property> props) obj)
            {
                //我写的是什么几把
                return (int)(obj.time*1000);
            }

            public static ProgressiveFrameValueComparer Default { get; } = new ProgressiveFrameValueComparer();
        }

        public static ProgressiveKeyFrames Compress(ProgressiveKeyFrames key_frames)
        {
            key_frames.Timeline = key_frames.Timeline.Distinct(ProgressiveFrameValueComparer.Default).ToList();

            return key_frames;
        }
    }
}
