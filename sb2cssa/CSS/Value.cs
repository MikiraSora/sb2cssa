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

        public abstract bool ValueCompare(Value other);

        public static bool operator ==(Value a, Value b) => a.ValueCompare(b);

        public static bool operator !=(Value a, Value b) => !(a == b);

        public abstract override string ToString();
    }

    public class StringValue : Value
    {
        public StringValue()
        {

        }

        public StringValue(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public override string FormatAsCSSSupport(FormatSetting setting)
        {
            return Value;
        }

        public override bool ValueCompare(Value other) => other is StringValue o && o.Value == Value;

        public override string ToString() => Value;
    }
}
