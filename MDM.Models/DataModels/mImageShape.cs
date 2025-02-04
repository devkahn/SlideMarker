using MDM.Commons.Enum;
using MDM.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.DataModels
{
    public class mImageShape : mShape
    {
        public mImageShape()
        {
            this.ShapeType = eShapeType.Image.GetHashCode();
        }



        [ColumnHeader("ImageName")]
        public string ImageName { get; set; } = Guid.NewGuid().ToString();

        [ColumnHeader("Extension")]
        public string Extension { get; set; } = ".png";
        
    }
}
