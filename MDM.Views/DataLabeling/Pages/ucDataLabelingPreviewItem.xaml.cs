using MDM.Helpers;
using MDM.Models.ViewModels;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Win32;
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

namespace MDM.Views.DataLabeling.Pages
{
    /// <summary>
    /// ucDataLabelingPreviewItem.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucDataLabelingPreviewItem : UserControl
    {
        public ucDataLabelingPreviewItem()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btn_ConnectLine_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                vmItem data = btn.DataContext as vmItem;
                if(data == null) return;    

                TextBox tb = btn.Tag as TextBox;
                if (tb == null) return;

                string[] splits = TextHelper.SplitText(tb.Text);

                string result = string.Empty;
                foreach (string s in splits) result += s;
                data.SetText(result);
                tb.Text = result;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_ImageTitleEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {

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

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void txtbox_ImageTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox tb = sender as TextBox;
                if (tb == null) return;

                vmItem data = tb.DataContext as vmItem;
                if (data == null) return;

                if(data.ItemType == Commons.Enum.eItemType.Image)
                {
                    data.SetTitle(tb.Text);
                    data.SetImageText(data.Temp.Title, data.Temp.LineText);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void txtbox_ImageTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox tb = sender as TextBox;
                if (tb == null) return;

                vmItem data = tb.DataContext as vmItem;
                if (data == null) return;

                //data.SetPreviewItem();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void Grid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                Grid grid = sender as Grid;
                if (grid == null) return;

                vmItem data = grid.DataContext as vmItem;
                if (data == null) return;

                List<string> headers = new List<string>();

                vmItem parent = data.Parent;
                while (parent != null)
                {
                    headers.Add(parent.Temp.LineText);
                    parent = parent.Parent;
                }

                headers.Reverse();

                this.stackpanel_Headers.Children.Clear();
                for (int i = 0; i < headers.Count; i++)
                {
                    string header = i == 0 ? "- " : "└ ";
                    header += headers[i];

                    TextBlock newTb = new TextBlock();
                    newTb.Text = header;
                    newTb.Margin = new Thickness(i * 10, 2, 0, 2);
                    this.stackpanel_Headers.Children.Add(newTb);    
                }


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

        private void btn_Align_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                vmItem data = btn.DataContext as vmItem;
                if (data == null) return;

                TextBox tb = btn.Tag as TextBox;
                if (tb == null) return;

                
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void txtbox_OriginText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox tb = sender as TextBox;
                if (tb == null) return;

                vmItem data = tb.DataContext as vmItem;
                if (data == null) return;

                if (data.ItemType != Commons.Enum.eItemType.Image)
                {
                    data.SetText(tb.Text);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void txtbox_OriginText_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
