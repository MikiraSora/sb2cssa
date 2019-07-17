using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI;
using ReOsuStoryboardPlayer;
using ReOsuStoryboardPlayer.Core.Base;
using ReOsuStoryboardPlayer.Core.Parser.Collection;
using ReOsuStoryboardPlayer.Core.Parser.Reader;
using ReOsuStoryboardPlayer.Core.Parser.Stream;
using ReOsuStoryboardPlayer.Kernel;
using ReOsuStoryboardPlayer.Parser;
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
            var dir_path = "372552 yuiko - Azuma no Sora kara Hajimaru Sekai (Short)";

            BeatmapFolderInfo folder_info = BeatmapFolderInfo.Parse(dir_path,null);
            var sb_instance = StoryboardInstance.Load(folder_info);

            CSSInstance css = new CSSInstance();

            var objects = sb_instance.Updater.StoryboardObjectList;

            foreach (var sbo in objects)
            {
                sbo.CalculateAndApplyBaseFrameTime();
                var result = StoryboardConverter.ConvertStoryboardObject(sbo);

                css.FormatableCSSElements.AddRange(result.keyframes);
                css.FormatableCSSElements.Add(result.selector);

                SetupWidthHeightProperties(sbo, result.selector, dir_path);
            }

            var css_content = css.FormatAsCSSSupport(null);

            var css_save_path = Path.Combine(folder_info.folder_path, "result.css");
            File.WriteAllText(css_save_path, css_content);

            var html_content = GenerateHtml(css_save_path, css, objects);
            var html_save_path = Path.Combine(folder_info.folder_path, "result_html.html");
            File.WriteAllText(html_save_path, html_content);

            //Console.ReadLine();
        }

        private static string GenerateHtml(string css_save_path, CSSInstance css, IEnumerable<StoryboardObject> objects)
        {
            StringBuilder html_generator = new StringBuilder();

            html_generator.AppendLine("<!doctype html>");
            html_generator.AppendLine("<html>");

            #region head

            html_generator.AppendLine("<head>");
            html_generator.AppendLine("   <meta charset=\"utf-8\"/>");

            html_generator.AppendLine($"  <link rel=\"stylesheet\" type=\"text/css\" href=\"{Path.GetFileName(css_save_path)}\" />");
            html_generator.AppendLine("   <title>无标题文档</title>");

            html_generator.AppendLine("</head>");

            #endregion

            #region body

            html_generator.AppendLine("<body>");

            foreach (var item in css.FormatableCSSElements.OfType<Selector>())
            {
                html_generator.AppendLine($"    <div class=\"{item.Name.TrimStart('.')}\"></div>");
            }

            html_generator.AppendLine("</body>");

            #endregion

            html_generator.AppendLine("</html>");

            return html_generator.ToString();
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

            selector.Properties.Add(new Property("height", $"{height}px"));
            selector.Properties.Add(new Property("width", $"{width}px"));
        }
    }
}
