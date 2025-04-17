using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using MDM.Models.DataModels;
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


        public static mBook ToBook(this XmlElement book)
        {
            mBook output = new mBook();

            foreach (var item in book.ChildNodes)
            {
                XmlElement element = item as XmlElement;
                if (element == null) continue;

                Dictionary<string, string> attribues = GetAttribute(element);
                if (attribues.ContainsKey("id")) output.Idx = attribues["id"];
                
                switch (element.Name)
                {
                    case "properties": SetPorperiesToElement(output, element); break;
                    case "labels": output.Labels = element.ToLabels(); break;
                    case "chapter":output.Chapters.Add(element.ToChapter()); break;
                    default: break;
                }
                
            }

            return output;
        }

        private static void SetPorperiesToElement(mBook book, XmlElement element)
        {
            Dictionary<string, string> properies = element.ToProperties();

            foreach (string key in properies.Keys)
            {
                string value = properies[key];
                switch (key)
                {
                    case "title": book.Title = value; break;
                    case "edition": book.Edition = value; break;
                    case "author": book.Author = value; break;
                    case "type": book.Type = value; break;
                    case "config": book.Configuration = value; break;
                    case "create_time": book.CreateDate = value; break;
                    case "update_time": book.UpdateDate = value; break;
                    default:
                        break;
                }
            }

        }
        private static void SetPorperiesToElement(mChapter chapter, XmlElement element)
        {
            Dictionary<string, string> properies = element.ToProperties();

            foreach (string key in properies.Keys)
            {
                string value = properies[key];
                switch (key)
                {
                    case "title":chapter.Title = value; break;
                    case "author": chapter.Author = value; break;
                    case "type": chapter.Type = value; break;
                    case "always_top": chapter.AlwaysTop = value; break;
                    case "create_time": chapter.CreateDate = value; break;
                    case "update_time": chapter.UpdateDate = value; break;
                    default:
                        break;
                }
            }

        }
        private static void SetPorperiesToElement(mElement ele, XmlElement element)
        {
            Dictionary<string, string> properies = element.ToProperties();

            foreach (string key in properies.Keys)
            {
                string value = properies[key];
                switch (key)
                {
                    case "creator": break;
                    case "type": ele.Type = value; break;
                    case "config": ele.Configuration = value; break;
                    case "create_time": ele.CreateDate = value; break;
                    case "update_time": ele.UpdateDate = value; break;
                    default:
                        break;
                }
            }

        }

        public static Dictionary<string, string> ToProperties(this XmlElement properies)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();

            foreach (var item in properies.ChildNodes)
            {
                XmlElement prop = item as XmlElement;
                if (prop == null) continue;

                string key = prop.Attributes[0].Value;
                string value = prop.Attributes[1].Value;
                output.Add(key, value);

            }

             return output; 
        }
        public static List<mLabel> ToLabels(this XmlElement labels)
        {
            List<mLabel> output = new List<mLabel>();

            foreach (var item in labels.ChildNodes)
            {
                XmlElement lbl = item as XmlElement;
                if (lbl == null) continue;

                mLabel newLabel = new mLabel();
                foreach (XmlAttribute attr in lbl.Attributes)
                {
                    switch (attr.Name)
                    {
                        case "id":  newLabel.Id = attr.Value; break;
                        case "name": newLabel.Name = attr.Value; break;
                        case "color": newLabel.Color = attr.Value; break;
                        default: break;
                    }
                }
                output.Add(newLabel);
            }

            return output;
        }
        public static mChapter ToChapter(this XmlElement chapter)
        {
            mChapter output = new mChapter();

            foreach (var item in chapter.ChildNodes)
            {
                XmlElement element = item as XmlElement;
                if (element == null) continue;

                switch (element.Name)
                {
                    case "properties": SetPorperiesToElement(output, element); break;
                    case "element": output.Elements.Add(element.ToElement()); break;
                    default:
                        string eMsg = string.Format("NEW ELEMENT : {0}", element.Name);
                        MessageHelper.ShowErrorMessage("새로운 Element", eMsg);
                        break;
                }
            }

            return output;
        }
        public static mElement ToElement(this XmlElement element)
        {
            mElement output = new mElement();

            Dictionary<string, string> attribues = GetAttribute(element);
            if (attribues.ContainsKey("id")) output.Idx = attribues["id"];


            foreach (var item in element.ChildNodes)
            {
                XmlElement child = item as XmlElement;
                if (child == null) continue;
                
                switch (child.Name)
                {
                    case "properties": SetPorperiesToElement(output, child); break;
                    case "content": output.Value = child.ToContent(); break;
                    default:
                        string eMsg = string.Format("NEW ELEMENT : {0}", child.Name);
                        MessageHelper.ShowErrorMessage("새로운 Element", eMsg);
                        break;
                }
            }

            return output;
        }

        private static Dictionary<string, string> GetAttribute(XmlElement child)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();

            foreach (var item in child.Attributes)
            {
                XmlAttribute attr = item as XmlAttribute;
                if (attr == null) continue;

                string key = attr.Name;
                string value = attr.Value;
                output.Add(key, value);
            }

            return output;
        }
        private static string ToContent(this XmlElement content)
        {
            string value = content.InnerText;
            return value; 
        }
    }
}
