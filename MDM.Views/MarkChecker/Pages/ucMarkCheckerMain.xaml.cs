using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using MDM.Commons.Enum;
using MDM.Helpers;
using MDM.Models.DataModels;
//using MDM.Models.DataModels.ManualWorksXMLs;
using MDM.Models.ViewModels;
using Microsoft.Office.Interop.PowerPoint;
using static OfficeOpenXml.ExcelErrorValue;

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

                        mItem conItem = new mItem();
                        conItem.ItemType = content.ContentsType;
                        conItem.Level = GetContentLevel(content);
                        conItem.LineText = content.Contents;
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


                DirectoryInfo dInfo = fInfo.Directory;
                List<mHeading> headings = null;
                FileInfo headerInfo = dInfo.GetFiles("*.headers", SearchOption.TopDirectoryOnly).OrderBy(x => x.Name).LastOrDefault();
                if(headerInfo != null)
                {
                    string headerJsonString = File.ReadAllText(headerInfo.FullName);
                    headings = JsonHelper.ToObject<List<mHeading>>(headerJsonString);
                }
                if (headings == null) headings = new List<mHeading>();

                List<vmItem> allitems = new List<vmItem>();
                foreach (mSlide slide in slides)
                {
                    vmSlide newSlide = new vmSlide(slide);
                    newSlide.SetParentMaterial(newMaterial);
                    if (headings.Count > 0)
                    {
                        foreach (vmItem item in newSlide.Items) allitems.Add(item);
                        continue;
                    }
                    newSlide.ConvertAndSetContents();
                }

                foreach (mHeading heading in headings)
                {
                    SetHeading(heading, null, newMaterial, allitems);
                }

                #region 이전 코드
                /*
            

                */

                /*
                Dictionary<int, List<mContent>> slides = new Dictionary<int, List<mContent>>();
                foreach (mContent content in list) 
                {
                    int slideNum = content.SlideIdx;
                    if (!slides.ContainsKey(slideNum)) slides.Add(slideNum, new List<mContent>());
                    slides[slideNum].Add(content);
                }
                foreach (int key in slides.Keys)
                {
                    mSlide newSlide = new mSlide();
                    newSlide.SlideNumber = key;

                    List<mContent> contents = slides[key];
                    Dictionary<int, List<mHeading>> levelOfHeadings = new Dictionary<int, List<mHeading>>();
                    for (int i = 1; i <= 10; i++)
                    {
                        List<string> names = new List<string>();
                        switch (i)
                        {
                            case 1: names = contents.Select(x => x.Heading1String).Distinct().ToList(); break;
                            case 2: names = contents.Select(x => x.Heading2String).Distinct().ToList(); break;
                            case 3: names = contents.Select(x => x.Heading3String).Distinct().ToList(); break;
                            case 4: names = contents.Select(x => x.Heading4String).Distinct().ToList(); break;
                            case 5: names = contents.Select(x => x.Heading5String).Distinct().ToList(); break;
                            case 6: names = contents.Select(x => x.Heading6String).Distinct().ToList(); break;
                            case 7: names = contents.Select(x => x.Heading7String).Distinct().ToList(); break;
                            case 8: names = contents.Select(x => x.Heading8String).Distinct().ToList(); break;
                            case 9: names = contents.Select(x => x.Heading9String).Distinct().ToList(); break;
                            case 10: names = contents.Select(x => x.Heading10String).Distinct().ToList(); break;
                            default: break;
                        }
                        List<mHeading> headingList = new List<mHeading>();
                        foreach (string name in names)
                        {
                            if(string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name)) continue;
                            mHeading newHeading = new mHeading();
                            newHeading.Level = i;
                            newHeading.Name = name; 
                        }
                        levelOfHeadings.Add(i, headingList);
                    }
                }
                */

                /*
                foreach (int slideNum in slides.Keys)
                {
                    mSlide slide = new mSlide();
                    slide.SlideNumber = slideNum;

                    //List<mItem> items = new List<mItem>();
                    int order = 1;
                    
                    foreach (mContent content in slides[slideNum])
                    {
                        int level = 1;
                        #region Heading
                        
                        for (int i = 1; i < 10; i++)
                        {
                            string value = string.Empty;
                            switch (i)
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

                            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value)) continue;

                            mItem newItem = new mItem();
                            newItem.Order = order++;
                            newItem.ItemType = eItemType.Header.GetHashCode();
                            newItem.Level = level;
                            newItem.LineText = value;

                            mShape newShape = new mShape(eShapeType.Text);
                            newShape.Top = newItem.Order;
                            newShape.Text = value;
                            newShape.Lines.Add(newItem);

                            slide.Shapes.Add(newShape);

                            level++;
                        }
                        #endregion
                        #region 본문

                        string contentValue = content.Contents;
                        if (string.IsNullOrEmpty(contentValue) || string.IsNullOrWhiteSpace(contentValue)) continue;

                        mItem conItem = new mItem();
                        conItem.Order = order++;
                        conItem.ItemType = content.ContentsType;
                        conItem.Level = level++;
                        conItem.LineText = contentValue;

                        mShape conShape = new mShape((eShapeType)conItem.ItemType);
                        conShape.Top = conItem.Order;
                        conShape.Text = contentValue;
                        conShape.Lines.Add(conItem);
                        slide.Shapes.Add(conShape);

                        #endregion
                        slide.Description = content.Description;
                    }

                    vmSlide newSlide = new vmSlide(slide);
                    newSlide.SetParentMaterial(newMaterial);

                    vmHeading lastHeading = null;
                    foreach (vmItem item in newSlide.Items)
                    {
                        if(item.ItemType == eItemType.Header)
                        {
                            vmHeading sameHeading = newMaterial.Headings.Where(x => x.Temp.Level == item.Temp.Level && x.Temp.Name == item.Temp.LineText).FirstOrDefault();
                            if (sameHeading == null)
                            {
                                mHeading heading = new mHeading();
                                heading.Level= item.Temp.Level;
                                heading.Name = item.Temp.LineText;

                                vmHeading newHeading = new vmHeading(heading);
                                newHeading.SetParentMaterial(newMaterial);
                                newHeading.SetParent(lastHeading);
                                lastHeading = newHeading;
                            }
                        }
                        else
                        {
                            vmContent newContent = new vmContent(item);
                            newContent.SetParentMaterial(newMaterial);
                            newContent.SetParentHeading(lastHeading);   
                        }
                    }
                }

                */

                /*
                 * 
             
                Dictionary<int, vmSlide> slides = new Dictionary<int, vmSlide>();
                foreach (mContent content in contentList)
                {
                    int slideNum = content.SlideIdx;
                    if (!slides.ContainsKey(slideNum))
                    {
                        mSlide slide = new mSlide();
                        slide.Index = slideNum;
                        slides.Add(slideNum, new vmSlide(slide));
                    }

                    vmSlide newSlide = slides[slideNum];

                    List<mItem> headers = new List<mItem>();
                    List<mHeading> headings = new List<mHeading>();

                    mItem head1Item = null;
                    mHeading heading1 = null;
                    if (!string.IsNullOrEmpty(content.Heading1String))
                    {
                        head1Item = new mItem();
                        head1Item.Level = 1;
                        head1Item.LineText = content.Heading1String.Trim();
                        head1Item.ItemType = (int)eItemType.Header;
                        headers.Add(head1Item);

                        heading1 = new mHeading();
                        heading1.Level = head1Item.Level;
                        heading1.Name = head1Item.LineText;
                        headings.Add(heading1);
                    }
                    mItem head2Item = null;
                    mHeading heading2 = null;
                    if (!string.IsNullOrEmpty(content.Heading2String))
                    {
                        head2Item = new mItem();
                        head2Item.Level = 2;
                        head2Item.LineText = content.Heading2String.Trim();
                        head2Item.ItemType = (int)eItemType.Header;
                        headers.Add(head2Item);

                        heading2 = new mHeading();
                        heading2.Level = head2Item.Level;
                        heading2.Name = head2Item.LineText;
                        headings.Add(heading2);
                    }
                    mItem head3Item = null;
                    mHeading heading3 = null;
                    if (!string.IsNullOrEmpty(content.Heading3String))
                    {
                        head3Item = new mItem();
                        head3Item.Level = 3;
                        head3Item.LineText = content.Heading3String.Trim();
                        head3Item.ItemType = (int)eItemType.Header;
                        headers.Add(head3Item);

                        heading3 = new mHeading();
                        heading3.Level = head3Item.Level;
                        heading3.Name = head3Item.LineText;
                        headings.Add(heading3);
                    }
                    mItem head4Item = null;
                    mHeading heading4 = null;
                    if (!string.IsNullOrEmpty(content.Heading4String))
                    {
                        head4Item = new mItem();
                        head4Item.Level = 4;
                        head4Item.LineText = content.Heading4String.Trim();
                        head4Item.ItemType = (int)eItemType.Header;
                        headers.Add(head4Item);

                        heading4 = new mHeading();
                        heading4.Level = head4Item.Level;
                        heading4.Name = head4Item.LineText;
                        headings.Add(heading4);
                    }
                    mItem head5Item = null;
                    mHeading heading5 = null;
                    if (!string.IsNullOrEmpty(content.Heading5String))
                    {
                        head5Item = new mItem();
                        head5Item.Level = 5;
                        head5Item.LineText = content.Heading5String.Trim();
                        head5Item.ItemType = (int)eItemType.Header;
                        headers.Add(head5Item);

                        heading5 = new mHeading();
                        heading5.Level = head5Item.Level;
                        heading5.Name = head5Item.LineText;
                        headings.Add(heading5);
                    }
                    mItem head6Item = null;
                    mHeading heading6 = null;
                    if (!string.IsNullOrEmpty(content.Heading6String))
                    {
                        head6Item = new mItem();
                        head6Item.Level = 5;
                        head6Item.LineText = content.Heading6String.Trim();
                        head6Item.ItemType = (int)eItemType.Header;
                        headers.Add(head6Item);

                        heading6 = new mHeading();
                        heading6.Level = head6Item.Level;
                        heading6.Name = head6Item.LineText;
                        headings.Add(heading6);
                    }
                    mItem head7Item = null;
                    mHeading heading7 = null;
                    if (!string.IsNullOrEmpty(content.Heading7String))
                    {
                        head7Item = new mItem();
                        head7Item.Level = 5;
                        head7Item.LineText = content.Heading7String.Trim();
                        head7Item.ItemType = (int)eItemType.Header;
                        headers.Add(head7Item);

                        heading7 = new mHeading();
                        heading7.Level = head7Item.Level;
                        heading7.Name = head7Item.LineText;
                        headings.Add(heading7);
                    }
                    mItem head8Item = null;
                    mHeading heading8 = null;
                    if (!string.IsNullOrEmpty(content.Heading8String))
                    {
                        head8Item = new mItem();
                        head8Item.Level = 5;
                        head8Item.LineText = content.Heading8String.Trim();
                        head8Item.ItemType = (int)eItemType.Header;
                        headers.Add(head8Item);

                        heading8 = new mHeading();
                        heading8.Level = head8Item.Level;
                        heading8.Name = head8Item.LineText;
                        headings.Add(heading8);
                    }
                    mItem head9Item = null;
                    mHeading heading9 = null;
                    if (!string.IsNullOrEmpty(content.Heading9String))
                    {
                        head9Item = new mItem();
                        head9Item.Level = 5;
                        head9Item.LineText = content.Heading9String.Trim();
                        head9Item.ItemType = (int)eItemType.Header;
                        headers.Add(head9Item);

                        heading9 = new mHeading();
                        heading9.Level = head9Item.Level;
                        heading9.Name = head9Item.LineText;
                        headings.Add(heading9);
                    }
                    mItem head10Item = null;
                    mHeading heading10 = null;
                    if (!string.IsNullOrEmpty(content.Heading10String))
                    {
                        head10Item = new mItem();
                        head10Item.Level = 5;
                        head10Item.LineText = content.Heading10String.Trim();
                        head10Item.ItemType = (int)eItemType.Header;
                        headers.Add(head10Item);

                        heading10 = new mHeading();
                        heading10.Level = head9Item.Level;
                        heading10.Name = head9Item.LineText;
                        headings.Add(heading10);
                    }

                    mItem contentItem = new mItem();
                    contentItem.LineText = content.Contents.Trim();
                    contentItem.ItemType = content.ContentsType;

                    foreach (mItem header in headers)
                    {
                        //Item
                        vmItem sameItem = newSlide.Items.Where(x => x.Temp.LineText == header.LineText).FirstOrDefault();
                        if (sameItem == null)
                        {
                            sameItem = new vmItem(header);

                            vmItem parentItem = newSlide.Items.Where(x => x.ItemType == eItemType.Header && x.Temp.Level == header.Level - 1).LastOrDefault();
                            if (parentItem != null)
                            {
                                sameItem.SetParentItem(parentItem);
                                parentItem.AddChild(sameItem);
                            }

                            newSlide.Items.Add(sameItem);
                        }

                        // Heading
                        vmHeading sameHeading = newMaterial.Headings.Where(x => x.Temp.Name == header.LineText).FirstOrDefault();
                        if (sameHeading == null)
                        {
                            int index = headers.IndexOf(header);
                            mHeading hd = headings[index];
                            sameHeading = new vmHeading(hd);
                            sameHeading.SetParentMaterial(newMaterial);

                            if (index != 0)
                            {
                                mHeading phd = headings[index - 1];

                                vmHeading parentHeading = newMaterial.Headings.Where(x => x.Temp.Name == phd.Name).FirstOrDefault();
                                if (parentHeading != null) sameHeading.SetParent(parentHeading);// parentHeading.AddChild(sameHeading);
                            }





                        }

                        //Content
                        if (headers.Last() == header)
                        {
                            vmItem newConItem = new vmItem(contentItem);
                            newConItem.SetParentItem(sameItem);
                            sameItem.AddChild(newConItem);
                            newSlide.Items.Add(newConItem);
                        }
                    }
                    vmContent newContent = new vmContent(content);
                    newMaterial.AddContent(newContent);
                    newContent.SetHeading();
                }

                foreach (vmSlide sl in slides.Values) newMaterial.AddSlide(sl);

                
                */

                #endregion
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
