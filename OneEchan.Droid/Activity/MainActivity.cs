using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using OneEchan.Droid.Adapter;
using System.Linq;
using Android.Support.V4.Widget;
using System.Net;
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using OneEchan.Droid.Activity;
using OneEchan.Shared;
using OneEchan.Droid.Common.Helpers;
using System.Threading.Tasks;
using OneEchan.Shared.Common.Helper;
using System.Net.Http;

namespace OneEchan.Droid
{
    [Activity(Label = "OneEchan", MainLauncher = true, Icon = "@mipmap/logo", Theme = "@style/AppTheme.NoActionBar")]
    public class MainActivity : AppCompatActivity
    {
        private ExRecyclerView _recyclerView;
        private SwipeRefreshLayout _refresher;
        private bool _isLoading;
        private int _page = 0;
        private bool _hasMore = true;
        private LinearLayoutManager _layoutManager;

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainPage);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            ((LinearLayout.LayoutParams)toolbar.LayoutParameters).SetMargins(0, StatusBarHelper.GetStatusBarHeight(this), 0, 0);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "OneEchan";
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
            if (_isLoading || !_hasMore)
                return;
            _isLoading = true;
            try
            {
                var item = await Core.Common.Api.Home.GetList(_page++, LanguageHelper.PrefLang);
                if (!item.Success)
                    return;
                _hasMore = item.HasMore;
                (_recyclerView.ViewAdapter as MainListAdapter).Add(item.List.ToList());
            }
            catch (Exception e) when (e is WebException || e is HttpRequestException)
            {

            }
            _isLoading = false;
        }

        private async Task Refresh()
        {
            try
            {
                _page = 0;
                var item = await Core.Common.Api.Home.GetList(_page++, LanguageHelper.PrefLang);
                if (!item.Success)
                    return;
                _hasMore = item.HasMore;
                var ada = new MainListAdapter(item.List.ToList());
                ada.ItemClick += Ada_ItemClick;
                _recyclerView.ViewAdapter = ada;
            }
            catch (Exception e) when(e is WebException || e is HttpRequestException)
            {
                Toast.MakeText(this, "Error,can not get the list", ToastLength.Short).Show();
            }
            _refresher.Refreshing = false;
        }

        private void Ada_ItemClick(object sender, int e)
        {
            var item = (_recyclerView.ViewAdapter as MainListAdapter).Items[e];
            var intent = new Intent(this, typeof(DetailActivity));
            Bundle bundle = new Bundle();
            bundle.PutString("name", item.Name);
            bundle.PutInt("id", item.ID);
            intent.PutExtras(bundle);
            StartActivity(intent);
        }
    }
}

