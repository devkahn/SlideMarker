using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDM.Models.DataModels.ManualWorksXMLs;

namespace MDM.Models.Attributes
{
    public class xmlElementTypeAttribute : Attribute
    {
        public eXMLElementType Type { get; set; } = eXMLElementType.Normal;


        public xmlElementTypeAttribute(eXMLElementType type)
        {
            this.Type = type;
        }
    }
    
}
