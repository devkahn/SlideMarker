using MDM.Helpers;
using MDM.Models.Attributes;
using MDM.Models.DataModels.ManualWorksXMLs;
using MDM.Models.ViewModels;
using MDM.Views.Controls.XMLProperyValues;
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

namespace MDM.Views.MarkChecker.Pages.XMLSettings
{
    /// <summary>
    /// ucXMLSettingHeading5.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucXMLSettingHeading5 : UserControl
    {
        public eXMLElementType ContentType => eXMLElementType.heading5;
        public vmMaterial Material { get; set; }
        public ucXMLSettingHeading5(vmMaterial material)
        {
            this.Material = material;
            InitializeComponent();
            BindPropertyList();
            BindConFigureList();
        }


        private void BindPropertyList()
        {
            PropertyInfo[] pInfos = this.Material.XMLSets.Heading5Element.GetType().GetProperties();

            this.propertyList.Items.Clear();
            foreach (PropertyInfo pInfo in pInfos)
            {
                bool hasSubProperty = pInfo.CustomAttributes.Where(x => x.AttributeType == typeof(xmlSubPropertyAttribute)).Any();
                if (!hasSubProperty) continue;

                bool hasDescription = pInfo.CustomAttributes.Where(x => x.AttributeType == typeof(DescriptionAttribute)).Any();
                if (!hasDescription) continue;

                vmXMLProperty newProp = new vmXMLProperty(pInfo);
                UserControl panel = ctrlXMLPropValue.GetValueContent(pInfo);
                if (pInfo.Name == "ElementType")
                {
                    panel = ctrlXMLPropValue.GetValueContent(pInfo, this.ContentType);
                    panel.IsEnabled = false;
                }
                else
                {
                    panel = ctrlXMLPropValue.GetValueContent(pInfo);
                }
                newProp.SetValuePanel(panel);
                this.propertyList.Items.Add(newProp);
            }
        }
        private void BindConFigureList()
        {
            PropertyInfo[] pInfos = this.Material.XMLSets.Heading5Element.Config.GetType().GetProperties();

            this.conFigureList.Items.Clear();
            foreach (PropertyInfo pInfo in pInfos)
            {
                xmlElementTypeAttribute typeAttr = pInfo.GetCustomAttribute(typeof(xmlElementTypeAttribute)) as xmlElementTypeAttribute;
                if (typeAttr == null) continue;
                if (!typeAttr.Types.Contains(eXMLElementType.NONE) && !typeAttr.Types.Contains(this.ContentType)) continue;

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
            xmlElement option = this.Material.XMLSets.Heading5Element;

            foreach (vmXMLProperty propItem in this.propertyList.Items)
            {

                PropertyInfo pInfo = propItem.Origin;
                if (pInfo == null) continue;

                var value = propItem.ValuePanel.GetType().GetProperty("Value").GetValue(propItem.ValuePanel, null);
                pInfo.SetValue(option, value);
            }


            foreach (vmXMLProperty configItem in this.conFigureList.Items)
            {
                PropertyInfo pInfo = configItem.Origin;
                if (pInfo == null) continue;

                var value = configItem.ValuePanel.GetType().GetProperty("Value").GetValue(configItem.ValuePanel, null);
                pInfo.SetValue(option.Config, value);
            }

            this.Material.XMLSets.Heading5Element = option;
        }
    }
}
