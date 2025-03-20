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
    /// ucXMLSettingHeading3.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucXMLSettingHeading3 : UserControl
    {
        public eXMLElementType ContentType => eXMLElementType.heading3;
        public vmMaterial Material { get; set; }
        public ucXMLSettingHeading3(vmMaterial material)
        {
            this.Material = material;
            InitializeComponent();
            BindPropertyList();
            BindConFigureList();
        }


        private void BindPropertyList()
        {
            PropertyInfo[] pInfos = this.Material.XMLSets.Heading3Element.GetType().GetProperties();

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
            PropertyInfo[] pInfos = new xmlElementConfig().GetType().GetProperties();// this.Material.XMLSets.Heading3Element.Config.GetType().GetProperties();

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
            xmlElement option = new xmlElement();
            

            foreach (vmXMLProperty property in this.propertyList.Items)
            {

                var value = property.ValuePanel.GetType().GetProperty("Value").GetValue(property.ValuePanel, null);
                option.GetType().GetProperty(property.Origin.Name).SetValue(option, value);
            }

            xmlElementConfig newConfig = new xmlElementConfig();
            foreach (vmXMLProperty config in this.conFigureList.Items)
            {
                var value = config.ValuePanel.GetType().GetProperty("Value").GetValue(config.ValuePanel, null);
                newConfig.GetType().GetProperty(config.Origin.Name).SetValue(newConfig, value);
            }

            option.Config = newConfig;
            this.Material.XMLSets.Heading3Element = option;
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
