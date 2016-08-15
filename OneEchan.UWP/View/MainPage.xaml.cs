using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using OneEchan.ViewModel;
using OneEchan.View;
using OneEchan.Core.Models;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace OneEchan
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainViewModel MainVM { get; } = new MainViewModel();

        public MainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as ListResult;
            Frame.Navigate(typeof(AnimateDetailPage), new AnimateDetailViewModel(item.ID,item.Name));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainVM.Refresh();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var menu = Resources["TitleMenu"] as MenuFlyout;
            menu.ShowAt(null, (sender as Button).TransformToVisual(null).TransformPoint(new Point()));
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AboutPage));
        }
    }
}
