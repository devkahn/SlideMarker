using System;
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
    /// ucXMLSettingChapter.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucXMLSettingChapter : UserControl
    {
        public vmMaterial Material { get; set; }
        public ucXMLSettingChapter(vmMaterial material)
        {
            this.Material = material;
            InitializeComponent();
            BindPropertyList();
            BindConFigureList();
        }
        
        private void BindPropertyList()
        {
            PropertyInfo[] pInfos = this.Material.XMLSets.Chapter.GetType().GetProperties();

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
            PropertyInfo[] pInfos = this.Material.XMLSets.Chapter.Config.GetType().GetProperties();

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
            xmlChapter chapterOption = this.Material.XMLSets.Chapter;

            foreach (vmXMLProperty propItem in this.propertyList.Items)
            {
                PropertyInfo pInfo = propItem.Origin;
                if (pInfo == null) continue;

                var value = propItem.ValuePanel.GetType().GetProperty("Value").GetValue(propItem.ValuePanel, null);
                pInfo.SetValue(chapterOption, value);
            }


            foreach (vmXMLProperty configItem in this.conFigureList.Items)
            {
                PropertyInfo pInfo = configItem.Origin;
                if (pInfo == null) continue;

                var value = configItem.ValuePanel.GetType().GetProperty("Value").GetValue(configItem.ValuePanel, null);
                pInfo.SetValue(chapterOption.Config, value);
            }

            this.Material.XMLSets.Chapter = chapterOption;
        }


        
    }
}
