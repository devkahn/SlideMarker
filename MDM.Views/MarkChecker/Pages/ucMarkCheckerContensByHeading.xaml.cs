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
using OfficeOpenXml.Style.XmlAccess;

namespace MDM.Views.MarkChecker.Pages
{
    /// <summary>
    /// ucMarkCheckerContensByHeading.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucMarkCheckerContensByHeading : UserControl
    {
        private vmMaterial _Material = null;
        public vmMaterial Material
        {
            get => _Material;
            set
            {
                _Material = value;

                int minLevel = value.Headings.Min(x => x.Temp.Level);
                this.treeview_Header.ItemsSource = value.Headings.Where(x => x.Temp.Level == minLevel);
            }
        }


        public ucMarkCheckerContensByHeading()
        {
            InitializeComponent();
        }


        private bool IsTreeChanged = false;
        private void treeview_Header_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (e.NewValue == null) return;
                if (this.contentsPresenter == null) return;

                this.IsTreeChanged = true;

                vmHeading selectedItem = e.NewValue as vmHeading;
                if (selectedItem == null || selectedItem.Contents.Count < 0)
                {
                    this.IsTreeChanged = false;
                    return;
                }

                this.contentsPresenter.Content = selectedItem.Contents;

                this.IsTreeChanged = false;

            }
            catch (Exception ee)
            {
                this.IsTreeChanged = false;
                ErrorHelper.ShowError(ee);
            }
        }

        private void listbox_Children_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.IsTreeChanged) return;

                if (e.AddedItems == null) return;
                if (this.contentsPresenter == null) return;

                foreach (var item in e.AddedItems)
                {
                    vmHeading selectedItem = item as vmHeading;
                    if (selectedItem == null) return;
                    if (selectedItem.Contents.Count() < 0) return;

                    this.contentsPresenter.Content = selectedItem.Contents;
                }
                

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_SelectionRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vmHeading selectedItem = this.treeview_Header.SelectedItem as vmHeading;
                if (selectedItem == null) return;

                vmHeading parent = selectedItem.Parent;
                foreach (vmHeading child in selectedItem.Children)
                {
                    //child.SetParentHeading(parent);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
