using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.Attributes
{
    public class TableNameAttribute : Attribute
    {
        public string TableName { get; set; } = string.Empty;


        public TableNameAttribute(string tablename)
        {
            TableName = tablename;
        }
    }
    
}
