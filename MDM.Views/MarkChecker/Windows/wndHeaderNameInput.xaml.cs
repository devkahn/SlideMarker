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

namespace MDM.Views.MarkChecker.Windows
{
    /// <summary>
    /// wndHeaderNameInput.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class wndHeaderNameInput : Window
    {
        public string HeaderName { get; set; } = string.Empty;
        public wndHeaderNameInput()
        {
            InitializeComponent();
        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            if(TextHelper.IsNoText(this.txtbox_NewName.Text))
            {
                string eMsg = "추가할 제목을 입력하세요";
                MessageHelper.ShowErrorMessage("새 제목 입력", eMsg);
                return;
            }

            this.HeaderName = this.txtbox_NewName.Text;
            this.DialogResult = true;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.HeaderName = string.Empty;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtbox_NewName.Focus();
        }
    }
}
