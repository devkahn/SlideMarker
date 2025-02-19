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

namespace MDM.Views.MarkChecker.Pages.RuleSetPages
{
    /// <summary>
    /// ucRuleCheckSameNameFinder.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucRuleCheckSameNameFinder : UserControl
    {
        private vmMaterial _Material = null;
        public vmMaterial Material
        {
            get => _Material;
            set
            {
                _Material = value;
                if (value == null) return;

            }
        }

        public ucRuleCheckSameNameFinder()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                this.Material = e.NewValue as vmMaterial;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_CheckStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<vmHeading> checkitems = new List<vmHeading>();

                foreach (vmHeading heading in this.Material.Headings)
                {
                    if (!heading.Children.Any()) continue;

                    List<string> childHeaders = new List<string>();
                    foreach (var item in heading.Children)
                    {
                        string name = item.Temp.Name;
                        if(childHeaders.Contains(name))
                        {
                            checkitems.Add(item);
                            break;
                        }
                        else
                        {
                            childHeaders.Add(name);
                        }
                    }
                }

                this.listbox_parentheader.ItemsSource = checkitems;
                this.listbox_parentheader.SelectedIndex = 0;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
