using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.DataModels.ManualWorksXMLs
{
    public class mChapter
    {
        public string Idx { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string AlwaysTop { get; set; } = string.Empty;


        public string CreateDate { get; set; } = string.Empty;
        public string UpdateDate { get; set; } = string.Empty;

        public List<mElement> Elements { get; set; } = new List<mElement>();
    }
}
