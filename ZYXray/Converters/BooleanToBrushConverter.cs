using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ZYXray.Converters
{
    public class BooleanToBrushConverter : IValueConverter
    {
        private SolidColorBrush grey = new SolidColorBrush(Color.FromRgb(0x8e, 0x8e, 0x8e));
        private SolidColorBrush blue = new SolidColorBrush(Color.FromRgb(0x00, 0xbd, 0xf7));

        public object Convert(object value, Type targetType, object parameter,
           System.Globalization.CultureInfo culture)
        {
            bool isOn = (bool)value;

            if (isOn)
            {
                return blue;
            }
            else
            {
                return grey;
            }     
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
