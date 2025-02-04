using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.Attributes
{
    public class PropOrderAttribute : Attribute
    {
        public int PropOrder { get; set; } = 0;


        public PropOrderAttribute(int order)
        {
            PropOrder = order;
        }
    }
}
