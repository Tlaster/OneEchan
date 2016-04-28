using System;
using System.Collections.Generic;
using System.Text;

namespace OneEchan.Shared.Common.Helper
{
    internal static class UpdateTimeHelper
    {
        public static string GetUpdate(TimeSpan time)
        {
            if (time.Days != 0)
            {
                return $"{time.Days} days ago";
            }
            if (time.Hours != 0)
            {
                return $"{time.Hours} hours ago";
            }
            if (time.Minutes != 0)
            {
                return $"{time.Minutes} minutes ago";
            }
            return "Just now";
        }
    }
}
