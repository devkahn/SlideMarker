using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MDM.Views.Resources.Styles
{
    public partial class resMDMStyle_Datagrid : ResourceDictionary
    {
        public resMDMStyle_Datagrid()
        {
            InitializeComponent();
        }

        private void DataGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                if(sender is DataGrid) e.Handled = true; // 기본 동작을 막음
            }
        }
    }
}
