using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OneEchan.UWP.Common
{
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



        public CustomMediaTransportControls()
        {
            this.DefaultStyleKey = typeof(CustomMediaTransportControls);
        }

    }
}
