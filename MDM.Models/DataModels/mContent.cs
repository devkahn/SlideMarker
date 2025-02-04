using MDM.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Models.DataModels
{
    [TableName("t_Content")]
    public class mContent : mModelBase
    {
        [PropOrder(11)]
        [ColumnHeader("MaterialIdx")]
        public int MaterialIdx { get; set; } = -1;

        [PropOrder(12)]
        [ColumnHeader("SlideIdx")]
        public int SlideIdx { get; set; } =  -1;

        [PropOrder(13)]
        [ColumnHeader("Heading1Idx")]
        public int Heading1Idx { get; set; } = -1;

        [PropOrder(14)]
        [ColumnHeader("Heading2Idx")]
        public int Heading2Idx { get; set; } = -1;

        [PropOrder(15)]
        [ColumnHeader("Heading3Idx")]
        public int Heading3Idx { get; set; } = -1;

        [PropOrder(16)]
        [ColumnHeader("Heading4Idx")]
        public int Heading4Idx { get; set; } = -1;

        [PropOrder(17)]
        [ColumnHeader("Heading5Idx")]
        public int Heading5Idx { get; set; } = -1;

        [PropOrder(18)]
        [ColumnHeader("Heading6Idx")]
        public int Heading6Idx { get; set; } = -1;

        [PropOrder(19)]
        [ColumnHeader("Heading7Idx")]
        public int Heading7Idx { get; set; } = -1;

        [PropOrder(20)]
        [ColumnHeader("Heading8Idx")]
        public int Heading8Idx { get; set; } = -1;

        [PropOrder(21)]
        [ColumnHeader("Heading9Idx")]
        public int Heading9Idx { get; set; } = -1;

        [PropOrder(22)]
        [ColumnHeader("Heading10Idx")]
        public int Heading10Idx { get; set; } = -1;

        [PropOrder(23)]
        [ColumnHeader("Contents")]
        public string Contents { get; set; } = string.Empty;
    }
}
