using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MDM.Models.DataModels.ManualWorksXMLs;
using MDM.Models.ViewModels;

namespace MDM.Helpers
{
    public static class XMLHelper
    {
        public static string ToXML(this xmlBook obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(xmlBook));


            return string.Empty;
        }


        public static string GenerateUUId(int byteNum)
        {
            byte[] randomBytes = new byte[byteNum];
            RandomNumberGenerator.Create().GetBytes(randomBytes);

            return BitConverter.ToString(randomBytes).Replace("-", "").ToLower();
        }

        public static string ConvertContents(string value)
        {
            return string.Format("<![CDATA[{0}]]>", value);
        }
    }
}
