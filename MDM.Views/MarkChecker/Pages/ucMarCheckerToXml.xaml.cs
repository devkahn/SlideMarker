using Markdig;
using MDM.Helpers;
using MDM.Models.Attributes;
using MDM.Models.DataModels;
using MDM.Models.DataModels.ManualWorksXMLs;
using MDM.Models.ViewModels;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using Clipboard = System.Windows.Clipboard;
using Path = System.IO.Path;
using UserControl = System.Windows.Controls.UserControl;

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


        List<xmlImage> ImageList = new List<xmlImage>();
        private void btn_Export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                ImageList = new List<xmlImage>();
                this.ucMarCheckerToXmlAllSetting.UpdaetOptions();
                xmlSet option = this.Material.XMLSets;
                
                XmlDocument xmlDoc = new XmlDocument();
                XmlDeclaration xmlDecl = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                xmlDoc.AppendChild(xmlDecl);

                XmlElement bookElement = xmlDoc.CreateElement("book");
                xmlDoc.AppendChild(bookElement);
                SetBookProperties(xmlDoc, bookElement, option.Book);

                XmlElement chapterElement = xmlDoc.CreateElement("chapter");
                bookElement.AppendChild(chapterElement);
                SetChapterProperties(xmlDoc, chapterElement, option.Chapter);

                foreach (vmHeading rootHeading in this.Material.RootHeadings)
                {
                    SetXmlElement(rootHeading,xmlDoc, chapterElement, option);
                }


                #region 이전 코드
                //xmlBook bookElement = GetBookElement(option);
                //bookElement.Title = this.Material.Temp.Name;

                //if(bookElement.Type == eXMLBookType.BOOK)
                //{
                //    SetChildrenBookType(bookElement, option);
                //}
                //else
                //{
                //    SetChildrenArticleType(bookElement, option);
                //}


                //string xmlString = ConvertToXML(bookElement);
                #endregion

                string folderName = DateTime.Now.ToString("yyyyMMddHHmmss");
                string folderPath = Path.Combine(this.Material.DirectoryPath, string.Format("{0}_{1}", "book", folderName));
                if(!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                string bookFolderPath = Path.Combine(folderPath, "book");
                if (!Directory.Exists(bookFolderPath)) Directory.CreateDirectory(bookFolderPath);
                string bookFileName = string.Format("{0}.xml", bookElement.GetAttribute("id"));
                string bookTargetPath = Path.Combine(bookFolderPath, bookFileName);
                using (StreamWriter writer = new StreamWriter(bookTargetPath, false, Encoding.UTF8)) xmlDoc.Save(writer);

                string imageFolderPath = Path.Combine(folderPath, "image");
                if (!Directory.Exists(imageFolderPath)) Directory.CreateDirectory(imageFolderPath);


                string createDateString = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds().ToString();
                XmlDocument imageDoc = new XmlDocument();
                XmlDeclaration imageXmlDecl = imageDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                imageDoc.AppendChild(imageXmlDecl);
                XmlElement imagesElement = imageDoc.CreateElement("images");
                imageDoc.AppendChild(imagesElement);
                foreach (xmlImage image in this.ImageList)
                {
                    XmlElement imageElement = imageDoc.CreateElement("image");
                    imageElement.SetAttribute("id", image.FileName);
                    imagesElement.AppendChild(imageElement);    

                    XmlElement prorps = imageDoc.CreateElement("properties");
                    imageElement.AppendChild(prorps);

                    PropertyInfo[] pInfos = image.GetType().GetProperties();
                    foreach (PropertyInfo p in pInfos)
                    {
                        xmlSubPropertyAttribute subPropAtt = p.GetCustomAttribute(typeof(xmlSubPropertyAttribute)) as xmlSubPropertyAttribute;
                        if (subPropAtt == null) continue;

                        var value = p.GetValue(image, null);
                        string valueString = ConvertToString(value);//.ToLower();

                        XmlElement xmlProp = imageDoc.CreateElement("property");
                        xmlProp.SetAttribute("name", subPropAtt.Prorperty.Name);
                        xmlProp.SetAttribute("value", valueString);
                        prorps.AppendChild(xmlProp);
                    }

                    {
                        XmlElement xmlProp = imageDoc.CreateElement("property");
                        xmlProp.SetAttribute("name", "creator");
                        xmlProp.SetAttribute("value", "DLENC");
                        prorps.AppendChild(xmlProp);
                    }
                    {
                        
                        XmlElement xmlProp = imageDoc.CreateElement("property");
                        xmlProp.SetAttribute("name", "create_time");
                        xmlProp.SetAttribute("value", createDateString);
                        prorps.AppendChild(xmlProp);
                    }

                    string targetFileName = Path.Combine(imageFolderPath, image.FileName);
                    File.Copy(image.FilePath, targetFileName);
                }

                string imageFileName = string.Format("{0}.xml", "images");
                string imageTargetPath = Path.Combine(imageFolderPath, imageFileName);
                using (StreamWriter writer = new StreamWriter(imageTargetPath, false, Encoding.UTF8)) imageDoc.Save(writer);

                CheckBox cb = btn.Tag as CheckBox;
                if (cb != null && cb.IsChecked.HasValue && cb.IsChecked.Value)
                {
                    string targetName = string.Format("{0}_{1}.zip", "book", folderName);
                    string targetPath = Path.Combine(this.Material.DirectoryPath, targetName);
                    ZipFile.CreateFromDirectory(folderPath, targetPath);
                }

                Debug.WriteLine("XML Export!!!!!!");
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }



        private void SetXmlElement(vmHeading heading, XmlDocument xmlDoc, XmlElement chapterElement, xmlSet option)
        {
            XmlElement headingElement = xmlDoc.CreateElement("element");
            chapterElement.AppendChild(headingElement);

            if(heading.HeadingType == Commons.Enum.eHeadingType.ExportNote)
            {
                int slideNum = heading.Contents.Any() ? heading.Contents.Min(x => int.Parse(x.Display_SlideNum.ToString())) : 0;
                SetContentProperties(slideNum , xmlDoc, headingElement, option.NoteElement);

                //Config
                {
                    XmlElement xmlProp = xmlDoc.CreateElement("property");
                    xmlProp.SetAttribute("name", "config");
                    xmlProp.SetAttribute("value", JsonHelper.ToJsonString(option.NoteElement.Config));
                    headingElement.GetElementsByTagName("properties")[0].AppendChild(xmlProp);
                }

                XmlElement content = xmlDoc.CreateElement("content");
                headingElement.AppendChild(content);
                
                string contentString = string.Format("<strong>{0}</strong>", heading.Temp.Name);

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
                                contentDiv +=  ConvertListTextToHtml(subCon.Temp.Temp.LineText);
                                break;
                            case Commons.Enum.eContentType.UnOrderList:
                                contentDiv += ConvertListTextToHtml(subCon.Temp.Temp.LineText);
                                break;
                            case Commons.Enum.eContentType.Image:
                                xmlImage image = null;
                                if (subCon.ContentType == Commons.Enum.eContentType.Image) image = SetImageElementOption(subCon, option.ImageElement);
                                if (image != null) this.ImageList.Add(image);
                                string img = string.Format("<img src=\"/r/image/get/{0}\" width=\"{1}\" height=\"{2}\"/>", image.FileName, image.Width >600? 600 : image.Width, "auto");
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


                XmlCDataSection section = xmlDoc.CreateCDataSection(contentString);
                content.AppendChild(section);
            }
            else
            {
                xmlElement headingOption = null;
                switch (heading.Temp.Level)
                {
                    case 1: 
                        headingOption = option.Heading1Element;
                        headingOption.ElementType = eXMLElementType.heading1;
                        break;
                    case 2: 
                        headingOption = option.Heading2Element;
                        headingOption.ElementType = eXMLElementType.heading2;
                        break;
                    case 3: 
                        headingOption = option.Heading3Element;
                        headingOption.ElementType = eXMLElementType.heading3;
                        break;
                    case 4:
                        headingOption = option.Heading4Element;
                        headingOption.ElementType = eXMLElementType.heading4;
                        break;
                    case 5: 
                        headingOption = option.Heading5Element;
                        headingOption.ElementType = eXMLElementType.heading5;
                        break;
                    default: 
                        headingOption = option.TextElement;
                        headingOption.ElementType = eXMLElementType.normal;
                        break;
                }


                int slideNum = heading.Contents.Any() ? heading.Contents.Min(x => int.Parse(x.Display_SlideNum.ToString())) : 0;
                SetHeadingProperties(slideNum, xmlDoc, headingElement, headingOption);

                XmlElement headling1Name = xmlDoc.CreateElement("content");
                headingElement.AppendChild(headling1Name);
                string headingString = TextHelper.CleansingForXML(heading.Temp.Name);
                XmlCDataSection headingNameSection = xmlDoc.CreateCDataSection(headingString);
                headling1Name.AppendChild(headingNameSection);

                foreach (vmContent con in heading.Contents)
                {
                    if (!con.IsEnable) continue;

                    XmlElement contentElement = xmlDoc.CreateElement("element");
                    chapterElement.AppendChild(contentElement);
                    xmlElement optionElement = null;
                    switch (con.ContentType)
                    {
                        case Commons.Enum.eContentType.NormalText: 
                            optionElement = option.TextElement;
                            optionElement.ElementType = eXMLElementType.normal;
                            break;
                        case Commons.Enum.eContentType.OrderList: 
                            optionElement = option.OrderedListElement;
                            optionElement.ElementType = eXMLElementType.ordered_list;
                            break;
                        case Commons.Enum.eContentType.UnOrderList: 
                            optionElement = option.UnorderedListElement;
                            optionElement.ElementType = eXMLElementType.unordered_list;
                            break;
                        case Commons.Enum.eContentType.Image: 
                            optionElement = option.ImageElement;
                            optionElement.ElementType = eXMLElementType.image;
                            break;
                        case Commons.Enum.eContentType.Table: 
                            optionElement = option.TableElement;
                            optionElement.ElementType = eXMLElementType.table;
                            break;
                        default: 
                            optionElement = option.TextElement;
                            optionElement.ElementType = eXMLElementType.normal;
                            break;
                    }

                    SetContentProperties(int.Parse(con.Display_SlideNum.ToString()), xmlDoc, contentElement, optionElement);


                    // image
                    xmlImage image = null;
                    if (con.ContentType == Commons.Enum.eContentType.Image) image = SetImageElementOption(con, optionElement);
                    if (image != null) this.ImageList.Add(image);

                    // table
          

                    //Config
                    {
                        XmlElement xmlProp = xmlDoc.CreateElement("property");
                        xmlProp.SetAttribute("name", "config");
                        xmlProp.SetAttribute("value", JsonHelper.ToJsonString(optionElement.Config));
                        contentElement.GetElementsByTagName("properties")[0].AppendChild(xmlProp);
                    }

                    XmlElement content = xmlDoc.CreateElement("content");
                    contentElement.AppendChild(content);
                    string contentString = TextHelper.CleansingForXML(con.Temp.Temp.LineText); //con.Temp.Temp.LineText.Remove((char)8203);
                    if (con.ContentType == Commons.Enum.eContentType.Image) contentString = image.FileName;
                    if (con.ContentType == Commons.Enum.eContentType.Table)
                    {
                        contentString = con.Temp_TableHTML;
                        string pattern = @"!\[\]\((.*?)\)";
                        MatchCollection matches = Regex.Matches(contentString, pattern);

                        foreach (Match match in matches)
                        {
                            string fileName = match.Groups[1].Value;
                            string imagePath = Path.Combine(this.Material.DirectoryPath, fileName);
                            xmlImage imgInTable = new xmlImage();

                            using (Bitmap bitmap = new Bitmap(imagePath))
                            {
                                int width = bitmap.Width;   // 가로 크기
                                int height = bitmap.Height; // 세로 크기

                                imgInTable.Height = (option.ImageElement.Config as xmlImageConfig).Height = height;
                                imgInTable.Width = (option.ImageElement.Config as xmlImageConfig).Width = width;
                                (option.ImageElement.Config as xmlImageConfig).Caption = string.IsNullOrEmpty(con.Temp.Temp.Title) ? TextHelper.GetImageTitleFromMarkdown(con.Temp.Temp.LineText) : con.Temp.Temp.Title;
                            }

                            imgInTable.Name = fileName;
                            imgInTable.FileName = XMLHelper.GenerateUUId(8);
                            imgInTable.FilePath = imagePath;
                            imgInTable.Size = new FileInfo(imagePath).Length;


                            string imgHtml = string.Format("<img src=\"/r/image/get/{0}\" width=\"100%\" height=\"auto\"/>", imgInTable.FileName);
                            contentString = contentString.Replace(match.Value, imgHtml);

                            this.ImageList.Add(imgInTable);
                        }
                    }
                    XmlCDataSection section = xmlDoc.CreateCDataSection(contentString);
                    content.AppendChild(section);
                }

                foreach (vmHeading item in heading.Children)
                {
                    SetXmlElement(item, xmlDoc, chapterElement, option);
                }
            }
        }

        private xmlImage SetImageElementOption(vmContent con, xmlElement optionElement)
        {
            string lineString = con.Temp.Temp.LineText;
            string fileName = TextHelper.GetImageFileNameFromMarkdown(lineString);
            if (!fileName.ToLower().EndsWith(".png")) fileName += ".png";

            xmlImage newImage = new xmlImage();

            string imagePath = Path.Combine(this.Material.DirectoryPath, fileName);
            using (Bitmap bitmap = new Bitmap(imagePath))
            {
                int width = bitmap.Width;   // 가로 크기
                int height = bitmap.Height; // 세로 크기

                xmlImageConfig imageConfig = optionElement.Config as xmlImageConfig;
                if (imageConfig == null) imageConfig = new xmlImageConfig();

                newImage.Height = imageConfig.Height = height;
                newImage.Width = imageConfig.Width = width;
                imageConfig.Caption = string.IsNullOrEmpty(con.Temp.Temp.Title) ? TextHelper.GetImageTitleFromMarkdown(con.Temp.Temp.LineText) : con.Temp.Temp.Title;

                optionElement.Config = imageConfig;
            }

            
            newImage.Name = fileName;
            newImage.FileName = XMLHelper.GenerateUUId(8);
            newImage.FilePath = imagePath;
            newImage.Size = new FileInfo(imagePath).Length;
           

            return newImage;

        }

        private void SetBasicProperty(XmlDocument rootDoc, XmlElement parentProps)
        {
            //create
            {
                XmlElement xmlProp = rootDoc.CreateElement("property");
                xmlProp.SetAttribute("name", "create");
                xmlProp.SetAttribute("value", "DLENC");
                parentProps.AppendChild(xmlProp);
            }

            DateTime nowTime = DateTime.Now;
            //create_time
            {
                XmlElement xmlProp = rootDoc.CreateElement("property");
                xmlProp.SetAttribute("name", "create_time");
                xmlProp.SetAttribute("value", ((DateTimeOffset)nowTime).ToUnixTimeMilliseconds().ToString());
                parentProps.AppendChild(xmlProp);
            }

            //update_time
            {
                XmlElement xmlProp = rootDoc.CreateElement("property");
                xmlProp.SetAttribute("name", "update_time");
                xmlProp.SetAttribute("value", ((DateTimeOffset)nowTime).ToUnixTimeMilliseconds().ToString());
                parentProps.AppendChild(xmlProp);
            }
        }
        private void SetBookProperties(XmlDocument rootDoc, XmlElement element, xmlBook bookOption)
        {
            string id = XMLHelper.GenerateUUId(8);
            element.SetAttribute("id", id);

            XmlElement props = rootDoc.CreateElement("properties");
            element.AppendChild(props);

            PropertyInfo[] pInfos = bookOption.GetType().GetProperties();
            foreach (PropertyInfo p in pInfos)
            {
                xmlSubPropertyAttribute subPropAtt = p.GetCustomAttribute(typeof(xmlSubPropertyAttribute)) as xmlSubPropertyAttribute;
                if (subPropAtt == null) continue;

                var value = p.GetValue(bookOption, null);
                string valueString = ConvertToString(value);

                XmlElement xmlProp = rootDoc.CreateElement("property");
                xmlProp.SetAttribute("name", subPropAtt.Prorperty.Name);
                if (subPropAtt.Prorperty.Name == "type") valueString = "ARTICLE";
                xmlProp.SetAttribute("value", valueString);
                props.AppendChild(xmlProp);
            }

            //Config
            {
                XmlElement xmlProp = rootDoc.CreateElement("property");
                xmlProp.SetAttribute("name", "config");
                xmlProp.SetAttribute("value", JsonHelper.ToJsonString(bookOption.Config));
                props.AppendChild(xmlProp);
            }

            SetBasicProperty(rootDoc, props);
        }
        private void SetChapterProperties(XmlDocument rootDoc, XmlElement element, xmlChapter chapterOption)
        {
            string id = XMLHelper.GenerateUUId(8);
            element.SetAttribute("id", id);
            XmlElement props = rootDoc.CreateElement("properties");
            element.AppendChild(props);

            PropertyInfo[] pInfos = chapterOption.GetType().GetProperties();
            foreach (PropertyInfo p in pInfos)
            {
                xmlSubPropertyAttribute subPropAtt = p.GetCustomAttribute(typeof(xmlSubPropertyAttribute)) as xmlSubPropertyAttribute;
                if (subPropAtt == null) continue;

                var value = p.GetValue(chapterOption, null);
                string valueString = ConvertToString(value);

                XmlElement xmlProp = rootDoc.CreateElement("property");
                xmlProp.SetAttribute("name", subPropAtt.Prorperty.Name);
                if(subPropAtt.Prorperty.Name == "type" && valueString == "NONE")
                {
                    valueString = "CHAPTER";
                }
                xmlProp.SetAttribute("value", valueString);
                props.AppendChild(xmlProp);
            }

            //Config
            {
                XmlElement xmlProp = rootDoc.CreateElement("property");
                xmlProp.SetAttribute("name", "config");
                xmlProp.SetAttribute("value", JsonHelper.ToJsonString(chapterOption.Config));
                props.AppendChild(xmlProp);
            }

            SetBasicProperty(rootDoc, props);
        }
        private void SetHeadingProperties(int slideNum, XmlDocument rootDoc, XmlElement element, xmlElement heading1Option)
        {
            string id = XMLHelper.GenerateUUId(8);
            element.SetAttribute("id", id);
            XmlElement props = rootDoc.CreateElement("properties");
            element.AppendChild(props);

            PropertyInfo[] pInfos = heading1Option.GetType().GetProperties();
            foreach (PropertyInfo p in pInfos)
            {
                xmlSubPropertyAttribute subPropAtt = p.GetCustomAttribute(typeof(xmlSubPropertyAttribute)) as xmlSubPropertyAttribute;
                if (subPropAtt == null) continue;

                var value = p.GetValue(heading1Option, null);
                string valueString = ConvertToString(value);//.ToLower();

                XmlElement xmlProp = rootDoc.CreateElement("property");
                xmlProp.SetAttribute("name", subPropAtt.Prorperty.Name);
                if (subPropAtt.Prorperty.Name == "alias") valueString = id + "_" + slideNum.ToString("0000");
                //if (subPropAtt.Prorperty.Name == "type") valueString = "heading1" ;
                xmlProp.SetAttribute("value", valueString);
                props.AppendChild(xmlProp);
            }

            //Config
            {
                XmlElement xmlProp = rootDoc.CreateElement("property");
                xmlProp.SetAttribute("name", "config");
                xmlProp.SetAttribute("value", JsonHelper.ToJsonString(heading1Option.Config));
                props.AppendChild(xmlProp);
            }

            SetBasicProperty(rootDoc, props);
        }
        private void SetContentProperties(int slideNum, XmlDocument rootDoc, XmlElement element, xmlElement contentOption)
        {
            string id = XMLHelper.GenerateUUId(8);
            element.SetAttribute("id", id);
            XmlElement props = rootDoc.CreateElement("properties");
            element.AppendChild(props);


            PropertyInfo[] pInfos = contentOption.GetType().GetProperties();
            foreach (PropertyInfo p in pInfos)
            {
                xmlSubPropertyAttribute subPropAtt = p.GetCustomAttribute(typeof(xmlSubPropertyAttribute)) as xmlSubPropertyAttribute;
                if (subPropAtt == null) continue;

                var value = p.GetValue(contentOption, null);
                string valueString = ConvertToString(value);//.ToLower();

                XmlElement xmlProp = rootDoc.CreateElement("property");
                xmlProp.SetAttribute("name", subPropAtt.Prorperty.Name);
                if (subPropAtt.Prorperty.Name == "alias") valueString = id + "_" + slideNum.ToString("0000");
                xmlProp.SetAttribute("value", valueString);
                props.AppendChild(xmlProp);
            }

    

            SetBasicProperty(rootDoc, props);
        }
        private void SetNoteContentProperties(XmlDocument rootDoc, XmlElement element, xmlElement noteOption)
        {
            string id = XMLHelper.GenerateUUId(8);
            element.SetAttribute("id", id);
            XmlElement props = rootDoc.CreateElement("properties");
            element.AppendChild(props);


            PropertyInfo[] pInfos = noteOption.GetType().GetProperties();
            foreach (PropertyInfo p in pInfos)
            {
                xmlSubPropertyAttribute subPropAtt = p.GetCustomAttribute(typeof(xmlSubPropertyAttribute)) as xmlSubPropertyAttribute;
                if (subPropAtt == null) continue;

                var value = p.GetValue(noteOption, null);
                string valueString = ConvertToString(value);//.ToLower();

                XmlElement xmlProp = rootDoc.CreateElement("property");
                xmlProp.SetAttribute("name", subPropAtt.Prorperty.Name);
                xmlProp.SetAttribute("value", valueString);
                props.AppendChild(xmlProp);
            }



            SetBasicProperty(rootDoc, props);
        }




        public string ConvertListTextToHtml(string input)
        {
            string[] lines = TextHelper.SplitText(input);

            // Track the depth of ordered and unordered lists
            int orderedListDepth = 0;
            int unorderedListDepth = 0;


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
                    if(parentLine != null)
                    {
                        parentLine.Children.Add(newLine);
                    }
                }
            }

            
            string html  = GetHtmlList(listLines);
            return html;
        }

        private string GetHtmlList(List<mListLine> listLines)
        {
            string html = string.Empty;
            if (listLines == null || listLines.Count() == 0) return string.Empty;

            html += listLines.First().IsOrderedList ? "<ol>" : "<ul>";
            foreach (mListLine item in listLines)
            {
                html += $"<li>{item.LineString.Trim()}</li>";
                html += GetHtmlList(item.Children);
            }
            html+= listLines.First().IsOrderedList ? "</ol>" : "</ul>";

            return html;
        }

        private string ConvertTableStringToHTML(string lineText)
        {
            string output = string.Empty;

            var pipline = new MarkdownPipelineBuilder().Build();
            output = Markdown.ToHtml(lineText, pipline);

            return output;
        }

        private string ConvertToString(object value)
        {
            string output = string.Empty;

            if (value != null)
            {
                if (value is string[])
                {
                    string[] items = value as string[];
                    foreach (var item in items)
                    {
                        output += item;
                        if (item != items.Last()) output += ",";
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
            XmlWriterSettings settings = new XmlWriterSettings();
            //{
            //    Encoding = new UTF8Encoding(false),
            //    Indent = true,
            //    NewLineOnAttributes = true,
            //    OmitXmlDeclaration = true // XML 선언을 생략
            //};
            settings.Encoding = new UTF8Encoding(true);
            //settings.Indent = true;
            //settings.NewLineOnAttributes = true;
            settings.NewLineChars = "\n";
            settings.OmitXmlDeclaration = true;



            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(stringWriter, settings))
                {
                    writer.WriteStartDocument();
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

                    writer.WriteEndDocument();
                }

                //serializer.Serialize(stringWriter, book);
                output = stringWriter.ToString();
            }

            return output;
        }




        private xmlElement GetContentElement(vmContent content, xmlSet option)
        {
            switch (content.Temp.ItemType)
            {
                case Commons.Enum.eItemType.Text: return GetTextContentElement(content, option);
                case Commons.Enum.eItemType.Image: return GetImageContentElement(content, option.ImageElement);
                case Commons.Enum.eItemType.Table: return GetTableContentElement(content, option.TableElement);
                default: return null;
            }
        }
        private xmlElement GetHeadlingElement(vmHeading heading, xmlElement headingOption)
        {
            xmlElement output = new xmlElement();


            return output;
        }
        private xmlChapter GetHeadingChapter(vmHeading heading, xmlChapter chapterOption)
        {
            xmlChapter output = new xmlChapter();

            return output;
        }
        private xmlChapter GetChapterElement(xmlChapter originTarget)
        {
            xmlChapter chapter = new xmlChapter();

            chapter.Author = originTarget.Author;
            chapter.Alias = originTarget.Alias;
            chapter.Title = originTarget.Title;
            chapter.SubTitle = originTarget.SubTitle;
            chapter.Type = originTarget.Type;
            chapter.AlwaysTop = originTarget.AlwaysTop;
            chapter.Config = originTarget.Config;

            chapter.Properties.Clear();
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


            return chapter;
        }
        private xmlBook GetBookElement(xmlSet setValue)
        {
            xmlBook book = new xmlBook();// setValues.Book.Copy();
            book.Id = this.Material.Temp.Uid;
            if (Guid.TryParse(book.Id, out Guid bookUid)) book.Id = this.Material.Temp.Uid = XMLHelper.GenerateUUId(8);
            book.Author = setValue.Book.Author;
            
            book.SubTitle = setValue.Book.SubTitle;
            book.Edition = setValue.Book.Edition;
            book.Keywords = setValue.Book.Keywords;
            book.Type = setValue.Book.Type;
            book.Locale = setValue.Book.Locale;
            book.Tags = setValue.Book.Tags;

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

            return book;
        }
        private xmlElement GetTextContentElement(vmContent content, xmlSet option)
        {

            string text = content.Temp.Temp.LineText;
            string[] lines = TextHelper.SplitText(text);

            if(lines.Count() == 1 )
            {
                return GetNormalTextElement(content, option.TextElement);
            }
            else
            {
                return GetUnorderListTextElement(content, option.UnorderedListElement);
            }
        }
        private xmlElement GetNormalTextElement(vmContent content, xmlElement textOption)
        {
            xmlElement normalTextElement = new xmlElement();
            normalTextElement.Config = textOption.Config;
            normalTextElement.Alias = textOption.Alias;
            normalTextElement.ElementType = eXMLElementType.normal;



            return normalTextElement;
        }
        private xmlElement GetOrderListTextElement(vmContent content, xmlElement orderOption)
        {
            xmlElement orderElement = new xmlElement();
            orderElement.Config = orderOption.Config;
            orderElement.Alias = orderOption.Alias;
            orderElement.ElementType = eXMLElementType.ordered_list;



            return orderElement;
        }
        private xmlElement GetUnorderListTextElement(vmContent content, xmlElement unoderOption)
        {
            xmlElement unOrderElement = new xmlElement();
            unOrderElement.Config = unoderOption.Config;
            unOrderElement.Alias = unoderOption.Alias;
            unOrderElement.ElementType = eXMLElementType.ordered_list;



            return unOrderElement;
        }
        private xmlElement GetImageContentElement(vmContent content, xmlElement imageOption)
        {
            xmlElement imageElement = new xmlElement();
            imageElement.Config = imageOption.Config;
            imageElement.Alias = imageOption.Alias;
            imageElement.ElementType = imageOption.ElementType;

            imageElement.Id = content.Temp.Temp.Uid;
            if (Guid.TryParse(imageElement.Id, out Guid elementUid)) imageElement.Id = content.Temp.Temp.Uid = XMLHelper.GenerateUUId(8);
            imageElement.Content = content.Temp.Temp.LineText;

            // 속성
            PropertyInfo[] pInfos = imageElement.GetType().GetProperties();
            foreach (PropertyInfo p in pInfos)
            {
                xmlSubPropertyAttribute subPropAtt = p.GetCustomAttribute(typeof(xmlSubPropertyAttribute)) as xmlSubPropertyAttribute;
                if (subPropAtt == null) continue;

                var value = p.GetValue(imageElement, null);
                subPropAtt.Prorperty.Value = ConvertToString(value);
                imageElement.Properties.Add(subPropAtt.Prorperty);
            }

            // config
            string jsonString = JsonHelper.ToJsonString(imageElement.Config);
            xmlSubProperty configProp = new xmlSubProperty("config", jsonString);
            imageElement.Properties.Add(configProp);

            return imageElement;
        }
        private xmlElement GetTableContentElement(vmContent content, xmlElement tableOption)
        {
            xmlElement tableElement = new xmlElement();
            tableElement.Config = tableOption.Config;
            tableElement.Alias = tableOption.Alias;
            tableElement.ElementType = tableOption.ElementType;

            tableElement.Id = content.Temp.Temp.Uid;
            if (Guid.TryParse(tableElement.Id, out Guid elementUid)) tableElement.Id = content.Temp.Temp.Uid = XMLHelper.GenerateUUId(8);
            tableElement.Content = ConvertTableStringToHTML(content.Temp.Temp.LineText);

            // 속성
            PropertyInfo[] pInfos = tableElement.GetType().GetProperties();
            foreach (PropertyInfo p in pInfos)
            {
                xmlSubPropertyAttribute subPropAtt = p.GetCustomAttribute(typeof(xmlSubPropertyAttribute)) as xmlSubPropertyAttribute;
                if (subPropAtt == null) continue;

                var value = p.GetValue(tableElement, null);
                subPropAtt.Prorperty.Value = ConvertToString(value);
                tableElement.Properties.Add(subPropAtt.Prorperty);
            }

            // config
            string jsonString = JsonHelper.ToJsonString(tableElement.Config);
            xmlSubProperty configProp = new xmlSubProperty("config", jsonString);
            tableElement.Properties.Add(configProp);

            return tableElement;
        }
        private void SetChildrenBookType(xmlBook bookElement, xmlSet option)
        {

            bookElement.Chapters.Clear();
            foreach (vmHeading root in this.Material.RootHeadings)
            {
                xmlChapter chapter = GetChapterElement(option.Chapter);// new xmlChapter();
                bookElement.Chapters.Add(chapter);

                chapter.Id = root.Temp.Uid;
                if (Guid.TryParse(chapter.Id, out Guid chapterUid)) chapter.Id = root.Temp.Uid = XMLHelper.GenerateUUId(8);
                chapter.Title = root.Temp.Name;

                chapter.Elements.Clear();
                //foreach (vmContent con in root.Contents)
                //{
                //    xmlElement contentElement = null;
                //    switch (con.Temp.ItemType)
                //    {
                //        case Commons.Enum.eItemType.Text: contentElement = null; break;
                //        case Commons.Enum.eItemType.Image: contentElement = GetImageContentElement(option.ImageElement, con); break;
                //        case Commons.Enum.eItemType.Table: contentElement = GetTableContentElement(option.TableElement, con); break;
                //        default: break;
                //    }
                //    if (contentElement == null) continue;

                //    chapter.Elements.Add(contentElement);
                //}

                //foreach (vmHeading item in root.Children)
                //{
                //    SetXmlElement(item, chapter);
                //}
            }

        }
        private void SetChildrenArticleType(xmlBook bookElement, xmlSet option)
        {
            bookElement.Chapters.Clear();

            xmlChapter chapter = GetChapterElement(option.Chapter);// new xmlChapter();
            bookElement.Chapters.Add(chapter);

            chapter.Id = XMLHelper.GenerateUUId(8);
            chapter.Title = bookElement.Title;

            chapter.Elements.Clear();

            foreach (vmHeading rootHeading in this.Material.RootHeadings)
            {
                SetXmlElement(rootHeading, chapter);
            }
        }

        private void SetXmlElement(vmHeading item, xmlChapter chapter)
        {
            xmlElement element = new xmlElement();
            switch (item.Temp.Level)
            {
                case 1: this.Material.XMLSets.Heading1Element.Duplicate(element); break;
                case 2: this.Material.XMLSets.Heading2Element.Duplicate(element); break;
                case 3: this.Material.XMLSets.Heading3Element.Duplicate(element); break;
                case 4: this.Material.XMLSets.Heading4Element.Duplicate(element); break;
                case 5: this.Material.XMLSets.Heading5Element.Duplicate(element); break;
                case 6: this.Material.XMLSets.Heading5Element.Duplicate(element); break;
            }
            if (element == null) return;
            element.Content = item.Temp.Name;

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
                    subPropAtt.Prorperty.Value = ConvertToString(value).ToLower();
                    element.Properties.Add(subPropAtt.Prorperty);
                }

                string jsonString = JsonHelper.ToJsonString(element.Config);
                xmlSubProperty configProp = new xmlSubProperty("config", jsonString);
                element.Properties.Add(configProp);
            }
            


            foreach (vmContent cont in item.Contents)
            {
                xmlElement contentElement = new xmlElement();
                switch (cont.ContentType)
                {
                    case Commons.Enum.eContentType.NormalText:
                        this.Material.XMLSets.TextElement.Duplicate(contentElement);
                        break;
                    case Commons.Enum.eContentType.OrderList:
                        this.Material.XMLSets.OrderedListElement.Duplicate(contentElement);
                        break;
                    case Commons.Enum.eContentType.UnOrderList:
                        this.Material.XMLSets.UnorderedListElement.Duplicate(contentElement);
                        break;
                    case Commons.Enum.eContentType.Image:
                        this.Material.XMLSets.ImageElement.Duplicate(contentElement);
                        break;
                    case Commons.Enum.eContentType.Table:
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
                    subPropAtt.Prorperty.Value = ConvertToString(value).ToLower();
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

        private void btn_UUID_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string output = string.Empty;

                for (int i = 0; i < 1000; i++)
                {
                    output += XMLHelper.GenerateUUId(8);
                    output += "\n";
                }

                Clipboard.SetText(output);
            }
            catch (Exception ee)
            {

                
            }
        }

        private void btn_AllSetting_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
