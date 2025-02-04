using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MDM.Helpers;

namespace MDM.Views.Resources.Styles
{
    public partial class resMDMStyle_Textbox : ResourceDictionary
    {
        public resMDMStyle_Textbox()
        {
            InitializeComponent();
        }


        private void txtbox_Number_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!TextHelper.IsTextNuberic(e.Text)) e.Handled = true;
        }
        private void txtbox_Number_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!TextHelper.IsTextNuberic(e.DataObject.GetData(typeof(string)) as string)) e.CancelCommand();
        }
    }
}
