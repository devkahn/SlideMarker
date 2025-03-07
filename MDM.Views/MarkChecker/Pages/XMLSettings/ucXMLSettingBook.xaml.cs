using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
using MDM.Models.Attributes;
using MDM.Models.DataModels.ManualWorksXMLs;
using MDM.Models.ViewModels;
using MDM.Views.Controls.XMLProperyValues;

namespace MDM.Views.MarkChecker.Pages.XMLSettings
{
    /// <summary>
    /// ucXMLSettingBook.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucXMLSettingBook : UserControl
    {
        public vmMaterial Material { get; set; }
        
        public ucXMLSettingBook(vmMaterial material)
        {
            this.Material = material;
            InitializeComponent();

            BindPropertyList();
            BindConFigureList();
        }
        
        private void BindPropertyList()
        {
            PropertyInfo[] pInfos = this.Material.XMLSets.Book.GetType().GetProperties();

            this.propertyList.Items.Clear();
            foreach (PropertyInfo pInfo in pInfos)
            {
                bool hasSubProperty = pInfo.CustomAttributes.Where(x => x.AttributeType == typeof(xmlSubPropertyAttribute)).Any();
                if (!hasSubProperty) continue;

                bool hasDescription = pInfo.CustomAttributes.Where(x => x.AttributeType == typeof(DescriptionAttribute)).Any();
                if (!hasDescription) continue;

                vmXMLProperty newProp = new vmXMLProperty(pInfo);
                UserControl panel = ctrlXMLPropValue.GetValueContent(pInfo);
                newProp.SetValuePanel(panel);
                this.propertyList.Items.Add(newProp);
            }
        }
        private void BindConFigureList()
        {
            this.Material.XMLSets.Book.Config = new xmlBookConfig();

            PropertyInfo[] pInfos = this.Material.XMLSets.Book.Config.GetType().GetProperties();

            this.conFigureList.Items.Clear();
            foreach (PropertyInfo pInfo in pInfos)
            {
                bool hasDescription = pInfo.CustomAttributes.Where(x => x.AttributeType == typeof(DescriptionAttribute)).Any();
                if (!hasDescription) continue;

                vmXMLProperty newProp = new vmXMLProperty(pInfo);
                UserControl panel = ctrlXMLPropValue.GetValueContent(pInfo);
                newProp.SetValuePanel(panel);
                this.conFigureList.Items.Add(newProp);
            }
        }

        public void SetProperty()
        {
            xmlBook newBookElement = new xmlBook();

            foreach (vmXMLProperty property in this.propertyList.Items)
            {

                var value = property.ValuePanel.GetType().GetProperty("Value").GetValue(property.ValuePanel, null);
                newBookElement.GetType().GetProperty(property.Origin.Name).SetValue(newBookElement, value);
            }

            xmlBookConfig newConfig = new xmlBookConfig();
            foreach (vmXMLProperty config in this.conFigureList.Items)
            {
                var value = config.ValuePanel.GetType().GetProperty("Value").GetValue(config.ValuePanel, null);
                newBookElement.Config.GetType().GetProperty(config.Origin.Name).SetValue(newBookElement.Config, value);
            }

            this.Material.XMLSets.Book = newBookElement;
        }

        private void propertyList_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //DataGrid dg = sender as DataGrid;
                //if (dg == null) return;

                //if (e.Key == Key.Enter)
                //{
                //    e.Handled = true;  // Enter키 기본 동작(행 이동) 취소
                //}
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_SettingCompleted_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetProperty();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
