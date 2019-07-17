using ReOsuStoryboardPlayer.Core.Commands;
using sb2cssa.CSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.Converter.CommandValueConverters
{
    public class ScaleCommandConverter : CommandValueConverterBase<ScaleCommand>
    {
        public override IEnumerable<Property> Convert(ScaleCommand command, float time)
        {
            var v = command.CalculateValue(CalculateInterploter(command, time));

            yield return new Property("transform", $"scale({v})");
        }
    }
}
