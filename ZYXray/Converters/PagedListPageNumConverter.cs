using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ZYXray.Converters
{
    /// <summary>
    /// 获取指定导航页面的总数
    /// </summary>
    public class PagedListPageNumConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int numPerPage = int.Parse(values[0].ToString());
            ICollection list = (ICollection)(values[1]);

            List<object> newList = new List<object>();

            int x =  (int)Math.Ceiling((float)list.Count / (float)numPerPage);

            return x;
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

