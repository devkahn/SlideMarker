using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows;

namespace MDM.Views.Converters
{
    // count the number of TreeViewItems before reaching ScrollContentPresenter
    public class ParentCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int parentCount = 1;
            DependencyObject o = VisualTreeHelper.GetParent(value as DependencyObject);
            while (o != null && o.GetType().FullName != "System.Windows.Controls.ScrollContentPresenter")
            {
                if (o.GetType().FullName == "System.Windows.Controls.TreeViewItem")
                    parentCount += 1;
                o = VisualTreeHelper.GetParent(o);
            }
            return parentCount;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
