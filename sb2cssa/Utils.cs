using ReOsuStoryboardPlayer.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb2cssa
{
    public static class Utils
    {
        public static string GetStoryboardIdentityName(StoryboardObject obj) => $"{obj.FileLine}_" + string.Join("", obj.ImageFilePath.ToLower().Replace('/', '_').Replace('\\', '_').Replace('.', '_').Where(x => (x >= 'a' && x <= 'z') || (x >= '0' && x <= '9') || x == '_'));
    }
}
