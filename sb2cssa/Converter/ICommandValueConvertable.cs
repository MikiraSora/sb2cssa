using ReOsuStoryboardPlayer.Core.Commands;
using sb2cssa.CSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.Converter
{
    public interface ICommandValueConvertable
    {
        IEnumerable<Property> Convert(Command command,float time);
    }
}
