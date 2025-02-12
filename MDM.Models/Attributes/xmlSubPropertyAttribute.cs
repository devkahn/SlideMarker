using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDM.Models.DataModels.ManualWorksXMLs;

namespace MDM.Models.Attributes
{
    public class xmlSubPropertyAttribute : Attribute
    {
        public xmlSubProperty prorperty { get; set; } = null;


        public xmlSubPropertyAttribute(string name)
        {
            xmlSubProperty newProp = new xmlSubProperty();
            newProp.Name = name;
            this.prorperty = newProp;
        }
    }
}
