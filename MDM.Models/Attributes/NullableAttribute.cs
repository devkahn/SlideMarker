using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.Attributes
{
    public class NullableAttribute : Attribute
    {
        public bool Nullalble { get; set; } = false;

        public NullableAttribute(bool isNullable)
        {
            this.Nullalble = isNullable;
        }
    }
    
}
