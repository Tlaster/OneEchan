using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneEchan.Core.Common.Extensions;
using Windows.UI.Xaml.Data;

namespace OneEchan.Converter
{
    public class DateTimeToShortTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => DateTime.Parse(value.ToString()).DiffForHumans();
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
