using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.CSS.Animation
{
    public class FromToKeyFrames : KeyFrames
    {
        private Selector _from = new Selector("from");

        private Selector _to = new Selector("to");

        public FromToKeyFrames(string name) => Name = name;

        public List<Property> From => _from.Properties;
        public List<Property> To => _to.Properties;

        public override string FormatAsCSSSupport(FormatSetting setting)
        {
            var n = Environment.NewLine;
            var format = $"@keyframes {Name} {{ {_from.FormatAsCSSSupport(setting)} {_to.FormatAsCSSSupport(setting)} }}";

            return format;
        }
    }
}
