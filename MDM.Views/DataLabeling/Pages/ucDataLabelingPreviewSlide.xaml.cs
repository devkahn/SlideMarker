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
using MDM.Views.DataLabeling.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace MDM.Views.DataLabeling.Pages
{
    /// <summary>
    /// ucDataLabelingPreviewSlide.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucDataLabelingPreviewSlide : UserControl
    {
        public Border BottomPanel => this.border_BottomPanel;


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
                vmMaterial current = this.DataContext as vmMaterial;
                if (current == null) return;

                vmSlide currentSlide = current.CurrentSlide;
                if(currentSlide == null) return;

                this.listbox_Preview.ItemsSource = null;
                this.listbox_Preview.ItemsSource = currentSlide.Items;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        public void PreviewRendering()
        {
      
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                //PreviewRendering();
                var items =this.listbox_Preview.ItemsSource;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void listbox_Preview_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void btn_ShowSlidePreviewWindow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                wndSlidePreview wndSP = new wndSlidePreview();
                wndSP.PreviewSlidePage.DataContext = this.DataContext;
                wndSP.Show();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
