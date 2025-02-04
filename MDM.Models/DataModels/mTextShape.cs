using MDM.Commons.Enum;
using MDM.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.DataModels
{
    public class mTextShape : mShape
    {
        public mTextShape()
        {
            this.ShapeType = eShapeType.Text.GetHashCode();
        }


        [ColumnHeader("Text")]
        public string Text { get; set; } = string.Empty;
        
    }
}
