using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using MDM.Helpers;
using MDM.Models.Attributes;
using MDM.Models.DataModels.ManualWorksXMLs;
using MDM.Models.ViewModels;
using MDM.Views.MarkChecker.Pages.XMLSettings;
using Microsoft.Office.Core;
using OfficeOpenXml;
using Path = System.IO.Path;

namespace MDM.Views.MarkChecker.Pages
{
    /// <summary>
    /// ucMarCheckerToXml.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucMarCheckerToXml : UserControl
    {
      

        private vmMaterial _Material = null;
        public vmMaterial Material
        {
            get => _Material;
            set
            {
                _Material = value;
                this.ucMarCheckerToXmlAllSetting.Material = value;
                
            }
        }
        public ucMarCheckerToXml()
        {
            InitializeComponent();
        }

        private void treeview_Header_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
          
        }

        private void btn_Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ucMarCheckerToXmlAllSetting.UpdaetOptions();
                xmlSet setValue = this.Material.XMLSets;

                xmlBook book = new xmlBook();// setValues.Book.Copy();
                setValue.Book.Duplicate(book);
                book.Id = this.Material.Temp.Uid;
                if (Guid.TryParse(book.Id, out Guid bookUid)) book.Id = this.Material.Temp.Uid = XMLHelper.GenerateUUId(8);

                {
                    PropertyInfo[] pInfos = book.GetType().GetProperties();
                    foreach (PropertyInfo p in pInfos)
                    {
                        xmlSubPropertyAttribute subPropAtt = p.GetCustomAttribute(typeof(xmlSubPropertyAttribute)) as xmlSubPropertyAttribute;
                        if (subPropAtt == null) continue;

                        var value = p.GetValue(book, null);
                        subPropAtt.Prorperty.Value = ConvertToString(value);
                        book.Properties.Add(subPropAtt.Prorperty);
                    }

                    string jsonString = JsonHelper.ToJsonString(book.Config);
                    xmlSubProperty configProp = new xmlSubProperty("config", jsonString);
                    book.Properties.Add(configProp);
                }
                

                book.Chapters.Clear();
                foreach (vmHeading root in this.Material.RootHeadings)
                {
                    xmlChapter chapter = new xmlChapter();
                    setValue.Chapter.Duplicate(chapter);

                    chapter.Properties.Clear();
                    chapter.Elements.Clear();
                    {
                        book.Chapters.Add(chapter);
                        chapter.Id = root.Temp.Uid;
                        if (Guid.TryParse(chapter.Id, out Guid chapterUid)) chapter.Id = root.Temp.Uid = XMLHelper.GenerateUUId(8);
                        chapter.Title = root.Temp.Name;

                        PropertyInfo[] pInfos = chapter.GetType().GetProperties();
                        foreach (PropertyInfo p in pInfos)
                        {
                            xmlSubPropertyAttribute subPropAtt = p.GetCustomAttribute(typeof(xmlSubPropertyAttribute)) as xmlSubPropertyAttribute;
                            if (subPropAtt == null) continue;

                            var value = p.GetValue(chapter, null);
                            subPropAtt.Prorperty.Value = ConvertToString(value);
                            chapter.Properties.Add(subPropAtt.Prorperty);
                        }

                        string jsonString = JsonHelper.ToJsonString(chapter.Config);
                        xmlSubProperty configProp = new xmlSubProperty("config", jsonString);
                        chapter.Properties.Add(configProp);
                    }
                    
                    foreach (vmContent con in root.Contents)
                    {
                        xmlElement contentElement = new xmlElement();
                        switch (con.Temp.ItemType)
                        {
                            case Commons.Enum.eItemType.Text: this.Material.XMLSets.TextElement.Duplicate(contentElement); break;
                            case Commons.Enum.eItemType.Image: this.Material.XMLSets.ImageElement.Duplicate(contentElement); break;
                            case Commons.Enum.eItemType.Table: this.Material.XMLSets.TableElement.Duplicate(contentElement); break;
                            default: break;
                        }
                        if (contentElement == null) continue;


                        chapter.Elements.Add(contentElement);
                        contentElement.Id = con.Temp.Temp.Uid;
                        if (Guid.TryParse(contentElement.Id, out Guid elementUid)) contentElement.Id = con.Temp.Temp.Uid = XMLHelper.GenerateUUId(8);
                        contentElement.Content = con.Temp.Temp.LineText;

                        PropertyInfo[] pInfos = contentElement.GetType().GetProperties();
                        foreach (PropertyInfo p in pInfos)
                        {
                            xmlSubPropertyAttribute subPropAtt = p.GetCustomAttribute(typeof(xmlSubPropertyAttribute)) as xmlSubPropertyAttribute;
                            if (subPropAtt == null) continue;

                            var value = p.GetValue(contentElement, null);
                            subPropAtt.Prorperty.Value = ConvertToString(value);
                            contentElement.Properties.Add(subPropAtt.Prorperty);
                        }

                        string jsonString = JsonHelper.ToJsonString(contentElement.Config);
                        xmlSubProperty configProp = new xmlSubProperty("config", jsonString);
                        contentElement.Properties.Add(configProp);
                    }

                    foreach (vmHeading item in root.Children)
                    {
                        SetXmlElement(item, chapter);
                    }

                }
       
