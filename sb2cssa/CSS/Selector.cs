using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.CSS
{
    public class Selector : IFormatable
    {
        public Selector(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public List<Property> Properties { get; set; } = new List<Property>();

        public string FormatAsCSSSupport(FormatSetting setting)
        {
            var format = $"{Name} {{ {string.Join(Environment.NewLine,Properties.Select(x=>x.FormatAsCSSSupport(setting)))} }}";
            return format;
        }
    }
}
