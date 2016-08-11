using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using MediaPlayer;
using OneEchan.Core.Common.Api.Model;
using OneEchan.Shared;
using OneEchan.Shared.Common.Helper;
using UIKit;

namespace OneEchan.iOS.Controller
{
    public partial class DetailCollectionViewController : UICollectionViewController
    {
        public List<AnimateSetModel> List { get; private set; } = new List<AnimateSetModel>();
        public UIRefreshControl RefreshControl { get; } = new UIRefreshControl();
        private const string _detailCellId = "DetailCell";
        private bool _isLoading;

        public int ID { get; internal set; }

        public DetailCollectionViewController(IntPtr handle) : base(handle)
        {
        }

        private async void Refresh()
        {
            if (_isLoading) return;
            _isLoading = true;
            InvokeOnMainThread(() => RefreshControl.BeginRefreshing());
            var item = await Core.Common.Api.Detail.GetDetail(ID, LanguageHelper.PrefLang);
            List = item.SetList;
            CollectionView.ReloadData();
            InvokeOnMainThread(() => RefreshControl.EndRefreshing());
            _isLoading = false;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }
        public override nint GetItemsCount(UICollectionView collectionView, nint section) => List.Count;
        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (DetailCell)CollectionView.DequeueReusableCell(_detailCellId, indexPath);
            cell.SetImage(List[indexPath.Row].FileThumb,List[indexPath.Row].FileName);
            return cell;
        }
        MPMoviePlayerController moviePlayer;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            RefreshControl.ValueChanged += (sender, e) => Refresh();
            CollectionView.AddSubview(RefreshControl);
            var deletate = new CustomFlowLayoutDelegate();
            deletate.ItemClick += (sender,e) =>
             {
                 Core.Common.Api.Detail.GetVideo(ID, int.Parse(List[e.Index].FileName), LanguageHelper.PrefLang);
                 if (moviePlayer == null)
                 {
                     moviePlayer = new MPMoviePlayerController();
                     View.AddSubview(moviePlayer.View);
                     moviePlayer.ShouldAutoplay = true;
                 }
                 moviePlayer.ContentUrl = NSUrl.FromString(List[e.Index].FilePath);
                 moviePlayer.SetFullscreen(true, false);
                 moviePlayer.PrepareToPlay();
                 moviePlayer.Play();
             };
            CollectionView.Delegate = deletate;
            CollectionView.RegisterClassForCell(typeof(DetailCell), _detailCellId);
            CollectionView.ContentInset = new UIEdgeInsets(4, 4, 4, 4);
            Refresh();
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