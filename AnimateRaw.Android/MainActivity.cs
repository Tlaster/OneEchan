using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using AnimateRaw.Android.Adapter;
using System.Net.Http;
using System.Linq;
using AnimateRaw.Android.Model;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace AnimateRaw.Android
{
    [Activity(Label = "Animate Raw", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : ListActivity
    {
        protected override async void OnCreate(Bundle bundle)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                ActionBar.SetIcon(new ColorDrawable(Color.Transparent));
                ActionBar.SetBackgroundDrawable(new ColorDrawable(Color.MediumVioletRed));
                ActionBar.SetDisplayShowTitleEnabled(true);
                ActionBar.Title = "Animate Raw";
                Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Window.SetStatusBarColor(Color.MediumVioletRed);
                Window.SetNavigationBarColor(Color.MediumVioletRed);
            }

            base.OnCreate(bundle);
            using (var client = new HttpClient())
            {
                var jsstr = await client.GetStringAsync("http://tlaster.me/getanimate");

                var list = (from item in (JArray)JsonConvert.DeserializeObject(jsstr)
                            select new AnimateListModel
                            {
                                ID = item.Value<int>("ID"),
                                Name = item.Value<string>("Name"),
                                LastUpdate = DateTime.Now - DateTime.Parse(item.Value<string>("LastUpdate")),
                            }).OrderBy(a => a.LastUpdate).ToList();
                ListAdapter = new MainListAdapter(this, list);
            }
            
        }
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            base.OnListItemClick(l, v, position, id);
            var item = (l.Adapter as MainListAdapter).Items[position];
            var intent = new Intent(this, typeof(DetailActivity));
            Bundle bundle = new Bundle();
            bundle.PutString("name", item.Name);
            bundle.PutDouble("id", item.ID);
            intent.PutExtras(bundle);
            StartActivity(intent);
        }
    }
}

