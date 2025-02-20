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

namespace MDM.Views.Controls.XMLProperyValues
{
    /// <summary>
    /// ctrlXMLPropValueDigit.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ctrlXMLPropValueDigit : UserControl
    {
        public object Value
        {
            get
            {
                if (int.TryParse(this.value.Text, out int intValue)) return intValue;
                else if (double.TryParse(this.value.Text, out double doubleValuie)) return doubleValuie;
                else if (float.TryParse(this.value.Text, out float floatValue)) return floatValue;
                else return null;
            }
        }
        public ctrlXMLPropValueDigit(object defaultValue)
        {
            InitializeComponent();
            this.value.Text = defaultValue.ToString();
        }
    }
}
