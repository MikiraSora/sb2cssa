using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.CSS
{
    public abstract class Value : IFormatable
    {
        public abstract string FormatAsCSSSupport(FormatSetting setting);
    }

    public class StringValue : Value
    {
        public string Value { get; set; }

        public override string FormatAsCSSSupport(FormatSetting setting)
        {
            return Value;
        }
    }
}
