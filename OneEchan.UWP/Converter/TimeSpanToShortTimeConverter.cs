using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace OneEchan.Converter
{
    public class TimeSpanToShortTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language) =>
            Shared.Common.Helper.UpdateTimeHelper.GetUpdate(TimeSpan.Parse(value.ToString()));

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
