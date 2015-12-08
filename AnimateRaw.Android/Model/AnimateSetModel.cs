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

namespace AnimateRaw.Android.Model
{
    public class AnimateSetModel
    {
        public double ClickCount { get; internal set; }
        public string FileName { get; internal set; }
        public string FilePath { get; internal set; }
    }
}