using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using OneEchan.Core.Common.Api.Model;
using OneEchan.iOS.Controller;
using OneEchan.Shared;
using OneEchan.Shared.Common.Helper;
using UIKit;

namespace OneEchan.iOS
{
    public partial class ViewController : UITableViewController
    {
        private const string _mainListCellId = "MainListCell";
        private int _page = 0;
        private bool _hasMore = true;
        private bool _isLoading;

        public List<AnimateListModel> List { get; private set; } = new List<AnimateListModel>();
        public ViewController (IntPtr handle) : base (handle)
        {
            RefreshControl = new UIRefreshControl();
            RefreshControl.ValueChanged += (sender, e) => Refresh();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, (sender, e) => { });
            //NavigationItem.SetRightBarButtonItem(new UIBarButtonItem(new UIButton()),false);
            NavigationItem.BackBarButtonItem = new UIBarButtonItem(title: "", style: UIBarButtonItemStyle.Plain, target: null, action: null);
            NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
            NavigationController.NavigationBar.TintColor = UIColor.White;
            NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(199, 21, 133);
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
            InvokeOnMainThread(() => RefreshControl.BeginRefreshing());
            _page = 0;
            var item = await Core.Common.Api.Home.GetList(_page++, LanguageHelper.PrefLang);
            if (!item.Success)
            {
                InvokeOnMainThread(() => RefreshControl.EndRefreshing());
                _isLoading = false;
                return;
            }
            List = item.List.ToList();
            _hasMore = item.HasMore;
            TableView.ReloadData();
            InvokeOnMainThread(() => RefreshControl.EndRefreshing());
            _isLoading = false;
        }

        public override nint RowsInSection(UITableView tableView, nint section) => List.Count;

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(_mainListCellId);
            if (cell == null)
                cell = new UITableViewCell(UITableViewCellStyle.Subtitle, _mainListCellId);
            var row = indexPath.Row;
            cell.TextLabel.Text = List[row].Name;
            cell.DetailTextLabel.Text = UpdateTimeHelper.GetUpdate(List[row].LastUpdateTime);
            cell.DetailTextLabel.TextColor = UIColor.Gray;
            return cell;
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (indexPath.Row == List.Count - 1)
                LoadMore();
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var detail = this.Storyboard.InstantiateViewController(nameof(DetailCollectionViewController)) as DetailCollectionViewController;
            if (detail != null)
            {
                detail.Title = List[indexPath.Row].Name;
                detail.ID = List[indexPath.Row].ID;
                this.NavigationController.PushViewController(detail, true);
            }
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

