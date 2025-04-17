using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.DataModels.ManualWorksXMLs
{
    public class mBook
    {
        public string Idx { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Edition { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;  
        public string Type { get; set; } = string.Empty;
        public string Configuration { get; set; } = string.Empty;
        public string CreateDate { get; set; } = string.Empty;  
        public string UpdateDate { get; set; } = string.Empty;  

        public List<mLabel> Labels { get; set; } = new List<mLabel>();
        public List<mChapter> Chapters { get; set; } = new List<mChapter>();

    }
}
