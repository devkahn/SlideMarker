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
using MDM.Models.ViewModels;
using MDM.Views.MarkChecker.Pages.XMLSettings;

namespace MDM.Views.MarkChecker.Pages
{
    /// <summary>
    /// ucMarCheckerToXml.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucMarCheckerToXml : UserControl
    {
      

        private vmMaterial _Material = null;
        public vmMaterial Material
        {
            get => _Material;
            set
            {
                _Material = value;
                
            }
        }
        public ucMarCheckerToXml()
        {
            InitializeComponent();
        }

        private void treeview_Header_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
          
        }
    }
}
