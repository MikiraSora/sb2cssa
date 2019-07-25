using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.CSS.Animation
{
    public class ProgressiveKeyFrames : KeyFrames
    {
        public List<ProgressiveFrame> Timeline { get; set; } = new List<ProgressiveFrame>();

        public ProgressiveKeyFrames(string name) => Name = name;

        public override string FormatAsCSSSupport(FormatSetting setting)
        {
            var format = $"@keyframes {Name} {{{Environment.NewLine}"+string.Join(Environment.NewLine, Timeline.Select(x => {
                var t = (Math.Min(1,Math.Max(0,x.NormalizeTime))) * 100;
                return $"{t}% {{ {string.Join(" ",x.ChangedProperties.Select(q=>q.FormatAsCSSSupport(setting)))} }}";
            }))+ $"{Environment.NewLine}}}";

            return format;
        }
    }
}
