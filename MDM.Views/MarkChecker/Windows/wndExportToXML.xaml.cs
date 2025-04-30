using MDM.Models.ViewModels;
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
using System.Windows.Shapes;

namespace MDM.Views.MarkChecker.Windows
{
    /// <summary>
    /// wndExportToXML.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class wndExportToXML : Window
    {
        private vmMaterial _Material = null;
        public vmMaterial Material
        {
            get => _Material;
            set
            {
                _Material = value;
                this.DataContext = value;
            }
        }



        public wndExportToXML()
        {
            InitializeComponent();
        }
    }
}
