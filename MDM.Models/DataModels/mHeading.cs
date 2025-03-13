using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.DataModels
{
    public class mHeading : mModelBase 
    {
        public int MaterialIdx { get; set; } = -1;

        public int HeadintTypeCode { get; set; } = 0;
        public int Level { get; set; } = -1;
        public string Name { get; set; } = string.Empty;

        public List<mHeading> Children { get; set; } = new List<mHeading>();
        public List<mContent> Contents { get; set; } = new List<mContent>();
    }
}
