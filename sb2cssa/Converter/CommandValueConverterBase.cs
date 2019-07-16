using ReOsuStoryboardPlayer.Core.Commands;
using sb2cssa.CSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.Converter
{
    public abstract class CommandValueConverterBase<T> : ICommandValueConvertable where T : Command
    {
        public abstract IEnumerable<Property> Convert(T command,float time);

        public IEnumerable<Property> Convert(Command command,float time) => Convert((T)command, time);

        public float CalculateInterploter(Command command,float time)
        {
            float t;

            if (time <= command.StartTime)
                t = 0;
            else if (time >= command.EndTime)
                t = 1;
            else
                t = ((time - command.StartTime) / (command.EndTime - command.StartTime));

            return t;
        }
    }
}
