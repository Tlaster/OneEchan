using OneEchan.ViewModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using OneEchan.UWP.ViewModel;
using OneEchan.Core.Models;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace OneEchan
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AnimateDetailPage : Page
    {
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
            var item = e.ClickedItem as DetailList;
            Frame.Navigate(typeof(AnimatePlayPage), new WatchPageViewModel(DetailVM.ID, item.Set));
        }
    }
}
