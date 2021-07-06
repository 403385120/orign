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
    /// 获取指定导航页面的内容
    /// </summary>
    public class PagedListItemsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int pageNum = int.Parse(values[0].ToString());
            int numPerPage = int.Parse(values[1].ToString());
            ICollection list = (ICollection)(values[2]);

            List<object> newList = new List<object>();

            int startIndex = (pageNum - 1) * numPerPage;
            int index = 0;
            foreach (var item in list)
            {
                index++;

                if (index-1 < startIndex) continue;
                if (index-1 >= startIndex + numPerPage) break;

                newList.Add(item);
            }

            return newList;
        }

        public object[] ConvertBack(object value, Type[] targetTypes,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

