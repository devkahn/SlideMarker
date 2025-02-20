using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MDM.Models.ViewModels
{
    public partial class vmXMLConfigure : vmViewModelbase
    {
        private PropertyInfo _Origin = null;
    }
    public partial class vmXMLConfigure : vmViewModelbase
    {
        public vmXMLConfigure(PropertyInfo pInfo) 
        {
            this.Origin = pInfo;
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

        public string Description { get; private set; } = string.Empty;
        public UserControl ValuePanel { get; private set; } = null;
    }
    public partial class vmXMLConfigure : vmViewModelbase
    {
        public override void InitializeDisplay()
        {
            DescriptionAttribute descAtt = this.Origin.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (descAtt != null) this.Description = descAtt.Description;
        }

        public override void SetInitialData()
        {
            
        }
        public void SetValuePanel(UserControl panel)
        {
            this.ValuePanel = panel;
        }

        public override object UpdateOriginData()
        {
            return null;
        }
    }
}
