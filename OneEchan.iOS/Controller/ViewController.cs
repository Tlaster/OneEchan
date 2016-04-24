using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using MonoTouch.Dialog;
using OneEchan.Core.Common.Api.Model;
using OneEchan.iOS.DataSource;
using OneEchan.Shared;
using UIKit;

namespace OneEchan.iOS
{
	public partial class ViewController : UITableViewController
    {
        private int _page = 0;
        private bool _hasMore = true;
        private bool _isLoading;

        public List<AnimateListModel> List { get; private set; }
        public ViewController (IntPtr handle) : base (handle)
        {
            var source = new MainListDataSource(this);
            source.LoadMore += (sender, e) => LoadMore();
            TableView.Source = source;
            List = new List<AnimateListModel>();
            RefreshControl = new UIRefreshControl();
            RefreshControl.ValueChanged += (sender, e) => Refresh();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
            NavigationController.NavigationBar.TintColor = UIColor.White;
            NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(199, 21, 133);
            InvokeOnMainThread(() => RefreshControl.BeginRefreshing());
            Refresh();
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        private async void Refresh()
        {
            if (_isLoading) return;
            _isLoading = true;
            _page = 0;
            var item = await Core.Common.Api.Home.GetList(_page++, LanguageHelper.PrefLang);
            if (!item.Success)
                return;
            List = item.List.ToList();
            _hasMore = item.HasMore;
            TableView.ReloadData();
            InvokeOnMainThread(() => RefreshControl.EndRefreshing());
            _isLoading = false;
        }

        private async void LoadMore()
        {
            if (!_hasMore || _isLoading) return;
            _isLoading = true;
            var item = await Core.Common.Api.Home.GetList(_page++, LanguageHelper.PrefLang);
            if (!item.Success) return;
            _hasMore = item.HasMore;
            List.AddRange(item.List);
            TableView.ReloadData();
            _isLoading = false;
        }   
    }
}

