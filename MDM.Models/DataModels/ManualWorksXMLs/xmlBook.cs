using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MDM.Models.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MDM.Models.DataModels.ManualWorksXMLs
{

    public class xmlBook : xmlBase
    {
        [JsonIgnore]
        [XmlElement("chapter")]
        public List<xmlChapter> Chapters { get; set; } = new List<xmlChapter>();

        [XmlIgnore]
        [JsonIgnore]
        public xmlBookConfig Config { get; set; } = new xmlBookConfig();



        [XmlIgnore]
        [xmlSubProperty("author")]
        [Nullable(false)]
        [Description("저자로서 사용자 아이디를 입력")]
        [DefaultValue("DLENC")]
        public string Author { get; set; } = "DLENC";

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
        [xmlSubProperty("edition")]
        [Nullable(true)]
        [Description("에디션")]
        public string Edition { get; set; } = null;

        [XmlIgnore]
        [xmlSubProperty("keywords")]
        [Nullable(true)]
        [Description("키워드")]
        public string[] Keywords { get; set; } = null;

        [XmlIgnore]
        [xmlSubProperty("type")]
        [Nullable(false)]
        [Description("유형으로 대문자 BOOK 또는 ARTICLE을 입력합니다.")]
        [DefaultValue(eXMLBookType.BOOK)]
        public eXMLBookType Type { get; set; } 

        [XmlIgnore]
        [xmlSubProperty("locale")]
        [Nullable(false)]
        [Description("언어로 한국어는 ko, 일본어는 ja, 영어는 en_US를 입력합니다.")]
        [DefaultValue(eXMLLocale.ko)]
        public eXMLLocale Locale { get; set; } 

        [XmlIgnore]
        [xmlSubProperty("tags")]
        [Nullable(true)]
        [Description("라벨을 입력합니다.")]
        public string[] Tags { get; set; } = null;


        

        public void Duplicate(xmlBook target)
        {
            target.Author = this.Author;
            target.Title = this.Title;
            target.SubTitle = this.SubTitle;
            target.Edition = this.Edition;
            target.Keywords = this.Keywords;
            target.Type = this.Type;
            target.Locale = this.Locale;
            target.Tags = this.Tags;

        }
    }


    public class xmlBookConfig
    {
        [JsonProperty("disable_pdf_download")]
        [Description("웹 뷰어에서 PDF 내려받기를 허용하지 않습니다.")]
        [DefaultValue(false)]
        public bool IsDisablePdfDownload { get; set; } = false;

        [JsonProperty("disable_epub_download")]
        [Description("웹 뷰어에서 EPUB 내려받기를 허용하지 않습니다.")]
        [DefaultValue(false)]
        public bool IsDisableEpubDownload { get; set; } = false;

        [JsonProperty("disable_html_download")]
        [Description("웹 뷰어에서 HTML 내려받기를 허용하지 않습니다.")]
        [DefaultValue(false)]
        public bool IsDisableHtmlDownload { get; set; } = false;

        [JsonProperty("use_arabic_for_front_matter")]
        [Description("권두 구성 페이지 번호로 아라비아 숫자를 사용합니다.")]
        [DefaultValue(false)]
        public bool IsUseArabicForFrontMatter { get; set; } = false;


        [JsonProperty("start_chapter_in_odd")]
        [Description("홀수 페이지에서 장을 시작합니다.")]
        [DefaultValue(true)]
        public bool IsStartChapterInOdd { get; set; } = true;

        [JsonProperty("hide_toc")]
        [Description("차례에 특정 제목 단계 아래가 보이지 않도록 합니다.")]
        [DefaultValue(false)]
        public bool IsHideToc { get; set; } = false;

        [JsonProperty("hide_toc_level")]
        [Description("차례에 보여주지 않을 제목 단계를 설정합니다.")]

        public string HideLevelinContext { get; set; } = string.Empty;

        [JsonProperty("unfold_toc")]
        [Description("기본으로 차례에 특정 제목 단계까지 열리도록 합니다.")]
        [DefaultValue(false)]
        public bool IsUnfoldToc { get; set; } = false;

        [JsonProperty("unfold_toc_level")]
        [Description("기본으로 차례에서 열 제목 단계를 설정합니다.")]
        public string UnfoldLevelinContext { get; set; } = string.Empty;

        [JsonProperty("indent_after_heading")]
        [Description("제목 단락 다음에 들여쓰기를 합니다.")]
        [DefaultValue(true)]
        public bool IsIndentAfterHeading { get; set; } = true;

        [JsonProperty("support_comment")]
        [Description("댓글을 지원합니다.")]
        [DefaultValue(false)]
        public bool IsSupportComment { get; set; } = false;

        [JsonProperty("support_feedback")]
        [Description("피드백을 지원합니다.")]
        [DefaultValue(true)]
        public bool IsSupportFeedback { get; set; } = true;

        [JsonProperty("support_sharing")]
        [Description("공유하기를 지원합니다.")]
        [DefaultValue(false)]
        public bool IsSupportSharing { get; set; } = false;

        [JsonProperty("disable_access_of_crawler")]
        [Description("검색 엔진의 접근을 막습니다.")]
        [DefaultValue(true)]
        public bool IsDisableAccessOfCrawler { get; set; } = true;
    }


    public enum eXMLBookType
    {
        [Description("BOOK")]
        BOOK,
        [Description("ARTICLE")]
        ARTICLE
    }
    public enum eXMLLocale
    {
        [Description("한국어")]
        ko,
        [Description("일본어")]
        ja,
        [Description("영어")]
        en_US
    }
}
