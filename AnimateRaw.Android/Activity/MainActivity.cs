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
using AnimateRaw.Shared.Model;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Support.V4.Widget;
using System.Net;
using System.Collections.ObjectModel;
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using AnimateRaw.Android.Activity;
using Java.Util;
using AnimateRaw.Shared;
using AnimateRaw.Android.Common.Helpers;

namespace AnimateRaw.Android
{
    [Activity(Label = "Animate Raw", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme.NoActionBar")]
    public class MainActivity : AppCompatActivity
    {
        private ExRecyclerView _recyclerView;
        private SwipeRefreshLayout _refresher;
        private bool _isLoading;
        private int _page = 0;
        private bool _hasMore = true;
        private LinearLayoutManager _layoutManager;
        private string _serverLink => $"http://oneechan.moe/api/list?page={_page++}&prefLang={LanguageHelper.PrefLang}";

        protected override async void OnCreate(Bundle bundle)
        {
#if !DEBUG
            Xamarin.Insights.Initialize(XamarinInsights.ApiKey, this);
#endif
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainPage);
            var toolbar = FindViewById<global::Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            ((LinearLayout.LayoutParams)toolbar.LayoutParameters).SetMargins(0, StatusBarHelper.GetStatusBarHeight(this), 0, 0);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Animate Raw";
            _recyclerView = FindViewById<ExRecyclerView>(Resource.Id.MainPageRecyclerView);
            _layoutManager = new LinearLayoutManager(this);
            _recyclerView.ViewLayoutManager = _layoutManager;
            _recyclerView.LoadMore += (sender, e) => LoadMore();
            _refresher = FindViewById<ScrollChildSwipeRefreshLayout>(Resource.Id.MainPageRefresher);
            _refresher.SetColorSchemeResources(Resource.Color.MediumVioletRed);
            _refresher.Refresh += async delegate { await Refresh(); };
            _refresher.Post(() => _refresher.Refreshing = true);
            await Refresh();
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.Menu_About:
                    StartActivity(new Intent(this, typeof(AboutActivity)));
                    break;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.home, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        private async void LoadMore()
        {
            if (_isLoading)
                return;
            _isLoading = true;
            using (var client = new HttpClient())
            {
                var jsstr = await client.GetStringAsync(_serverLink);
                var obj = (JObject)JsonConvert.DeserializeObject(jsstr);
                _hasMore = (bool)obj["HasMore"];
                var list = (from item in (JArray)obj["List"]
                            select new AnimateListModel
                            {
                                ID = item.Value<int>("ID"),
                                Name = item.Value<string>("Name"),
                                LastUpdateBeijing = DateTime.Parse(item.Value<string>("LastUpdate")),
                            }).ToList();
                (_recyclerView.ViewAdapter as MainListAdapter).Add(list);
                _isLoading = false;
            }
        }

        private async System.Threading.Tasks.Task Refresh()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    _page = 0;
                    var jsstr = await client.GetStringAsync(_serverLink);
                    var obj = (JObject)JsonConvert.DeserializeObject(jsstr);
                    _hasMore = (bool)obj["HasMore"];
                    var list = (from item in (JArray)obj["List"]
                                select new AnimateListModel
                                {
                                    ID = item.Value<int>("ID"),
                                    Name = item.Value<string>("Name"),
                                    LastUpdateBeijing = DateTime.Parse(item.Value<string>("LastUpdate")),
                                }).ToList();
                    var ada = new MainListAdapter(list);
                    ada.ItemClick += Ada_ItemClick;
                    _recyclerView.ViewAdapter = ada;
                    _refresher.Refreshing = false;
                }
            }
            catch (Exception e) when(e is WebException || e is HttpRequestException)
            {
                Toast.MakeText(this, "Error,can not get the list", ToastLength.Short).Show();
                _refresher.Refreshing = false;
            }
        }

        private void Ada_ItemClick(object sender, int e)
        {
            var item = (_recyclerView.ViewAdapter as MainListAdapter).Items[e];
            var intent = new Intent(this, typeof(DetailActivity));
            Bundle bundle = new Bundle();
            bundle.PutString("name", item.Name);
            bundle.PutDouble("id", item.ID);
            intent.PutExtras(bundle);
            StartActivity(intent);
        }
    }
}

