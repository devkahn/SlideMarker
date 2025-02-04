using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDM.Models.Attributes;
using Microsoft.Office.Interop.PowerPoint;
using Newtonsoft.Json;

namespace MDM.Models.DataModels
{
    [TableName("t_Slide")]
    public class mSlide :mModelBase
    {
        [PropOrder(11)]
        [ColumnHeader("MaterialId")]
        public int MaterialId { get; set; } = -1;

        [PropOrder(12)]
        [ColumnHeader("SlideId")]
        public int SlideId { get; set; } = -1;

        [PropOrder(13)]
        [ColumnHeader("SlideIndex")]
        public int Index { get; set; } = -1;

        [PropOrder(14)]
        [ColumnHeader("Name")]
        public string Name { get; set; } = string.Empty;

        [PropOrder(15)]
        [ColumnHeader("SlideNumber")]
        public int SlideNumber { get; set; } = -1;

        [PropOrder(16)]
        [ColumnHeader("Status")]
        public int Status { get; set; } = -1;

        [PropOrder(17)]
        [ColumnHeader("Description")]
        public string Description { get; set; } = string.Empty;

        [PropOrder(18)]
        [JsonProperty("Textshapes")]
        public List<mShape> Shapes { get; set; } = new List<mShape>();

        [JsonIgnore]
        public Slide Origin { get; set; } = null;
    }
}
