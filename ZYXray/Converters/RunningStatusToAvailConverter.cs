using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using XRayClient.DeviceController;

namespace ZYXray.Converters
{
    public class RunningStatusToAvailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            ERunningStatus run = (ERunningStatus)value;

            if (run == ERunningStatus.Running
                || run == ERunningStatus.RunningPending)
            {
                return false;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
