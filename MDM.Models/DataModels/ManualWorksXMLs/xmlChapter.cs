using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MDM.Models.Attributes;
using Newtonsoft.Json;

namespace MDM.Models.DataModels.ManualWorksXMLs
{
    public class xmlChapter : xmlBase
    {
        [JsonIgnore]
        [XmlElement("element")]
        public List<xmlElement> Elements { get; set; }

        [Nullable(true)]
        [xmlSubProperty("config")]
        public xmlChapterConfig Config { get; set; }


        [XmlIgnore]
        [xmlSubProperty("author")]
        [Nullable(true)]
        [Description("저자로서 사용자 아이디를 입력")]
        [DefaultValue("DLENC")]
        public string Author { get; set; } = "DLENC";

        [XmlIgnore]
        [xmlSubProperty("alias")]
        [Nullable(true)]
        [Description("사용자 정의 아이디")]
        public string Alias { get; set; } = string.Empty;


        [XmlIgnore]
        [xmlSubProperty("title")]
        [Nullable(false)]
        [Description("제목")]
        public string Title { get; set; }

        [XmlIgnore]
        [xmlSubProperty("subtitle")]
        [Nullable(true)]
        [Description("부제목")]
        public string SubTitle { get; set; } = null;

        [XmlIgnore]
        [xmlSubProperty("type")]
        [Nullable(false)]
        [Description("장 유형")]
        public object Type { get; set; } = null;

        [XmlIgnore]
        [xmlSubProperty("always_top")]
        [Nullable(true)]
        [Description("장을 부와 동일한 최상위 단계로 하려면 true입력")]
        [DefaultValue(false)]
        public bool AlwaysTop { get; set; } = false;
    }

    public class xmlChapterConfig
    {
        [JsonProperty("edit_disabled ")]
        [Description("편집할 수 없도록 하기")]
        [DefaultValue(false)]
        public bool IsEditDisabled { get; set; } = false;

        [JsonProperty("renumbered")]
        [Description("장 번호 새로 매기기")]
        [DefaultValue(false)]
        public bool IsRenumbered { get; set; } = false;

        [JsonProperty("export_exclude")]
        [Description("감추기")]
        public string ExportExclude { get; set; } = string.Empty;

        [JsonProperty("hide_toc")]
        [Description("차례에 특정 제목 단계 아래가 보이지 않도록 합니다.")]
        [DefaultValue(false)]
        public bool IsHideToc { get; set; } = false;

        [JsonProperty("hide_toc_level")]
        [Description("차례에 보여주지 않을 제목 단계를 설정합니다.")]
        public string HideLevelinContext { get; set; } = string.Empty;  

        [JsonProperty("hide_chapter_toc")]
        [Description("장 차례 숨기기")]
        [DefaultValue(false)]
        public bool IsHideChapterToc { get; set; } = false;

        [JsonProperty("appendix_part")]
        [Description("부록 장을 위한 부로 설정합니다. 부 장에만 설정합니다.")]
        [DefaultValue(false)]
        public bool IsAppendixPart { get; set; } = false;

        [JsonProperty("section_view")]
        [Description("절 모습을 설정합니다.")]
        [DefaultValue(eXMLChapterSectionView.TAB)]
        public eXMLChapterSectionView SectionView { get; set; } = eXMLChapterSectionView.TAB;

        [JsonProperty("external_url")]
        [Description("웹 뷰어 차례에서 장을 클릭했을 때 이동할 외부 주소를 입력합니다.")]
        public string ExternalUrl { get; set; } = string.Empty;
    }

    public enum eXMLChapterSectionView
    {
        [Description("TAB")]
        TAB,
        [Description("ACCORDION")]
        ACCORDION
    }
}
