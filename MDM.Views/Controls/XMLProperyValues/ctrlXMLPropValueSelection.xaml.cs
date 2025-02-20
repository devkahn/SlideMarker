using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MDM.Helpers;

namespace MDM.Views.Controls.XMLProperyValues
{
    /// <summary>
    /// ctrlXMLPropValueSelection.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ctrlXMLPropValueSelection : UserControl
    {
        public object Value
        {
            get
            {
                ComboBoxItem selected = this.combo.SelectedItem as ComboBoxItem;
                if (selected == null) return null;

                return selected.Tag;
            }
        }

   

        public ctrlXMLPropValueSelection(object defaultValue)
        {
            InitializeComponent();

            Type type = defaultValue.GetType();
            if(type.BaseType == typeof(Enum))
            {
                var values = Enum.GetValues(type);

                ComboBoxItem selectd = null;
                List<ComboBoxItem> items = new List<ComboBoxItem>();
                foreach (var item in values)
                {
                    ComboBoxItem newItem = new ComboBoxItem();
                    newItem.Uid = item.GetHashCode().ToString();
                    newItem.Content = EnumHelpers.GetDescription((item as Enum));
                    newItem.Tag = item as Enum;
                    items.Add(newItem);
                    if (item.GetHashCode() == defaultValue.GetHashCode()) selectd = newItem;
                }

                this.combo.ItemsSource = items;
                this.combo.SelectedItem = selectd;
            }
        }
       
    }
}
