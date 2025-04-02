using MDM.Commons.Enum;
using MDM.Helpers;
using MDM.Models.DataModels;
using MDM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MDM.Views.MarkChecker.Pages
{
    /// <summary>
    /// ucMarkCheckerMain.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucMarkCheckerMain : UserControl
    {
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
                FileInfo fInfo = FileHelper.GetOpenFileInfo();
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


                    Dictionary<int, List<mContent>> slideDic = new Dictionary<int, List<mContent>>();
                    foreach (mContent content in contentList)
                    {
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

                            int level = 1;
                            mItem header1 = GetSameItem(level++, content, items);
                            if (header1 != null && !items.Contains(header1)) items.Add(header1);
                            mItem header2 = GetSameItem(level++, content, items);
                            if (header2 != null && !items.Contains(header2)) items.Add(header2);
                            mItem header3 = GetSameItem(level++, content, items);
                            if (header3 != null && !items.Contains(header3)) items.Add(header3);
                            mItem header4 = GetSameItem(level++, content, items);
                            if (header4 != null && !items.Contains(header4)) items.Add(header4);
                            mItem header5 = GetSameItem(level++, content, items);
                            if (header5 != null && !items.Contains(header5)) items.Add(header5);
                            mItem header6 = GetSameItem(level++, content, items);
                            if (header6 != null && !items.Contains(header6)) items.Add(header6);
                            mItem header7 = GetSameItem(level++, content, items);
                            if (header7 != null && !items.Contains(header7)) items.Add(header7);
                            mItem header8 = GetSameItem(level++, content, items);
                            if (header8 != null && !items.Contains(header8)) items.Add(header8);
                            mItem header9 = GetSameItem(level++, content, items);
                            if (header9 != null && !items.Contains(header9)) items.Add(header9);
                            mItem header10 = GetSameItem(level++, content, items);
                            if (header10 != null && !items.Contains(header10)) items.Add(header10);
                            
                            if(key == 237)
                            {
                             
                            }

                            mItem conItem = new mItem();
                            conItem.ItemType = content.ContentsType;
                            conItem.Level = GetContentLevel(content);
                            conItem.LineText = content.Contents;
                            if(conItem.ItemType == eItemType.Image.GetHashCode()) conItem.Title = TextHelper.GetImageTitleFromMarkdown(conItem.LineText);
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
                //xmlBook book = new xmlBook();
                //book.Id = XMLHelper.GenerateUUId(8);
                //book.Title = this.Material.Temp.Name;
                //book.CreateDate = DateTime.Now;
                //book.Type = eXMLBookType.BOOK;
                //book.Locale = eXMLLocale.ko;
                //book.Edition = "수행지침";
                //book.Tags.Append("수행지침");
                //book.Tags.Append(this.Material.Temp.Name);

                
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
