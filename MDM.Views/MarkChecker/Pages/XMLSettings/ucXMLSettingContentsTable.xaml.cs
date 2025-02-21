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
    /// ucXMLSettingContentsTable.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucXMLSettingContentsTable : UserControl
    {
        public eXMLElementType ContentType => eXMLElementType.Table;
        public ucXMLSettingContentsTable()
        {
            InitializeComponent();
            BindPropertyList();
            BindConFigureList();
        }

        private void BindPropertyList()
        {
            PropertyInfo[] pInfos = typeof(xmlElement).GetProperties();

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
            PropertyInfo[] pInfos = typeof(xmlElementConfig).GetProperties();

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
    }
}
