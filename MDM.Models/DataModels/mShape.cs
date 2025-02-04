using MDM.Commons.Enum;
using MDM.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.DataModels
{
    [TableName("t_Shape")]
    public class mShape : mModelBase
    {
        public mShape() 
        {
            this.ShapeType = eShapeType.None.GetHashCode();
        }
        public mShape(eShapeType shapeType)
        {
            this.ShapeType = shapeType.GetHashCode();
            this.Text = Guid.NewGuid().ToString();  
        }

        [PropOrder(11)]
        [ColumnHeader("SlideId")]
        public int ParentSlideIdx { get; set; } = -1;

        [PropOrder(12)]
        [ColumnHeader("ShapeType")]
        public int ShapeType { get; set; }

        [PropOrder(13)]
        [ColumnHeader("ShapeId")]
        public int ShapeId { get; set; } = -1;

        [PropOrder(14)]
        [ColumnHeader("AlternativeText")]
        public string AlternativeText { get; set; } = string.Empty;

        [PropOrder(15)]
        [ColumnHeader("Title")]
        public string Title { get; set; } = string.Empty;

        [PropOrder(16)]
        [ColumnHeader("Name")]
        public string Name { get; set; } = string.Empty;

        [PropOrder(17)]
        [ColumnHeader("Left")]
        public float Left { get; set; } = -1;

        [PropOrder(18)]
        [ColumnHeader("Top")]
        public float Top { get; set; } = -1;

        [PropOrder(19)]
        [ColumnHeader("Width")]
        public float Width { get; set; } = -1;

        [PropOrder(20)]
        [ColumnHeader("Height")]
        public float Height { get; set; } = -1;

        [PropOrder(21)]
        [ColumnHeader("DistanceFromOrigin")]
        public double DistanceFromOrigin { get; set; } = -1;

        [PropOrder(22)]
        [ColumnHeader("Text")]
        public string Text { get; set; } = string.Empty;

        [PropOrder(23)]
        [ColumnHeader("DataTable")]
        public string DataTable { get; set; } = string.Empty;

        [PropOrder(24)]
        [ColumnHeader("TableHeaderRowCount")]
        public int TableHeaderRowCount { get; set; } = 1;



        public List<mItem> Lines { get; set; } = new List<mItem>();
    }
}
