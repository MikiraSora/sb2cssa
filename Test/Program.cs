using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReOsuStoryboardPlayer;
using ReOsuStoryboardPlayer.Core.Parser.Collection;
using ReOsuStoryboardPlayer.Core.Parser.Reader;
using ReOsuStoryboardPlayer.Core.Parser.Stream;
using sb2cssa;
using sb2cssa.Converter;
using sb2cssa.CSS;
using sb2cssa.CSS.Animation;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            OsuFileReader reader = new OsuFileReader("supersb.osb");

            Setting.EnableSplitMoveScaleCommand = false;

            VariableCollection collection = new VariableCollection(new VariableReader(reader).EnumValues());

            EventReader er = new EventReader(reader, collection);

            StoryboardReader r = new StoryboardReader(er);

            var sbo = r.EnumValues().FirstOrDefault();

            var timeline = sbo.CommandMap.Values.First();

            var result = StoryboardConverter.ConverterTimelineToKeyFrames(timeline);

            var css=result.frames.FormatAsCSSSupport(null);
        }
    }
}
