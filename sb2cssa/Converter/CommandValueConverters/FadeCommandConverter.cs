using ReOsuStoryboardPlayer.Core.Commands;
using sb2cssa.CSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa.Converter.CommandValueConverters
{
    public class FadeCommandConverter : CommandValueConverterBase<FadeCommand>
    {
        public override IEnumerable<Property> Convert(FadeCommand command,float time)
        {
            var v = command.CalculateValue(CalculateInterploter(command,time));

            yield return new Property("opacity", $"{v}");
        }
    }
}
