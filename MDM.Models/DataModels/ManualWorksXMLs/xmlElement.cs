using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml.Serialization;
using MDM.Models.Attributes;
using System.ComponentModel;
using System.Xml.Linq;

namespace MDM.Models.DataModels.ManualWorksXMLs
{
    public class xmlElement :xmlBase
    {
        [JsonIgnore]
        [XmlElement("content")]
       // [XmlAttribute(typeof(XCData))]
        public string Content { get; set; } = string.Empty;


        [Nullable(true)]
        
        public xmlElementConfig Config { get; set; } = new xmlElementConfig();



        [XmlIgnore]
        [xmlSubProperty("alias")]
        [Nullable(true)]
        [Description("사용자 정의 아이디")]
        public string Alias { get; set; } = string.Empty;

        [XmlIgnore]
        [xmlSubProperty("type")]
        [Nullable(false)]
        [Description("단락 유형")]
        [DefaultValue(eXMLElementType.NONE)]
        public eXMLElementType ElementType { get; set; } = eXMLElementType.NONE;

        public void Duplicate(xmlElement target)
        {
            target.Config = this.Config;
            target.Alias = this.Alias;
            target.ElementType = this.ElementType;
        }
    }

    public class xmlElementConfig
    {
        [xmlElementType(eXMLElementType.NONE)]
        [JsonProperty("export_exclude")]
        [Description("웹 뷰어에서 PDF로 내려받기를 허용하지 않습니다.")]
        public string ExportExclude { get; set; } = string.Empty;





    }



    public class xmlHeading1Config :xmlElementConfig
    {
        [xmlElementType(eXMLElementType.heading1)]
        [JsonProperty("section_view")]
        [Description("절 모습을 설정합니다.")]
        [DefaultValue(eXMLElementSectionView.NONE)]
        public eXMLElementSectionView SectionView { get; set; } = eXMLElementSectionView.NONE;
    }
    public class xmlNormalTextConfig : xmlElementConfig
    {
        [xmlElementType(eXMLElementType.normal, eXMLElementType.note, eXMLElementType.tip, eXMLElementType.caution)]
        [JsonProperty("ignore_step_indent")]
        [Description("단계 단락 이후 들여쓰기를 하지 않습니다.")]
        [DefaultValue(false)]
        public bool IgnoreStepIndent { get; set; } = false;
    }
    public class xmlOrderListConfig : xmlElementConfig
    {

        [xmlElementType(eXMLElementType.ordered_list)]
        [JsonProperty("renumbered")]
        [Description("새로 번호를 시작합니다.")]
        [DefaultValue(true)]
        public bool IsRenumbered { get; set; } = true;
    }
    public class xmlUnOrderListConfig :xmlElementConfig
    { 

    }
    public class xmlTableConfig : xmlElementConfig
    {
        [xmlElementType(eXMLElementType.table)]
        [JsonProperty("table_layout")]
        [Description("표 레이아웃")]
        [DefaultValue(eXMLElementTableLayout.auto)]
        public eXMLElementTableLayout TableLayout { get; set; } = eXMLElementTableLayout.auto;

        /// <summary>
        /// 쉼표로 구분하여 각 열의 너비를 설정합니다. 예) 30%, 200, ?
        /// 단위로 “%”를 사용할 수 있습니다.
        /// 열 너비를 “?”로 설정하면 해당 열 너비를 자동으로 결정합니다.
        /// </summary>
        [xmlElementType(eXMLElementType.table)]
        [JsonProperty("table_layout_setting")]
        [Description("표 레이아웃이 user일 때만 설정합니다.")]
        public string TableLayoutSetting { get; set; } = string.Empty;

        [xmlElementType(eXMLElementType.image, eXMLElementType.table)]
        [JsonProperty("omit_caption")]
        [Description("제목 생략")]
        [DefaultValue(false)]
        public bool OmitCaption { get; set; } = false;
    }
    public class xmlImageConfig : xmlElementConfig
    {

        [xmlElementType(eXMLElementType.image)]
        [JsonProperty("mode")]
        [Description("그림 단락 모드")]
        [DefaultValue(eXLMElementMode.file)]
        public string Mode { get; set; } = eXLMElementMode.file.ToString();

        [xmlElementType(eXMLElementType.image, eXMLElementType.table)]
        [JsonProperty("omit_caption")]
        [Description("제목 생략")]
        [DefaultValue(false)]
        public bool OmitCaption { get; set; } = false;


        [xmlElementType(eXMLElementType.image)]
        [JsonProperty("caption")]
        [Description("그림 제목")]
        public string Caption { get; set; } = string.Empty;


        [xmlElementType(eXMLElementType.image)]
        [JsonProperty("width")]
        [Description("그림 너비(픽셀 단위)")]
        public int Width { get; set; } = 0;

        [xmlElementType(eXMLElementType.image)]
        [JsonProperty("height")]
        [Description("그림 높이(픽셀 단위)")]
        public int Height { get; set; } = 0;

