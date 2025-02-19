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
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MDM.Helpers;
using MDM.Models.ViewModels;

namespace MDM.Views.MarkChecker.Pages.RuleSetPages
{
    /// <summary>
    /// ucRuleCheckContentsSync.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucRuleCheckContentsSync : UserControl
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

        public ucRuleCheckContentsSync()
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
    }
}
