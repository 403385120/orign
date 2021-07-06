using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ZYXray.Converters
{
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int i = 0;

            try
            {
                foreach (var enumValue in Enum.GetValues(value.GetType()))
                {
                    if (enumValue.ToString() == value.ToString())
                    {
                        string transFrom = "LL_" + value.GetType().ToString().Split(new char[] { '.' }).LastOrDefault();

                        /* We can not use (int)enumValue here because we're not sure if the enum starts with index 0 */
                        string trans = LangHelper.getTranslation(transFrom, i);

                        if (transFrom == trans)
                        {
                            return enumValue.ToString();
                        }

                        return trans;
                    }

                    i++;
                }

                throw new Exception("Not found");
            }
            catch
            {
                return "Unfied";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
