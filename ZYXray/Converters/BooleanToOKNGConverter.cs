using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ZYXray.Models;
using System.Collections.Generic;
using XRayClient.BatteryCheckManager;

namespace ZYXray.Converters
{
    public class BooleanToOKNGConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int index = int.Parse(values[0].ToString());
            int subIndex = int.Parse(values[1].ToString());
            List<RecheckRecord> records = (List<RecheckRecord>)values[2];

            if (records.Count <= index) return "NG";

            RecheckRecord record = records[index];
            if(subIndex == 0)
            {
                return record.BackResult ? "OK" : "NG";
            }
            else
            {
                return record.FrontResult ? "OK" : "NG";
            }           
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
          object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
