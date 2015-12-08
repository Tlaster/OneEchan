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
    public class AnimateListModel
    {
        public double ID { get; internal set; }
        public TimeSpan LastUpdate { get; internal set; }
        public string Name { get; internal set; }
    }
}