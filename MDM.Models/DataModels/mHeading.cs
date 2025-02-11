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

        
        public int Level { get; set; } = -1;
        public string Name { get; set; } = string.Empty;
        

    }
}
