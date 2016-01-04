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

namespace AnimateRaw.Android
{
    [Activity(Label = "Animate Raw", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private ExRecyclerView _recyclerView;
        private SwipeRefreshLayout _refresher;
        private bool _isLoading;
        private int _page = 0;
        private bool _hasMore = true;
        private LinearLayoutManager _layoutManager;

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
            else
            {
                Title = "Animate Raw";
            }
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainPage);
            _recyclerView = FindViewById<ExRecyclerView>(Resource.Id.MainPageRecyclerView);
            _recyclerView.OnScroll += recyclerView_OnScroll;
            _layoutManager = new LinearLayoutManager(this);
            _recyclerView.ViewLayoutManager = _layoutManager;
            _refresher = FindViewById<ScrollChildSwipeRefreshLayout>(Resource.Id.MainPageRefresher);
            _refresher.SetColorSchemeResources(Resource.Color.MediumVioletRed);
            _refresher.Refresh += refresher_Refresh;
            _refresher.Post(() => _refresher.Refreshing = true);
            await Refresh();
        }

        private void recyclerView_OnScroll(object sender, OnScrollEventArgs e)
        {
            if (e.dy > 0)
            {
                var visibleItemCount = _layoutManager.ChildCount;
                var totalItemCount = _layoutManager.ItemCount;
                var pastVisiblesItems = _layoutManager.FindFirstVisibleItemPosition();

                if (!_isLoading)
                {
                    if ((visibleItemCount + pastVisiblesItems) >= totalItemCount - 3)
                    {
                        _isLoading = true;
                        LoadMore();
                    }
                }
            }
        }
       

        private async void LoadMore()
        {
            using (var client = new HttpClient())
            {
                var jsstr = await client.GetStringAsync($"http://tlaster.me/api/list?page={_page++}");
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
        

        private async void refresher_Refresh(object sender, EventArgs e)
        {
            await Refresh();
        }

        private async System.Threading.Tasks.Task Refresh()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    _page = 0;
                    var jsstr = await client.GetStringAsync($"http://tlaster.me/api/list?page={_page++}");
                    var obj = (JObject)JsonConvert.DeserializeObject(jsstr);
                    _hasMore = (bool)obj["HasMore"];
                    var list = (from item in (JArray)obj["List"]
                                select new AnimateListModel
                                {
                                    ID = item.Value<int>("ID"),
                                    Name = item.Value<string>("Name"),
                                    LastUpdateBeijing = DateTime.Parse(item.Value<string>("LastUpdate")),
                                }).ToList();
                    var ada = new MainListAdapter(this, list);
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

