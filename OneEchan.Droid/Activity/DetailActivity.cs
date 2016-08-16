using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
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
using System.Net.Http;
using OneEchan.Core.Api;
using Android.Views;
using OneEchan.Core.Models;
using Android.Support.Design.Widget;

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
            SetSupportActionBar(FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar));
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
                var item = await Detail.GetDetail(_id, LanguageHelper.PrefLang);
                var ada = new DetailListAdapter(item.List);
                ada.ItemClick += Ada_ItemClick;
                _exRecyclerView.ViewAdapter = ada;
            }
            catch (Exception e) when (e is WebException || e is HttpRequestException)
            {
                Toast.MakeText(this, "Error,can not get the detail", ToastLength.Short).Show();
            }
            _refresher.Refreshing = false;
        }

        private async void Ada_ItemClick(object sender, int e)
        {
            var item = await Detail.GetVideo(_id, (_exRecyclerView.ViewAdapter as DetailListAdapter).Items[e].Set, LanguageHelper.PrefLang);

            var dlg = new BottomSheetDialog(this);
            var layout = new LinearLayout(this)
            {
                Orientation = Orientation.Vertical
            };
            layout.AddView(GetHeaderText("Choose Quality"));
            if (item.OriginalQuality != null)
                layout.AddView(CreateRow("Source", item.OriginalQuality));
            if (item.HighQuality != null)
                layout.AddView(CreateRow("720P", item.HighQuality));
            if (item.MediumQuality != null)
                layout.AddView(CreateRow("480P", item.MediumQuality));
            if (item.LowQuality != null)
                layout.AddView(CreateRow("240P", item.LowQuality));
            dlg.SetContentView(layout);
            dlg.SetCancelable(false);
            dlg.Show();
        }

        private View CreateRow(string action, string path)
        {
            var row = new LinearLayout(this)
            {
                Clickable = true,
                Orientation = Orientation.Horizontal,
                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, this.DpToPixels(48))
            };
            row.AddView(this.GetText(action));
            row.Click += (sender, args) =>
            {
                var intent = new Intent(Intent.ActionView);
                intent.SetDataAndType(Android.Net.Uri.Parse(path), "video/mp4");
                StartActivity(intent);
            };
            return row;
        }
        private TextView GetHeaderText(string text)
        {
            var layout = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, this.DpToPixels(56))
            {
                LeftMargin = this.DpToPixels(16)
            };
            var txt = new TextView(this)
            {
                Text = text,
                LayoutParameters = layout,
                Gravity = GravityFlags.CenterVertical
            };
            txt.SetTextSize(ComplexUnitType.Sp, 16);
            return txt;
        }


        protected virtual TextView GetText(string text)
        {
            var layout = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent)
            {
                TopMargin = this.DpToPixels(8),
                BottomMargin = this.DpToPixels(8),
                LeftMargin = this.DpToPixels(16)
            };

            var txt = new TextView(this)
            {
                Text = text,
                LayoutParameters = layout,
                Gravity = GravityFlags.CenterVertical
            };
            txt.SetTextSize(ComplexUnitType.Sp, 16);
            txt.SetTextColor(Android.Graphics.Color.Black);
            return txt;
        }

        private int DpToPixels(int dp)
        {
            var value = TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, this.Resources.DisplayMetrics);
            return Convert.ToInt32(value);
        }
    }
}