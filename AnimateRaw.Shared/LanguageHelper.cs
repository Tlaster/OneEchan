using System;
using System.Collections.Generic;
using System.Text;
#if WINDOWS_UWP
using Windows.Globalization;
using Windows.System.UserProfile;
#else
using Java.Util;
#endif

namespace AnimateRaw.Shared
{
    internal static class LanguageHelper
    {
        public static string PrefLang
        {
            get
            {
#if WINDOWS_UWP
                var lang = GlobalizationPreferences.Languages[0];

#else
                var lang = Locale.Default.Language.ToLower();
#endif
                if (lang.Contains("en"))
                {
                    lang = "en";
                }
                else if (lang.Contains("zh"))
                {
                    lang = "zh";
                }
                else if (lang.Contains("ru"))
                {
                    lang = "ru";
                }
                else
                {
                    lang = "jp";
                }
                return lang;
            }
        }
    }
}
