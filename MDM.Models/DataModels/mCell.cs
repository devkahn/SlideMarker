using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.DataModels
{
    public class mCell
    {
        public bool IsMerged { get; set; } = false;
        public int RowAddress { get; set; } = -1;
        public int ColumnAddress { get; set; } = -1;    
        public int RowSpan { get; set; } = 1;   
        public int ColumnSpan { get; set; } = 1;
        public string Value { get; set; } = string.Empty;
    }
}
