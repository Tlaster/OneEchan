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
using System.Net;
using System;
using System.Threading.Tasks;

namespace AnimateRaw.Android
{
    [Activity(Label = "Detail")]
    public class DetailActivity : Activity
    {
        private double _id;
        private ListView _listView;
        private string _name;
        private ScrollChildSwipeRefreshLayout _refresher;

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

            SetContentView(Resource.Layout.MainPage);
            _listView = FindViewById<ListView>(Resource.Id.MainPageListView);
            _listView.ItemClick += listView_ItemClick;
            _refresher = FindViewById<ScrollChildSwipeRefreshLayout>(Resource.Id.MainPageRefresher);
            _refresher.SetColorSchemeResources(Resource.Color.MediumVioletRed);
            _refresher.Refresh += refresher_Refresh;
            _refresher.Post(() => _refresher.Refreshing = true);
            await Refresh();
        }

        private async void refresher_Refresh(object sender, EventArgs e)
        {
            await Refresh();
        }

        private async void listView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = (_listView.Adapter as DetailListAdapter).Items[e.Position];
            try
            {
                using (var client = new HttpClient())
                    await client.GetStringAsync($"http://tlaster.me/getanimate?id={_id}&filename={item.FileName}");
            }
            catch
            {
            }
            var intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(global::Android.Net.Uri.Parse(item.FilePath), "video/mp4");
            StartActivity(intent);
        }

        private async Task Refresh()
        {
            try
            {
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
                    _listView.Adapter = new DetailListAdapter(this, list);
                    _refresher.Refreshing = false;
                }
            }
            catch (Exception e) when (e is WebException || e is HttpRequestException)
            {
                _refresher.Refreshing = false;
                Toast.MakeText(this, "Error,can not get the detail", ToastLength.Short).Show();
            }
        }

    }
}