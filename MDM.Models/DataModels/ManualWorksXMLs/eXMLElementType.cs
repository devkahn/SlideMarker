using System.ComponentModel;

namespace MDM.Models.DataModels.ManualWorksXMLs
{
    public enum eXMLElementType
    {
        [Description("보통")]
        normal,
        [Description("제목 1")]
        heading1,
        [Description("제목 2")]
        heading2,[Description("제목 3")]heading3,[Description("제목 4")]heading4,[Description("제목 5")]heading5,[Description("헤드라인")]headline,[Description("코드")]code,[Description("명령어")]command,[Description("줄 바꿈 유지")]pre,[Description("블럭 인용구")]blockquote,[Description("권두 명구")]epigraph,[Description("순서 목록")]ordered_list,[Description("비순서 목록")]unordered_list,[Description("정의 목록")]definition_list,[Description("콜아웃 목록")]callout_list,[Description("단계")]step1,[Description("노트")]note,[Description("팁")]tip,[Description("주의")]caution,[Description("교정")]proofreading,[Description("그림 단락")]image,[Description("표 단락")]table,[Description("차례")]toc,[Description("그림 차례")]list_of_figures,[Description("표 차례")]list_of_tables,[Description("코드 차례")]list_of_codes,[Description("용어집")]glossary,[Description("찾아보기")]index,[Description("자동 페이지 나누기")]page_break,[Description("빈 페이지")]blank_page,[Description("여백")]space,[Description("수평선")]horizontal_line,[Description("Float 하지 않기")]clear_float,[Description("레이아웃 페이지")]layout_page,[Description("디자인 페이지")]single_design_page,[Description("다른 단락 포함하기")]include,
    }

    
}
