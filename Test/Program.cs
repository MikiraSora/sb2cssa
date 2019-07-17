using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReOsuStoryboardPlayer;
using ReOsuStoryboardPlayer.Core.Base;
using ReOsuStoryboardPlayer.Core.Parser.Collection;
using ReOsuStoryboardPlayer.Core.Parser.Reader;
using ReOsuStoryboardPlayer.Core.Parser.Stream;
using sb2cssa;
using sb2cssa.Converter;
using sb2cssa.CSS;
using sb2cssa.CSS.Animation;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var osb_path = @"supersb.osb";
            var dir_path = Path.GetDirectoryName(Path.GetFullPath(osb_path));

            OsuFileReader reader = new OsuFileReader(osb_path);

            Setting.EnableSplitMoveScaleCommand = false;

            VariableCollection collection = new VariableCollection(new VariableReader(reader).EnumValues());

            EventReader er = new EventReader(reader, collection);

            StoryboardReader r = new StoryboardReader(er);

            CSSInstance css = new CSSInstance();

            foreach (var sbo in r.EnumValues().OfType<StoryboardObject>())
            {
                var result = StoryboardConverter.ConvertStoryboardObject(sbo);

                css.FormatableCSSElements.AddRange(result.keyframes);
                css.FormatableCSSElements.Add(result.selector);

                SetupWidthHeightProperties(sbo, result.selector, dir_path);
            }

            var css_content = css.FormatAsCSSSupport(null);

            File.WriteAllText(@"result.css", css_content);

            Console.ReadLine();
        }

        private static Dictionary<string, (int width, int height)> cache_pic_width = new Dictionary<string, (int width, int height)>();

        private static void SetupWidthHeightProperties(StoryboardObject obj, Selector selector, string dir_path)
        {
            int width = 0, height = 0;

            var pic_file = Path.Combine(dir_path, obj.ImageFilePath);

            if (cache_pic_width.TryGetValue(pic_file, out var p))
            {
                width = p.width;
                height = p.height;
            }
            else
            {
                using (Image<Rgba32> f = Image.Load<Rgba32>(pic_file))
                {
                    height = f.Height;
                    width = f.Width;

                    cache_pic_width[obj.ImageFilePath] = (width, height);
                }
            }

            selector.Properties.Add(new Property("height", $"{width}px"));
            selector.Properties.Add(new Property("width", $"{height}px"));
        }
    }
}
