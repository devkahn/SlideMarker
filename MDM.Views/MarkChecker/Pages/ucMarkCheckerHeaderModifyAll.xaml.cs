using MDM.Helpers;
using MDM.Models.ViewModels;
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

namespace MDM.Views.MarkChecker.Pages
{
    /// <summary>
    /// ucMarkCheckerHeaderModifyAll.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucMarkCheckerHeaderModifyAll : UserControl
    {
        public ucMarkCheckerHeaderModifyAll()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                vmHeading selectedHeading = e.NewValue as vmHeading;
                if (selectedHeading == null) return;

                List<structModifyAll> items = new List<structModifyAll>();
                foreach (vmHeading item in selectedHeading.Children)
                {
                    structModifyAll newITem = new structModifyAll();
                    newITem.Origin = item;
                    newITem.OriginName = item.Temp.Name;
                    newITem.TargetName = item.Temp.Name;    
                    items.Add(newITem);
                }

                this.datagrid.ItemsSource = items;  
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }

    struct structModifyAll
    {
        public  vmHeading Origin { get; set; }
        public string OriginName { get; set; } 
        public string TargetName { get; set; }
    }
}
