using MDM.Commons.Enum;
using MDM.Helpers;
using MDM.Models.DataModels;
using MDM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.AccessControl;
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

namespace MDM.Views.MarkChecker.Pages
{
    /// <summary>
    /// ucMarkCheckerContentsCheckingText.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucMarkCheckerContentsCheckingText : UserControl
    {
        private vmMaterial _Material = null;
        public vmMaterial Material
        {
            get => _Material;
            set
            {
                _Material = value;
                this.Origin.Clear();
                if (value != null)
                {
                    foreach (vmContent con in value.Contents)
                    {
                        bool hasSame= this.Origin.Any(x => x.Temp.Temp.Uid == con.Temp.Temp.Uid);
                        if (hasSame) continue;

                        switch (con.Temp.ItemType)
                        {
                            case eItemType.Text:

                                string text = con.Temp.Temp.LineText;
                                string[] lines = TextHelper.SplitText(text);
                                if (lines.Length == 1)
                                {
                                    con.ContentType = eContentType.NormalText;
                                }
                                else
                                {
                                    int length = lines.Length;
                                    Dictionary<int, string> temp = new Dictionary<int, string>();
                                    for (int i = 0; i < length; i++) temp.Add(i, lines[i]);

                                    int digitCnt = 0;
                                    int total = 0;
                                    foreach (string item in temp.Values)
                                    {
                                        if (TextHelper.IsNoText(item)) continue;
                                        char firstChar = item.First();
                                        if (char.IsWhiteSpace(firstChar)) firstChar = item[1];
                                        if (char.IsWhiteSpace(firstChar)) continue;

                                        if (char.IsDigit(item.First())) digitCnt++;
                                        total++;
                                    }

                                    if (total / 2 < digitCnt)
                                    {
                                        con.ContentType = eContentType.OrderList;
                                    }
                                    else
                                    {
                                        con.ContentType = eContentType.UnOrderList;
                                    }
                                }

                                this.Origin.Add(con);
                                continue;
                            default: continue;
                        }
                    }
                }

                this.txtbox_SearchKeyword.Text = string.Empty;
                this.rBtn_All.IsChecked = true;
                BindList();
                BindPageComboBox();
            }
        }



        public ObservableCollection<vmContent> Origin { get; set; } = new ObservableCollection<vmContent>();






        public eContentType TextType { get; set; } = eContentType.All;
        public string SearchKeyword { get; set; } = string.Empty;

        public ucMarkCheckerContentsCheckingText()
        {
            InitializeComponent();
        }


        public void BindList(string keyword = "", int page = -1)
        {
            ObservableCollection<vmContent> list = new ObservableCollection<vmContent>();
            foreach (vmContent con in this.Origin)
            {
                if(this.TextType != eContentType.All)
                {
                    if(con.ContentType != this.TextType) continue;
                }

                if(!string.IsNullOrEmpty(keyword))
                {
                    if (!con.Temp.Temp.LineText.ToUpper().Contains(keyword.ToUpper())) continue;
                }

                if(page > 0)
                {
                    if (con.Display_SlideNum.ToString() != page.ToString()) continue;
                }


                list.Add(con);
            }

            this.listbox_headers.ItemsSource = list;
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
        private string RemoveHeaderMark(string input)
        {
            string output = string.Empty;
            string[] lines = TextHelper.SplitText(input);

            char[] mark = { '-', '>', '*', '▷', '▣' };
            foreach (string ln in lines)
            {
                string newLine = ln;
                if (TextHelper.IsNoText(newLine)) continue;

                if (mark.Contains(newLine.First()))
                {
                    newLine = newLine.Substring(1).Trim();
                }
                else if (newLine.StartsWith("\t"))
                {
                    string indent = string.Empty;
                    while (newLine.First() == '\t')
                    {
                        newLine = newLine.Substring(1);
                        indent += "  ";
                    }
                    if (mark.Contains(newLine.First())) newLine = newLine.Substring(1);

                    newLine = newLine.Insert(0, indent);
                }

                output += newLine;
                if (ln != lines.Last()) output += "\n";
            }


            return output;
        }


        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton rBtn = sender as RadioButton;
                if (rBtn == null) return;

                bool isCodeValid = int.TryParse(rBtn.Uid, out int code);
                if (!isCodeValid) code = 0;

                this.TextType = (eContentType)code;
                BindList();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void txtbox_SearchKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if(e.Key == Key.Enter)
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

        private void TextBox_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            try
            {

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

                string text = tb.Text;
                data.SetNewContent(text);
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

                    string output = string.Empty;
                    switch (uid)
                    {
                        case "start": output = RemoveStartTrim(content.Temp_Content); break;
                        case "end": output = RemoveEndTrim(content.Temp_Content); break;
                        case "startend": output = RemoveTrim(content.Temp_Content); break;
                        case "all": output = RemoveAllEmpty(content.Temp_Content); break;
                        default: output = content.Temp_Content; break;
                    }
                    //if (output != content.Temp_Content) heading.SetName(output);

                    content.Temp_Content = output;
                    if (this.check_ModifySync.IsChecked == true) content.SetNewContent();
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
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

                    string output = string.Empty;
                    switch (uid)
                    {
                        case "toOne": output = ChangeNameToOne(item.Temp_Content); break;
                        case "removeEmpty": output = RemoveNoTextLine(item.Temp_Content); break;
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

        private void btn_AllSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                if (btn.Uid == "1") this.listbox_headers.SelectAll();
                else this.listbox_headers.UnselectAll();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
                
            }
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

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


                    BindList(this.SearchKeyword,page);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_RemoveContent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<vmContent> removalList = new List<vmContent>();
                foreach (vmContent item in this.listbox_headers.SelectedItems) removalList.Add(item);


                foreach (vmContent item in removalList)
                {
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

        private void btn_AllApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent item in this.listbox_headers.SelectedItems) item.SetNewContent(item.Temp_Content);
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
                foreach (vmContent item in this.listbox_headers.SelectedItems)
                {
                    string origin = item.Temp.Temp.LineText;
                    item.Temp_Content = origin;
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
                    string text = item.Temp_Content;
                    item.Temp_Content = text.Substring(1);
                    if (this.check_ModifySync.IsChecked == true) item.SetNewContent();
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void rBtn_SelectType(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton rBtn = sender as RadioButton;
                if (rBtn == null) return;

                string uid = rBtn.Uid;
                bool isDigit = int.TryParse(uid, out int code);
                if (!isDigit) code = 2211;

                foreach (vmContent item in this.listbox_headers.SelectedItems)
                {
                    item.SetContentType((eContentType)code);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void RadioButton_Single_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton rBtn = sender as RadioButton;
                if (rBtn == null) return;

                vmContent data = rBtn.DataContext as vmContent;
                if (data == null) return;

                string uid = rBtn.Uid;
                bool isDigit = int.TryParse(uid, out int code);
                if (!isDigit) code = 2211;

                data.SetContentType((eContentType)code);
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

                data.Temp_Content = data.Temp.Temp.LineText;
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

                data.SetNewContent(data.Temp_Content);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_KeywordRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string removePhase =this.txtbox_RemoveKeyword.Text;
                foreach (vmContent item in this.listbox_headers.SelectedItems)
                {
                    string text = item.Temp_Content;
                    if(text.Contains(removePhase))
                    {
                        text = text.Replace(removePhase, "");
                        item.Temp_Content = text;
                        if (this.check_ModifySync.IsChecked == true) item.SetNewContent();
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
                
            }
        }

        private void btn_AddMark_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent item in this.listbox_headers.SelectedItems)
                {
                    string text = TextHelper.Preprocessing(item.Temp_Content);
                    item.Temp_Content = text;
                    if (this.check_ModifySync.IsChecked == true) item.SetNewContent();
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_RemoveMark_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent item in this.listbox_headers.SelectedItems)
                {
                    string text = RemoveHeaderMark(item.Temp_Content);
                    item.Temp_Content = text;
                    if (this.check_ModifySync.IsChecked == true) item.SetNewContent();
                }
            }
            catch (Exception ee)
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
                    string text = item.Temp_Content;
                    item.Temp_Content = text.Substring(0, text.Length-1);
                    if (this.check_ModifySync.IsChecked == true) item.SetNewContent();
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_ConvertToXML_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_AllTextChecked_clicked(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
