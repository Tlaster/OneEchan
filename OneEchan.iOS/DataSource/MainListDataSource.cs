using System;
using System.Collections.Generic;
using System.Text;
using Foundation;
using OneEchan.Core.Common.Api.Model;
using OneEchan.iOS.Common.Event;
using UIKit;

namespace OneEchan.iOS.DataSource
{
    public class MainListDataSource : UITableViewSource
    {
        private const string _mainListCellId = "MainListCell";
        public event EventHandler LoadMore;
        public event EventHandler<ItemClickEventArgs<AnimateListModel>> ItemClick;

        ViewController controller;

        public MainListDataSource(ViewController controller)
        {
            this.controller = controller;
        }

        public override nint RowsInSection(UITableView tableView, nint section) => controller.List.Count;

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(_mainListCellId);
            if (cell == null)
                cell = new UITableViewCell(UITableViewCellStyle.Subtitle, _mainListCellId);
            var row = indexPath.Row;
            cell.TextLabel.Text = controller.List[row].Name;
            cell.DetailTextLabel.Text = GetUpdate(controller.List[row].LastUpdateTime);
            cell.DetailTextLabel.TextColor = UIColor.Gray;
            return cell;
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            if (indexPath.Row == controller.List.Count - 1)
                LoadMore?.Invoke(this, new EventArgs());
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            ItemClick?.Invoke(this, new ItemClickEventArgs<AnimateListModel>(controller.List[indexPath.Row]));
        }

        private string GetUpdate(TimeSpan time)
        {
            if (time.Days != 0)
            {
                return $"{time.Days} days ago";
            }
            if (time.Hours != 0)
            {
                return $"{time.Hours} hours ago";
            }
            if (time.Minutes != 0)
            {
                return $"{time.Minutes} minutes ago";
            }
            return "Just now";
        }
    }
}
