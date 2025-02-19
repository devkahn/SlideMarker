﻿using System;
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
        public vmHeading SelectedTargetHeader { get; set; } = null;
        public wndTargetHeaderSelection()
        {
            InitializeComponent();
        }

        private void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            this.SelectedTargetHeader = this.tree.SelectedItem as vmHeading;

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
      
    }
}
