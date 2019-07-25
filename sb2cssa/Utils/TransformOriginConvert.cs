using ReOsuStoryboardPlayer.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.Utils
{
    public static class TransformOriginConvert 
    {
        private readonly static Dictionary<Anchor, (float w_offset,float h_offset)> AnchorVectorMap = new Dictionary<Anchor, (float w_offset, float h_offset)>()
        {
            {Anchor.TopLeft,(0f,0f)},
            {Anchor.TopCentre,(0.5f, 0.0f)},
            {Anchor.TopRight,(1f, 0f)},
            {Anchor.CentreLeft,(0f, 0.5f)},
            {Anchor.Centre,(0.5f, 0.5f)},
            {Anchor.CentreRight,(1f, 0.5f)},
            {Anchor.BottomLeft,(0f, 1f)},
            {Anchor.BottomCentre,(0.5f, 1f)},
            {Anchor.BottomRight,(1f, 1f)}
        };

        public static Anchor? Convert((float w_offset, float h_offset) offset)
        {
            return AnchorVectorMap.Where(x => x.Value == offset).Select(x => x.Key).FirstOrDefault();
        }

        public static (float w_offset, float h_offset) Convert(Anchor anchor)
        {
            return AnchorVectorMap.TryGetValue(anchor, out var hv) ? hv : Convert(Anchor.Centre);
        }
    }
}
