using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ZYXray.Converters
{
    public class NullToDefaultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if(value != null)
                return value;

            if (targetType == null) return null;

            return Activator.CreateInstance(targetType);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value != null)
                return value;

            if(targetType == null) return null;

            return Activator.CreateInstance(targetType);
        }
    }
}
