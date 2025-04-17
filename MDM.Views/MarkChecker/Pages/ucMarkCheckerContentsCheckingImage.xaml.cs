using MDM.Commons.Enum;
using MDM.Helpers;
using MDM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using static OfficeOpenXml.ExcelErrorValue;


namespace MDM.Views.MarkChecker.Pages
{
    /// <summary>
    /// ucMarkCheckerContentsCheckingImage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucMarkCheckerContentsCheckingImage : UserControl
    {
        private vmMaterial _Material = null;
        private string _SearchKeyword = string.Empty;
        public vmMaterial Material
        {
            get => _Material;
            set
            {
                _Material = value;
                SetOriginList();

                this.txtbox_SearchKeyword.Text = string.Empty;
                this.rBtn_All.IsChecked = true;
                BindList();
                BindPageComboBox();
            }
        }

        

        public ObservableCollection<vmContent> Origin { get; set; } = new ObservableCollection<vmContent>();


        public bool? ImageStatus { get; set; } = null;
        public string SearchKeyword
        {
            get => _SearchKeyword;
            set
            {
                _SearchKeyword = value;
                this.btn_RemoveSearchKeyword.Visibility = string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;
            }
        }


        public ucMarkCheckerContentsCheckingImage()
        {
            InitializeComponent();
        }


        public void SetOriginList()
        {
            this.Origin.Clear();
            if (this.Material != null)
            {
                foreach (vmContent con in this.Material.Contents)
                {
                    bool hasSame = this.Origin.Any(x => x.Temp.Temp.Uid == con.Temp.Temp.Uid);
                    if (hasSame) continue;

                    switch (con.Temp.ItemType)
                    {
                        case eItemType.Image:
                            con.ContentType = eContentType.Image;
                            this.Origin.Add(con);
                            continue;
                        default: continue;
                    }
                }
            }
        }
        public void BindList(string keyword = "", int page = -1)
        {
            if (this.listbox_headers == null) return;


            ObservableCollection<vmContent> list = new ObservableCollection<vmContent>();
            foreach (vmContent con in this.Origin)
            {
                if (!con.IsEnable) continue;

                if (this.ImageStatus.HasValue)
                {
                    if (con.IsContentsValid != this.ImageStatus.Value) continue;
                }


                //if (this.TextType != eContentType.All)
                //{
                //    if (con.ContentType != this.TextType) continue;
                //}

                if (!string.IsNullOrEmpty(keyword))
                {
                    if (!con.Temp.Temp.LineText.ToUpper().Contains(keyword.ToUpper())) continue;
                }

                if (page > 0)
                {
                    if (con.Display_SlideNum.ToString() != page.ToString()) continue;
                }


                list.Add(con);
            }

            this.listbox_headers.ItemsSource = list.OrderBy(x => x.Temp.ParentShape.ParentSlide.Temp.SlideNumber).ThenBy(x => x.Temp.Temp.Order);
        }
        private void BindPageComboBox()
        {
            List<ComboBoxItem> items = new List<ComboBoxItem>();

            ComboBoxItem allCombobox = new ComboBoxItem();
            allCombobox.Uid = "0";
            allCombobox.Content = "전체";
            items.Add(allCombobox);

            var nums = this.Origin.Select(x => x.Display_SlideNum).Distinct();
            foreach (var num in nums)
            {
                ComboBoxItem newItem = new ComboBoxItem();
                newItem.Uid = num.ToString();
                newItem.Content = num.ToString();

                items.Add(newItem);
            }

            this.combo_Page.ItemsSource = items;
            this.combo_Page.SelectedIndex = 0;
        }
        private string ChangeNameToOne(string input)
        {
            string output = string.Empty;

            string[] lines = TextHelper.SplitText(input);
            foreach (string ln in lines)
            {
                output += ln;
            }

            return output;
        }
        private string RemoveNoTextLine(string input)
        {
            string output = string.Empty;

            string[] lines = TextHelper.SplitText(input);
            foreach (string ln in lines)
            {
                if (TextHelper.IsNoText(ln)) continue;
                if (ln != lines.First()) output += "\n";
                output += ln;

            }

            return output;
        }
        private string RemoveAllEmpty(string input)
        {
            string output = string.Empty;

            //output = input.TrimStart();

            string[] lines = TextHelper.SplitText(input);
            foreach (string ln in lines)
            {
                string line = ln;
                line = ln.RemoveEmtpy();

                if (ln != lines.First()) output += "\n";
                output += line;
            }

            return output;
        }
        private string RemoveTrim(string input)
        {
            string output = string.Empty;

            string[] lines = TextHelper.SplitText(input);
            foreach (string ln in lines)
            {
                string line = ln;
                if (TextHelper.IsNoText(line)) continue;
                if (char.IsWhiteSpace(ln.First())) line = line.Substring(1);
                if (char.IsWhiteSpace(ln.Last())) line = line.TrimEnd();

                if (ln != lines.First()) output += "\n";
                output += line;
            }

            return output;
        }
        private string RemoveEndTrim(string input)
        {
            string output = string.Empty;

            string[] lines = TextHelper.SplitText(input);
            foreach (string ln in lines)
            {
                string line = ln;
                if (TextHelper.IsNoText(line)) continue;
                if (char.IsWhiteSpace(ln.Last())) line = line.TrimEnd();

                if (ln != lines.First()) output += "\n";
                output += line;
            }

            return output;
        }
        private string RemoveStartTrim(string input)
        {
            string output = string.Empty;

            string[] lines = TextHelper.SplitText(input);
            foreach (string ln in lines)
            {
                string line = ln;
                if (TextHelper.IsNoText(line)) continue;
                if (char.IsWhiteSpace(ln.First())) line = line.Substring(1);

                if (ln != lines.First()) output += "\n";
                output += line;
            }

            return output;
        }



        private void txtbox_SearchKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    this.SearchKeyword = this.txtbox_SearchKeyword.Text;
                    BindList(this.SearchKeyword);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.SearchKeyword = this.txtbox_SearchKeyword.Text;
                BindList(this.SearchKeyword);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton rBtn = sender as RadioButton;
                if (rBtn == null) return;

                string uid = rBtn.Uid;

                if(string.IsNullOrEmpty(uid))
                {
                    this.ImageStatus = null;
                    
                }
                else
                {
                    this.ImageStatus = uid == "1";
                }

                
                BindList();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void combo_Page_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems == null || e.AddedItems.Count == 0) return;

