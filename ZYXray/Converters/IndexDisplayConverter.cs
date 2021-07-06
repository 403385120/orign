using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ZYXray.Converters
{
    public class IndexDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                System.Globalization.CultureInfo culture)
        {
            int val = int.Parse(value.ToString());

            return val + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                 System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
