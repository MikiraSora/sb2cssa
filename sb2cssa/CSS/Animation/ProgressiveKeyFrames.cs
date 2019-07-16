using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.CSS.Animation
{
    public class ProgressiveKeyFrames : KeyFrames
    {
        public List<(float normalize_time, List<Property> changed_prop_list)> Timeline { get; set; } = new List<(float normalize_time, List<Property> changed_prop_list)>();

        public ProgressiveKeyFrames(string name) => Name = name;

        public override string FormatAsCSSSupport(FormatSetting setting)
        {
            var format = $"@keyframes {Name} {{{Environment.NewLine}"+string.Join(Environment.NewLine, Timeline.Select(x => {
                var t = (Math.Min(1,Math.Max(0,x.normalize_time))) * 100;
                return $"{(int)t}% {{ {string.Join(" ",x.changed_prop_list.Select(q=>q.FormatAsCSSSupport(setting)))} }}";
            }))+ $"{Environment.NewLine}}}";

            return format;
        }
    }
}
