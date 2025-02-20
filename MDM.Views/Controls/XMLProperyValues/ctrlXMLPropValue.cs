using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MDM.Models.DataModels.ManualWorksXMLs;

namespace MDM.Views.Controls.XMLProperyValues
{
    public static class ctrlXMLPropValue
    {
        public static UserControl GetValueContent(PropertyInfo pInfo)
        {

            object defaultValue = null;
            DefaultValueAttribute dvAttr = pInfo.GetCustomAttribute(typeof(DefaultValueAttribute)) as DefaultValueAttribute;
            if (dvAttr != null) defaultValue = dvAttr.Value;

            string typeName = pInfo.PropertyType.Name;
            if (typeName == typeof(string).Name) return new ctrlXMLPropValueString(defaultValue);
            if (typeName == typeof(eXMLBookType).Name) return new ctrlXMLPropValueSelection(defaultValue);
            if (typeName == typeof(eXMLLocale).Name) return new ctrlXMLPropValueSelection(defaultValue);
            if (typeName == typeof(eXMLChapterSectionView).Name) return new ctrlXMLPropValueSelection(defaultValue);
            if (typeName == typeof(string[]).Name) return new ucXMLPropValueAddableList(defaultValue);
            if (typeName == typeof(bool).Name) return new ctrlXMLPropValueBool(defaultValue);
            if (typeName == typeof(bool?).Name) return new ctrlXMLPropValueBool(defaultValue);
            if (typeName == typeof(int).Name) return new ctrlXMLPropValueDigit(defaultValue);
            if (typeName == typeof(int?).Name) return new ctrlXMLPropValueDigit(defaultValue);

            return null;
        }
    }
}
