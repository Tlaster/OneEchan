using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using MediaPlayer;
using OneEchan.Core.Api;
using OneEchan.Core.Models;
using OneEchan.Shared;
using OneEchan.Shared.Common.Helper;
using UIKit;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.iOS.Controller
{
    public partial class DetailCollectionViewController : UICollectionViewController
    {
        public List<DetailList> List { get; private set; } = new List<DetailList>();
        public UIRefreshControl RefreshControl { get; } = new UIRefreshControl();
        private const string _detailCellId = "DetailCell";
        private bool _isLoading;

        public int ID { get; internal set; }

        public DetailCollectionViewController(IntPtr handle) : base(handle)
        {
        }

        private async Task Refresh()
        {
            if (_isLoading) return;
            _isLoading = true;
            InvokeOnMainThread(() => RefreshControl.BeginRefreshing());
            var item = await Detail.GetDetail(ID, LanguageHelper.PrefLang);
            List = item.List;
            InvokeOnMainThread(() => { CollectionView.ReloadData(); RefreshControl.EndRefreshing(); });
            _isLoading = false;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section) => List.Count;
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (DetailCell)CollectionView.DequeueReusableCell(_detailCellId, indexPath);
            cell.SetImage(List[indexPath.Row].FileThumb,List[indexPath.Row].Set.ToString());
            return cell;
        }
        MPMoviePlayerController moviePlayer;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            RefreshControl.ValueChanged += (sender, e) => Task.Run(async () => await Refresh());
            CollectionView.AddSubview(RefreshControl);
            var deletate = new CustomFlowLayoutDelegate();
            deletate.ItemClick += async (sender, e) =>
             {
                 var item = await Detail.GetVideo(ID, List[e.Index].Set, LanguageHelper.PrefLang);
                 var picker = new UIPickerView();
                 var model = new QualityPickerViewModel(item);
                 model.ItemClick += (s2, e2) =>
                 {
                     View.WillRemoveSubview(picker);
                     if (moviePlayer == null)
                     {
                         moviePlayer = new MPMoviePlayerController();
                         View.AddSubview(moviePlayer.View);
                         moviePlayer.ShouldAutoplay = true;
                     }
                     moviePlayer.ContentUrl = NSUrl.FromString(item.ToDictionary().Values.ToArray()[e2.Index]);
                     moviePlayer.SetFullscreen(true, false);
                     moviePlayer.PrepareToPlay();
                     moviePlayer.Play();
                 };
                 View.AddSubview(picker);
                 picker.Hidden = false;
             };
            CollectionView.Delegate = deletate;
            CollectionView.RegisterClassForCell(typeof(DetailCell), _detailCellId);
            CollectionView.ContentInset = new UIEdgeInsets(4, 4, 4, 4);
            Task.Run(async () => await Refresh());
        }
    }

    public class QualityPickerViewModel : UIPickerViewModel
    {
        public event EventHandler<ItemClickEventArgs> ItemClick;

        private Dictionary<string, string> _item;

        public QualityPickerViewModel(SetResult item)
        {
            _item = item.ToDictionary();
        }

        public override nint GetComponentCount(UIPickerView picker) => 1;

        public override nint GetRowsInComponent(UIPickerView picker, nint component) => _item.Count;

        public override string GetTitle(UIPickerView picker, nint row, nint component) => _item.Keys.ToArray()[row];
        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            ItemClick?.Invoke(this, new ItemClickEventArgs(Convert.ToInt32(row)));
        }
    }


    public class ItemClickEventArgs : EventArgs
    {
        public int Index { get; }
        public ItemClickEventArgs(int index)
        {
            Index = index;
        }
    }
    public class CustomFlowLayoutDelegate : UICollectionViewDelegateFlowLayout
    {
        public event EventHandler<ItemClickEventArgs> ItemClick;
        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            ItemClick?.Invoke(this, new ItemClickEventArgs(indexPath.Row));
        }
        public override CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            var colCount = 2f;
            if (UIDevice.CurrentDevice.Model.ToLower().Contains("ipad")) colCount = 3f;
            var width = UIScreen.MainScreen.Bounds.Width / colCount - 8;
            return new CGSize(width, width / 16f * 9f);
        }
        public override nfloat GetMinimumInteritemSpacingForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section) 
            => 4f;
        public override nfloat GetMinimumLineSpacingForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section) 
            => 4f;
    }
    public class DetailCell : UICollectionViewCell
    {
        private UIImageView imageView;
        private UILabel titleView;

        [Export("initWithFrame:")]
        public DetailCell(CGRect frame) : base(frame)
        {
            BackgroundColor = UIColor.FromRGB(199, 21, 133);
        }
        public void SetImage(string uri,string title)
        {
            var colCount = 2f;
            if (UIDevice.CurrentDevice.Model.ToLower().Contains("ipad")) colCount = 3f;
            var width = UIScreen.MainScreen.Bounds.Width / colCount - 8;
            var height = width / 16f * 9f;
            var img = ScaledToSize(FromUrl(uri), new CGSize(width, height));
            if (imageView == null)
            {
                imageView = new UIImageView(img);
                ContentView.AddSubview(imageView);
                titleView = new UILabel
                {
                    Text = title,
                    TextColor = UIColor.White,
                    BackgroundColor = UIColor.FromRGBA(0, 0, 0, 100),
                    TextAlignment = UITextAlignment.Right
                };
                titleView.Frame = new CGRect(0d, height - 20d, width, 20d);
                imageView.AddSubview(titleView);
            }
            else
            {
                imageView.Image = img;
                titleView.Text = title;
            }
        }

        private UIImage ScaledToSize(UIImage img, CGSize size)
        {
            if (img.Size == size)
            {
                return img;
            }
            UIGraphics.BeginImageContextWithOptions(size, false, 0f);
            img.Draw(new CGRect(0f, 0f, size.Width, size.Height));
            var simg = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return simg;
        }
        private UIImage FromUrl(string uri)
        {
            using (var url = new NSUrl(uri))
            using (var data = NSData.FromUrl(url))
                return UIImage.LoadFromData(data);
        }

    }
}