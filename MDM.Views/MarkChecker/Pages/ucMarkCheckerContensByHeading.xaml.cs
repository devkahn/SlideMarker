using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using MDM.Models.DataModels;
using MDM.Models.ViewModels;
using MDM.Views.MarkChecker.Pages.RuleSetPages;
using MDM.Views.MarkChecker.Windows;
using OfficeOpenXml.Style.XmlAccess;
using static OfficeOpenXml.ExcelErrorValue;

namespace MDM.Views.MarkChecker.Pages
{
    /// <summary>
    /// ucMarkCheckerContensByHeading.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucMarkCheckerContensByHeading : UserControl
    {
        private bool IsTreeChanged = false;

        private vmMaterial _Material = null;
        public vmMaterial Material
        {
            get => _Material;
            set
            {
                _Material = value;
                this.DataContext = value;
                
            }
        }

        private Dictionary<string, UserControl> RuleSetPages { get; set; } = new Dictionary<string, UserControl>();


        

        public ucMarkCheckerContensByHeading()
        {
            InitializeComponent();
        }

        private void treeview_Header_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (e.NewValue == null) return;



                vmHeading selectedItem = e.NewValue as vmHeading;
                this.Material.CurrentHeading = selectedItem;





            }
            catch (Exception ee)
            {
                this.IsTreeChanged = false;
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_ChildrenFold_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                int code = int.Parse(btn.Uid);

                string caption = btn.Content.ToString();


                vmHeading selectedItem = this.treeview_Header.SelectedItem as vmHeading;
                if (selectedItem == null) return;

                foreach (vmHeading item in selectedItem.Children)
                {
                    item.TreeExpand(code ==1 ? false: true);
                }

                
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<mHeading> rootHeadings = new List<mHeading>();
                foreach (vmHeading root in this.Material.RootHeadings)
                {
                    mHeading headingData = root.UpdateOriginData() as mHeading;
                    if(headingData != null) rootHeadings.Add(headingData);
                }

                string jsonString = JsonHelper.ToJsonString(rootHeadings);

                string targetPath = System.IO.Path.Combine(this.Material.DirectoryPath, string.Format("{0}_{1}.headers", this.Material.Temp.Name ,DateTime.Now.ToString("yyyyMMddHHmmss")));
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                File.WriteAllText(targetPath, jsonString);

                string msg = "저장 완료";
                MessageHelper.ShowMessage("", msg);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void listbox_RuleSet_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.contentPresenter == null) return;

                ListBoxItem selectedRule = this.listbox_RuleSet.SelectedItem as ListBoxItem;
                if (selectedRule == null) return;

                string uid = selectedRule.Uid;
                if(!this.RuleSetPages.ContainsKey(uid))
                {
                    UserControl uc = null;
                    switch (uid)
                    {
                        case "1": uc = new ucRuleCheckTreePreprocessing(); break;
                        case "101": uc = new ucRuleCheckContentsSync(); break;
                        case "102": uc = new ucRuleCheckSameNameFinder(); break;
                        case "999": uc = new ucRuleCheckUserModify(); break;
                        default: break;
                    }
                    this.RuleSetPages.Add(uid, uc);
                }

                UserControl page = this.RuleSetPages[uid];
                if(page != null)
                {
                    page.DataContext = null;
                    page.DataContext = this.Material;
                }
                

                this.contentPresenter.Content = null;
                this.contentPresenter.Content = page;
            }
            catch (Exception ee) 
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
