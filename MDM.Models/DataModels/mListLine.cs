using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.DataModels
{
    public class mListLine
    {
        public mListLine(string line, int num)
        {
            this.Num = num;

            int hashCount = line.TakeWhile(x => x == '#').Count();
            int starCount = line.TakeWhile(x => x == '*').Count();
            this.IsOrderedList = hashCount == 0 ? false : true;
            if(this.IsOrderedList)
            {
                this.Depth = hashCount;
            }
            else
            {
                this.Depth = starCount;
            }

            this.LineString = this.IsOrderedList? line.TrimStart('#').Trim() :  line.TrimStart('*').Trim();

            this.Children = new List<mListLine>();
        }

        public int Num { get; set; } = 0;
        public int Depth { get; set; } = 1;
        public bool IsOrderedList { get; set; } = true;
        public string LineString { get; set; } = string.Empty;
        public List<mListLine> Children { get; set; } = null;
             
    }
}
