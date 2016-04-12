using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AnimateRaw.Android.Common.Helpers
{
    internal static class StatusBarHelper
    {
        public static int GetStatusBarHeight(Context context)
        {
            int result = 0;
            int resourceId = context.Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
            {
                result = context.Resources.GetDimensionPixelSize(resourceId);
            }
            return result;
        }
    }
}