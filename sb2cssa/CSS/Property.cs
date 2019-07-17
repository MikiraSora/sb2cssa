using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.CSS
{
    public class Property : IFormatable
    {
        protected Property() { }
        public Property(string name) => Name = name;
        public Property(string name, string value)
        {
            Name = name;
            Value = new StringValue() { Value = value };
        }

        public string Name { get; set; }
        public Value Value { get; set; }

        public string FormatAsCSSSupport(FormatSetting setting)
        {
            //color:red; 
            //font-size:14px;
            var format = $"{Name}:{Value.FormatAsCSSSupport(setting)};";
            return format;
        }

        public static bool operator ==(Property a, Property b)
        {
            return a.Name == a.Name && a.Value == b.Value;
        }

        public static bool operator !=(Property a, Property b) => !(a == b);

        public override string ToString() => $"{Name}:{Value.ToString()}";
    }
}
