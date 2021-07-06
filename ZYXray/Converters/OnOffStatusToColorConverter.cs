using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ZYXray.Converters
{
    public class OnOffStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
           System.Globalization.CultureInfo culture)
        {
            if ((bool) value)
            {
                return System.Windows.Media.Brushes.Green;
            }
            return System.Windows.Media.Brushes.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
           System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
