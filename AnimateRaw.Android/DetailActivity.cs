using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AnimateRaw.Android.Adapter;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Net;
using AnimateRaw.Shared.Model;

namespace AnimateRaw.Android
{
    [Activity(Label = "Detail")]
    public class DetailActivity : ListActivity
    {
        private double _id;
        private string _name;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _name = Intent.Extras.GetString("name");
            _id = Intent.Extras.GetDouble("id");
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                ActionBar.SetIcon(new ColorDrawable(Color.Transparent));
                ActionBar.SetBackgroundDrawable(new ColorDrawable(Color.MediumVioletRed));
                ActionBar.SetDisplayShowTitleEnabled(true);
                ActionBar.Title = _name;
                Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Window.SetStatusBarColor(Color.MediumVioletRed);
                Window.SetNavigationBarColor(Color.MediumVioletRed);
            }
            else
            {
                Title = _name;
                
            }
            using (var client = new HttpClient())
            {
                var jsstr = await client.GetStringAsync($"http://tlaster.me/getanimate?id={_id}");
                var list = (from item in (JArray)((JObject)JsonConvert.DeserializeObject(jsstr))["SetList"]
                           select new AnimateSetModel
                           {
                               ClickCount = item.Value<double>("ClickCount"),
                               FileName = item.Value<string>("FileName"),
                               FilePath = item.Value<string>("FilePath"),
                           }).OrderBy(a => a.FileName).ToList();
                ListAdapter = new DetailListAdapter(this, list);
            }
        }
        protected override async void OnListItemClick(ListView l, View v, int position, long id)
        {
            base.OnListItemClick(l, v, position, id);
            var item = (l.Adapter as DetailListAdapter).Items[position];
            using (var client = new HttpClient())
                await client.GetStringAsync($"http://tlaster.me/getanimate?id={_id}&filename={item.FileName}");
            var intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(Uri.Parse(item.FilePath), "video/mp4");
            StartActivity(intent);
        }
    }
}