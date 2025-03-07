using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace MDM.Views.Controls.XMLProperyValues
{
    /// <summary>
    /// ucXMLPropValueAddableList.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucXMLPropValueAddableList : UserControl
    {
        public string Value
        {
            get
            {
                string output = string.Empty;

                foreach (ListBoxItem item in this.list.Items)
                {
                    if (!string.IsNullOrEmpty(output)) output += ",";
                    output += item.Uid;
                }

                return output;
            }
        }
      

        public ucXMLPropValueAddableList(object defaultvalue)
        {
            InitializeComponent();
        }

        private void btn_AddWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddWord();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                ListBoxItem lbItem = btn.TemplatedParent as ListBoxItem;
                if (lbItem == null) return;

                this.list.Items.Remove(lbItem);
            }
            catch (Exception ee) 
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void value_KeyDown(object sender, KeyEventArgs e)
            {
            try
            {
                if(e.Key == Key.Enter)
                {
                    AddWord();
                }
                
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void AddWord()
        {
            string value = this.value.Text;

            if (!this.Value.Contains(value))
            {
                ListBoxItem newItem = new ListBoxItem();
                newItem.Uid = value;
                newItem.Content = value;

                this.list.Items.Add(newItem);
            }
            this.value.Text = string.Empty;
            this.value.Focus();
        }
    }
}
