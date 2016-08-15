using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Core.Common.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime ToUtc(this DateTime? dateTime)
        {
            if (dateTime == null)
                return default(DateTime);
            return dateTime.Value.ToUniversalTime();
        }
        public static string DiffForHumans(this DateTime dateTime)
        {
            var time = DateTimeSpan.CompareDates(dateTime, DateTime.UtcNow);
            if (time.Years != 0)
                return $"{time.Years} years ago";
            if (time.Months != 0)
                return $"{time.Months} months ago";
            if (time.Days != 0)
                return $"{time.Days} days ago";
            if (time.Hours != 0)
                return $"{time.Hours} hours ago";
            if (time.Minutes != 0)
                return $"{time.Minutes} minutes ago";
            return "Just now";
        }
    }
}
