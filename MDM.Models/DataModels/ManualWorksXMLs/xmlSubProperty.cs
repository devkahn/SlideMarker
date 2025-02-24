using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MDM.Models.DataModels.ManualWorksXMLs
{
    public class xmlSubProperty
    {

        public xmlSubProperty()
        {

        }
        public xmlSubProperty(string name)
        {
            Name = name;
        }
        public xmlSubProperty(string name, string value)
        {
            Name = name;
            Value = value;
        }

        [XmlAttribute("name")]
        public string Name { get; set; } = string.Empty;

        [XmlAttribute("value")]
        public string Value { get; set; } = string.Empty;
    }
}
