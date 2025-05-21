using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Xml;
using System.Xml.Serialization;
using MDM.Models;
using MDM.Models.DataModels;
using MDM.Models.DataModels.ManualWorksXMLs;
using MDM.Models.ViewModels;
using Microsoft.Office.Interop.PowerPoint;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

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
        public static string ConvertListTextToHtml(string input)
        {
            string[] lines = TextHelper.SplitText(input);

            List<mListLine> listLines = new List<mListLine>();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                mListLine newLine = new mListLine(line, i);
                if (newLine.Depth == 1)
                {
                    listLines.Add(newLine);
                }
                else
                {
                    mListLine parentLine = listLines.Where(x => x.Depth < newLine.Depth).OrderBy(x => x.Num).LastOrDefault();
                    if (parentLine != null)
                    {
                        parentLine.Children.Add(newLine);
                    }
                }
            }


            string html = GetHtmlList(listLines);
            return html;
        }
        private static string GetHtmlList(List<mListLine> listLines)
        {
            string html = string.Empty;
            if (listLines == null || listLines.Count() == 0) return string.Empty;

            html += listLines.First().IsOrderedList ? "<ol>" : "<ul>";
            foreach (mListLine item in listLines)
            {
                html += $"<li>{item.LineString.Trim()}</li>";
                html += GetHtmlList(item.Children);
            }
            html += listLines.First().IsOrderedList ? "</ol>" : "</ul>";

            return html;
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

            int cnt = 0;
            foreach (var item in chapter.ChildNodes)
            {
                XmlElement element = item as XmlElement;
                if (element == null) continue;

                
                switch (element.Name)
                {
                    case "properties": SetPorperiesToElement(output, element); break;
                    case "element": output.Elements.Add(element.ToElement(++cnt)); break;
                    default:
                        string eMsg = string.Format("NEW ELEMENT : {0}", element.Name);
                        MessageHelper.ShowErrorMessage("새로운 Element", eMsg);
                        break;
                }
            }

            return output;
        }
        public static mElement ToElement(this XmlElement element, int index)
        {
            mElement output = new mElement();
            output.Order = index;

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


        const string ElementName_properties = "properties";
        const string ElementName_Id = "id";
        public static XmlElement GetBookXmlElement(XmlDocument doc, vmMaterial material)
        {
            XmlElement bookElement = doc.CreateElement("book");
            bookElement.SetAttribute(ElementName_Id, material.Temp.Uid);
            doc.AppendChild(bookElement);

            XmlElement props = doc.CreateElement(ElementName_properties);
            bookElement.AppendChild(props);
            {
                // title
                XmlElement titleProp = GetPropertXmlElement(doc, "title", material.Temp.Name);
                if (titleProp != null) props.AppendChild(titleProp);
                // edition
                XmlElement editionProp = GetPropertXmlElement(doc, "edition", material.Temp.Name.Contains("수행지침") ? "수행지침" : "수행지침");
                if (editionProp != null) props.AppendChild(editionProp);
                // author
                XmlElement authorProp = GetPropertXmlElement(doc, "author", "DLENC");
                if (authorProp != null) props.AppendChild(authorProp);
                // type
                XmlElement typeProp = GetPropertXmlElement(doc, "type", "BOOK");
                if (typeProp != null) props.AppendChild(typeProp);
                // locale
                XmlElement localeProp = GetPropertXmlElement(doc, "locale", "ko");
                // config
                string configValue = GetConfigValue(material.XMLSets.Book.Config);
                XmlElement configProp = GetPropertXmlElement(doc, "config", configValue);
                if (configProp != null) props.AppendChild(configProp);
                // creator
                XmlElement creatorProp = GetPropertXmlElement(doc, "create", "DLENC");
                if (creatorProp != null) props.AppendChild(creatorProp);
                // create_time
                string createValue = GetCreateDateValue(material.Temp.Name);
                XmlElement createTimeProp = GetPropertXmlElement(doc, "create_time", createValue);
                if (createTimeProp != null) props.AppendChild(createTimeProp);
                // update_time
                XmlElement updateTimeProp = GetPropertXmlElement(doc, "update_time", createValue);
                if (updateTimeProp != null) props.AppendChild(updateTimeProp);
            }

            return bookElement;
        }
        public static List<XmlElement> GetChapterElements(XmlDocument doc, vmMaterial material)
        {
            List<XmlElement> output = new List<XmlElement>();

            foreach (vmHeading root in material.RootHeadings)
            {
                if (!root.IsEnabled) continue;

                XmlElement chapterElement = doc.CreateElement("chapter");
                chapterElement.SetAttribute("id", root.Temp.Uid);

                XmlElement props = doc.CreateElement(ElementName_properties);
                chapterElement.AppendChild(props);
                {
                    // title
                    XmlElement titleProp = GetPropertXmlElement(doc, "title", root.Temp.Name);
                    if (titleProp != null) props.AppendChild(titleProp);
                    // type
                    XmlElement typeProp = GetPropertXmlElement(doc, "type", "CHAPTER");
                    if (typeProp != null) props.AppendChild(typeProp);
                    // always_top
                    XmlElement alwaysTopProp = GetPropertXmlElement(doc, "always_top", "false");
                    if (alwaysTopProp != null) props.AppendChild(alwaysTopProp);
                    // config
                    string configValue = GetConfigValue(material.XMLSets.Chapter.Config);
                    XmlElement configProp = GetPropertXmlElement(doc, "config", configValue);
                    if (configProp != null) props.AppendChild(configProp);
                    // creator
                    XmlElement creatorProp = GetPropertXmlElement(doc, "create", "DLENC");
                    if (creatorProp != null) props.AppendChild(creatorProp);
                    // create_time
                    string createValue = GetCreateDateValue(material.Temp.Name);
                    XmlElement createTimeProp = GetPropertXmlElement(doc, "create_time", createValue);
                    if (createTimeProp != null) props.AppendChild(createTimeProp);
                    // update_time
                    XmlElement updateTimeProp = GetPropertXmlElement(doc, "update_time", createValue);
                    if (updateTimeProp != null) props.AppendChild(updateTimeProp);
                }

                List<XmlElement> contentElements = GetContentsElements(doc, root.Contents);
                foreach (XmlElement content in contentElements)
                {
                    chapterElement.AppendChild(content);
                }

                SetChildHeadingElement(doc, chapterElement, root.Children);

                output.Add(chapterElement);
            }

            return output;
        }
        private static void SetChildHeadingElement(XmlDocument doc, XmlElement chapterElement, ReadOnlyObservableCollection<vmHeading> children)
        {
            foreach (vmHeading heading in children)
            {
                if (!heading.IsEnabled) continue;

                if (heading.HeadingType == Commons.Enum.eHeadingType.ExportNote)
                {
                    SetNoteHeadingElement(doc, chapterElement, heading);
                }
                else
                {
                    XmlElement headingElement = doc.CreateElement("element");
                    headingElement.SetAttribute("id", heading.Temp.Uid);
                    chapterElement.AppendChild(headingElement);

                    XmlElement props = doc.CreateElement(ElementName_properties);
                    headingElement.AppendChild(props);
                    {
                        // type
                        int level = heading.Temp.Level - 1;
                        if (level < 1) level = 1;
                        if (level > 5) level = 5;
                        string typeString = $"heading{level}";
                        XmlElement typeProp = GetPropertXmlElement(doc, "type", typeString);
                        if (typeProp != null) props.AppendChild(typeProp);
                        // alias
                        string pageNum = heading.Display_StartNum == null ? "0000" : heading.Display_StartNum.ToString();
                        string aliasValue = $"{heading.Temp.Uid}_{int.Parse(pageNum).ToString("0000")}";
                        XmlElement aliasProp = GetPropertXmlElement(doc, "alias", aliasValue);
                        if (aliasProp != null) props.AppendChild(aliasProp);
                        // config
                        string configValue = GetConfigValue(heading);
                        XmlElement configProp = GetPropertXmlElement(doc, "config", configValue);
                        if (configProp != null) props.AppendChild(configProp);
                        // creator
                        XmlElement creatorProp = GetPropertXmlElement(doc, "create", "DLENC");
                        if (creatorProp != null) props.AppendChild(creatorProp);
                        // create_time
                        string createValue = GetCreateDateValue(heading.ParentMaterial.Temp.Name);
                        XmlElement createTimeProp = GetPropertXmlElement(doc, "create_time", createValue);
                        if (createTimeProp != null) props.AppendChild(createTimeProp);
                        // update_time
                        XmlElement updateTimeProp = GetPropertXmlElement(doc, "update_time", createValue);
                        if (updateTimeProp != null) props.AppendChild(updateTimeProp);
                    }

                    XmlElement cdataElement = doc.CreateElement("content");
                    headingElement.AppendChild(cdataElement);
                    string valueString = heading.Temp.Name;
                    XmlCDataSection section = doc.CreateCDataSection(valueString);
                    cdataElement.AppendChild(section);

                    List<XmlElement> contentElements = GetContentsElements(doc, heading.Contents);
                    foreach (XmlElement content in contentElements)
                    {
                        chapterElement.AppendChild(content);
                    }
                }

                SetChildHeadingElement(doc, chapterElement, heading.Children);
            }
        }
        private static void SetNoteHeadingElement(XmlDocument doc, XmlElement chapterElement, vmHeading heading)
        {
            XmlElement noteElement = doc.CreateElement("element");
            noteElement.SetAttribute("id", heading.Temp.Uid);
            chapterElement.AppendChild(noteElement);

            XmlElement props = doc.CreateElement(ElementName_properties);
            noteElement.AppendChild(props);
            {
                // type
                string typeString = $"note";
                XmlElement typeProp = GetPropertXmlElement(doc, "type", typeString);
                if (typeProp != null) props.AppendChild(typeProp);
                // alias
                string pageNum = heading.Display_StartNum == null ? "0000" : heading.Display_StartNum.ToString();
                string aliasValue = $"{heading.Temp.Uid}_{int.Parse(pageNum).ToString("0000")}";
                XmlElement aliasProp = GetPropertXmlElement(doc, "alias", aliasValue);
                if (aliasProp != null) props.AppendChild(aliasProp);
                // config
                string configValue = GetConfigValue(heading.ParentMaterial.XMLSets.NoteElement.Config);
                XmlElement configProp = GetPropertXmlElement(doc, "config", configValue);
                if (configProp != null) props.AppendChild(configProp);
                // creator
                XmlElement creatorProp = GetPropertXmlElement(doc, "create", "DLENC");
                if (creatorProp != null) props.AppendChild(creatorProp);
                // create_time
                string createValue = GetCreateDateValue(heading.ParentMaterial.Temp.Name);
                XmlElement createTimeProp = GetPropertXmlElement(doc, "create_time", createValue);
                if (createTimeProp != null) props.AppendChild(createTimeProp);
                // update_time
                XmlElement updateTimeProp = GetPropertXmlElement(doc, "update_time", createValue);
                if (updateTimeProp != null) props.AppendChild(updateTimeProp);
            }

            XmlElement content = doc.CreateElement("content");
            noteElement.AppendChild(content);


            string contentString = string.Format("<strong>{0}</strong>", heading.Temp.Name);
            foreach (vmContent subCon in heading.Contents)
            {
                if (!subCon.IsEnable) continue;

                string contentDiv = "<div>";
                switch (subCon.ContentType)
                {
                    case Commons.Enum.eContentType.NormalText:
                        contentDiv += string.Format("<p>{0}</P>", subCon.Temp.Temp.LineText);
                        break;
                    case Commons.Enum.eContentType.OrderList:
                        contentDiv += ConvertListTextToHtml(subCon.Temp.Temp.LineText);
                        break;
                    case Commons.Enum.eContentType.UnOrderList:
                        contentDiv += ConvertListTextToHtml(subCon.Temp.Temp.LineText);
                        break;
                    case Commons.Enum.eContentType.Image:
                        string img = string.Format("<img src=\"/r/image/get/{0}\"/>", subCon.Temp.Temp.LineText);
                        contentDiv += img;
                        break;
                    case Commons.Enum.eContentType.Table:
                        break;
                    default:
                        break;
                }
                contentDiv += "</div>";
                contentString += contentDiv;
            }
            foreach (vmHeading subHeading in heading.Children)
            {
                string output = "<div>";

                output += string.Format("* {0}", subHeading.Temp.Name);
                foreach (vmContent subCon in subHeading.Contents)
                {
                    if (!subCon.IsEnable) continue;

                    string contentDiv = string.Empty;
                    switch (subCon.ContentType)
                    {
                        case Commons.Enum.eContentType.NormalText:
                            contentDiv += string.Format("<p>{0}</P>", subCon.Temp.Temp.LineText);
                            break;
                        case Commons.Enum.eContentType.OrderList:
                            contentDiv += ConvertListTextToHtml(subCon.Temp.Temp.LineText);
                            break;
                        case Commons.Enum.eContentType.UnOrderList:
                            contentDiv += ConvertListTextToHtml(subCon.Temp.Temp.LineText);
                            break;
                        case Commons.Enum.eContentType.Image:
                            xmlImage image = null;
                            if (subCon.ContentType == Commons.Enum.eContentType.Image) image = SetImageElementOption(subCon, subCon.Material.XMLSets.ImageElement);
                            if (image != null) subHeading.ParentMaterial.ImageList.Add(image);
                            string img = string.Format("<img src=\"/r/image/get/{0}\" width=\"{1}\" height=\"{2}\"/>", image.FileName, image.Width > 600 ? 600 : image.Width, "auto");
                            contentDiv += img;
                            break;
                        case Commons.Enum.eContentType.Table:
                            break;
                        default:
                            break;
                    }
                    output += contentDiv;
                }

                output += "</div>";

                //contentString += "\n";
                contentString += output;
            }

            XmlCDataSection section = doc.CreateCDataSection(contentString);
            content.AppendChild(section);
        }
        private static xmlImage SetImageElementOption(vmContent con, xmlElement optionElement)
        {
            string lineString = con.Temp.Temp.LineText;
            string fileName = TextHelper.GetImageFileNameFromMarkdown(lineString);
            if (!fileName.ToLower().EndsWith(".png")) fileName += ".png";

            xmlImage newImage = new xmlImage();

            string imagePath = Path.Combine(con.Material.DirectoryPath, fileName);
            using (Bitmap bitmap = new Bitmap(imagePath))
            {
                int width = bitmap.Width;   // 가로 크기
                int height = bitmap.Height; // 세로 크기

                xmlImageConfig imageConfig = optionElement.Config as xmlImageConfig;
                if (imageConfig == null) imageConfig = new xmlImageConfig();

                newImage.Height = imageConfig.Height = height;
                newImage.Width = imageConfig.Width = width;
                imageConfig.Caption = string.IsNullOrEmpty(con.Temp.Temp.Title) ? TextHelper.GetImageTitleFromMarkdown(con.Temp.Temp.LineText) : con.Temp.Temp.Title;

                con.XmlConfig = imageConfig;
            }


            newImage.Name = fileName;
            newImage.FileName = XMLHelper.GenerateUUId(8);
            newImage.FilePath = imagePath;
            newImage.Size = new FileInfo(imagePath).Length;


            return newImage;

        }

        private static List<XmlElement> GetContentsElements(XmlDocument doc, ReadOnlyObservableCollection<vmContent> contents)
        {
            List<XmlElement> output = new List<XmlElement>();

            foreach (vmContent content in contents)
            {
                if (!content.IsEnable) continue;

                XmlElement contentElement = doc.CreateElement("element");
                contentElement.SetAttribute("id", content.Temp.Temp.Uid);

                XmlElement props = doc.CreateElement(ElementName_properties);
                contentElement.AppendChild(props);
                {
                    // type
                    string typeString = GetTypeValue(content);
                    XmlElement typeProp = GetPropertXmlElement(doc, "type", typeString);
                    if (typeProp != null) props.AppendChild(typeProp);
                    // alias
                    string aliasValue = $"{content.Temp.Temp.Uid}_{int.Parse(content.Display_SlideNum.ToString()).ToString("0000")}";
                    XmlElement aliasProp = GetPropertXmlElement(doc, "alias", aliasValue);
                    if (aliasProp != null) props.AppendChild(aliasProp);

                }

                XmlElement cdataElement = doc.CreateElement("content");
                contentElement.AppendChild(cdataElement);
                string valueString = string.Empty;
                switch (content.ContentType)
                {
                    
                    case Commons.Enum.eContentType.OrderList:
                    case Commons.Enum.eContentType.UnOrderList:
                        valueString = GetListValueString(content);
                        break;
                    case Commons.Enum.eContentType.Image: 
                        xmlImage image = null;
                        if (content.ContentType == Commons.Enum.eContentType.Image) image = SetImageElementOption(content, content.Material.XMLSets.ImageElement);
                        if (image != null)
                        {
                            (content.Material.XMLSets.ImageElement.Config as xmlImageConfig).Width = image.Width;
                            (content.Material.XMLSets.ImageElement.Config as xmlImageConfig).Height = image.Height;
                            content.ParentHeading.ParentMaterial.ImageList.Add(image);
                        }
                        
                        valueString = image.FileName;
                        break;
                    case Commons.Enum.eContentType.Table: 
                        valueString = GetTableValueString(content);
                        
                        break;
                    case Commons.Enum.eContentType.NormalText:
                    default:
                        valueString = TextHelper.CleansingForXML(content.Temp.Temp.LineText);
                        break;
                }

                // config
                string configValue = GetConfigValue(content);
                XmlElement configProp = GetPropertXmlElement(doc, "config", configValue);
                if (configProp != null) props.AppendChild(configProp);
                // creator
                XmlElement creatorProp = GetPropertXmlElement(doc, "create", "DLENC");
                if (creatorProp != null) props.AppendChild(creatorProp);
                // create_time
                string createValue = GetCreateDateValue();
                XmlElement createTimeProp = GetPropertXmlElement(doc, "create_time", createValue);
                if (createTimeProp != null) props.AppendChild(createTimeProp);
                // update_time
                XmlElement updateTimeProp = GetPropertXmlElement(doc, "update_time", createValue);
                if (updateTimeProp != null) props.AppendChild(updateTimeProp);

                XmlCDataSection section = doc.CreateCDataSection(valueString);
                cdataElement.AppendChild(section);

                output.Add(contentElement);
            }

            return output;
        }

        private static string GetListValueString(vmContent content)
        {
            string output = string.Empty;

            string text = content.Temp.Temp.LineText;
            string[] lines = TextHelper.SplitText(text);

            Dictionary<int, mTextLine> lineInfos = new Dictionary<int, mTextLine>();
            for (int i = 0; i < lines.Length; i++)
            {
                string value = lines[i];

                mTextLine newLineItem = new mTextLine();
                newLineItem.LineNumber = i;
                newLineItem.Level = TextHelper.GetLineLevel(value);
                newLineItem.LineText = TextHelper.GetTextWithoutNumericListMark(value);
                newLineItem.Mark = TextHelper.GetManualWorksListMark(value);

                lineInfos.Add(i, newLineItem);
            }

            string preMark = string.Empty;

            foreach (int key in lineInfos.Keys.ToList())
            {
                mTextLine lineItem = lineInfos[key];
                if (TextHelper.IsNoText(lineItem.LineText)) continue;

                string newLine = string.Empty;
                string mark = string.Empty;
                if (lineItem.Mark == "+")
                {
                    for (int i = 0; i < lineItem.Level - 1; i++) mark += preMark;
                    if (lineItem.LineText.First() != '+') lineItem.LineText = lineItem.LineText.Insert(0, "+ ");
                    newLine = $"{mark}{lineItem.LineText}";
                }
                else
                {
                    
                    for (int i = 0; i < lineItem.Level; i++) mark += lineItem.Mark;
                    newLine = $"{mark} {lineItem.LineText}";
                    preMark = lineItem.Mark;
                }
                output += newLine + "\n";
            }
            return output.TrimEnd();
        }
        private static string GetImageValueString(vmContent content)
        {
            string output = string.Empty;

            xmlImage image = null;
            string lineString = content.Temp.Temp.LineText;
            string fileName = TextHelper.GetImageFileNameFromMarkdown(lineString);
            if (!fileName.ToLower().EndsWith(".png")) fileName += ".png";

            xmlImage newImage = new xmlImage();

            string imagePath = Path.Combine(content.Material.DirectoryPath, fileName);
            using (Bitmap bitmap = new Bitmap(imagePath))
            {
                int width = bitmap.Width;   // 가로 크기
                int height = bitmap.Height; // 세로 크기

                xmlImageConfig imageConfig = content.Material.XMLSets.ImageElement.Config as xmlImageConfig;
                if (imageConfig == null) imageConfig = new xmlImageConfig();

                newImage.Height = imageConfig.Height = height;
                newImage.Width = imageConfig.Width = width;
                imageConfig.Caption = string.IsNullOrEmpty(content.Temp.Temp.Title) ? TextHelper.GetImageTitleFromMarkdown(content.Temp.Temp.LineText) : content.Temp.Temp.Title;

                content.XmlConfig = imageConfig;
            }


            newImage.Name = fileName;
            newImage.FileName = XMLHelper.GenerateUUId(8);
            newImage.FilePath = imagePath;
            newImage.Size = new FileInfo(imagePath).Length;


            

            if (image != null) content.Material.ImageList.Add(image);
            output = newImage.FileName;

            return output;
        }
        private static string GetTableValueString(vmContent content)
        {
            string output = content.Temp_TableHTML;
            string pattern = @"!\[\]\((.*?)\)";
            MatchCollection matches = Regex.Matches(output, pattern);

            foreach (Match match in matches)
            {
                string fileName = match.Groups[1].Value;
                string imagePath = Path.Combine(content.Material.DirectoryPath, fileName);
                xmlImage imgInTable = new xmlImage();

                using (Bitmap bitmap = new Bitmap(imagePath))
                {
                    int width = bitmap.Width;   // 가로 크기
                    int height = bitmap.Height; // 세로 크기

                    xmlImageConfig imageConfig = new xmlImageConfig();

                    imgInTable.Height = imageConfig.Height = height;
                    imgInTable.Width = imageConfig.Width = width;
                    imageConfig.Caption = string.IsNullOrEmpty(content.Temp.Temp.Title) ? TextHelper.GetImageTitleFromMarkdown(content.Temp.Temp.LineText) : content.Temp.Temp.Title;
                }

                imgInTable.Name = fileName;
                imgInTable.FileName = XMLHelper.GenerateUUId(8);
                imgInTable.FilePath = imagePath;
                imgInTable.Size = new FileInfo(imagePath).Length;


                string imgHtml = string.Format("<img src=\"/r/image/get/{0}\" width=\"100%\" height=\"auto\"/>", imgInTable.FileName);
                output = output.Replace(match.Value, imgHtml);

                content.Material.ImageList.Add(imgInTable);
            }


            return output;
        }
        private static string GetTypeValue(vmContent content)
        {
            switch (content.ContentType)
            {
                case Commons.Enum.eContentType.OrderList: return "ordered_list";
                case Commons.Enum.eContentType.UnOrderList: return "unordered_list";
                case Commons.Enum.eContentType.Image: return "image";
                case Commons.Enum.eContentType.Table: return "table";
                case Commons.Enum.eContentType.NormalText:
                default:
                    return "normal";
            }
        }

        private static string GetConfigValue(vmHeading heading)
        {
            switch (heading.Temp.Level)
            {
                case 1: return JsonHelper.ToJsonString(heading.ParentMaterial.XMLSets.Heading1Element.Config);
                case 2: return JsonHelper.ToJsonString(heading.ParentMaterial.XMLSets.Heading2Element.Config);
                case 3: return JsonHelper.ToJsonString(heading.ParentMaterial.XMLSets.Heading3Element.Config);
                case 4: return JsonHelper.ToJsonString(heading.ParentMaterial.XMLSets.Heading4Element.Config);
                case 5: return JsonHelper.ToJsonString(heading.ParentMaterial.XMLSets.Heading5Element.Config);
                case 6: return JsonHelper.ToJsonString(heading.ParentMaterial.XMLSets.Heading5Element.Config);
                case 7: return JsonHelper.ToJsonString(heading.ParentMaterial.XMLSets.Heading5Element.Config);
                case 8: return JsonHelper.ToJsonString(heading.ParentMaterial.XMLSets.Heading5Element.Config);
                case 9: return JsonHelper.ToJsonString(heading.ParentMaterial.XMLSets.Heading5Element.Config);
                default: return JsonHelper.ToJsonString(heading.ParentMaterial.XMLSets.Heading1Element.Config);
            }
        }
        private static string GetConfigValue(vmContent content)
        {
            switch (content.ContentType)
            {
                case Commons.Enum.eContentType.NormalText: return JsonHelper.ToJsonString(content.Material.XMLSets.TextElement.Config);
                case Commons.Enum.eContentType.OrderList: return JsonHelper.ToJsonString(content.Material.XMLSets.OrderedListElement.Config);
                case Commons.Enum.eContentType.UnOrderList: return JsonHelper.ToJsonString(content.Material.XMLSets.UnorderedListElement.Config);
                case Commons.Enum.eContentType.Image: return JsonHelper.ToJsonString(content.Material.XMLSets.ImageElement.Config);
                case Commons.Enum.eContentType.Table: return JsonHelper.ToJsonString(content.Material.XMLSets.TableElement.Config);
                default:
                    return JsonHelper.ToJsonString(new xmlNormalTextConfig());
                    
            }
        }
        private static string GetConfigValue(xmlChapterConfig xmlChapterConfig)
        {
            return JsonHelper.ToJsonString(xmlChapterConfig);
        }
        private static string GetConfigValue(xmlBookConfig xmlBookConfig)
        {
            return JsonHelper.ToJsonString(xmlBookConfig);
        }
        private static string GetConfigValue(xmlElementConfig xmlNoteConfig)
        {
            return JsonHelper.ToJsonString(xmlNoteConfig);
        }



        private static string GetCreateDateValue(string name = "")
        {
            string output = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds().ToString();

            if(!string.IsNullOrEmpty(name))
            {
                string[] names = name.Split('_');
                if (names.Length > 1)
                {
                    string date = names.Last();
                    DateTime time = DateTime.Now;
                    if(int.TryParse(date, out int dateNum))
                    {
                        if (date.Length == 8)
                        {
                            string year = date.Substring(0, 4);
                            string month = date.Substring(4, 2);
                            string day = date.Substring(6, 2);
                            time = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                        }
                        else if (date.Length == 6)
                        {
                            string year = date.Substring(0, 2);
                            string month = date.Substring(2, 2);
                            string day = date.Substring(4, 2);
                            time = new DateTime(2000 + int.Parse(year), int.Parse(month), int.Parse(day));
                        }
                    }

                    output = ((DateTimeOffset)time).ToUnixTimeMilliseconds().ToString();
                }
            }

            return output;
        }
        private static XmlElement GetPropertXmlElement(XmlDocument doc,  string propName, string value)
        {
            XmlElement output = doc.CreateElement("property");

            output.SetAttribute("name", propName);  
            output.SetAttribute("value", value);

            return output;
        }
    }
}
