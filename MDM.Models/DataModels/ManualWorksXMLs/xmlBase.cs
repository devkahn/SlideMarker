using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
        public List<xmlSubProperty> Properties { get; set; } = new List<xmlSubProperty>();


        [XmlIgnore]
        [Nullable(true)]
        [DefaultValue("DLENC")]
        public string Creator { get; set; } = "DLENC";

        [XmlIgnore]
        [Nullable(true)]
        public DateTime? CreateDate { get; set; } = DateTime.Now;

        [XmlIgnore]
        [Nullable(true)]
        public DateTime? UpdateDate { get; set; } = DateTime.Now;


        //public T Copy<T>() where T : new()
        //{
        //    //T output = new T();
        //    T output = Activator.CreateInstance<T>();

        //    PropertyInfo[] prorperties = typeof(T).GetProperties();
        //    foreach (var property in prorperties)
        //    {
        //        if (property.CanRead && property.CanWrite)
        //        {
        //            property.SetValue(output, property.GetValue(this));
        //        }
        //    }

        //    return output;
        //}
    }

    
}
