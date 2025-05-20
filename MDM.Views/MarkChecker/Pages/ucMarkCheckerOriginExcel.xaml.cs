using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Shell;
using MDM.Helpers;
using MDM.Models.DataModels;
using MDM.Models.ViewModels;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.PowerPoint;

namespace MDM.Views.MarkChecker.Pages
{
    /// <summary>
    /// ucMarkCheckerOriginExcel.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucMarkCheckerOriginExcel : UserControl
    {

        private vmMaterial _Material = null;
        public vmMaterial Material
        {
            get => _Material;
            set
            {
                _Material = value;
                this.dg_Contents.ItemsSource = value.Contents;
            }
        }


        public ucMarkCheckerOriginExcel()
        {
            InitializeComponent();
            
        }

        private void btn_FindPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<vmContent> checkContent = this.Material.Contents.ToList();
                List<vmSlide> slides = this.ucSlideList.listbox_Pages.ItemsSource as List<vmSlide>;

                int lastPageNum = 0;
                List<vmContent> tempList = new List<vmContent>();
                foreach (vmContent item in checkContent)
                {
                    if (item.Display_SlideNum.ToString() != "-1") continue;

                    if(item.Temp.ItemType != Commons.Enum.eItemType.Text)
                    {
                        vmContent last = tempList.LastOrDefault();
                        if (last != null && last.ParentHeading != item.ParentHeading) tempList.Clear();
                        tempList.Add(item);
                    }
                    else
                    {
                        int pageCnt = 0;
                        for (int num = 0; num < slides.Count; num++)
                        {
                            if (pageCnt == 5) break;

                            vmContent last = tempList.LastOrDefault();
                            if (last != null && last.ParentHeading != item.ParentHeading) tempList.Clear();
                            tempList.Add(item);

                            vmSlide current = slides[num];
                            if(current.Status.Status != Commons.Enum.ePageStatus.None) continue;
                            if (current.Temp.Index < lastPageNum) continue;

                            bool hasContent = HasContents(current, item);
                            pageCnt++;

                            if(hasContent)
                            {
                                lastPageNum = current.Temp.Index;
                                foreach (vmContent temp in tempList) temp.SetSlideNum(lastPageNum);
                                tempList.Clear();
                            }
                            else
                            {

                            }
                            if (tempList.Count == 0) break;
                        }
                        

                    }

                }

                tempList.Clear();
                 List<vmContent> noPageContents = checkContent.Where(x => x.Display_SlideNum.ToString() == "-1").ToList();
                foreach (vmContent noPageCon in noPageContents)
                {
                    if (noPageCon.Temp.ItemType != Commons.Enum.eItemType.Text)
                    {
                        vmContent last = tempList.LastOrDefault();
                        if (last != null && last.ParentHeading != noPageCon.ParentHeading) tempList.Clear();
                        tempList.Add(noPageCon);
                    }
                    else
                    {
                        int index = checkContent.IndexOf(noPageCon);
                        if (index == 0) continue;
                        if (index == checkContent.Count() - 1) continue;

                        vmContent preCon = checkContent[index - 1];
                        int preConPage = int.Parse(preCon.Display_SlideNum.ToString());
                        while (preConPage == -1)
                        {
                            int preIndex = checkContent.IndexOf(preCon);
                            if (preIndex == 0) break;
                            preCon = checkContent[preIndex - 1];
                            preConPage = int.Parse(preCon.Display_SlideNum.ToString());
                        }
                        if (preConPage == -1) continue;
                        vmContent nextCon = checkContent[index + 1];
                        int nextConPage = int.Parse(nextCon.Display_SlideNum.ToString());
                        while (nextConPage == -1)
                        {
                            int nexIndex = checkContent.IndexOf(nextCon);
                            if (nexIndex == checkContent.Count() - 1) break;
                            nextCon = checkContent[nexIndex + 1];
                            nextConPage = int.Parse(nextCon.Display_SlideNum.ToString());
                        }
                        if (nextConPage == -1) continue;

                        tempList.Add(noPageCon);
                        if (preConPage == nextConPage)
                        {
                            foreach (vmContent temp in tempList) temp.SetSlideNum(preConPage);
                            tempList.Clear();
                            continue;
                        }
                        else if (preConPage + 1 == nextConPage)
                        {

                            if (noPageCon.ParentHeading == preCon.ParentHeading)
                            {
                                foreach (vmContent temp in tempList) temp.SetSlideNum(preConPage);
                                tempList.Clear();
                                continue;
                            }
                            if (noPageCon.ParentHeading == nextCon.ParentHeading)
                            {
                                foreach (vmContent temp in tempList) temp.SetSlideNum(nextConPage);
                                tempList.Clear();
                                continue;
                            }
                        }
                        else
                        {
                            var selectSlides = slides.Where(x => preConPage < x.Temp.Index && x.Temp.Index < nextConPage);
                            foreach (vmSlide item in selectSlides)
                            {
                                bool hasContent = HasContent2(item, noPageCon);
                                if (hasContent)
                                {
                                    lastPageNum = item.Temp.Index;

                                    foreach (vmContent temp in tempList)
                                    {
                                        temp.SetSlideNum(lastPageNum);
                                        Debug.WriteLine($"Content : {temp.Temp.Temp.Idx} / PageNum : {lastPageNum}");
                                    }
                                    tempList.Clear();
                                }
                                else
                                {

                                }
                            }


                        }







                    }

                    
                }

                this.txtblock_NopageCount.Text = checkContent.Where(x => x.Display_SlideNum.ToString() == "-1").Count().ToString();

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_FindPage_Click2(object sender, RoutedEventArgs e)
        {
            try
            {
                List<vmContent> checkContent = this.Material.Contents.ToList();
                List<vmSlide> slides = this.ucSlideList.listbox_Pages.ItemsSource as List<vmSlide>;

                FirstCheck(checkContent, slides);
                BetweenCheck(checkContent, slides);
                ForthCheck(checkContent, slides);
                ThirdCheck(checkContent, slides);
                


                





                #region 이전 코드
                /*
                tempList.Clear();
                List<vmContent> noPageContents = checkContent.Where(x => x.Display_SlideNum.ToString() == "-1").ToList();
                foreach (vmContent noPageCon in noPageContents)
                {
                    if (noPageCon.Temp.ItemType != Commons.Enum.eItemType.Text)
                    {
                        vmContent last = tempList.LastOrDefault();
                        if (last != null && last.ParentHeading != noPageCon.ParentHeading) tempList.Clear();
                        tempList.Add(noPageCon);
                    }
                    else
                    {
                        int index = checkContent.IndexOf(noPageCon);
                        if (index == 0) continue;
                        if (index == checkContent.Count() - 1) continue;

                        vmContent preCon = checkContent[index - 1];
                        int preConPage = int.Parse(preCon.Display_SlideNum.ToString());
                        while (preConPage == -1)
                        {
                            int preIndex = checkContent.IndexOf(preCon);
                            if (preIndex == 0) break;
                            preCon = checkContent[preIndex - 1];
                            preConPage = int.Parse(preCon.Display_SlideNum.ToString());
                        }
                        if (preConPage == -1) continue;
                        vmContent nextCon = checkContent[index + 1];
                        int nextConPage = int.Parse(nextCon.Display_SlideNum.ToString());
                        while (nextConPage == -1)
                        {
                            int nexIndex = checkContent.IndexOf(nextCon);
                            if (nexIndex == checkContent.Count() - 1) break;
                            nextCon = checkContent[nexIndex + 1];
                            nextConPage = int.Parse(nextCon.Display_SlideNum.ToString());
                        }
                        if (nextConPage == -1) continue;

                        tempList.Add(noPageCon);
                        if (preConPage == nextConPage)
                        {
                            foreach (vmContent temp in tempList) temp.SetSlideNum(preConPage);
                            tempList.Clear();
                            continue;
                        }
                        else if (preConPage + 1 == nextConPage)
                        {

                            if (noPageCon.ParentHeading == preCon.ParentHeading)
                            {
                                foreach (vmContent temp in tempList) temp.SetSlideNum(preConPage);
                                tempList.Clear();
                                continue;
                            }
                            if (noPageCon.ParentHeading == nextCon.ParentHeading)
                            {
                                foreach (vmContent temp in tempList) temp.SetSlideNum(nextConPage);
                                tempList.Clear();
                                continue;
                            }
                        }
                        else
                        {
                            var selectSlides = slides.Where(x => preConPage < x.Temp.Index && x.Temp.Index < nextConPage);
                            foreach (vmSlide item in selectSlides)
                            {
                                bool hasContent = HasContent2(item, noPageCon);
                                if (hasContent)
                                {
                                    lastPageNum = item.Temp.Index;

                                    foreach (vmContent temp in tempList)
                                    {
                                        temp.SetSlideNum(lastPageNum);
                                        Debug.WriteLine($"Content : {temp.Temp.Temp.Idx} / PageNum : {lastPageNum}");
                                    }
                                    tempList.Clear();
                                }
                                else
                                {

                                }
                            }
                        }
                    }
                }
                 */
                #endregion


                this.txtblock_NopageCount.Text = this.Material.Contents.Where(x => x.Display_SlideNum.ToString() == "-1").Count().ToString();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_FindPage_Click3(object sender, RoutedEventArgs e)
        {
            try
            {
                List<vmSlide> slides = this.ucSlideList.listbox_Pages.ItemsSource as List<vmSlide>;

                Dictionary<vmHeading, List<vmSlide>> rootSlides = new Dictionary<vmHeading, List<vmSlide>>();
                foreach (vmHeading root in this.Material.RootHeadings)
                {
                    if (!rootSlides.ContainsKey(root)) rootSlides.Add(root, new List<vmSlide>());

                    string rootName = PreprocessingText(root.Temp.Name);
                    foreach (vmSlide slide in slides)
                    {
                        vmShape firstShape = slide.Shapes.FirstOrDefault();
                        if (firstShape == null) continue;


                        string origin = PreprocessingText(firstShape.Temp.Text);


                        if (origin.Contains(rootName) || rootName.Contains(origin))
                        {
                            rootSlides[root].Add(slide);
                        }
                        else
                        {
                            double similarity = TextHelper.CalculateSimilary2(origin, rootName);
                            if (similarity > 70)
                            {
                                rootSlides[root].Add(slide);
                            }
                        }



                    }
                }




                SetPageToContent(this.Material.RootHeadings.ToList(), slides);
                this.txtblock_NopageCount.Text = this.Material.Contents.Where(x => x.Display_SlideNum.ToString() == "-1").Count().ToString();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_FindPage_Click4(object sender, RoutedEventArgs e)
        {
            try
            {
                List<vmContent> checkContent = this.Material.Contents.ToList();
                List<vmSlide> slides = this.ucSlideList.listbox_Pages.ItemsSource as List<vmSlide>;

                CheckWithHeadingContent(checkContent, slides);
                BetweenCheck(checkContent, slides);




                this.txtblock_NopageCount.Text = this.Material.Contents.Where(x => x.Display_SlideNum.ToString() == "-1").Count().ToString();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void CheckWithHeadingContent(List<vmContent> checkContent, List<vmSlide> slides)
        {
            List<vmContent> tempList = new List<vmContent>();

            int lastSlideIndex = 0;
            foreach (vmContent con in checkContent)
            {
                int page = int.Parse(con.Display_SlideNum.ToString());
                if (page != -1) continue;

                if (con.Temp.ItemType != Commons.Enum.eItemType.Text)
                {
                    vmContent last = tempList.LastOrDefault();
                    if (last != null && last.ParentHeading != con.ParentHeading) tempList.Clear();
                    tempList.Add(con);
                    continue;
                }

                int pageCnt = 0;
                for (int num = lastSlideIndex; num < slides.Count; num++)
                {
                    if (pageCnt == 3) break;

                    vmContent last = tempList.LastOrDefault();
                    if (last != null && last.ParentHeading != con.ParentHeading) tempList.Clear();
                    tempList.Add(con);

                    vmSlide current = slides[num];
                    if (current.Status.Status != Commons.Enum.ePageStatus.None) continue;

                    bool hasContent = HasContent3(current, con);
                    pageCnt++;

                    if (hasContent)
                    {
                        lastSlideIndex = num;

                        int slideNum = int.Parse(current.Display_Index.ToString());
                        foreach (vmContent temp in tempList) temp.SetSlideNum(slideNum);
                        tempList.Clear();
                    }
                    else
                    {

                    }
                    if (tempList.Count == 0) break;
                }


                #region 이전코드
                /*
                 
                 string headerValue = PreprocessingText(con.ParentHeading.Temp.Name);
                string contentValue = PreprocessingText(con.Temp.Temp.LineText);

                int limitCnt = 0;
                for (int i = lastSlideIndex; i < slides.Count; i++)
                {
                    if (limitCnt == 3) break;
                    limitCnt++;

                    vmSlide currentSlide = slides[i];
                    bool hasHeader = HasSlideText(currentSlide, contentValue);
                    if(hasHeader)
                    {
                        bool hasContent = HasSlideText(currentSlide, headerValue);
                        
                        if (hasContent)
                        {
                            vmContent last = tempList.LastOrDefault();
                            if (last != null && last.ParentHeading != con.ParentHeading) tempList.Clear();
                            tempList.Add(con);
                            int slideNum = int.Parse(currentSlide.Display_Index.ToString());
                            foreach (vmContent temp in tempList) temp.SetSlideNum(slideNum);
                            lastSlideIndex = i;
                            tempList.Clear();
                        }
                    }
                } 

                 */
                #endregion
            }
        }








        private bool HasSlideText(vmSlide current, string headerValue)
        {
            for (int i = 0; i < current.Shapes.Count; i++)
            {
                vmShape shape = current.Shapes[i];
                string origin = PreprocessingText(shape.Temp.Text);

                if (origin.Contains(headerValue) || headerValue.Contains(origin)) return true;
                double similarity = TextHelper.CalculateSimilary2(origin, headerValue);
                if (similarity > 70) return true;
            }

            return false;
        }

        private void ForthCheck(List<vmContent> checkContent, List<vmSlide> slides)
        {
            int lastPageNum = 0;
            List<vmContent> tempList = new List<vmContent>();

            List<vmContent> noPageContents = checkContent.Where(x => x.Display_SlideNum.ToString() == "-1").ToList();
            foreach (vmContent noPageCon in noPageContents)
            {
                if (noPageCon.Temp.ItemType != Commons.Enum.eItemType.Text)
                {
                    vmContent last = tempList.LastOrDefault();
                    if (last != null && last.ParentHeading != noPageCon.ParentHeading) tempList.Clear();
                    tempList.Add(noPageCon);
                }
                else
                {
                    int index = checkContent.IndexOf(noPageCon);
                    if (index == 0) continue;
                    if (index == checkContent.Count() - 1) continue;

                    vmContent preCon = checkContent[index - 1];
                    int preConPage = int.Parse(preCon.Display_SlideNum.ToString());
                    while (preConPage == -1)
                    {
                        int preIndex = checkContent.IndexOf(preCon);
                        if (preIndex == 0) break;
                        preCon = checkContent[preIndex - 1];
                        preConPage = int.Parse(preCon.Display_SlideNum.ToString());
                    }
                    if (preConPage == -1) continue;
                    vmContent nextCon = checkContent[index + 1];
                    int nextConPage = int.Parse(nextCon.Display_SlideNum.ToString());
                    while (nextConPage == -1)
                    {
                        int nexIndex = checkContent.IndexOf(nextCon);
                        if (nexIndex == checkContent.Count() - 1) break;
                        nextCon = checkContent[nexIndex + 1];
                        nextConPage = int.Parse(nextCon.Display_SlideNum.ToString());
                    }
                    if (nextConPage == -1) continue;

                    tempList.Add(noPageCon);
                    if (preConPage == nextConPage)
                    {
                        foreach (vmContent temp in tempList) temp.SetSlideNum(preConPage);
                        tempList.Clear();
                        continue;
                    }
                    else if (preConPage + 1 == nextConPage)
                    {

                        if (noPageCon.ParentHeading == preCon.ParentHeading)
                        {
                            foreach (vmContent temp in tempList) temp.SetSlideNum(preConPage);
                            tempList.Clear();
                            continue;
                        }
                        if (noPageCon.ParentHeading == nextCon.ParentHeading)
                        {
                            foreach (vmContent temp in tempList) temp.SetSlideNum(nextConPage);
                            tempList.Clear();
                            continue;
                        }
                    }
                    else
                    {
                        var selectSlides = slides.Where(x => preConPage < x.Temp.Index && x.Temp.Index < nextConPage);
                        foreach (vmSlide item in selectSlides)
                        {
                            bool hasContent = HasContent2(item, noPageCon);
                            if (hasContent)
                            {
                                lastPageNum = item.Temp.Index;

                                foreach (vmContent temp in tempList)
                                {
                                    temp.SetSlideNum(lastPageNum);
                                    Debug.WriteLine($"Content : {temp.Temp.Temp.Idx} / PageNum : {lastPageNum}");
                                }
                                tempList.Clear();
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }
        }
        private void ThirdCheck(List<vmContent> checkContent, List<vmSlide> slides)
        {
            List<vmContent> noPageContents = checkContent.Where(x => x.Display_SlideNum.ToString() == "-1").ToList();

            foreach (vmContent con in noPageContents)
            {
                int index = checkContent.IndexOf(con);
                if (index == 0) continue;
                if (index == checkContent.Count() - 1) continue;

                vmContent preCon = checkContent[index - 1];
                int preSlideNum = int.Parse(preCon.Display_SlideNum.ToString());
                if (preSlideNum == -1) continue;

                string headerUid = GetHeaderString(con);
                string preHeaderUid = GetHeaderString(preCon);
                if (headerUid == preHeaderUid)
                {
                    con.SetSlideNum(preSlideNum);
                }
                else
                {
                    string preConParentHeader = preCon.ParentHeading.Temp.Name;
                    preConParentHeader = PreprocessingText(preConParentHeader);
                    if(preConParentHeader == PreprocessingText("전문가NOTE"))
                    {
                        con.SetSlideNum(preSlideNum + 1);
                    }
                }
            }
        }
        private void BetweenCheck(List<vmContent> checkContent, List<vmSlide> slides)
        {
            List<vmContent> tempList = new List<vmContent>();
            vmContent preContent = null;
            foreach (vmContent item in checkContent)
            {
                int page = int.Parse(item.Display_SlideNum.ToString());

                if (page == -1)
                {
                    tempList.Add(item);
                }
                else
                {
                    vmContent current = item;
                    int nextPage = int.Parse(current.Display_SlideNum.ToString());

                    if (tempList.Count() != 0)
                    {
                        if (preContent != null)
                        {
                            int prePage = int.Parse(preContent.Display_SlideNum.ToString());

                            if (prePage == nextPage)
                            {
                                foreach (vmContent noPageCon in tempList) noPageCon.SetSlideNum(prePage);
                            }
                            /*
                            else if (prePage + 1 == nextPage)
                            {
                                foreach (vmContent noPageCon in tempList)
                                {
                                    int lv = 1;
                                    while (lv <= 10)
                                    {
                                        string noPageHeaderUid = string.Empty;
                                        string preHeaderUid = string.Empty;
                                        string nextHeaderUid = string.Empty;

                                        switch (lv++)
                                        {
                                            case 1:
                                                if (noPageCon.Heading1 != null) noPageHeaderUid = noPageCon.Heading1.Temp.Uid;
                                                if (preContent.Heading1 != null) preHeaderUid = preContent.Heading1.Temp.Uid;
                                                if (current.Heading1 != null) nextHeaderUid = current.Heading1.Temp.Uid;
                                                break;
                                            case 2:
                                                if (noPageCon.Heading2 != null) noPageHeaderUid = noPageCon.Heading2.Temp.Uid;
                                                if (preContent.Heading2 != null) preHeaderUid = preContent.Heading2.Temp.Uid;
                                                if (current.Heading2 != null) nextHeaderUid = current.Heading2.Temp.Uid;
                                                break;
                                            case 3:
                                                if (noPageCon.Heading3 != null) noPageHeaderUid = noPageCon.Heading3.Temp.Uid;
                                                if (preContent.Heading3 != null) preHeaderUid = preContent.Heading3.Temp.Uid;
                                                if (current.Heading3 != null) nextHeaderUid = current.Heading3.Temp.Uid;
                                                break;
                                            case 4:
                                                if (noPageCon.Heading4 != null) noPageHeaderUid = noPageCon.Heading4.Temp.Uid;
                                                if (preContent.Heading4 != null) preHeaderUid = preContent.Heading4.Temp.Uid;
                                                if (current.Heading4 != null) nextHeaderUid = current.Heading4.Temp.Uid;
                                                break;
                                            case 5:
                                                if (noPageCon.Heading5 != null) noPageHeaderUid = noPageCon.Heading5.Temp.Uid;
                                                if (preContent.Heading5 != null) preHeaderUid = preContent.Heading5.Temp.Uid;
                                                if (current.Heading5 != null) nextHeaderUid = current.Heading5.Temp.Uid;
                                                break;
                                            case 6:
                                                if (noPageCon.Heading6 != null) noPageHeaderUid = noPageCon.Heading6.Temp.Uid;
                                                if (preContent.Heading6 != null) preHeaderUid = preContent.Heading6.Temp.Uid;
                                                if (current.Heading6 != null) nextHeaderUid = current.Heading6.Temp.Uid;
                                                break;
                                            case 7:
                                                if (noPageCon.Heading7 != null) noPageHeaderUid = noPageCon.Heading7.Temp.Uid;
                                                if (preContent.Heading7 != null) preHeaderUid = preContent.Heading7.Temp.Uid;
                                                if (current.Heading7 != null) nextHeaderUid = current.Heading7.Temp.Uid;
                                                break;
                                            case 8:
                                                if (noPageCon.Heading8 != null) noPageHeaderUid = noPageCon.Heading8.Temp.Uid;
                                                if (preContent.Heading8 != null) preHeaderUid = preContent.Heading8.Temp.Uid;
                                                if (current.Heading8 != null) nextHeaderUid = current.Heading8.Temp.Uid;
                                                break;
                                            case 9:
                                                if (noPageCon.Heading9 != null) noPageHeaderUid = noPageCon.Heading9.Temp.Uid;
                                                if (preContent.Heading9 != null) preHeaderUid = preContent.Heading9.Temp.Uid;
                                                if (current.Heading9 != null) nextHeaderUid = current.Heading9.Temp.Uid;
                                                break;
                                            case 10:
                                                if (noPageCon.Heading10 != null) noPageHeaderUid = noPageCon.Heading10.Temp.Uid;
                                                if (preContent.Heading10 != null) preHeaderUid = preContent.Heading10.Temp.Uid;
                                                if (current.Heading10 != null) nextHeaderUid = current.Heading10.Temp.Uid;
                                                break;
                                            default:
                                                break;
                                        }

                                        bool isSamePre = noPageHeaderUid == preHeaderUid;
                                        bool isSameNext = noPageHeaderUid == nextHeaderUid;

                                        if (isSamePre && isSameNext) continue;

                                        if (isSamePre)
                                        {
                                            noPageCon.SetSlideNum(prePage);
                                            break;
                                        }
                                        if (isSameNext)
                                        {
                                            noPageCon.SetSlideNum(nextPage);
                                            break;
                                        }
                                    }
                                }
                            }
                                */
                            else
                            {
                                var betweenSlides = slides.Where(x => prePage < int.Parse(x.Display_Index.ToString()) && int.Parse(x.Display_Index.ToString()) < nextPage);
                                if (betweenSlides.Count() == 1)
                                {
                                    vmSlide between = betweenSlides.First();
                                    int settingPage = int.Parse(between.Display_Index.ToString());
                                    foreach (vmContent noPageCon in tempList) noPageCon.SetSlideNum(settingPage);
                                }
                            }
                        
                        }
                        else
                        {
                            foreach (vmContent noPageCon in tempList) noPageCon.SetSlideNum(nextPage);
                        }
                    }





                    preContent = current;
                    tempList.Clear();
                }




            }
        }
        private void FirstCheck(List<vmContent> checkContent,  List<vmSlide> slides)
        {
            int lastPageNum = 0;
            List<vmContent> tempList = new List<vmContent>();
            foreach (vmContent item in checkContent)
            {
                if (item.Display_SlideNum.ToString() != "-1") continue;

                if (item.Temp.ItemType != Commons.Enum.eItemType.Text)
                {
                    vmContent last = tempList.LastOrDefault();
                    if (last != null && last.ParentHeading != item.ParentHeading) tempList.Clear();
                    tempList.Add(item);
                }
                else
                {
                    int pageCnt = 0;
                    int lastIndex = 0;
                    for (int num = lastIndex; num < slides.Count; num++)
                    {
                        if (pageCnt == 5) break;

                        vmContent last = tempList.LastOrDefault();
                        if (last != null && last.ParentHeading != item.ParentHeading) tempList.Clear();
                        tempList.Add(item);

                        vmSlide current = slides[num];
                        if (current.Status.Status != Commons.Enum.ePageStatus.None) continue;
                        if (current.Temp.Index < lastPageNum) continue;

                        bool hasContent = HasContents(current, item);
                        pageCnt++;

                        if (hasContent)
                        {
                            lastIndex = num;
                            lastPageNum = current.Temp.Index;
                            foreach (vmContent temp in tempList) temp.SetSlideNum(lastPageNum);
                            tempList.Clear();
                        }
                        else
                        {

                        }
                        if (tempList.Count == 0) break;
                    }


                }

            }
        }


        

        private void SetPageToContent(List<vmHeading> children, List<vmSlide> slides)
        {
            foreach (vmHeading header in children)
            {
                string headingName = PreprocessingText(header.Temp.Name);

                List<vmSlide> filteredSlides = FilteredSlides(slides, headingName);
                if (filteredSlides.Count() == 0) return;

                int lastPageNum = filteredSlides.Min(x => x.Temp.SlideNumber);
                List<vmContent> tempList = new List<vmContent>();
                foreach (vmContent content in header.Contents)
                {
                    tempList.Add(content);
                    if (content.Temp.ItemType != Commons.Enum.eItemType.Text) continue;

                    string value = content.Temp.Temp.LineText;

                    foreach (vmSlide fSlide in filteredSlides)
                    {
                        if (fSlide.Temp.SlideNumber < lastPageNum) continue;

                        bool hasContent = HasOriginContent(fSlide, content);
                        if (hasContent)
                        {
                            lastPageNum = fSlide.Temp.Index;
                            foreach (vmContent temp in tempList) temp.SetSlideNum(lastPageNum);
                            tempList.Clear();
                        }

                        if (tempList.Count == 0) break;
                    }


                }

                SetPageToContent(header.Children.ToList(), filteredSlides);

            }
        }

        private List<vmSlide> FilteredSlides(List<vmSlide> slides, string headingName)
        {
            List<vmSlide> output = new List<vmSlide>();

            foreach (vmSlide item in slides)
            {
                for (int i = 0; i < item.Shapes.Count; i++)
                {
                    vmShape shape = item.Shapes[i];
                    string origin = PreprocessingText(shape.Temp.Text);
                    if (origin.Contains(headingName) || headingName.Contains(origin))
                    {
                        output.Add(item);
                        break;
                    }
                    else
                    {
                        double similarity = TextHelper.CalculateSimilary2(origin, headingName);
                        if (similarity > 70)
                        {
                            output.Add(item);
                            break;
                        }
                    }
                }
            }

            return output;
        }

        private bool HasContents(vmSlide current, vmContent con)
        {
            string parentHeading = PreprocessingText(con.ParentHeading.Temp.Name);
            string paparentHeading = con.ParentHeading.Parent == null ? string.Empty : PreprocessingText(con.ParentHeading.Parent.Temp.Name);
            string content = PreprocessingText(con.Temp.Temp.LineText);

            bool hasParenaHeading = false;
            bool hasHeading = false;
            for (int i = 0; i < current.Shapes.Count; i++)
            {
                vmShape shape = current.Shapes[i];
                string origin = PreprocessingText(shape.Temp.Text);

                if (!hasParenaHeading)
                {
                    if(origin.Contains(paparentHeading) || paparentHeading.Contains(origin))
                    {
                        hasParenaHeading = true;
                        i = i < 2 ? -1 : i - 2;
                        continue;
                    }
                    else
                    {
                        double similarity = TextHelper.CalculateSimilary2(origin, paparentHeading);
                        if (similarity > 70)
                        {
                            hasParenaHeading = true;
                            i = i < 2 ? -1 : i - 2;
                            continue;
                        }
                    }
                }

                if (hasParenaHeading && !hasHeading )
                {
                    if (origin.Contains(parentHeading) || parentHeading.Contains(origin))
                    {
                        hasHeading = true;
                        i = i < 2 ? -1: i - 2;
                        continue;
                    }
                    else
                    {
                        double similarity = TextHelper.CalculateSimilary2(origin, parentHeading);
                        if (similarity > 70)
                        {
                            hasHeading = true;
                            i = i < 2 ? -1 : i - 2;
                            continue;
                        }
                    }
                    
                }

                if (hasParenaHeading && hasHeading)
                {
                    if (origin.Contains(content) || content.Contains(origin)) return true;
                    double similarity = TextHelper.CalculateSimilary2(origin, content);
                    if (similarity > 70) return true;
                }
            }
            return false;
        }
        private bool HasOriginContent(vmSlide current, vmContent con)
        {
            string content = PreprocessingText(con.Temp.Temp.LineText);
            for (int i = 0; i < current.Shapes.Count; i++)
            {
                vmShape shape = current.Shapes[i];
                string origin = PreprocessingText(shape.Temp.Text);


                if (origin.Contains(content) || content.Contains(origin)) return true;
                double similarity = TextHelper.CalculateSimilary2(origin, content);
                if (similarity > 70) return true;
            }

            return false;
        }
        private bool HasContent2(vmSlide current, vmContent con)
        {
            string parentHeading = PreprocessingText(con.ParentHeading.Temp.Name);
            string paparentHeading = con.ParentHeading.Parent == null ? string.Empty : PreprocessingText(con.ParentHeading.Parent.Temp.Name);
            string content = PreprocessingText(con.Temp.Temp.LineText);
            
            for (int i = 0; i < current.Shapes.Count; i++)
            {
                vmShape shape = current.Shapes[i];
                string origin = PreprocessingText(shape.Temp.Text);

                {
                    if (origin.Contains(content) || content.Contains(origin)) return true;
                    double similarity = TextHelper.CalculateSimilary2(origin, content);
                    if (similarity > 70) return true;
                }
            }
            return false;
        }
        private bool HasContent3(vmSlide current, vmContent con)
        {
            List<vmHeading> headers = new List<vmHeading>();
            vmHeading parentHeader = con.ParentHeading;
            while(parentHeader != null)
            {
                headers.Add(parentHeader);
                parentHeader = parentHeader.Parent;
            }

            //headers.Reverse();
            List<vmShape> reverseShapes = current.Shapes.ToList();
            reverseShapes.Reverse();
                

            int lastShapeIndex = 0;
            for (int headerIndex = 1; headerIndex < headers.Count; headerIndex++)
            {
                vmHeading item = headers[headerIndex];

                bool hasHeader = false;
                string headerValue = PreprocessingText(item.Temp.Name);
                for (int shapeIndex = lastShapeIndex; shapeIndex < current.Shapes.Count; shapeIndex++)
                {
                    vmShape shape = reverseShapes[shapeIndex];
                    string origin = PreprocessingText(shape.Temp.Text);

                    if (origin.Contains(headerValue) || headerValue.Contains(origin))
                    {
        
                        shapeIndex = shapeIndex < 2 ? -1 : shapeIndex - 2;
                        lastShapeIndex = shapeIndex + 1;
                        hasHeader = true;
                        break;
                    }
                    else
                    {
                        double similarity = TextHelper.CalculateSimilary2(origin, headerValue);
                        if (similarity > 70)
                        {
                            shapeIndex = shapeIndex < 2 ? -1 : shapeIndex - 2;
                            lastShapeIndex = shapeIndex + 1;
                            hasHeader = true;
                            break;
                        }
                    }
                }

       

             
                if (!hasHeader) return false;
            }
       

            string contentValue = PreprocessingText(con.Temp.Temp.LineText);
            bool hasContent = false;

            
            for (int i = 0; i < current.Shapes.Count; i++)
            {
                vmShape shape = current.Shapes[i];
                string origin = PreprocessingText(shape.Temp.Text);

                if (origin.Contains(contentValue))
                {
                    if (current.Display_Index.ToString() == "19")
                    {

                    }
                    return true;
                }
                else
                {
                    
                    double similarity = TextHelper.CalculateSimilary2(origin, contentValue);
                    if (similarity > 70)
                    {
                        if (current.Display_Index.ToString() == "19")
                        {

                        }


                        return true;
                    }
                }

            }

            return hasContent;
        }
        private string PreprocessingText(string value)
        {
            char[] marks = { '#', '*' };

            string output = string.Empty;
            foreach (string ln in TextHelper.SplitText(value))
            {
                if (TextHelper.IsNoText(ln)) continue;

                string line = TextHelper.RemoveEmtpy(ln);
                line = TextHelper.RemoveSpecialChar(line);

                if (TextHelper.IsNoText(line)) continue;
                while (marks.Contains(line.First())) line = line.Substring(1);

                output += line.ToLower();
            }

            return output;
        }
        private string GetHeaderString(vmContent content)
        {
            string output = string.Empty;

            List<string> uids = new List<string>();

            vmHeading parent = content.ParentHeading;
            while (parent != null)
            {
                string uid = parent.Temp.Uid;
                uids.Add(uid);
                parent = parent.Parent;
            }

            uids.Reverse();

            foreach (string uid in uids)
            {
                output += uid;
            }


            return output;

        }

        private void btn_reset_click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent item in this.Material.Contents)
                {
                    item.SetSlideNum(-1);
                }
                this.txtblock_NopageCount.Text = this.Material.Contents.Where(x => x.Display_SlideNum.ToString() == "-1").Count().ToString();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
