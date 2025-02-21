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
    /// ctrlXMLPropValueBool.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ctrlXMLPropValueBool : UserControl
    {
        public object Value
        {
            get
            {
                if (this.rBtn_True.IsChecked == true) return true;
                else if (this.rBtn_False.IsChecked == true) return false;
                else return null;
            }
        }
        public ctrlXMLPropValueBool(object defaultValue)
        {
            InitializeComponent();
            bool? isTrue = (bool?)defaultValue;
            if(isTrue.HasValue)
            {
                if (isTrue.Value)
                {
                    this.rBtn_True.IsChecked = true;
                }
                else
                {
                    this.rBtn_False.IsChecked = true;
                }
            }
            else
            {
                this.rBtn_False.IsChecked = false;
                this.rBtn_True.IsChecked = false;
            }
            
            
            
        }
    }
}
