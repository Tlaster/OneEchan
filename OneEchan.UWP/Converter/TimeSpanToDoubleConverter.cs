using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace OneEchan.UWP.Converter
{
    public class TimeSpanToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => TimeSpan.Parse(value.ToString()).TotalSeconds;

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => TimeSpan.FromSeconds(double.Parse(value.ToString()));

    }
}
