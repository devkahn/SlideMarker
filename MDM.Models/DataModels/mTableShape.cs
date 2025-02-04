using MDM.Commons.Enum;
using MDM.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.DataModels
{
    public class mTableShape : mShape
    {
        public mTableShape()
        {
            this.ShapeType = eShapeType.Table.GetHashCode();
        }


        [ColumnHeader("Table")]
        public DataTable Table { get; set; } = null;
    }
}
