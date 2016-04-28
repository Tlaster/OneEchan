using System;
using System.Collections.Generic;
using System.Text;
#if WINDOWS_UWP
using Windows.Globalization;
using Windows.System.UserProfile;
#elif __IOS__
using Foundation;
#else
using Java.Util;
#endif

namespace OneEchan.Shared.Common.Helper
{
    internal static class LanguageHelper
    {
        public static string PrefLang
        {
            get
            {
#if WINDOWS_UWP
                var lang = GlobalizationPreferences.Languages[0].ToLower();
#elif __IOS__
                var lang = NSLocale.PreferredLanguages[0].ToLower();
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
