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
using MDM.Helpers;
using MDM.Models.ViewModels;

namespace MDM.Views.MarkChecker.Windows
{
    /// <summary>
    /// wndTargetHeaderSelection.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class wndTargetHeaderSelection : Window
    {
        public vmHeading OriginHeader { get; set; } = null;
        public vmHeading SelectedTargetHeader { get; set; } = null;
        public wndTargetHeaderSelection(vmHeading originHeading)
        {
            this.OriginHeader = originHeading;
            InitializeComponent();
        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            this.SelectedTargetHeader = this.tree.SelectedItem as vmHeading;

            if(this.OriginHeader == this.SelectedTargetHeader)
            {
                string eMsg = "동일한 제목을 선택했습니다.\n다시 선택하세요.";
                MessageHelper.ShowErrorMessage("", eMsg);
                return;
            }

            if(this.SelectedTargetHeader == null)
            {
                string eMsg = "타겟을 설정하세요.";
                MessageHelper.ShowErrorMessage("", eMsg);
                return;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void treeview_Header_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.SelectedTargetHeader = this.tree.SelectedItem as vmHeading;
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e) => this.Close();

        private void btn_GoToRoot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.OriginHeader.SetParent(null);
                this.DialogResult = false;
                this.Close();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
