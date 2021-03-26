using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DeepSearcher.Converters
{
    public class SizeTextToRealSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long index = System.Convert.ToInt64(value);
            switch (index)
            {
                case 0:
                    return 1;
                case 1:
                    return 1024;
                case 2:
                    return 1024*1024;
                case 3:
                    return 1024*1024*1024;
            }

            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
