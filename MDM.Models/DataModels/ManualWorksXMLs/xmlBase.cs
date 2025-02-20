using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MDM.Models.Attributes;

namespace MDM.Models.DataModels.ManualWorksXMLs
{
    public class xmlBase
    {

        [Nullable(false)]
        [XmlAttribute("id")]
        public string Id { get; set; }

        [Nullable(false)]
        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public List<xmlSubProperty> Properties { get; set; }



        [Nullable(true)]
        [xmlSubProperty("creator")]
        [DefaultValue("DLENC")]
        public string Creator { get; set; } = "DLENC";

        [Nullable(true)]
        [xmlSubProperty("create_time")]
        public DateTime? CreateDate { get; set; }

        [Nullable(true)]
        [xmlSubProperty("update_time")]
        public DateTime? UpdateDate { get; set; }
    }

    
}