                string xmlString = ConvertToXML(book);
                string targetPath = Path.Combine(this.Material.DirectoryPath, this.Material.Temp.Name +"_"+ DateTime.Now.ToString("yyyyMMddhhmmss") +".xml");
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                File.WriteAllText(targetPath, xmlString);
                Debug.WriteLine("XML Export!!!!!!");
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private string ConvertToString(object value)
        {
            string output = string.Empty;

            if(value != null)
            {
                if( value is string[])
                {
                    string[] items = value as string[];
                    foreach (var item in items)
                    {
                        output += item;
                        if(item != items.Last()) output += ",";
                    }
                }
                else
                {
                    output = value.ToString();  
                }
            }

            return output;
        }

        private string ConvertToXML(xmlBook book)
        {
            string output = string.Empty;

            XmlSerializer serializer = new XmlSerializer(typeof(xmlBook));
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineOnAttributes = true,
                OmitXmlDeclaration = true // XML 선언을 생략
            };

 

            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(stringWriter, settings))
                {
                    writer.WriteStartElement("book");
                    writer.WriteAttributeString("id", book.Id);

                    writer.WriteStartElement("properties");
                    foreach (var prop in book.Properties)
                    {
                        writer.WriteStartElement("property");
                        writer.WriteAttributeString("name", prop.Name);
                        writer.WriteAttributeString("value", prop.Value);
                        writer.WriteEndElement();
                    }
                    book.UpdateDate = book.CreateDate = DateTime.Now;
                    
                    writer.WriteStartElement("property");
                    writer.WriteAttributeString("name", "creator");
                    writer.WriteAttributeString("value", book.Creator);
                    writer.WriteEndElement();
                    writer.WriteStartElement("property");
                    writer.WriteAttributeString("name", "create_time");
                    writer.WriteAttributeString("value", ((DateTimeOffset)book.CreateDate.Value).ToUnixTimeMilliseconds().ToString());
                    writer.WriteEndElement();
                    writer.WriteStartElement("property");
                    writer.WriteAttributeString("name", "update_time");
                    writer.WriteAttributeString("value", ((DateTimeOffset)book.UpdateDate.Value).ToUnixTimeMilliseconds().ToString());
                    writer.WriteEndElement();


                    writer.WriteEndElement();

                    foreach (var chapter in book.Chapters)
                    {
                        writer.WriteStartElement("chapter");
                        writer.WriteAttributeString("id", chapter.Id);
                        writer.WriteStartElement("properties");
                        foreach (var prop in chapter.Properties)
                        {
                            writer.WriteStartElement("property");
                            writer.WriteAttributeString("name", prop.Name);
                            writer.WriteAttributeString("value", prop.Value);
                            writer.WriteEndElement();
                        }

                        writer.WriteStartElement("property");
                        writer.WriteAttributeString("name", "creator");
                        writer.WriteAttributeString("value", chapter.Creator);
                        writer.WriteEndElement();
                        writer.WriteStartElement("property");
                        writer.WriteAttributeString("name", "create_time");
                        writer.WriteAttributeString("value", ((DateTimeOffset)chapter.CreateDate.Value).ToUnixTimeMilliseconds().ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("property");
                        writer.WriteAttributeString("name", "update_time");
                        writer.WriteAttributeString("value", ((DateTimeOffset)chapter.UpdateDate.Value).ToUnixTimeMilliseconds().ToString());
                        writer.WriteEndElement();


                        writer.WriteEndElement();
                        foreach (var element in chapter.Elements)
                        {
                            writer.WriteStartElement("element");
                            writer.WriteAttributeString("id", element.Id);
                            writer.WriteStartElement("properties");
                            foreach (var prop in element.Properties)
                            {
                                writer.WriteStartElement("property");
                                writer.WriteAttributeString("name", prop.Name);
                                writer.WriteAttributeString("value", prop.Value);
                                writer.WriteEndElement();
                            }

                            writer.WriteStartElement("property");
                            writer.WriteAttributeString("name", "creator");
                            writer.WriteAttributeString("value", element.Creator);
                            writer.WriteEndElement();
                            writer.WriteStartElement("property");
                            writer.WriteAttributeString("name", "create_time");
                            writer.WriteAttributeString("value", ((DateTimeOffset)element.CreateDate.Value).ToUnixTimeMilliseconds().ToString());
                            writer.WriteEndElement();
                            writer.WriteStartElement("property");
                            writer.WriteAttributeString("name", "update_time");
                            writer.WriteAttributeString("value", ((DateTimeOffset)element.UpdateDate.Value).ToUnixTimeMilliseconds().ToString());
                            writer.WriteEndElement();



                            writer.WriteEndElement();
                            writer.WriteStartElement("content");
                            writer.WriteCData(element.Content);
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement(); 
                }

                //serializer.Serialize(stringWriter, book);
                output = stringWriter.ToString();
            }

            return output;
        }

        private void SetXmlElement(vmHeading item, xmlChapter chapter)
        {
            xmlElement element = new xmlElement();
            switch (item.Temp.Level)
            {
                case 2: this.Material.XMLSets.Heading1Element.Duplicate(element); break;
                case 3: this.Material.XMLSets.Heading2Element.Duplicate(element); break;
                case 4: this.Material.XMLSets.Heading3Element.Duplicate(element); break;
                case 5: this.Material.XMLSets.Heading4Element.Duplicate(element); break;
                case 6: this.Material.XMLSets.Heading5Element.Duplicate(element); break;
            }
            if (element == null) return;

            element.Properties.Clear();
            {
                chapter.Elements.Add(element);
                element.Id = XMLHelper.GenerateUUId(8);
                if (Guid.TryParse(element.Id, out Guid headingUid)) element.Id = item.Temp.Uid = XMLHelper.GenerateUUId(8);
                element.Content = item.Temp.Name;

                PropertyInfo[] pInfos = element.GetType().GetProperties();
                foreach (PropertyInfo p in pInfos)
                {
                    xmlSubPropertyAttribute subPropAtt = p.GetCustomAttribute(typeof(xmlSubPropertyAttribute)) as xmlSubPropertyAttribute;
                    if (subPropAtt == null) continue;

                    var value = p.GetValue(element, null);
                    subPropAtt.Prorperty.Value = ConvertToString(value);
                    element.Properties.Add(subPropAtt.Prorperty);
                }

                string jsonString = JsonHelper.ToJsonString(element.Config);
                xmlSubProperty configProp = new xmlSubProperty("config", jsonString);
                element.Properties.Add(configProp);
            }
            


            foreach (vmContent cont in item.Contents)
            {
                xmlElement contentElement = new xmlElement();
                switch (cont.Temp.ItemType)
                {
                    case Commons.Enum.eItemType.Text:
                        this.Material.XMLSets.TextElement.Duplicate(contentElement);
                        break;
                    case Commons.Enum.eItemType.Image:
                        this.Material.XMLSets.ImageElement.Duplicate(contentElement);
                        break;
                    case Commons.Enum.eItemType.Table:
                        this.Material.XMLSets.TableElement.Duplicate(contentElement);
                        break;
                    default:
                        break;
                }
                if (contentElement == null) continue;
                

                chapter.Elements.Add(contentElement);
                contentElement.Id = cont.Temp.Temp.Uid;
                if (Guid.TryParse(contentElement.Id, out Guid elementUid)) contentElement.Id = cont.Temp.Temp.Uid = XMLHelper.GenerateUUId(8);
                contentElement.Content = cont.Temp.Temp.LineText;

                PropertyInfo[] pInfos = contentElement.GetType().GetProperties();
                foreach (PropertyInfo p in pInfos)
                {
                    xmlSubPropertyAttribute subPropAtt = p.GetCustomAttribute(typeof(xmlSubPropertyAttribute)) as xmlSubPropertyAttribute;
                    if (subPropAtt == null) continue;

                    var value = p.GetValue(contentElement);
                    subPropAtt.Prorperty.Value = ConvertToString(value);
                    contentElement.Properties.Add(subPropAtt.Prorperty);
                }

                string jsonString = JsonHelper.ToJsonString(contentElement.Config);
                xmlSubProperty configProp = new xmlSubProperty("config", jsonString);
                contentElement.Properties.Add(configProp);


            }
           

            foreach (vmHeading child in item.Children)
            {
                SetXmlElement(child, chapter);
            }
        }
    }
}
