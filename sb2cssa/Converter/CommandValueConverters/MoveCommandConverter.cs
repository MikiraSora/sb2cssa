using ReOsuStoryboardPlayer.Core.Commands;
using sb2cssa.CSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.Converter.CommandValueConverters
{
    public class MoveCommandConverter : CommandValueConverterBase<MoveCommand>
    {
        public override IEnumerable<Property> Convert(MoveCommand command,float time)
        {
            var v = command.CalculateValue(CalculateInterploter(command,time));

            yield return new Property("transform", $"translateX({v.X}px)");
            yield return new Property("transform", $"translateY({v.Y}px)");
        }
    }
}
