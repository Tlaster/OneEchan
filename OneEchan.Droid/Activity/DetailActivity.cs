using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OneEchan.Droid.Adapter;
using System.Net;
using System;
using System.Threading.Tasks;
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using Android.Util;
using OneEchan.Shared;
using OneEchan.Droid.Common.Helpers;
using OneEchan.Shared.Common.Helper;

namespace OneEchan.Droid
{
    [Activity(Label = "Detail", Theme = "@style/AppTheme.NoActionBar")]
    public class DetailActivity : AppCompatActivity
    {
        private int _id;
        private ExRecyclerView _exRecyclerView;
        private string _name;
        private ScrollChildSwipeRefreshLayout _refresher;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _name = Intent.Extras.GetString("name");
            _id = Intent.Extras.GetInt("id");
            SetContentView(Resource.Layout.MainPage);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
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
                var item = await Core.Common.Api.Detail.GetDetail(_id, LanguageHelper.PrefLang);
                var ada = new DetailListAdapter(item.SetList.ToList());
                ada.ItemClick += Ada_ItemClick;
                _exRecyclerView.ViewAdapter = ada;
            }
            catch (Exception e) when (e is WebException || e is HttpRequestException)
            {
                Toast.MakeText(this, "Error,can not get the detail", ToastLength.Short).Show();
            }
            _refresher.Refreshing = false;
        }

        private void Ada_ItemClick(object sender, int e)
        {
            var item = (_exRecyclerView.ViewAdapter as DetailListAdapter).Items[e];
            try
            {
                Core.Common.Api.Detail.AddClick(_id, int.Parse(item.FileName), LanguageHelper.PrefLang);
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