using System;
using System.Collections.Generic;
using System.Text;

namespace OneEchan.Shared.Common.Helper
{
    internal static class UpdateTimeHelper
    {
        public static string GetUpdate(TimeSpan? time)
        {
			if (time == null) return "";
			if (time.Value.Days != 0)
            {
				return $"{time.Value.Days} days ago";
            }
			if (time.Value.Hours != 0)
            {
				return $"{time.Value.Hours} hours ago";
            }
			if (time.Value.Minutes != 0)
            {
				return $"{time.Value.Minutes} minutes ago";
            }
            return "Just now";
        }
    }
}
