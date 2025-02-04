using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MDM.Views.Converters
{
    public class DoubleToHalfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;

            bool isDouble = double.TryParse(value.ToString(), out double result);
            return isDouble ? result * 0.5 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;

            bool isDouble = double.TryParse(value.ToString(), out double result);
            return isDouble ? result * 2 : 0;
        }
    }
}
