using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.CSS
{
    public class CSSInstance : IFormatable
    {
        public List<IFormatable> FormatableCSSElements { get; } = new List<IFormatable>();

        public string FormatAsCSSSupport(FormatSetting setting)
        {
            return string.Join(Environment.NewLine, FormatableCSSElements.Select(x=>x.FormatAsCSSSupport(null)));
        }
    }
}
