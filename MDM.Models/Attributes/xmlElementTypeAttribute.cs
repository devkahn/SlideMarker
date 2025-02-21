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
        public List<eXMLElementType> Types { get; private set; } 


        public xmlElementTypeAttribute(params eXMLElementType[] types)
        {
            this.Types = new List<eXMLElementType>(types);
        }
    }
    
}
