using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.DataModels.ManualWorksXMLs
{
    public class mElement
    {
        public string Idx { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        public int Order { get; set; } = -1;
        public string Value { get; set; } = string.Empty;
        public string Configuration { get; set; } = string.Empty;
        public string CreateDate { get; set; } = string.Empty;
        public string UpdateDate { get; set; } = string.Empty;
    }
}
