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
using MDM.Commons.Enum;
using MDM.Helpers;
using MDM.Models.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace MDM.Views.Pages.DataLabeling
{
    /// <summary>
    /// ucDataLabelingPreviewSlide.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucDataLabelingPreviewSlide : UserControl
    {
        private vmSlide CurrentSlide => this.DataContext as vmSlide;

        public ucDataLabelingPreviewSlide()
        {
            InitializeComponent();
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                PreviewRendering();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PreviewRendering();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        public void PreviewRendering()
        {
            if (this.CurrentSlide == null) return;

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;

            foreach (UIElement item in this.CurrentSlide.PreviewItems) sp.Children.Add(item);

            this.contentPresenter_Preview.Content = null;
            this.contentPresenter_Preview.Content = sp;
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                //PreviewRendering();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
