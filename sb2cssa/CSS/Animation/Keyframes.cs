using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.CSS.Animation
{
    public abstract class KeyFrames:IFormatable
    {
        public string Name { get; set; }

        public abstract string FormatAsCSSSupport(FormatSetting setting);
    }
}
