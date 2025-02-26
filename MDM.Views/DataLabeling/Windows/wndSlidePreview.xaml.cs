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
using MDM.Views.DataLabeling.Pages;

namespace MDM.Views.DataLabeling.Windows
{
    /// <summary>
    /// wndSlidePreview.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class wndSlidePreview : Window
    {
        public ucDataLabeling ParentPage { get; set; }

        public wndSlidePreview()
        {
            InitializeComponent();
            this.PreviewSlidePage.DataContext = this.DataContext;
            
        }
        public wndSlidePreview(ucDataLabeling parentPage)
        {
            this.ParentPage = parentPage;
            InitializeComponent();
            this.PreviewSlidePage.DataContext = this.DataContext;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.ParentPage.PreviewWindow = null;
        }
    }
}
