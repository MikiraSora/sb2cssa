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
using sb2cssa.CSS.Tools;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var dir_path = "ssss";

            BeatmapFolderInfo folder_info = BeatmapFolderInfo.Parse(dir_path,null);

            Setting.EnableSplitMoveScaleCommand = false;

            var sb_instance = StoryboardInstance.Load(folder_info);

            CSSInstance css = new CSSInstance();

            var objects = sb_instance.Updater.StoryboardObjectList;

            foreach (var sbo in objects)
            {
                sbo.CalculateAndApplyBaseFrameTime();

                var result = StoryboardConverter.ConvertStoryboardObject(sbo, dir_path);

                css.FormatableCSSElements.AddRange(result.keyframes);

                css.FormatableCSSElements.Add(result.selector);
            }

            //AppendVisualField(css);

            var css_content = css.FormatAsCSSSupport(null);

            var css_save_path = Path.Combine(folder_info.folder_path, "result.css");
            File.WriteAllText(css_save_path, css_content);

            var html_content = GenerateHtml(css_save_path, css, objects);
            var html_save_path = Path.Combine(folder_info.folder_path, "result_html.html");
            File.WriteAllText(html_save_path, html_content);

            //Console.ReadLine();
        }

        private static void AppendVisualField(CSSInstance css)
        {
            Selector bg = new Selector(".visual_filed");
            bg.Properties.Add(new Property("width", "640px"));
            bg.Properties.Add(new Property("height", "480px"));
            bg.Properties.Add(new Property("position", "fixed"));
            bg.Properties.Add(new Property("background-color", "#7B68EE20"));

            css.FormatableCSSElements.Add(bg);
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
    }
}
