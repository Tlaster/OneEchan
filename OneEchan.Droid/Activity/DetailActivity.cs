using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneEchan.Droid.Adapter;
using OneEchan.Shared.Model;
using System.Net;
using System;
using System.Threading.Tasks;
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using Android.Util;
using OneEchan.Shared;
using OneEchan.Droid.Common.Helpers;

namespace OneEchan.Droid
{
    [Activity(Label = "Detail", Theme = "@style/AppTheme.NoActionBar")]
    public class DetailActivity : AppCompatActivity
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
            SetContentView(Resource.Layout.MainPage);
            var toolbar = FindViewById<global::Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            ((LinearLayout.LayoutParams)toolbar.LayoutParameters).SetMargins(0, StatusBarHelper.GetStatusBarHeight(this), 0, 0);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = _name;
            _exRecyclerView = FindViewById<ExRecyclerView>(Resource.Id.MainPageRecyclerView);
            var disp = WindowManager.DefaultDisplay;
            var met = new DisplayMetrics();
            disp.GetMetrics(met);
            var heightm = met.HeightPixels;
            var widthm = met.WidthPixels;
            _exRecyclerView.ViewLayoutManager = new GridLayoutManager(this, heightm > widthm ? 2 : 3);
            _refresher = FindViewById<ScrollChildSwipeRefreshLayout>(Resource.Id.MainPageRefresher);
            _refresher.SetColorSchemeResources(Resource.Color.MediumVioletRed);
            _refresher.Refresh += async delegate { await Refresh(); };
            _refresher.Post(() => _refresher.Refreshing = true);
            await Refresh();
        }
        
        private async Task Refresh()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var jsstr = await client.GetStringAsync($"http://oneechan.moe/api/detail?id={_id}&prefLang={LanguageHelper.PrefLang}");
                    var list = (from item in (JArray)((JObject)JsonConvert.DeserializeObject(jsstr))["SetList"]
                                select new AnimateSetModel
                                {
                                    ClickCount = item.Value<double>("ClickCount"),
                                    FileName = item.Value<string>("FileName"),
                                    FilePath = item.Value<string>("FilePath"),
                                    FileThumb = item.Value<string>("FileThumb"),
                                }).OrderBy(a => a.FileName).ToList();
                    var ada = new DetailListAdapter(list);
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

        private void Ada_ItemClick(object sender, int e)
        {
            var item = (_exRecyclerView.ViewAdapter as DetailListAdapter).Items[e];
            try
            {
                using (var client = new HttpClient())
                    client.GetStringAsync($"http://oneechan.moe/api/detail?id={_id}&filename={item.FileName}&prefLang={LanguageHelper.PrefLang}");
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