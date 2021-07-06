using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ZYXray.Converters
{
    public class XRayConnectionToImageConverter : IValueConverter
    {
        BitmapImage on = new BitmapImage(new Uri("pack://application:,,,/ZYXray;component/assets/Xray_circle_on.png"));
        BitmapImage off = new BitmapImage(new Uri("pack://application:,,,/ZYXray;component/assets/Xray_circle_off.png"));

        public object Convert(object value, Type targetType, object parameter,
           System.Globalization.CultureInfo culture)
        {
            bool isOn = (bool)value;

            if (isOn)
            {
                return on;
            }
            else
            {
                return off;
            }
          
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
