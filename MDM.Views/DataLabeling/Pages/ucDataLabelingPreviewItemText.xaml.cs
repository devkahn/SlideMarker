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
    /// ucDataLabelingPreviewItemText.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucDataLabelingPreviewItemText : UserControl
    {
        private bool OnSaving { get; set; } = false;
        private string OriginText { get; set; } = string.Empty;

        public ucDataLabelingPreviewItemText()
        {
            InitializeComponent();
        }

        private void btn_ConnectLine_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                vmItem data = btn.DataContext as vmItem;
                if (data == null) return;

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
                TextBox tb = this.txtbox_OriginText;
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

        

        private void txtbox_OriginText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox tb = this.txtbox_OriginText;
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

        private void btn_ToList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = this.txtbox_OriginText.Text;
                this.txtbox_OriginText.Text = TextHelper.Preprocessing(text);
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

        private void btn_Trim_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = this.txtbox_OriginText.Text;
                string[] lines = TextHelper.SplitText(text);

                string output = string.Empty;
                foreach (string ln in lines)
                {
                    output += ln.Trim();
                    output += "\n";
                }
                this.txtbox_OriginText.Text = output.Trim();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
                
            }
        }

        private void btn_Numbering_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = this.txtbox_OriginText.Text;
                this.txtbox_OriginText.Text = TextHelper.Preprocessing(text);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);

            }
        }

        private void btn_NoToList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string output = string.Empty;
                string[] lines = TextHelper.SplitText(this.txtbox_OriginText.Text);

                char[] mark = { '-', '>', '*', '▷', '▣' };
                foreach (string ln in lines)
                {
                    string newLine = ln;
                    if (mark.Contains(newLine.First()))
                    {
                        newLine = newLine.Substring(1);
                    }
                    else if(newLine.StartsWith("\t"))
                    {
                        string indent = string.Empty;
                        while (newLine.First() == '\t')
                        {
                            newLine = newLine.Substring(1);
                            indent += "  ";
                        }
                        if (mark.Contains(newLine.First())) newLine = newLine.Substring(1);

                        newLine = newLine.Insert(0, indent);
                    }

                    output += newLine;
                    if (ln != lines.Last()) output += "\n";
                }
                this.txtbox_OriginText.Text = output;
               
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);

            }
        }
    }
}