                foreach (ComboBoxItem item in e.AddedItems)
                {
                    string uid = item.Uid;
                    bool isPageValid = int.TryParse(uid, out int page);
                    if (!isPageValid) page = -1;


                    BindList(this.SearchKeyword, page);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_AllSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                string uid = btn.Uid;
                if (uid == 1.ToString()) this.listbox_headers.SelectAll();
                else this.listbox_headers.UnselectAll();

                
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_ConvertToXML_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_LineControl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                string uid = btn.Uid;
                foreach (vmContent item in this.listbox_headers.SelectedItems)
                {
                    if (item == null) continue;
                    if (TextHelper.IsNoText(item.Temp_Title)) continue;

                    string output = string.Empty;
                    switch (uid)
                    {
                        case "toOne": output = ChangeNameToOne(item.Temp_Title); break;
                        case "removeEmpty": output = RemoveNoTextLine(item.Temp_Title); break;
                        default: output = item.Temp_Content; break;
                    }
                    item.Temp_Content = output;
                    if (this.check_ModifySync.IsChecked == true) item.SetNewContent();
                }

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_RemovewEmpty_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                string uid = btn.Uid;
                foreach (vmContent content in this.listbox_headers.SelectedItems)
                {
                    if (content == null) continue;
                    if (TextHelper.IsNoText(content.Temp_Title)) continue;

                    string output = string.Empty;
                    switch (uid)
                    {
                        case "start": output = RemoveStartTrim(content.Temp_Title); break;
                        case "end": output = RemoveEndTrim(content.Temp_Title); break;
                        case "startend": output = RemoveTrim(content.Temp_Title); break;
                        case "all": output = RemoveAllEmpty(content.Temp_Title); break;
                        default: output = content.Temp_Title; break;
                    }
                    //if (output != content.Temp_Content) heading.SetName(output);

                    content.Temp_Title = output;
                    if (this.check_ModifySync.IsChecked == true) content.SetNewContent();
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_RemoveFirst_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent item in this.listbox_headers.SelectedItems)
                {
                    if (TextHelper.IsNoText(item.Temp_Title)) continue;

                    string text = item.Temp_Title;
                    item.Temp_Title = text.Substring(1);
                    if (this.check_ModifySync.IsChecked == true) item.SetNewContent();
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_KeywordRemove_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_AddMark_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_RemoveMark_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rBtn_SelectType(object sender, RoutedEventArgs e)
        {

        }

        private void btn_RemoveContent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<vmContent> removalList = new List<vmContent>();
                foreach (vmContent item in this.listbox_headers.SelectedItems) removalList.Add(item);


                foreach (vmContent item in removalList)
                {
                    item.IsEnable = false;
                    this.Material.RemoveContent(item);
                    this.Origin.Remove(item);
                }


                BindList(this.SearchKeyword);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }


        private void btn_AllReSet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent data in this.listbox_headers.SelectedItems)
                {

                    string title = TextHelper.GetImageTitleFromMarkdown(data.Temp.Temp.LineText);
                    string fileName = TextHelper.GetImageFileNameFromMarkdown(data.Temp.Temp.LineText);
                    fileName = Path.GetFileNameWithoutExtension(fileName);

                    data.Temp.SetImageText(title, fileName);
                    data.InitializeDisplay();
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_AllApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent data in this.listbox_headers.SelectedItems)
                {
                    string fileName = TextHelper.GetImageFileNameFromMarkdown(data.Temp_Content);
                    fileName = Path.GetFileNameWithoutExtension(fileName);

                    data.SetNewImageContent(data.Temp_Title, fileName);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void TextBox_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (this.check_ModifySync.IsChecked != true) return;



                TextBox tb = sender as TextBox;
                if (tb == null) return;

                vmContent data = tb.DataContext as vmContent;
                if (data == null) return;

                string newText = tb.Text;
                string fileName = TextHelper.GetImageFileNameFromMarkdown(data.Temp_Content);
                fileName = Path.GetFileNameWithoutExtension(fileName);

                if (this.check_ModifySync.IsChecked == true) data.SetNewImageContent(newText, fileName);
            }
            catch (Exception ee )
            {
                ErrorHelper.ShowError(ee);
                
            }
        }

        private void btn_RemoveLast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent item in this.listbox_headers.SelectedItems)
                {
                    string text = item.Temp_Title;
                    if (TextHelper.IsNoText(text)) continue;
                    item.Temp_Title = text.Substring(0, text.Length - 1);
                    if (this.check_ModifySync.IsChecked == true) item.SetNewContent();
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_ApplySingle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.check_ModifySync.IsChecked == true) return;
                Button btn = sender as Button;
                if (btn == null) return;

                vmContent data = btn.DataContext as vmContent;
                if (data == null) return;

                string newText = data.Temp_Title;
                string fileName = TextHelper.GetImageFileNameFromMarkdown(data.Temp_Content);
                fileName = Path.GetFileNameWithoutExtension(fileName);

                data.SetNewImageContent(newText, fileName);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_ResetSingle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.check_ModifySync.IsChecked == true) return;

                Button btn = sender as Button;
                if (btn == null) return;

                vmContent data = btn.DataContext as vmContent;
                if (data == null) return;

                string title = TextHelper.GetImageTitleFromMarkdown(data.Temp.Temp.LineText);
                string fileName = TextHelper.GetImageFileNameFromMarkdown(data.Temp.Temp.LineText);
                fileName = Path.GetFileNameWithoutExtension(fileName);

                data.Temp.SetImageText(title, fileName);
                data.InitializeDisplay();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.check_ModifySync.IsChecked != true) return;

                TextBox tb = sender as TextBox;
                if (tb == null) return;

                vmContent data = tb.DataContext as vmContent;
                if (data == null) return;

                string newText = tb.Text;
                string fileName = TextHelper.GetImageFileNameFromMarkdown(data.Temp_Content);
                fileName = Path.GetFileNameWithoutExtension(fileName);

                
                if (this.check_ModifySync.IsChecked == true) data.SetNewImageContent(newText, fileName);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_RemoveSearchKeyword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.SearchKeyword = this.txtbox_SearchKeyword.Text = string.Empty;
                BindList();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_AllCheck_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent item in this.Origin)
                {
                    string pathString = this.Material.DirectoryPath;
                    string fileName = TextHelper.GetImageFileNameFromMarkdown(item.Temp.Temp.LineText);

                    string fullPath = Path.Combine(pathString, fileName);
                    if (File.Exists(fullPath)) 
                    {
                        item.IsContentsValid = true;
                        item.ImagePath = fullPath;
                    }
                    else
                    {
                        item.IsContentsValid = false;
                    }
                    
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
                
            }
        }

        private void btn_ImageChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                 vmContent data = btn.DataContext as vmContent;
                if (data == null) return;

                FileInfo fInfo = FileHelper.GetOpenFileInfo();
                if (fInfo == null) return;

                string fullPath = fInfo.FullName;

                string newFileName = Guid.NewGuid().ToString() ;
                string originImagePath = Path.Combine(this.Material.DirectoryPath, newFileName + ".png");
            

                data.ImagePath = string.Empty;
                File.Copy(fullPath, originImagePath, true);
                data.ImagePath = originImagePath;
                data.Temp.SetImageText(data.Temp.Temp.Title, newFileName);
                data.InitializeDisplay();
                data.IsContentsValid = true;

                BindList();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_ShowDirectory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string directory = this.Material.DirectoryPath;
                if(Directory.Exists(directory))
                {
                    Process.Start(directory);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_MoveToText_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                List<vmContent> seleectedList = new List<vmContent>();
                foreach (vmContent item in this.listbox_headers.SelectedItems) seleectedList.Add(item);

                foreach (vmContent item in seleectedList)
                {
                    item.Temp.SetItemType(eItemType.Text);

                    string[] lines = TextHelper.SplitText(item.Temp_Content);
                    if (lines.Length == 1) item.ContentType = eContentType.NormalText;

                    (this.Tag as ucMarkCheckerContentsChecking).ucMarkCheckerCheckingText.SetOriginList();


                    this.Origin.Remove(item);
                    BindList();
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_MoveToTable_Clcik(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                List<vmContent> seleectedList = new List<vmContent>();
                foreach (vmContent item in this.listbox_headers.SelectedItems) seleectedList.Add(item);

                foreach (vmContent item in seleectedList)
                {
                    item.Temp.SetItemType(eItemType.Table);

                    string[] lines = TextHelper.SplitText(item.Temp_Content);
                    if (lines.Length == 1) item.ContentType = eContentType.Table;

                    (this.Tag as ucMarkCheckerContentsChecking).ucmarkcheckerCheckingTable.SetOriginList();


                    this.Origin.Remove(item);
                    BindList();
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_RemoveALL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent item in this.listbox_headers.SelectedItems)
                {
                    item.Temp_Title = string.Empty;
                    if (this.check_ModifySync.IsChecked == true) item.SetNewContent();
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
