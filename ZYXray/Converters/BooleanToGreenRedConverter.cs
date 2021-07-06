using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZYXray.Models;
using XRayClient.BatteryCheckManager;

namespace ZYXray.Converters
{
    public class BooleanToGreenRedConverter : IMultiValueConverter
    {
        private SolidColorBrush red = new SolidColorBrush(Colors.Red);
        private SolidColorBrush green = new SolidColorBrush(Colors.Green);

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int index = int.Parse(values[0].ToString());
            int subIndex = int.Parse(values[1].ToString());
            List<RecheckRecord> records = (List<RecheckRecord>)values[2];

            if (records.Count <= index) return red;

            RecheckRecord record = records[index];
            if (subIndex == 0)
            {
                return record.BackResult ? green : red;
            }
            else
            {
                return record.FrontResult ? green : red;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
        object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
