using ReOsuStoryboardPlayer.Core.Commands;
using sb2cssa.CSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.Converter.CommandValueConverters
{
    public class ColorCommandConverter : CommandValueConverterBase<ColorCommand>
    {
        public override IEnumerable<Property> Convert(ColorCommand command,float time)
        {
            var v = command.CalculateValue(CalculateInterploter(command,time));

            yield return new Property("background-color", $"#{v.X.ToString("X2")}{v.Y.ToString("X2")}{v.Z.ToString("X2")}");
        }
    }
}
