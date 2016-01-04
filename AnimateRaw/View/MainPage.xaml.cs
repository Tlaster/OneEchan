using AnimateRaw.Shared.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AnimateRaw.Extension;
using System.ComponentModel;
using AnimateRaw.ViewModel;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace AnimateRaw
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainViewModel MainVM { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
            MainVM = new MainViewModel();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                await MainVM.GetFavorList();
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as AnimateListModel;
            Frame.Navigate(typeof(AnimateDetailPage), new AnimateDetailViewModel(item.ID,item.Name));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainVM.Refresh();
        }
    }
}
