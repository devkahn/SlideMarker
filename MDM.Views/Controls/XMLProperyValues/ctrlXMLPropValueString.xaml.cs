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
    /// ctrlXMLPropValueString.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ctrlXMLPropValueString : UserControl
    {
        public object Value => this.value.Text;
        public ctrlXMLPropValueString(object defaultValue)
        {
            InitializeComponent();
            if (defaultValue != null) this.value.Text = defaultValue.ToString();
        }
    }
}
