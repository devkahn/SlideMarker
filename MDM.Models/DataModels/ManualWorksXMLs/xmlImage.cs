using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDM.Models.Attributes;
using System.Xml.Serialization;
using System.ComponentModel;

namespace MDM.Models.DataModels.ManualWorksXMLs
{
    public class xmlImage : xmlBase
    {
        [XmlIgnore]
        [xmlSubProperty("name")]
        [Nullable(false)]
        [Description("그림 이름")]
        public string Name { get; set; }

        [XmlIgnore]
        [xmlSubProperty("filename")]
        [Nullable(false)]
        [Description("파일 이름")]
        public string FileName { get; set; }

        [XmlIgnore]
        [xmlSubProperty("size")]
        [Nullable(false)]
        [Description("byte를 단위로 하는 파일 크기")]
        public long Size { get; set; }

        [XmlIgnore]
        [xmlSubProperty("content_type")]
        [Nullable(false)]
        [Description("그림 파일 유형으로 image/png, image/jpeg, image/gif 만을 지원합니다.")]
        public string ContentType { get; set; } = "image/png";

        [XmlIgnore]
        public string FilePath { get; set; } = string.Empty;
    }
}
