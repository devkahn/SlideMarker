using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MDM.Models.Attributes;

namespace MDM.Models.DataModels
{
    [TableName("t_Item")]
    public class mItem :mModelBase
    {
        [PropOrder(10)]
        [ColumnHeader("ParentItemUid")]
        public string ParentItemUid { get; set; } = string.Empty;

        [PropOrder(11)]
        [ColumnHeader("ParentShapeIdx")]
        public int ParentShapeIdx { get; set; } = -1;

        [PropOrder(12)]
        [ColumnHeader("ParentItemIdx")]
        public int ParentItemIdx { get; set; } = -1;

        [PropOrder(13)]
        [ColumnHeader("ItemOrder")]
        public int Order { get; set; } = 0;

        [PropOrder(14)]
        [ColumnHeader("ItemType")]
        public int ItemType { get; set; } = -1;

        [PropOrder(15)]
        [ColumnHeader("Level")]
        public int Level { get; set; } = 1;

        [PropOrder(16)]
        [ColumnHeader("LineText")]
        public string LineText { get; set; } = string.Empty;

        [PropOrder(17)]
        [ColumnHeader("Title")]
        public string Title { get; set; } = string.Empty;
    }
}