        [xmlElementType(eXMLElementType.image)]
        [JsonProperty("image_float")]
        [Description("그림 Float")]
        [DefaultValue(eXMLElemnetImageFloat.NONE)]
        public eXMLElemnetImageFloat ImageFloat { get; set; } = eXMLElemnetImageFloat.NONE;

        [xmlElementType(eXMLElementType.image)]
        [JsonProperty("prevent_image_resize")]
        [Description("PDF에서 페이지 전환에 따른 그림 크기 줄이지 않기")]
        public bool? CanPreventImageResize { get; set; } = false;
    }
    public class xmlNoteCongif : xmlElementConfig
    {
        [xmlElementType(eXMLElementType.normal, eXMLElementType.note, eXMLElementType.tip, eXMLElementType.caution)]
        [JsonProperty("ignore_step_indent")]
        [Description("단계 단락 이후 들여쓰기를 하지 않습니다.")]
        [DefaultValue(false)]
        public bool IgnoreStepIndent { get; set; } = false;
    }











    public enum eXMLElementType
    {
        NONE=-1,

        [Description("보통")]
        normal,
        [Description("제목 1")]
        heading1,
        [Description("제목 2")]
        heading2,
        [Description("제목 3")]
        heading3,
        [Description("제목 4")]
        heading4,
        [Description("제목 5")]
        heading5,
        [Description("헤드라인")]
        headline,
        [Description("코드")]
        code,
        [Description("명령어")]
        command,
        [Description("줄 바꿈 유지")]
        pre,
        [Description("블럭 인용구")]
        blockquote,
        [Description("권두 명구")]
        epigraph,
        [Description("순서 목록")]
        ordered_list,
        [Description("비순서 목록")]
        unordered_list,
        [Description("정의 목록")]
        definition_list,
        [Description("콜아웃 목록")]
        callout_list,
        [Description("단계")]
        step1,
        [Description("노트")]
        note,
        [Description("팁")]
        tip,
        [Description("주의")]
        caution,
        [Description("교정")]
        proofreading,
        [Description("그림 단락")]
        image,
        [Description("표 단락")]
        table,
        [Description("차례")]
        toc,
        [Description("그림 차례")]
        list_of_figures,
        [Description("표 차례")]
        list_of_tables,
        [Description("코드 차례")]
        list_of_codes,
        [Description("용어집")]
        glossary,
        [Description("찾아보기")]
        index,
        [Description("자동 페이지 나누기")]
        page_break,
        [Description("빈 페이지")]
        blank_page,
        [Description("여백")]
        space,
        [Description("수평선")]
        horizontal_line,
        [Description("Float 하지 않기")]
        clear_float,
        [Description("레이아웃 페이지")]
        layout_page,
        [Description("디자인 페이지")]
        single_design_page,
        [Description("다른 단락 포함하기")]
        include
    }
    public enum eXMLElementTableLayout
    {
        [Description("자동")]
        auto,
        [Description("고정")]
        fix,
        [Description("설정")]
        user,
    }
    public enum eXLMElementMode
    {
        [Description("내 컴퓨터에서 가져오기")]
        file,
        [Description("다른 문서에서 가져오기")]
        reference,
        [Description("웹에서 가져오기")]
        url,
        [Description("웹에서 기타 요소 가져오기")]
        other
    }
    public enum eXMLElementLevel
    {
        [Description("장")]
        CHAPTER_TITLE = 0,
        [Description("제목 1")]
        HEADING1 = 1,
        [Description("제목 2")]
        HEADING2 = 2,
        [Description("제목 3")]
        HEADING3 = 3,
        [Description("제목 4")]
        HEADING4 = 4,
        [Description("제목 4")]
        HEADING5 = 5,
    }
    public enum eXMLElementSortType
    {
        
        letter_by_letter,
        word_by_word
    }
    public enum eXMLElementLayoytType
    {
        [Description("약표제지")]
        half_title_page,
        [Description("표제지")]
        title_page
    }
    public enum  eXMLElementHeadingAdjustment
    {
        [Description("제목을 두 단계 아래로")]
        Down2 = -2,
        [Description("제목을 한 단계 아래로")]
        Donw1 = -1,
        [Description("조정 하지 않음")]
        Not =0,
        [Description("제목을 한 단계 위로")]
        Up1 =1,
        [Description("제목을 두 단계 위로")]
        Up2 = 2

    }
    public enum eXMLElementSectionView
    {
        NONE = -1,
        TAB = 10,
        ACCORDION = 20,
    }
    public enum eXMLElementCodeLanguage
    {
        NONE = -1,
        XML , 
        HTML , 
        CSS , 
        JAVASCRIPT , 
        JSP , 
        C , 
        C_PLUS_PLUS , 
        C_SHARP , 
        JAVA , 
        OBJECTIVE_C , 
        RUBY , 
        PYTHON , 
        SQL
    }
    public enum eXMLElemnetImageFloat
    {
        NONE = -1,
        LEFT = 0,
        RIGHT = 1
    }


}
