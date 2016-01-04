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
using Android.Support.V7.Widget;

namespace AnimateRaw.Android
{
    [Activity(Label = "Detail")]
    public class DetailActivity : Activity
    {
        private double _id;
        private ExRecyclerView _exRecyclerView;
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
            _exRecyclerView = FindViewById<ExRecyclerView>(Resource.Id.MainPageRecyclerView);
            _exRecyclerView.ViewLayoutManager = new GridLayoutManager(this, 3);
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

        private async Task Refresh()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var jsstr = await client.GetStringAsync($"http://tlaster.me/api/detail?id={_id}");
                    var list = (from item in (JArray)((JObject)JsonConvert.DeserializeObject(jsstr))["SetList"]
                                select new AnimateSetModel
                                {
                                    ClickCount = item.Value<double>("ClickCount"),
                                    FileName = item.Value<string>("FileName"),
                                    FilePath = item.Value<string>("FilePath"),
                                    FileThumb = item.Value<string>("FileThumb"),
                                }).OrderBy(a => a.FileName).ToList();
                    var ada = new DetailListAdapter(this, list);
                    ada.ItemClick += Ada_ItemClick;
                    _exRecyclerView.ViewAdapter = ada;
                    _refresher.Refreshing = false;
                }
            }
            catch (Exception e) when (e is WebException || e is HttpRequestException)
            {
                _refresher.Refreshing = false;
                Toast.MakeText(this, "Error,can not get the detail", ToastLength.Short).Show();
            }
        }

        private async void Ada_ItemClick(object sender, int e)
        {
            var item = (_exRecyclerView.ViewAdapter as DetailListAdapter).Items[e];
            try
            {
                using (var client = new HttpClient())
                    await client.GetStringAsync($"http://tlaster.me/api/detail?id={_id}&filename={item.FileName}");
            }
            catch
            {
            }
            var intent = new Intent(Intent.ActionView);
            intent.SetDataAndType(global::Android.Net.Uri.Parse(item.FilePath), "video/mp4");
            StartActivity(intent);
        }
    }
}