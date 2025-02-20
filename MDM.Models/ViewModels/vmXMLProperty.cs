using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MDM.Models.Attributes;
using MDM.Models.DataModels.ManualWorksXMLs;

namespace MDM.Models.ViewModels
{
    public partial class vmXMLProperty
    {
        private PropertyInfo _Origin = null;
    }
    public partial class vmXMLProperty : vmViewModelbase
    {
        public vmXMLProperty(PropertyInfo origin)
        {
            this.Origin = origin;
        }

        public PropertyInfo Origin
        {
            get => _Origin; 
            private set
            {
                _Origin = value;
                InitializeDisplay();
            }
        }
        public xmlSubProperty SubPropery { get; private set; } = null;
        public bool IsNullable { get; private set; } = false;
        public string Description { get; private set; } = string.Empty;

        public UserControl ValuePanel { get; private set; } = null;
    }
    public partial class vmXMLProperty 
    {
        public override void InitializeDisplay()
        {
            xmlSubPropertyAttribute spAtt = this.Origin.GetCustomAttribute(typeof(xmlSubPropertyAttribute)) as xmlSubPropertyAttribute;
            if (spAtt != null) this.SubPropery = spAtt.Prorperty;

            NullableAttribute nullAtt = this.Origin.GetCustomAttribute(typeof(NullableAttribute)) as NullableAttribute;
            if (nullAtt != null) this.IsNullable = nullAtt.Nullalble;

            DescriptionAttribute descAtt = this.Origin.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            if(descAtt != null) this.Description = descAtt.Description;


            
        }

        public override void SetInitialData()
        {
         
        }
        public  void SetValuePanel(UserControl panel)
        {
            this.ValuePanel = panel;
        }
        public override object UpdateOriginData()
        {
            return null;
        }
    }
}
