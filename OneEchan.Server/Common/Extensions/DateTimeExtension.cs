using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.Common.Extensions
{
    public static class DateTimeExtension
    {
        private const string Iso8601DateFormat = "yyyy-MM-dd'T'HH:mm:ss.fff'Z'";
        public static string FormatIso8601Date(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString(Iso8601DateFormat, System.Globalization.CultureInfo.CurrentCulture);
        }
    }
}
