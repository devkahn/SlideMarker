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
        public eXMLElementType[] Types { get; set; } = { eXMLElementType.Normal };


        public xmlElementTypeAttribute(params eXMLElementType[] types)
        {
            foreach (eXMLElementType item in types)
            {
                this.Types.Append(item);
            }
        }
    }
    
}
