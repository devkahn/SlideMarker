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
using MDM.Models.ViewModels;

namespace MDM.Views.Pages.DataLabeling
{
    /// <summary>
    /// ucDataLabelingInfo.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucDataLabelingInfo : UserControl
    {
        private vmMaterial _Material = null;
        private vmMaterial Material
        {
            get => _Material;
            set
            {
                _Material = value;
                this.DataContext = value;
            }
        }


        public ucDataLabelingInfo()
        {
            InitializeComponent();
        }

        public void SetMaterial(vmMaterial material)
        {
            this.Material = material;
        }
    }
}
