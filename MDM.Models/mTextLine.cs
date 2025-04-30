using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models
{
    public class mTextLine
    {
        public int LineNumber { get; set; } = -1;
        public int Level { get; set; } = 0;
        public string Mark { get; set; }
        public string LineText { get; set; } = string.Empty;
    }
}
