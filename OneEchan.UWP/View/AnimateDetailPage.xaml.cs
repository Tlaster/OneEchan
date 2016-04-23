using OneEchan.Shared.Model;
using OneEchan.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace OneEchan
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AnimateDetailPage : Page
    {
        private AnimateSetModel _clickedItem;

        public AnimateDetailViewModel DetailVM { get; private set; }
        public AnimateDetailPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            DetailVM = e.Parameter as AnimateDetailViewModel;
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as AnimateSetModel;
            DetailVM.Click(item.FileName);
            Frame.Navigate(typeof(AnimatePlayPage), item.FilePath);
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_clickedItem.FilePath))
            {
                DataPackage data = new DataPackage();
                data.SetText(_clickedItem.FilePath);
                Clipboard.SetContent(data);
            }
        }
        
        public void RightClick(object sender, RoutedEventArgs e)
        {
            Point point;
            if (e is RightTappedRoutedEventArgs)
            {
                point = (e as RightTappedRoutedEventArgs).GetPosition(null);
                (e as RightTappedRoutedEventArgs).Handled = true;
            }
            else if (e is HoldingRoutedEventArgs)
            {
                point = (e as HoldingRoutedEventArgs).GetPosition(null);
                (e as HoldingRoutedEventArgs).Handled = true;
            }
            _clickedItem = (e.OriginalSource as FrameworkElement).DataContext as AnimateSetModel;
            if (_clickedItem != null)
            {
                var menu = Resources["RightClickMenu"] as MenuFlyout;
                menu.ShowAt(null, point);
            }
        }

        private async void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_clickedItem.FilePath))
            {
                await Launcher.LaunchUriAsync(new Uri(_clickedItem.FilePath), new LauncherOptions { ContentType = "video/mp4" });
            }
        }
    }
}
