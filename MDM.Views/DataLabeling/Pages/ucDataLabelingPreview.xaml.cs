using System.Windows.Controls;
using MDM.Models.ViewModels;

namespace MDM.Views.DataLabeling.Pages
{
    /// <summary>
    /// ucDataLabelingPreview.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucDataLabelingPreview : UserControl
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
        public ucDataLabelingPreview()
        {
            InitializeComponent();
        }
        public void SetMaterial(vmMaterial material)
        {
            this.Material = material;
        }
    }
}
