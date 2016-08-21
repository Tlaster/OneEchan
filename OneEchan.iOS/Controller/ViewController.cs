using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using OneEchan.Core.Api;
using OneEchan.Core.Models;
using OneEchan.iOS.Controller;
using OneEchan.Shared;
using OneEchan.Shared.Common.Helper;
using OneEchan.Core.Common.Extensions;
using UIKit;
using System.Threading.Tasks;

namespace OneEchan.iOS
{
    public partial class ViewController : UITableViewController
    {
        private const string _mainListCellId = "MainListCell";
        private int _page = 0;
        private bool _hasMore = true;
        private bool _isLoading;

        public List<ListResult> List { get; private set; } = new List<ListResult>();
        public ViewController (IntPtr handle) : base (handle)
        {
            RefreshControl = new UIRefreshControl();
            RefreshControl.ValueChanged += (sender, e) => Task.Run(async () => await Refresh());
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			NavigationItem.RightBarButtonItem = new UIBarButtonItem (UIImage.FromBundle ("ic_info_white"), UIBarButtonItemStyle.Bordered, ((sender, e) => {
				var about = this.Storyboard.InstantiateViewController(nameof(AboutViewController)) as AboutViewController;
				if (about != null)
				{
					this.NavigationController.PushViewController(about, true);
				}
			}));
            NavigationItem.BackBarButtonItem = new UIBarButtonItem(title: "", style: UIBarButtonItemStyle.Plain, target: null, action: null);
            NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
            NavigationController.NavigationBar.TintColor = UIColor.White;
            NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(199, 21, 133);
            Task.Run(async () => await Refresh());
        }

        private async Task Refresh()
        {
            if (_isLoading) return;
            _isLoading = true;
            InvokeOnMainThread(() => RefreshControl.BeginRefreshing());
            _page = 0;
            var item = await Home.GetList(_page++, LanguageHelper.PrefLang);
            List = item.List.ToList();
            _hasMore = _page < item.MaxPage;
            InvokeOnMainThread(() => { TableView.ReloadData(); RefreshControl.EndRefreshing(); });
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
            cell.DetailTextLabel.Text = List[row].Updated_At?.DiffForHumans();
            cell.DetailTextLabel.TextColor = UIColor.Gray;
            return cell;
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (indexPath.Row == List.Count - 1)
                Task.Run(async() => await LoadMore());
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
        
        private async Task LoadMore()
        {
            if (!_hasMore || _isLoading) return;
            _isLoading = true;
            var item = await Home.GetList(_page++, LanguageHelper.PrefLang);
            _hasMore = _page < item.MaxPage;
            List.AddRange(item.List);
            InvokeOnMainThread(() => TableView.ReloadData());
            _isLoading = false;
        }   
    }
}

