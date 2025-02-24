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
    /// ucXMLSettingContentsImages.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucXMLSettingContentsImages : UserControl
    {
        public eXMLElementType ContentType => eXMLElementType.Image;
        public vmMaterial Material { get; set; }

        public ucXMLSettingContentsImages(vmMaterial material)
        {
            this.Material = material;
            InitializeComponent();
            BindPropertyList();
            BindConFigureList();
        }



        private void BindPropertyList()
        {
            PropertyInfo[] pInfos = this.Material.XMLSets.ImageElement.GetType().GetProperties();

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
            PropertyInfo[] pInfos = this.Material.XMLSets.ImageElement.Config.GetType().GetProperties();

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
            xmlElement imageOption = this.Material.XMLSets.ImageElement;

            foreach (vmXMLProperty propItem in this.propertyList.Items)
            {

                PropertyInfo pInfo = propItem.Origin;
                if (pInfo == null) continue;

                var value = propItem.ValuePanel.GetType().GetProperty("Value").GetValue(propItem.ValuePanel, null);
                pInfo.SetValue(imageOption, value);
            }


            foreach (vmXMLProperty configItem in this.conFigureList.Items)
            {
                PropertyInfo pInfo = configItem.Origin;
                if (pInfo == null) continue;

                var value = configItem.ValuePanel.GetType().GetProperty("Value").GetValue(configItem.ValuePanel, null);
                pInfo.SetValue(imageOption.Config, value);
            }

            this.Material.XMLSets.ImageElement = imageOption;
        }

        private string ArrayValueSerialize(object value)
        {
            string output = string.Empty;

            string[] arrayValue = value as string[];
            foreach (var item in arrayValue)
            {
                output += item;
                if (arrayValue.Last() != item) output += ",";
            }

            return output;
        }

    }
}
