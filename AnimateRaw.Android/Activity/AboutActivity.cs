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
using Android.Support.V7.App;
using AnimateRaw.Android.Common.Helpers;

namespace AnimateRaw.Android.Activity
{
    [Activity(Label = "About", Theme = "@style/AppTheme.NoActionBar")]
    public class AboutActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AboutPage);
            var toolbar = FindViewById<global::Android.Support.V7.Widget.Toolbar>(Resource.Id.AboutToolbar);
            ((LinearLayout.LayoutParams)toolbar.LayoutParameters).SetMargins(0, StatusBarHelper.GetStatusBarHeight(this), 0, 0);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "About";
        }
        [Java.Interop.Export("Github_Click")]
        public void Github_Click(View view)
        {
            var intent = new Intent(Intent.ActionView);
            intent.SetData(global::Android.Net.Uri.Parse("https://github.com/Tlaster/AnimateRaw"));
            StartActivity(intent);
        }
    }
}