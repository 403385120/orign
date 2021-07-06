using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ZYXray.Converters
{
    public class BooleanToRecheckTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
                System.Globalization.CultureInfo culture)
        {
            bool val = bool.Parse(value.ToString());

            if (val)
            {
                return LangHelper.getTranslation("LL_ManualCheck");
            }
            else
            {
                return LangHelper.getTranslation("LL_FQA");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                 System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
