using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using MDM.Commons.Enum;
using MDM.Helpers;
using MDM.Models.DataModels;
//using MDM.Models.DataModels.ManualWorksXMLs;
using MDM.Models.ViewModels;
using Microsoft.Office.Interop.PowerPoint;
using OfficeOpenXml;
using static OfficeOpenXml.ExcelErrorValue;
using Path = System.IO.Path;

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

                string extension = fInfo.Extension;
                if(extension ==".json")
                {
                    LoadFromJson(fInfo);
                }
                else
                {
                    {

                    }
                }




            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void LoadFromJson(FileInfo fInfo)
        {
            mMaterial material = new mMaterial();
            material.Name = Path.GetFileNameWithoutExtension(fInfo.FullName);

            vmMaterial newMaterial = new vmMaterial(material);
            newMaterial.DirectoryPath = fInfo.DirectoryName;

            string jsonString = File.ReadAllText(fInfo.FullName);
            List<mContent> list = JsonHelper.ToObject<List<mContent>>(jsonString) as List<mContent>;
            Dictionary<mSlide, List<mContent>> slides = list.GroupBy(x => x.SlideIdx).ToDictionary(z => new mSlide() { Index = z.Key}, z => z.ToList());



            foreach (mSlide slide in slides.Keys)
            {
                List<mContent> contents = slides[slide];

                List<mItem> items = new List<mItem>();
                int order = 1;

                foreach (mContent content in contents)
                {
                    int level = 1;
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
                        newItem.ItemType = eItemType.Text.GetHashCode();
                        newItem.Level = level++;
                        newItem.LineText = value;

                        mShape newShape = new mShape(eShapeType.Text);
                        newShape.Top = newItem.Order;
                        newShape.Text = value;
                        newShape.Items.Add(newItem);

                        slide.Shapes.Add(newShape);
                    }

                    slide.Description = content.Description;

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
                    conShape.Items.Add(conItem);
                    slide.Shapes.Add(conShape);
                    
                }

                vmSlide newSlide = new vmSlide(slide);
                newSlide.SetParentMaterial(newMaterial);

            }




            #region 이전코드
            /*
            foreach (mContent content in list)
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
                            sameItem.SetParent(parentItem);
                        }
                        sameItem.SetParentSlide(newSlide);
                    }

                    // Heading
                    vmHeading sameHeading = newMaterial.Headings.Where(x => x.Temp.Name == header.LineText).FirstOrDefault();
                    if (sameHeading == null)
                    {
                        int index = headers.IndexOf(header);
                        mHeading hd = headings[index];
                        sameHeading = new vmHeading(hd);
                        sameHeading.SetMaterial(newMaterial);

                        if (index != 0)
                        {
                            mHeading phd = headings[index - 1];

                            vmHeading parentHeading = newMaterial.Headings.Where(x => x.Temp.Name == phd.Name).FirstOrDefault();
                            if (parentHeading != null) sameHeading.SetParent(parentHeading);
                        }
                    }

                    //Content
                    if (headers.Last() == header)
                    {
                        vmItem newConItem = new vmItem(contentItem);
                        newConItem.SetParent(sameItem);
                        newConItem.SetParentSlide(newSlide);
                    }
                }
                vmContent newContent = new vmContent(content);
                newContent.SetMaterial(newMaterial);
                newContent.SetHeading();
            }
            foreach (vmSlide sl in slides.Values) sl.SetParentMaterial(newMaterial);
             */
            #endregion

            this.Material = newMaterial;
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

