using Markdig.Helpers;
using MDM.Commons.Enum;
using MDM.Helpers;
using MDM.Models.Attributes;
using MDM.Models.DataModels;
using MDM.Models.DataModels.ManualWorksXMLs;
using MDM.Models.ViewModels;
using MDM.Views.MarkChecker.Windows;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using Application = Microsoft.Office.Interop.PowerPoint.Application;
using Shape = Microsoft.Office.Interop.PowerPoint.Shape;

namespace MDM.Views.MarkChecker.Pages
{
    /// <summary>
    /// ucMarkCheckerMain.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucMarkCheckerMain : UserControl
    {
        private Application _PowerPointApp = null;
        public Application PowerPointApp
        {
            get => _PowerPointApp;
            set
            {
                _PowerPointApp = value;
                this.mcExcelView.ucSlideList.Visibility = _PowerPointApp == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private vmMaterial _Material = null;
        public vmMaterial Material
        {
            get => _Material;
            set
            {
                _Material = value;
                this.DataContext = value;
                this.mcExcelView.Material = value;
                this.mcContentsByHeading.Material = value;
                this.smDataLabeling.SetMaterial(value);
                this.ucMarCheckerToXml.Material = value;
                this.mcContentChecking.Material = value;
            }
        }

        public ucMarkCheckerMain()
        {
            InitializeComponent();
        }

        private void btn_FileOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileInfo fInfo = FileHelper.GetOpenFileInfo("파일 열기", eFILE_TYPE.Json);
                if (fInfo == null) return;

                mMaterial material = new mMaterial();
                material.Name = Path.GetFileNameWithoutExtension(fInfo.FullName);
                vmMaterial newMaterial = new vmMaterial(material);
                newMaterial.DirectoryPath = fInfo.DirectoryName;

                List<vmItem> allitems = new List<vmItem>();
                DirectoryInfo dInfo = fInfo.Directory;
                List<mHeading> headings = null;
                FileInfo headerInfo = dInfo.GetFiles("*.headers", SearchOption.TopDirectoryOnly).OrderBy(x => x.Name).LastOrDefault();
                if (headerInfo != null)
                {
                    string headerJsonString = File.ReadAllText(headerInfo.FullName);
                    headings = JsonHelper.ToObject<List<mHeading>>(headerJsonString);
                }
                if (headings == null) headings = new List<mHeading>();
                

                if (fInfo.Extension == ".mdm")
                {
                    DBHelper.ConnectionSting = fInfo.FullName;
                    material = DBHelper.Read();

                    newMaterial = new vmMaterial(material);
                    newMaterial.DirectoryPath = fInfo.DirectoryName;
                    newMaterial.LoadChildren();
                    newMaterial.CurrentSlide = newMaterial.Slides.FirstOrDefault();
                    newMaterial.OrderSlides();

                    foreach (vmSlide newSlide in newMaterial.Slides)
                    {
                        if (headings.Count > 0)
                        {
                            foreach (vmItem item in newSlide.Items) allitems.Add(item);
                            continue;
                        }
                        newSlide.ConvertAndSetContents();
                    }
                }
                else
                {
                    string jsonString = File.ReadAllText(fInfo.FullName);
                    List<mContent> contentList = JsonHelper.ToObject<List<mContent>>(jsonString) as List<mContent>;


                    int cnt = 0;
                    Dictionary<int, List<mContent>> slideDic = new Dictionary<int, List<mContent>>();
                    foreach (mContent content in contentList)
                    {
                        content.Idx = (cnt++)*10;
                        int slideNum = content.SlideIdx;
                        if (!slideDic.ContainsKey(slideNum)) slideDic.Add(slideNum, new List<mContent>());
                        slideDic[slideNum].Add(content);
                    }
                    List<mSlide> slides = new List<mSlide>();
                    foreach (int key in slideDic.Keys)
                    {
                        mSlide newSlide = new mSlide();
                        newSlide.SlideNumber = key;
                        newSlide.Index = key;
                        slides.Add(newSlide);

                        List<mItem> items = new List<mItem>();
                        foreach (mContent content in slideDic[key])
                        {
                            mSlide sameSlide = slides.Where(x => x.SlideNumber == key).FirstOrDefault();
                            if (sameSlide == null)
                            {
                                sameSlide = new mSlide();
                                sameSlide.SlideNumber = content.SlideIdx;
                                sameSlide.Index = sameSlide.SlideNumber;
                                slides.Add(sameSlide);
                            }

                            sameSlide.Description += content.Description + "\n" + content.Message + "\n";
                            if (TextHelper.IsNoText(content.Contents))
                            {
                                sameSlide.Status = ePageStatus.Exception.GetHashCode();
                                continue;
                            }

                            sameSlide.Status = ePageStatus.Completed.GetHashCode();

                            #region 이전 코드
                            //int level = 1;
                            //mItem header1 = GetSameItem(level++, content, items);
                            //if (header1 != null && !items.Contains(header1)) items.Add(header1);
                            //mItem header2 = GetSameItem(level++, content, items);
                            //if (header2 != null && !items.Contains(header2)) items.Add(header2);
                            //mItem header3 = GetSameItem(level++, content, items);
                            //if (header3 != null && !items.Contains(header3)) items.Add(header3);
                            //mItem header4 = GetSameItem(level++, content, items);
                            //if (header4 != null && !items.Contains(header4)) items.Add(header4);
                            //mItem header5 = GetSameItem(level++, content, items);
                            //if (header5 != null && !items.Contains(header5)) items.Add(header5);
                            //mItem header6 = GetSameItem(level++, content, items);
                            //if (header6 != null && !items.Contains(header6)) items.Add(header6);
                            //mItem header7 = GetSameItem(level++, content, items);
                            //if (header7 != null && !items.Contains(header7)) items.Add(header7);
                            //mItem header8 = GetSameItem(level++, content, items);
                            //if (header8 != null && !items.Contains(header8)) items.Add(header8);
                            //mItem header9 = GetSameItem(level++, content, items);
                            //if (header9 != null && !items.Contains(header9)) items.Add(header9);
                            //mItem header10 = GetSameItem(level++, content, items);
                            //if (header10 != null && !items.Contains(header10)) items.Add(header10);
                            #endregion

                            SetHeaderItems(content, items);

                            mItem conItem = new mItem();
                            conItem.ItemType = content.ContentsType;
                            conItem.Level = GetContentLevel(content);
                            conItem.LineText = content.Contents;
                            conItem.Order = content.Idx;
                            if (conItem.ItemType == eItemType.Image.GetHashCode()) conItem.Title = TextHelper.GetImageTitleFromMarkdown(conItem.LineText);
                            items.Add(conItem);
                        }

                        foreach (mItem item in items)
                        {
                            mShape newShape = new mShape();
                            newShape.ShapeType = item.ItemType;
                            if (item.ItemType == 210) newShape.ShapeType = eShapeType.Text.GetHashCode();
                            newShape.Top = items.IndexOf(item);
                            newShape.Text = item.LineText;
                            newShape.Lines.Add(item);

                            newSlide.Shapes.Add(newShape);
                        }
                    }


                    foreach (mSlide slide in slides)
                    {
                        if(slide.Index == 237)
                        {
                            bool hasImage = slide.Shapes.Any(x => x.ShapeType == 222);
                            if(hasImage)
                            {

                            }
                        }
                        vmSlide newSlide = new vmSlide(slide);
                        newSlide.SetParentMaterial(newMaterial);
                        if (headings.Count > 0)
                        {
                            foreach (vmItem item in newSlide.Items) allitems.Add(item);
                            continue;
                        }
                        newSlide.ConvertAndSetContents();
                    }
                }
 
                foreach (mHeading heading in headings)
                {
                    SetHeading(heading, null, newMaterial, allitems);
                }


                this.Material = newMaterial;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_FileOpen_Click2(object sender, RoutedEventArgs e)
        {
            try
            {
                FileInfo fInfo = FileHelper.GetOpenFileInfo("파일 열기", eFILE_TYPE.Json);
                if (fInfo == null) return;
                DirectoryInfo dInfo = fInfo.Directory;


                mMaterial material = new mMaterial();
                material.Name = Path.GetFileNameWithoutExtension(fInfo.FullName);
                vmMaterial newMaterial = new vmMaterial(material);
                newMaterial.DirectoryPath = fInfo.DirectoryName;


                List<mHeading> headingList = new List<mHeading>();
                List<mContent> contentList = new List<mContent>();
                FileInfo headerInfo = dInfo.GetFiles("*.headers", SearchOption.TopDirectoryOnly).OrderBy(x => x.Name).LastOrDefault();
                if (headerInfo != null)
                {
                    string headerJsonString = File.ReadAllText(headerInfo.FullName);
                    List<mHeading> headings = JsonHelper.ToObject<List<mHeading>>(headerJsonString);
                    SetConetentList(headings, contentList, headingList);
                }
                else
                {
                    string jsonString = File.ReadAllText(fInfo.FullName);
                    contentList = JsonHelper.ToObject<List<mContent>>(jsonString) as List<mContent>;
                }
                { 
            
                    int cnt = 0;
                    Dictionary<int, List<mContent>> slideDic = new Dictionary<int, List<mContent>>();
                    foreach (mContent content in contentList)
                    {
                        if(content.Idx == -1) content.Idx = (cnt++) * 10;
                        int slideNum = content.SlideIdx;
                        if (!slideDic.ContainsKey(slideNum)) slideDic.Add(slideNum, new List<mContent>());
                        slideDic[slideNum].Add(content);
                    }


                    List<mSlide> slides = new List<mSlide>();
                    foreach (int key in slideDic.Keys)
                    {
                        mSlide newSlide = new mSlide();
                        newSlide.SlideNumber = key;
                        newSlide.Index = key;
                        slides.Add(newSlide);

                        List<mItem> items = new List<mItem>();
                        foreach (mContent content in slideDic[key])
                        {
                            

                        }




                        foreach (mItem item in items)
                        {
                            mShape newShape = new mShape();
                            newShape.ShapeType = item.ItemType;
                            if (item.ItemType == 210) newShape.ShapeType = eShapeType.Text.GetHashCode();
                            newShape.Top = items.IndexOf(item);
                            newShape.Text = item.LineText;
                            newShape.Lines.Add(item);

                            newSlide.Shapes.Add(newShape);
                        }
                    }


                    foreach (mSlide slide in slides)
                    {
                        if (slide.Index == 237)
                        {
                            bool hasImage = slide.Shapes.Any(x => x.ShapeType == 222);
                            if (hasImage)
                            {

                            }
                        }
                        vmSlide newSlide = new vmSlide(slide);
                        newSlide.SetParentMaterial(newMaterial);
                        
                        if(headingList.Count > 0)
                        {
                            newSlide.ConvertAndSetContents(headingList, contentList);
                        }
                        else
                        {
                            newSlide.ConvertAndSetContents();
                        }

                        
                    }
                }

                this.Material = newMaterial;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
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
        private void SetConetentList(List<mHeading> headings, List<mContent> contentList, List<mHeading> allHeadings)
        {
            foreach (mHeading item in headings)
            {
                allHeadings.Add(item);
                foreach (mContent content in item.Contents)
                {
                    contentList.Add(content);
                }

                SetConetentList(item.Children.ToList(), contentList, allHeadings);
            }
        }

        private void SetHeaderItems(mContent content, List<mItem> items, List<mHeading> allHeadings = null)
        {
            // Header Level 1
            int levelOrder = -1;
            string headingString = content.Heading1String;
            if (!string.IsNullOrEmpty(headingString))
            {
                mItem sameItem = items.Where(x => x.LineText == headingString).LastOrDefault();
                if (sameItem == null)
                {
                    sameItem = new mItem();
                    if (!string.IsNullOrEmpty(content.HeadingUid_1)) sameItem.Uid = content.HeadingUid_1;
                    sameItem.ItemType = eItemType.Header.GetHashCode();
                    sameItem.Level = 1;
                    sameItem.LineText = headingString;
                    items.Add(sameItem);
                }
                levelOrder = items.IndexOf(sameItem);
            }

            // Header Level 2
            headingString = content.Heading2String;
            if (!string.IsNullOrEmpty(headingString))
            {
                mItem sameItem = items.Where(x => items.IndexOf(x) > levelOrder && x.LineText == headingString).LastOrDefault();
                if (sameItem == null)
                {
                    sameItem = new mItem();
                    if (!string.IsNullOrEmpty(content.HeadingUid_2)) sameItem.Uid = content.HeadingUid_2;
                    sameItem.ItemType = eItemType.Header.GetHashCode();
                    sameItem.Level = 2;
                    sameItem.LineText = headingString;
                    items.Add(sameItem);
                }
                levelOrder = items.IndexOf(sameItem);
            }

            // Header Level 3
            headingString = content.Heading3String;
            if (!string.IsNullOrEmpty(headingString))
            {
                mItem sameItem = items.Where(x => items.IndexOf(x) > levelOrder && x.LineText == headingString).LastOrDefault();
                if (sameItem == null)
                {
                    sameItem = new mItem();
                    if (!string.IsNullOrEmpty(content.HeadingUid_3)) sameItem.Uid = content.HeadingUid_3;
                    sameItem.ItemType = eItemType.Header.GetHashCode();
                    sameItem.Level = 3;
                    sameItem.LineText = headingString;
                    items.Add(sameItem);
                }
                levelOrder = items.IndexOf(sameItem);
            }

            // Header Level 4
            headingString = content.Heading4String;
            if (!string.IsNullOrEmpty(headingString))
            {
                mItem sameItem = items.Where(x => items.IndexOf(x) > levelOrder && x.LineText == headingString).LastOrDefault();
                if (sameItem == null)
                {
                    sameItem = new mItem();
                    if (!string.IsNullOrEmpty(content.HeadingUid_4)) sameItem.Uid = content.HeadingUid_4;
                    sameItem.ItemType = eItemType.Header.GetHashCode();
                    sameItem.Level = 4;
                    sameItem.LineText = headingString;
                    items.Add(sameItem);
                }
                levelOrder = items.IndexOf(sameItem);
            }

            // Header Level 5
            headingString = content.Heading5String;
            if (!string.IsNullOrEmpty(headingString))
            {
                mItem sameItem = items.Where(x => items.IndexOf(x) > levelOrder && x.LineText == headingString).LastOrDefault();
                if (sameItem == null)
                {
                    sameItem = new mItem();
                    if (!string.IsNullOrEmpty(content.HeadingUid_5)) sameItem.Uid = content.HeadingUid_5;
                    sameItem.ItemType = eItemType.Header.GetHashCode();
                    sameItem.Level = 5;
                    sameItem.LineText = headingString;
                    items.Add(sameItem);
                }
                levelOrder = items.IndexOf(sameItem);
            }

            // Header Level 6
            headingString = content.Heading6String;
            if (!string.IsNullOrEmpty(headingString))
            {
                mItem sameItem = items.Where(x => items.IndexOf(x) > levelOrder && x.LineText == headingString).LastOrDefault();
                if (sameItem == null)
                {
                    sameItem = new mItem();
                    if (!string.IsNullOrEmpty(content.HeadingUid_6)) sameItem.Uid = content.HeadingUid_6;
                    sameItem.ItemType = eItemType.Header.GetHashCode();
                    sameItem.Level = 6;
                    sameItem.LineText = headingString;
                    items.Add(sameItem);
                }
                levelOrder = items.IndexOf(sameItem);
            }

            // Header Level 7
            headingString = content.Heading7String;
            if (!string.IsNullOrEmpty(headingString))
            {
                mItem sameItem = items.Where(x => items.IndexOf(x) > levelOrder && x.LineText == headingString).LastOrDefault();
                if (sameItem == null)
                {
                    sameItem = new mItem();
                    if (!string.IsNullOrEmpty(content.HeadingUid_7)) sameItem.Uid = content.HeadingUid_7;
                    sameItem.ItemType = eItemType.Header.GetHashCode();
                    sameItem.Level = 7;
                    sameItem.LineText = headingString;
                    items.Add(sameItem);
                }
                levelOrder = items.IndexOf(sameItem);
            }

            // Header Level 8
            headingString = content.Heading8String;
            if (!string.IsNullOrEmpty(headingString))
            {
                mItem sameItem = items.Where(x => items.IndexOf(x) > levelOrder && x.LineText == headingString).LastOrDefault();
                if (sameItem == null)
                {
                    sameItem = new mItem();
                    if (!string.IsNullOrEmpty(content.HeadingUid_8)) sameItem.Uid = content.HeadingUid_8;
                    sameItem.ItemType = eItemType.Header.GetHashCode();
                    sameItem.Level = 8;
                    sameItem.LineText = headingString;
                    items.Add(sameItem);
                }
                levelOrder = items.IndexOf(sameItem);
            }

            // Header Level 9
            headingString = content.Heading9String;
            if (!string.IsNullOrEmpty(headingString))
            {
                mItem sameItem = items.Where(x => items.IndexOf(x) > levelOrder && x.LineText == headingString).LastOrDefault();
                if (sameItem == null)
                {
                    sameItem = new mItem();
                    if (!string.IsNullOrEmpty(content.HeadingUid_9)) sameItem.Uid = content.HeadingUid_9;
                    sameItem.ItemType = eItemType.Header.GetHashCode();
                    sameItem.Level = 9;
                    sameItem.LineText = headingString;
                    items.Add(sameItem);
                }
                levelOrder = items.IndexOf(sameItem);
            }

            // Header Level 10
            headingString = content.Heading10String;
            if (!string.IsNullOrEmpty(headingString))
            {
                mItem sameItem = items.Where(x => items.IndexOf(x) > levelOrder && x.LineText == headingString).LastOrDefault();
                if (sameItem == null)
                {
                    sameItem = new mItem();
                    if (!string.IsNullOrEmpty(content.HeadingUid_10)) sameItem.Uid = content.HeadingUid_10;
                    sameItem.ItemType = eItemType.Header.GetHashCode();
                    sameItem.Level = 10;
                    sameItem.LineText = headingString;
                    items.Add(sameItem);
                }
                levelOrder = items.IndexOf(sameItem);
            }



    



        }

        private void SetHeading(mHeading heading, vmHeading parent ,vmMaterial material, List<vmItem> items)
        {
            vmHeading newHeading = new vmHeading(heading);
            newHeading.SetParentMaterial(material);
            newHeading.SetParent(parent);

            foreach (mContent content in heading.Contents)
            {
                vmItem sameItem = items.Where(x => x.Temp.ItemType == content.ContentsType && x.Temp.LineText == content.Contents).FirstOrDefault();
                if(sameItem == null)
                {
                    mItem item = new mItem();
                    item.ItemType = content.ContentsType;
                    item.Level = newHeading.Temp.Level + 1;
                    item.LineText = content.Contents;
                    if (item.ItemType == eItemType.Image.GetHashCode()) item.Title = TextHelper.GetImageTitleFromMarkdown(item.LineText);

                    sameItem = new vmItem(item);
                    vmSlide slide = material.Slides.Where(x=> x.Temp.SlideNumber == content.SlideIdx).FirstOrDefault();
                    vmShape shape = slide == null?  null : slide.Shapes.FirstOrDefault();
                    sameItem.SetParent(shape);
                }

                vmContent newContent = new vmContent(sameItem);
                newContent.SetParentMaterial(material);
                newContent.SetParentHeading(newHeading);
            }

            foreach (mHeading child in heading.Children)
            {
                SetHeading(child, newHeading, material, items);
            }
        }

        private int GetContentLevel(mContent content)
        {
            if (TextHelper.IsNoText(content.Heading1String)) return 1;
            if (TextHelper.IsNoText(content.Heading2String)) return 2;
            if (TextHelper.IsNoText(content.Heading3String)) return 3;
            if (TextHelper.IsNoText(content.Heading4String)) return 4;
            if (TextHelper.IsNoText(content.Heading5String)) return 5;
            if (TextHelper.IsNoText(content.Heading6String)) return 6;
            if (TextHelper.IsNoText(content.Heading7String)) return 7;
            if (TextHelper.IsNoText(content.Heading8String)) return 8;
            if (TextHelper.IsNoText(content.Heading9String)) return 9;
            return 10;
        }

        private mItem GetSameItem(int level, mContent content, List<mItem> items)
        {
            string value = string.Empty;
            switch (level)
            {
                case 1: value = content.Heading1String; break;
                case 2: value = content.Heading2String; break;
                case 3: value = content.Heading3String; break;
                case 4: value = content.Heading4String; break;
                case 5: value = content.Heading5String; break;
                case 6: value = content.Heading6String; break;
                case 7: value = content.Heading7String; break;
                case 8: value = content.Heading8String; break;
                case 9: value = content.Heading9String; break;
                case 10: value = content.Heading10String; break;
                default: break;
            }
            mItem output = items.Where(x => x.Level == level && x.LineText == value).FirstOrDefault();
            if (output == null && !TextHelper.IsNoText(value))
            {
                output = new mItem();
                output.Level = level; 
                output.ItemType = eItemType.Header.GetHashCode();
                output.LineText = value;
            }
            return output;
        }

        private void btn_ExportXml_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Material.ImageList.Clear();

                XmlDocument xmlDoc = new XmlDocument();
                XmlDeclaration xmlDecl = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                xmlDoc.AppendChild(xmlDecl);


                XmlElement bookElement = XMLHelper.GetBookXmlElement(xmlDoc, this.Material);
                List<XmlElement> chapterElements = XMLHelper.GetChapterElements(xmlDoc, this.Material);
                foreach (XmlElement chapter in chapterElements) bookElement.AppendChild(chapter);
                
                string folderName = DateTime.Now.ToString("yyyyMMddHHmmss");
                string folderPath = Path.Combine(this.Material.DirectoryPath, string.Format("{0}_{1}", "book", folderName));
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                string bookFolderPath = Path.Combine(folderPath, "book");
                if (!Directory.Exists(bookFolderPath)) Directory.CreateDirectory(bookFolderPath);
                string bookFileName = string.Format("{0}.xml", bookElement.GetAttribute("id"));
                string bookTargetPath = Path.Combine(bookFolderPath, bookFileName);
                using (StreamWriter writer = new StreamWriter(bookTargetPath, false, Encoding.UTF8)) xmlDoc.Save(writer);

                
                {
                    string imageFolderPath = Path.Combine(folderPath, "image");
                    if (!Directory.Exists(imageFolderPath)) Directory.CreateDirectory(imageFolderPath);

                    string createDateString = ((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds().ToString();
                    XmlDocument imageDoc = new XmlDocument();
                    XmlDeclaration imageXmlDecl = imageDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                    imageDoc.AppendChild(imageXmlDecl);
                    XmlElement imagesElement = imageDoc.CreateElement("images");
                    imageDoc.AppendChild(imagesElement);
                    foreach (xmlImage image in this.Material.ImageList)
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
                }
                


                string targetName = string.Format("{0}_{1}.zip", "book", folderName);
                string targetPath = Path.Combine(this.Material.DirectoryPath, targetName);
                ZipFile.CreateFromDirectory(folderPath, targetPath);


                string msg = "XML Export!!!";
                MessageHelper.ShowMessage("Export To XML", msg);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }



        private void btn_XMLFileOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileInfo fInfo = FileHelper.GetOpenFileInfo("파일 열기", eFILE_TYPE.XML);
                if (fInfo == null) return;

                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(fInfo.FullName);


                vmMaterial newMaterial = null;
                foreach (var item in xDoc.ChildNodes)
                {
                    XmlElement element = item as XmlElement;
                    if (element == null) continue;
                    if (element.Name != "book") continue;

                    mBook book = element.ToBook();
                    //Chapter 가져와야 함
                    mMaterial material = new mMaterial();
                    material.Name = book.Title;
                    material.ManualworksBook = book;
                    newMaterial = new vmMaterial(material);
                    newMaterial.DirectoryPath = fInfo.DirectoryName;
                    if (this.PowerPointApp != null) newMaterial.SetPresentation(this.PowerPointApp.ActivePresentation);
                    break;
                }

                List<mContent> contentList = new List<mContent>();
                List<mChapter> chapters = newMaterial.Temp.ManualworksBook.Chapters;
                if (newMaterial.Temp.ManualworksBook.Type == "ARTICLE")
                {
                    Dictionary<int, string> headings = new Dictionary<int, string>();
                    foreach (mElement item in chapters[0].Elements)
                    {
                        if (item.Type.ToLower().Contains("heading"))
                        {
                            int level = int.Parse(item.Type.Last().ToString());
                            if (!headings.ContainsKey(level)) headings.Add(level, string.Empty);
                            headings[level] = item.Value;

                            foreach (int key in headings.Keys.ToList())
                            {
                                if (level >= key) continue;
                                headings.Remove(key);
                            }
                        }
                        else
                        {
                            mContent newContent = new mContent();
                            newContent.Contents = item.Value;
                            foreach (int key in headings.Keys)
                            {
                                switch (key)
                                {
                                    case 1: newContent.Heading1String = headings[key]; break;
                                    case 2: newContent.Heading2String = headings[key]; break;
                                    case 3: newContent.Heading3String = headings[key]; break;
                                    case 4: newContent.Heading4String = headings[key]; break;
                                    case 5: newContent.Heading5String = headings[key]; break;
                                    case 6: newContent.Heading6String = headings[key]; break;
                                    case 7: newContent.Heading7String = headings[key]; break;
                                    case 8: newContent.Heading8String = headings[key]; break;
                                    case 9: newContent.Heading9String = headings[key]; break;
                                    case 10: newContent.Heading10String = headings[key]; break;
                                    default: break;
                                }
                            }

                            switch (item.Type)
                            {
                                case "image": newContent.ContentsType = 222; break;
                                case "table": newContent.ContentsType = 223; break;
                                default: newContent.ContentsType = 221; break;
                            }
                            contentList.Add(newContent);
                        }
                    }
                }
                else
                {
                    foreach (mChapter chap in chapters)
                    {
                        Dictionary<int, string> headings = new Dictionary<int, string>();
                        headings.Add(1, chap.Title);
                        foreach (mElement item in chap.Elements)
                        {
                            if (item.Type.ToLower().Contains("heading"))
                            {
                                int level = int.Parse(item.Type.Last().ToString());
                                if (!headings.ContainsKey(level)) headings.Add(level + 1, string.Empty);
                                headings[level] = item.Value;

                                foreach (int key in headings.Keys.ToList())
                                {
                                    if (level >= key) continue;
                                    headings.Remove(key);
                                }
                            }
                            else
                            {
                                mContent newContent = new mContent();
                                newContent.Contents = item.Value;
                                foreach (int key in headings.Keys)
                                {
                                    switch (key)
                                    {
                                        case 1: newContent.Heading1String = headings[key]; break;
                                        case 2: newContent.Heading2String = headings[key]; break;
                                        case 3: newContent.Heading3String = headings[key]; break;
                                        case 4: newContent.Heading4String = headings[key]; break;
                                        case 5: newContent.Heading5String = headings[key]; break;
                                        case 6: newContent.Heading6String = headings[key]; break;
                                        case 7: newContent.Heading7String = headings[key]; break;
                                        case 8: newContent.Heading8String = headings[key]; break;
                                        case 9: newContent.Heading9String = headings[key]; break;
                                        case 10: newContent.Heading10String = headings[key]; break;
                                        default: break;
                                    }
                                }

                                switch (item.Type)
                                {
                                    case "image": newContent.ContentsType = 222; break;
                                    case "table": newContent.ContentsType = 223; break;
                                    default: newContent.ContentsType = 221; break;
                                }
                                contentList.Add(newContent);
                            }
                        }
                    }
                }


                int cnt = 0;
                Dictionary<int, List<mContent>> slideDic = new Dictionary<int, List<mContent>>();
                foreach (mContent content in contentList)
                {
                    content.Idx = (cnt++) * 10;
                    int slideNum = content.SlideIdx;
                    if (!slideDic.ContainsKey(slideNum)) slideDic.Add(slideNum, new List<mContent>());
                    slideDic[slideNum].Add(content);
                }
                List<mSlide> slides = new List<mSlide>();
                foreach (int key in slideDic.Keys)
                {
                    mSlide newSlide = new mSlide();
                    newSlide.SlideNumber = key;
                    newSlide.Index = key;
                    slides.Add(newSlide);

                    List<mItem> items = new List<mItem>();
                    foreach (mContent content in slideDic[key])
                    {
                        mSlide sameSlide = slides.Where(x => x.SlideNumber == key).FirstOrDefault();
                        if (sameSlide == null)
                        {
                            sameSlide = new mSlide();
                            sameSlide.SlideNumber = content.SlideIdx;
                            sameSlide.Index = sameSlide.SlideNumber;
                            slides.Add(sameSlide);
                        }

                        sameSlide.Description += content.Description + "\n" + content.Message + "\n";
                        if (TextHelper.IsNoText(content.Contents))
                        {
                            sameSlide.Status = ePageStatus.Exception.GetHashCode();
                            continue;
                        }

                        sameSlide.Status = ePageStatus.Completed.GetHashCode();

                        SetHeaderItems(content, items);

                        mItem conItem = new mItem();
                        conItem.ItemType = content.ContentsType;
                        conItem.Level = GetContentLevel(content);
                        conItem.LineText = content.Contents;
                        conItem.Order = content.Idx;
                        if (conItem.ItemType == eItemType.Image.GetHashCode()) conItem.Title = TextHelper.GetImageTitleFromMarkdown(conItem.LineText);
                        items.Add(conItem);
                    }

                    foreach (mItem item in items)
                    {
                        mShape newShape = new mShape();
                        newShape.ShapeType = item.ItemType;
                        if (item.ItemType == 210) newShape.ShapeType = eShapeType.Text.GetHashCode();
                        newShape.Top = items.IndexOf(item);
                        newShape.Text = item.LineText;
                        newShape.Lines.Add(item);

                        newSlide.Shapes.Add(newShape);
                    }
                }
                foreach (mSlide slide in slides)
                {
                    if (slide.Index == 237)
                    {
                        bool hasImage = slide.Shapes.Any(x => x.ShapeType == 222);
                        if (hasImage)
                        {

                        }
                    }
                    vmSlide newSlide = new vmSlide(slide);
                    newSlide.SetParentMaterial(newMaterial);
                    //if (headings.Count > 0)
                    //{
                    //    foreach (vmItem item in newSlide.Items) allitems.Add(item);
                    //    continue;
                    //}
                    newSlide.ConvertAndSetContents();
                }
                this.Material = newMaterial;
                if (this.PowerPointApp != null) this.Material.SetPresentation(this.PowerPointApp.ActivePresentation);
                this.mcExcelView.ucSlideList.SetMaterial(this.Material);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_XMLFileOpen_Click2(object sender, RoutedEventArgs e)
        {
            try
            {
                FileInfo fInfo = FileHelper.GetOpenFileInfo("파일 열기", eFILE_TYPE.XML);
                if (fInfo == null) return;

                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(fInfo.FullName);


                vmMaterial newMaterial = null;
                foreach (var item in xDoc.ChildNodes)
                {
                    XmlElement element = item as XmlElement;
                    if (element == null) continue;
                    if (element.Name != "book") continue;

                    mBook book = element.ToBook();
                    //Chapter 가져와야 함
                    mMaterial material = new mMaterial();
                    material.Name = book.Title;
                    material.ManualworksBook = book;
                    newMaterial = new vmMaterial(material);
                    newMaterial.DirectoryPath = fInfo.DirectoryName;
                   if(this.PowerPointApp != null)  newMaterial.SetPresentation(this.PowerPointApp.ActivePresentation);
                    break;
                }

                List<mItem> itemList = new List<mItem>();
                List<mChapter> chapters = newMaterial.Temp.ManualworksBook.Chapters;
                
                foreach (mChapter chap in chapters)
                {
                    int conCount = (chapters.IndexOf(chap) +1) * 1000;

                    string h1Index = conCount.ToString();
                    h1Index += "00000";
                    mItem newItem = new mItem();
                    newItem.Order = int.Parse(h1Index);
                    newItem.LineText = chap.Title;
                    newItem.ItemType = eItemType.Header.GetHashCode();
                    newItem.Level = 1;
                    itemList.Add(newItem); 


                    List<mItem> items = GetItemList(chap);
                    foreach (mItem item in items)
                    {
                        string index = conCount.ToString();
                        index += item.Order.ToString();
                        item.Order = int.Parse(index);  
                        itemList.Add(item);
                    }
                }


                mSlide slide = new mSlide();
                slide.SlideNumber = -1;
                slide.Index = -1;
                
                foreach (mItem item in itemList)
                {
                    mShape newShape = new mShape();
                    newShape.ShapeType = item.ItemType;
                    if (item.ItemType == 210) newShape.ShapeType = eShapeType.Text.GetHashCode();
                    newShape.Top = item.Order;
                    newShape.Text = item.LineText;
                    newShape.Lines.Add(item);

                    slide.Shapes.Add(newShape);
                }

                vmSlide newSlide = new vmSlide(slide);
                newSlide.SetParentMaterial(newMaterial);
                newSlide.ConvertAndSetContents();

                this.Material = newMaterial;
                if(this.PowerPointApp != null) this.Material.SetPresentation(this.PowerPointApp.ActivePresentation);
                this.mcExcelView.ucSlideList.SetMaterial(this.Material);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private List<mItem> GetItemList(mChapter chap)
        {
            List<mItem> output = new List<mItem>();

            int cnt = 10000;
            foreach (mElement ele in chap.Elements.OrderBy(x=> x.Order))
            {
                mItem newItem = new mItem();
                newItem.Order = cnt + ele.Order;
                newItem.LineText = ele.Value;

                string typeString = ele.Type;
                if (typeString.ToLower().Contains("heading"))
                {
                    newItem.ItemType = eItemType.Header.GetHashCode();
                    newItem.Level = int.Parse(typeString.Last().ToString()) + 1;
                }
                else
                {
                    switch (ele.Type)
                    {
                        case "image": newItem.ItemType = eItemType.Image.GetHashCode(); break;
                        case "table": newItem.ItemType = eItemType.Table.GetHashCode(); break;
                        default: newItem.ItemType = eItemType.Text.GetHashCode(); break;
                    }
                    newItem.Level = 100;
                }
                output.Add(newItem);
            }
            return output;
        }

        private List<mContent> GetContentList(List<mElement> elements, bool isBookDocument = false)
        {
            List<mContent> output = new List<mContent>();

            string headgin1String = string.Empty;
            string headgin2String = string.Empty;
            string headgin3String = string.Empty;
            string headgin4String = string.Empty;
            string headgin5String = string.Empty;

            foreach (mElement elem in elements)
            {
                string elementType = elem.Type;
                if (elementType.ToLower().Contains("heading"))
                {
                    int level = int.Parse(elementType.Last().ToString());
                    switch (level)
                    {
                        case 1:
                            headgin1String = elem.Value;
                            headgin2String = string.Empty;
                            headgin3String = string.Empty;
                            headgin4String = string.Empty;
                            headgin5String = string.Empty;
                            break;
                        case 2:
                            headgin2String = elem.Value;
                            headgin3String = string.Empty;
                            headgin4String = string.Empty;
                            headgin5String = string.Empty;
                            break;
                        case 3:
                            headgin3String = elem.Value;
                            headgin4String = string.Empty;
                            headgin5String = string.Empty;
                            break;
                        case 4:
                            headgin4String = elem.Value;
                            headgin5String = string.Empty;
                            break;
                        case 5:
                            headgin5String = elem.Value;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    mContent newContent = new mContent();
                    newContent.ContentOrder = elements.IndexOf(elem) * 10;

                    newContent.Contents = elem.Value;

                    switch (elem.Type)
                    {
                        case "image": newContent.ContentsType = 222; break;
                        case "table": newContent.ContentsType = 223; break;
                        default:
                            int lineCnt = TextHelper.SplitText(elem.Value).Length;
                            if (lineCnt == 0) newContent.ContentsType = 2211;
                            else newContent.ContentsType = 2212;
                            break;
                    }

                    if(isBookDocument)
                    {
                        newContent.Heading2String = headgin1String;
                        newContent.Heading3String = headgin2String;
                        newContent.Heading4String = headgin3String;
                        newContent.Heading5String = headgin4String;
                        newContent.Heading6String = headgin5String;
                    }
                    else
                    {
                        newContent.Heading1String = headgin1String;
                        newContent.Heading2String = headgin2String;
                        newContent.Heading3String = headgin3String;
                        newContent.Heading4String = headgin4String;
                        newContent.Heading5String = headgin5String;
                    }
                    

                    output.Add(newContent);
                }
            }

            return output;
        }

        private void btn_Temp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<mSlide> slides = new List<mSlide>();
                List<vmSlide> slideList = new List<vmSlide>();
                if (this.PowerPointApp != null)
                {
                    this.PowerPointApp.SlideSelectionChanged += PowerPointApp_SlideSelectionChanged1;

                    List<Slide> originSlides = new List<Slide>();
                    if (this.PowerPointApp != null) foreach (Slide slide in this.PowerPointApp.ActivePresentation.Slides) originSlides.Add(slide);

                    //MessageBox.Show(originSlides.Count.ToString(), "originSlides");

                    foreach (Slide slide in originSlides)
                    {
                        mSlide sl = new mSlide();
                        #region Slide 데이터 객체
                        sl.SlideId = slide.SlideID;
                        sl.Index = slide.SlideIndex;
                        sl.Name = slide.Name;
                        sl.SlideNumber = slide.SlideNumber;
                        #endregion

                        List<Shape> shapes = new List<Shape>();
                        #region Slide Shape
                        foreach (Shape shape in slide.Shapes)
                        {
                            if (shape.Type == MsoShapeType.msoGroup)
                            {
                                shape.Ungroup();
                                continue;
                            }

                            bool isAny = shapes.Where(x => x != null && x.Id == shape.Id).Any();
                            if (!isAny) shapes.Add(shape);
                        }
                        var masterShapes = slide.Master.Shapes;
                        foreach (Shape shape in masterShapes)
                        {
                            if (shape.Type == MsoShapeType.msoGroup) shape.Ungroup();

                            bool isAny = shapes.Where(x => x.Id == shape.Id).Any();
                            if (!isAny) shapes.Add(shape);
                        }
                        #endregion

                        List<mShape> shapeInstances = new List<mShape>();
                        foreach (Shape shape in shapes)
                        {
                            if (shape.Type == MsoShapeType.msoAutoShape)
                            {
                                if (shape.HasTextFrame == MsoTriState.msoTrue && shape.TextFrame.HasText == MsoTriState.msoTrue)
                                {
                                    mShape newText = PowerpointHelper.GetTextShape(shape);
                                    shapeInstances.Add(newText);
                                }
                                //else
                                //{
                                //    mShape newImage = PowerpointHelper.GetImageShpe(shape);
                                //    shapeInstances.Add(newImage);
                                //}
                            }
                            else if (shape.HasTextFrame == MsoTriState.msoTrue && shape.TextFrame.HasText == MsoTriState.msoTrue)
                            {
                                mShape newText = PowerpointHelper.GetTextShape(shape);
                                shapeInstances.Add(newText);
                            }
                            //else if (shape.HasTable == MsoTriState.msoTrue)
                            //{
                            //    mShape newTable = PowerpointHelper.GetTableShape(shape);
                            //    shapeInstances.Add(newTable);
                            //}
                            //else if (shape.Type == MsoShapeType.msoPicture)
                            //{
                            //    mShape newImage = PowerpointHelper.GetImageShpe(shape);
                            //    shapeInstances.Add(newImage);
                            //}
                        }

                        sl.Shapes = shapeInstances;
                        sl.Shapes = sl.Shapes.OrderByOriginPoint();
                        sl.Origin = slide;

                        slides.Add(sl);
                    }
                }
                else
                {
                    FileInfo fInfo = FileHelper.GetOpenFileInfo();
                    if (fInfo == null) return;

                    string pptTextJsonString = File.ReadAllText(fInfo.FullName);
                    slides = JsonHelper.ToObject<List<mSlide>>(pptTextJsonString);
                }


                //MessageBox.Show(slides.Count.ToString(), "slides");
                this.Material.ClearSlides();
                foreach (mSlide item in slides)
                {
                    vmSlide newSlide = new vmSlide(item);
                    newSlide.SetParentMaterial(this.Material);
                    newSlide.OnModifyStatusChanged(true);
                    slideList.Add(newSlide);
                }
                this.mcExcelView.ucSlideList.BindPages(slideList, true);
                this.mcExcelView.ucSlideList.SetMaterial(this.Material, true);
                //MessageBox.Show(this.Material.Slides.Count.ToString(), "Material.Slides");
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void PowerPointApp_SlideSelectionChanged1(SlideRange SldRange)
        {
            try
            {
                int slideIndex = SldRange.SlideIndex;
                this.mcExcelView.ucSlideList.MovePage(slideIndex);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void TabItem_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                wndExportToXML wndEX = new wndExportToXML();
                wndEX.Material = this.Material;
                wndEX.ShowDialog();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
