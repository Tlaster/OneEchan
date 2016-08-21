using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OneEchan.Core.Models;
using OneEchan.Server.Models;

namespace OneEchan.Server.Common.Helpers
{
    internal class LanguageHelper
    {
        public static string GetLanguegeName(string language, AnimateList item)
        {
            if (item == null)
                return null;
            language = language.ToLower();
            if (language.Contains("zh"))
                return item.ZhTw ?? item.JaJp ?? item.EnUs;
            else if (language.Contains("ru"))
                    return item.RuRu ?? item.JaJp ?? item.EnUs;
            else if (language.Contains("jp"))
                return item.JaJp ?? item.EnUs;
            return item.EnUs;
        }

    }
}
