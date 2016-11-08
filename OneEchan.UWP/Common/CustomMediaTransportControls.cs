using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace OneEchan.UWP.Common
{
    [TemplatePart(Name = "EnableTitleButton", Type = typeof(AppBarButton))]
    public class CustomMediaTransportControls : MediaTransportControls
    {
        public object QualityList
        {
            get { return GetValue(QualityListProperty); }
            set { SetValue(QualityListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QualityList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QualityListProperty =
            DependencyProperty.Register("QualityList", typeof(object), typeof(CustomMediaTransportControls), new PropertyMetadata(null));



        public int SelectedQuality
        {
            get { return (int)GetValue(SelectedQualityProperty); }
            set { SetValue(SelectedQualityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedQuality.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedQualityProperty =
            DependencyProperty.Register("SelectedQuality", typeof(int), typeof(CustomMediaTransportControls), new PropertyMetadata(-1));

        private AppBarButton EnableTitleButton;

        public CustomMediaTransportControls()
        {
            this.DefaultStyleKey = typeof(CustomMediaTransportControls);
        }
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            EnableTitleButton = GetTemplateChild("EnableTitleButton") as AppBarButton;
            EnableTitleButton.Click += EnableTitleButton_Click;
        }
        private void EnableTitleButton_Click(object sender, RoutedEventArgs e)
        {
            var titlebar = CoreApplication.GetCurrentView().TitleBar;
            titlebar.ExtendViewIntoTitleBar = !titlebar.ExtendViewIntoTitleBar;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility == AppViewBackButtonVisibility.Collapsed ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
            if (titlebar.ExtendViewIntoTitleBar)
            {
                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Colors.Transparent;

            }
            else
            {
                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = ((SolidColorBrush)Application.Current.Resources["AppTheme"]).Color;
            }
        }
    }
}
