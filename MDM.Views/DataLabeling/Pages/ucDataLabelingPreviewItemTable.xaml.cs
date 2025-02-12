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

namespace MDM.Views.DataLabeling.Pages
{
    /// <summary>
    /// ucDataLabelingPreviewItemTable.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucDataLabelingPreviewItemTable : UserControl
    {
        private bool OnSaving { get; set; } = false;
        private string OriginText { get; set; } = string.Empty;
        public ucDataLabelingPreviewItemTable()
        {
            InitializeComponent();
        }

        private void btn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.OnSaving = true;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                vmItem data = btn.DataContext as vmItem;
                if (data == null) return;

                TextBox tb = this.txtbox_OriginText;
                if (tb == null) return;

                tb.Text = this.OriginText;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_Clipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                vmItem data = btn.DataContext as vmItem;
                if (data == null) return;

                TextBox tb = btn.Tag as TextBox;
                if (tb == null) return;

                Clipboard.SetText(tb.Text);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                vmItem newItem = e.NewValue as vmItem;
                if (newItem == null) return;

                this.OriginText = newItem.Temp.LineText;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_PreView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                vmItem data = btn.DataContext as vmItem;    
                if(data == null) return;

                TextBox tb = this.txtbox_OriginText;
                if (tb == null) return;

                data.SetPreviewItem(tb.Text);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_Completed_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                vmItem data = btn.DataContext as vmItem;
                if (data == null) return;

                TextBox tb = this.txtbox_OriginText;
                if (tb == null) return;

                this.OriginText = tb.Text;
                data.SetText(this.OriginText);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
