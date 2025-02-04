using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.Attributes
{
    public class ColumnHeaderAttribute : Attribute
    {
        public string ColumnValue { get; set; } = string.Empty;


        public ColumnHeaderAttribute(string columnHeader)
        {
            ColumnValue = columnHeader;
        }
    }
}
