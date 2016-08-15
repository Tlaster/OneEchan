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
#elif ANDROID
                var lang = Locale.Default.Language.ToLower();
#else 
                throw Exception("You should not call this");
#endif
                if (lang.Contains("en"))
                {
                    lang = "en-US";
                }
                else if (lang.Contains("zh"))
                {
                    lang = "zh-TW";
                }
                else if (lang.Contains("ru"))
                {
                    lang = "ru-RU";
                }
                else
                {
                    lang = "ja-JP";
                }
                return lang;
            }
        }
    }
}
