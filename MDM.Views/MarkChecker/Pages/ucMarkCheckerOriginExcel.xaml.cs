using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MDM.Helpers;
using MDM.Models.DataModels;
using MDM.Models.ViewModels;

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
                                    foreach (vmContent temp in tempList) temp.SetSlideNum(lastPageNum);
                                    tempList.Clear();
                                }
                                else
                                {

                                }
                            }


                        }







                    }

                    this.txtblock_NopageCount.Text = checkContent.Where(x => x.Display_SlideNum.ToString() == "-1").Count().ToString();
                }

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
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

                if (!hasParenaHeading )
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

        private string PreprocessingText(string value)
        {
            char[] marks = { '#', '*' };

            string output = string.Empty;
            foreach (string ln in TextHelper.SplitText(value))
            {
                if (TextHelper.IsNoText(ln)) continue;

                string line = TextHelper.RemoveEmtpy(ln);
                while (marks.Contains(line.First())) line = line.Substring(1);

                output += line.ToLower();
            }

            return output;
        }
    }
}
