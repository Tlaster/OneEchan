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

namespace AnimateRaw.Android
{
    [Activity(Label = "Animate Raw", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private ListView _listView;
        private SwipeRefreshLayout _refresher;

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
            _listView = FindViewById<ListView>(Resource.Id.MainPageListView);
            _listView.ItemClick += listView_ItemClick;
            _refresher = FindViewById<ScrollChildSwipeRefreshLayout>(Resource.Id.MainPageRefresher);
            _refresher.SetColorSchemeResources(Resource.Color.MediumVioletRed);
            _refresher.Refresh += refresher_Refresh;
            _refresher.Post(() => _refresher.Refreshing = true);
            await Refresh();
        }

        private void listView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = (_listView.Adapter as MainListAdapter).Items[e.Position];
            var intent = new Intent(this, typeof(DetailActivity));
            Bundle bundle = new Bundle();
            bundle.PutString("name", item.Name);
            bundle.PutDouble("id", item.ID);
            intent.PutExtras(bundle);
            StartActivity(intent);
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
                    var jsstr = await client.GetStringAsync("http://tlaster.me/getanimate");

                    var list = (from item in (JArray)JsonConvert.DeserializeObject(jsstr)
                                select new AnimateListModel
                                {
                                    ID = item.Value<int>("ID"),
                                    Name = item.Value<string>("Name"),
                                    LastUpdate = DateTime.Now - DateTime.Parse(item.Value<string>("LastUpdate")),
                                }).OrderBy(a => a.LastUpdate).ToList();
                    _listView.Adapter = new MainListAdapter(this, list);
                    _refresher.Refreshing = false;
                }
            }
            catch (Exception e) when(e is WebException || e is HttpRequestException)
            {
                Toast.MakeText(this, "Error,can not get the list", ToastLength.Short).Show();
                _refresher.Refreshing = false;
            }
        }
    }
}

