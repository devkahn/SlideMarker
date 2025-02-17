using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MDM.Commons.Enum
{
    public enum eItemType
    {
        [Description("-")]
        None = -1,

        [Description("전체")]
        All = 0,

        [Description("제목")]
        Header = 210,

        [Description("본문")]
        Content = 220,
        [Description("글")]
        Text = 221,
        [Description("그림")]
        Image = 222,
        [Description("표")]
        Table = 223,
    }

    public enum eShapeType
    {
        [Description("-")]
        None = -1,

        [Description("전체")]
        All = 0,

        [Description("글")]
        Text = 221,
        [Description("그림")]
        Image = 222,
        [Description("표")]
        Table = 223,
    }

    public enum ePageStatus
    {
        [Description("미완성")]
        None = -1,

        [Description("전체")]
        All = 0,

        [Description("완료")]
        Completed = 100,
        [Description("보류")]
        Hold = 110,
        [Description("예외")]
        Exception = 120,

        [Description("저장됨")]
        Saved = 910,
        [Description("변경됨")]
        Changed = 999
    }

    public enum eFILE_TYPE
    {
        [Description("모든 파일|*.*")]
        None,

        [Description("Excel 통합 문서|*.xlsx;*.xlsm;*.xlsb;*.xls;*.xltx;*.xltm;*.xlt;*.csv;*.txt;*.xml;*.ods;*.dbf;*.tab")]
        MSExcel,

        [Description("MPP 파일|*.mpp")]
        MSProject
    }
}
  
