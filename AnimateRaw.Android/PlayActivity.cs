
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using Android.Net;

namespace AnimateRaw.Android
{
    [Activity(Label = "Play")]
    public class PlayActivity : Activity
    {
        private VideoView player;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            var filePath = Intent.Extras.GetString("filePath");
            RequestWindowFeature(WindowFeatures.NoTitle);
            RequestedOrientation = ScreenOrientation.Landscape;
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            Window.AddFlags(WindowManagerFlags.Fullscreen);
            Window.DecorView.SystemUiVisibility = StatusBarVisibility.Hidden;
            SetContentView(Resource.Layout.PlayPage);
            player = FindViewById<VideoView>(Resource.Id.VideoPlayer);
            player.SetMediaController(new MediaController(this));
            player.SetVideoURI(Uri.Parse(filePath));
            player.RequestFocus();
            player.Start();
        }
    }
}