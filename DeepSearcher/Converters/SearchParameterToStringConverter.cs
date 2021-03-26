using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using DeepSearcher.Models;
using DeepSearcher.Models.TypeConverters;

namespace DeepSearcher.Converters
{
    internal class SearchParameterToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(SearchParameter))
            {
                return ((string)value).StringToSearchParameter();
            }

            
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(string))
            {
                return ((SearchParameter)value).SearchParameterToString();
            }

            return null;
        }
    }
}
