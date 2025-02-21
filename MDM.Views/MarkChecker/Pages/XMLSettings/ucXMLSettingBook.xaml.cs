using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
        public ucXMLSettingBook()
        {
            InitializeComponent();
            BindPropertyList();
            BindConFigureList();
        }


        private void BindConFigureList()
        {
            PropertyInfo[] pInfos = typeof(xmlBookConfig).GetProperties();

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
        private void BindPropertyList()
        {
            PropertyInfo[] pInfos = typeof(xmlBook).GetProperties();

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
                xmlBook newBookElement = new xmlBook();

                foreach (vmXMLProperty property in this.propertyList.Items)
                {
                    
                }

                xmlBookConfig newConfig = new xmlBookConfig();
                foreach (vmXMLConfigure config in this.conFigureList.Items)
                {

                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
