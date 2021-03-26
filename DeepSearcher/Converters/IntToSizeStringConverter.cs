using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DeepSearcher.Converters
{
    internal class IntToSizeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = System.Convert.ToDouble(value);
            int counter = 0;
            while (val >= 1000)
            {
                val /= 1000;
                counter++;
            }
            switch (counter)
            {
                case 0:
                    return string.Format("{0:0} Bytes", val);
                case 1:
                    return string.Format("{0:0} KB", val);
                case 2:
                    return string.Format("{0:0} MB", val);
                case 3:
                    return string.Format("{0:0} GB", val);
                case 4:
                    return string.Format("{0:0} TB", val);
            }

            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                if ((bool)value == true)
                    return "True";
                else
                    return "False";
            }
            return "False";
        }

    }
}