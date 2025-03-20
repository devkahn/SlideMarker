using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using MDM.Commons.Enum;
using MDM.Helpers;
using MDM.Models.ViewModels;
using Microsoft.Office.Interop.PowerPoint;

namespace MDM.Views.MarkChecker.Pages
{
    /// <summary>
    /// ucMarkCheckerContentsChecking.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucMarkCheckerContentsChecking : UserControl
    {
        private vmMaterial _Material = null;
        public vmMaterial Material
        {
            get => _Material;
            set
            {
                _Material = value;
                this.DataContext = value;

                this.AllContentsList.Clear();
                this.NormalTextContentList.Clear();
                this.OrderedTextContentList.Clear();
                this.UnOrderedTextContentList.Clear();
                this.ImageContentList.Clear();
                this.TableCotnentList.Clear();

                BindAllContents();

            }
        }

        public int ContentTypeCode { get; set; } = 0;

        public ObservableCollection<vmContent> AllContentsList { get; } = new ObservableCollection<vmContent>();


        public ObservableCollection<vmContent> NormalTextContentList { get; } = new ObservableCollection<vmContent>();
        public ObservableCollection<vmContent> OrderedTextContentList { get; } = new ObservableCollection<vmContent>();
        public ObservableCollection<vmContent> UnOrderedTextContentList { get; } = new ObservableCollection<vmContent>();
        public ObservableCollection<vmContent> ImageContentList { get; } = new ObservableCollection<vmContent>();
        public ObservableCollection<vmContent> TableCotnentList { get; } = new ObservableCollection<vmContent>();





        public ucMarkCheckerContentsChecking()
        {
            InitializeComponent();

            this.listbox_Contents.ItemsSource = this.AllContentsList;
            this.listbox_normalText.ItemsSource = this.NormalTextContentList;
            this.listbox_orderedText.ItemsSource = this.OrderedTextContentList;
            this.listbox_unorderedText.ItemsSource = this.UnOrderedTextContentList;

            this.listbox_ImageList.ItemsSource = this.ImageContentList;
            this.listbox_TableList.ItemsSource = this.TableCotnentList;
        }

        private void BindAllContents()
        {
            if (this.Material == null) return;

            this.AllContentsList.Clear();
            foreach (vmContent content in this.Material.Contents)
            {
                if (this.NormalTextContentList.Contains(content)) continue;
                if (this.OrderedTextContentList.Contains(content)) continue;
                if (this.UnOrderedTextContentList.Contains(content)) continue;
                if (this.ImageContentList.Contains(content)) continue;
                if (this.TableCotnentList.Contains(content)) continue;

                eItemType itemtype = (eItemType)this.ContentTypeCode;
                if(itemtype == eItemType.All)
                {


                    this.AllContentsList.Add(content);
                }
                else
                {
                    if (content.Temp.ItemType == itemtype) this.AllContentsList.Add(content);
                }
            }
            
        }

        private void listbox_Contents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.contentPresenter_Content == null) return;

                foreach (vmContent item in e.AddedItems)
                {
                    if (item == null) continue;

                    UserControl uc = null;
                    switch (item.Temp.ItemType)
                    {
                        case Commons.Enum.eItemType.Text: uc = new ucMarkCheckerContentsCheckingText(); break;
                        case Commons.Enum.eItemType.Image: uc = new ucMarkCheckerContentsCheckingImage(); break;
                        case Commons.Enum.eItemType.Table: uc = new ucMarkCheckerContentsCheckingTable(); break;
                    }

                    uc.DataContext = item;

                    this.contentPresenter_Content.Content = uc;
                }
                 
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

                int code = int.Parse(rBtn.Uid);

                this.ContentTypeCode = code;

                BindAllContents();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_NormalTextClssification_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.NormalTextContentList.Clear();
                foreach (vmContent content in this.Material.Contents)
                {
                    if (content.Temp.ItemType != eItemType.Text) continue;

                    string[] lines = TextHelper.SplitText(content.Temp.Temp.LineText);
                    if(lines.Length == 1)
                    {
                        this.NormalTextContentList.Add(content);
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_AllTrim_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent content in this.Material.Contents)
                {
                    if (content.Temp.ItemType != eItemType.Text) continue;

                    string trimString = content.Temp.Temp.LineText.Trim();
                    content.Temp.SetText(trimString);

                    content.InitializeDisplay();

                    //string[] lines = TextHelper.SplitText(content.Temp.Temp.LineText);
                    
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
                
            }
        }

        private void btn_FirstDigitRemoval_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent content in this.NormalTextContentList)
                {
                    string value = content.Temp.Temp.LineText;
                    if(char.IsDigit(value.First()))
                    {
                        value = value.Substring(1);
                        content.Temp.SetText(value);
                        content.InitializeDisplay();
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_FirstSpecialRemoval_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                char[] specialChars = { '.' };
                foreach (vmContent content in this.NormalTextContentList)
                {
                    string value = content.Temp.Temp.LineText;
                    if (!char.IsLetterOrDigit(value.First())) 
                    {
                        value = value.Substring(1);
                        content.Temp.SetText(value);
                        content.InitializeDisplay();
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_UnOrderedTextClassifiacation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.UnOrderedTextContentList.Clear();
                foreach (vmContent content in this.Material.Contents)
                {
                    if (content.Temp.ItemType != eItemType.Text) continue;

                    string[] lines = TextHelper.SplitText(content.Temp.Temp.LineText);
                    if (lines.Length > 1)
                    {
                        this.UnOrderedTextContentList.Add(content);
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_ToOrderList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;
                string uid = btn.Uid;
                
                if (uid == "Normal")
                {
                    var selectedItems = this.listbox_normalText.SelectedItems;
                    for (int i = 0; i < selectedItems.Count; i++)
                    {
                        vmContent item = selectedItems[i] as vmContent;
                        if (item == null) continue;

                        if (!this.OrderedTextContentList.Contains(item)) this.OrderedTextContentList.Add(item);
                        if (this.NormalTextContentList.Contains(item)) this.NormalTextContentList.Remove(item);
                    }
                }
                else if(uid == "Unorder")
                {
                    var selectedItems = this.listbox_unorderedText.SelectedItems;
                    for (int i = 0; i < selectedItems.Count; i++)
                    {
                        vmContent item = selectedItems[i] as vmContent;
                        if (item == null) continue;

                        if (!this.OrderedTextContentList.Contains(item)) this.OrderedTextContentList.Add(item);
                        if (this.UnOrderedTextContentList.Contains(item)) this.UnOrderedTextContentList.Remove(item);
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_ToNormalList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;
                string uid = btn.Uid;
                if (uid == "Order")
                {
                    var selectedItems = this.listbox_orderedText.SelectedItems;
                    for (int i = 0; i < selectedItems.Count; i++)
                    {
                        vmContent item = selectedItems[i] as vmContent;
                        if (item == null) continue;

                        if (!this.NormalTextContentList.Contains(item)) this.NormalTextContentList.Add(item);
                        if (this.OrderedTextContentList.Contains(item)) this.OrderedTextContentList.Remove(item);
                    }
                }
                else if( uid == "Unorder")
                {
                    var selectedItems = this.listbox_unorderedText.SelectedItems;
                    for (int i = 0; i < selectedItems.Count; i++)
                    {
                        vmContent item = selectedItems[i] as vmContent;
                        if (item == null) continue;

                        if (!this.NormalTextContentList.Contains(item)) this.NormalTextContentList.Add(item);
                        if (this.UnOrderedTextContentList.Contains(item)) this.UnOrderedTextContentList.Remove(item);
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_ToUnorder_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            string uid = btn.Uid;
            if (uid == "Normal")
            {
                var selectedItems = this.listbox_normalText.SelectedItems;
                for (int i = 0; i < selectedItems.Count; i++)
                {
                    vmContent item = selectedItems[i] as vmContent;
                    if (item == null) continue;

                    if (!this.UnOrderedTextContentList.Contains(item)) this.UnOrderedTextContentList.Add(item);
                    if (this.NormalTextContentList.Contains(item)) this.NormalTextContentList.Remove(item);
                }
            }
            else if (uid == "Order")
            {
                var selectedItems = this.listbox_orderedText.SelectedItems;
                for (int i = 0; i < selectedItems.Count; i++)
                {
                    vmContent item = selectedItems[i] as vmContent;
                    if (item == null) continue;

                    if (!this.UnOrderedTextContentList.Contains(item)) this.UnOrderedTextContentList.Add(item);
                    if (this.OrderedTextContentList.Contains(item)) this.OrderedTextContentList.Remove(item);
                }
            }
        }

        private void btn_UnOrder_FirstDigitRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent content in this.UnOrderedTextContentList)
                {
                    string newValue = string.Empty;

                    string value = content.Temp.Temp.LineText;
                    string[] lines = TextHelper.SplitText(value);
                    foreach (string ln in lines)
                    {
                        if (string.IsNullOrEmpty(ln)) continue;
                        if (char.IsDigit(ln.First()))
                        {
                            string newLn = ln.Substring(1);
                            newValue += newLn;
                        }
                        else
                        {
                            newValue += ln;
                        }
                        if(ln != lines.Last()) newValue += "\n";
                    }
                    content.Temp.SetText(newValue);
                    content.InitializeDisplay();
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_UnOrder_FirstSpecialRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent content in this.UnOrderedTextContentList)
                {
                    string newValue = string.Empty;

                    string value = content.Temp.Temp.LineText;
                    string[] lines = TextHelper.SplitText(value);
                    foreach (string ln in lines)
                    {
                        if (string.IsNullOrEmpty(ln)) continue;

                        if (!char.IsLetterOrDigit(ln.First()))
                        {
                            string newLn = ln.Substring(1);
                            newValue += newLn;
                        }
                        else
                        {
                            newValue += ln;
                        }
                        if (ln != lines.Last()) newValue += "\n";
                    }
                    content.Temp.SetText(newValue);
                    content.InitializeDisplay();
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_UnOrder_Marking_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent content in this.UnOrderedTextContentList)
                {
                    string newValue = string.Empty;

                    string value = content.Temp.Temp.LineText;
                    string[] lines = TextHelper.SplitText(value);
                    foreach (string ln in lines)
                    {
                        if (string.IsNullOrEmpty(ln)) continue;

                        if (ln.First() == '*')
                        {
                            newValue += ln;
                        }
                        else
                        {
                            if (char.IsLetterOrDigit(ln.First()))
                            {

                                newValue += string.Format("* {0}", ln);
                            }
                            else if (char.IsWhiteSpace(ln.First()))
                            {
                                string newLine = ln.Trim();
                                while (!char.IsLetterOrDigit(newLine.First())) newLine = newLine.Substring(1);

                                newValue += string.Format("** {0}", newLine.Trim());
                            }
                            else
                            {
                                newValue += ln;
                            }
                        }

                        
                        if (ln != lines.Last()) newValue += "\n";
                    }
                    content.Temp.SetText(newValue);
                    content.InitializeDisplay();
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_UnOrder_FirstEmptyRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent content in this.UnOrderedTextContentList)
                {
                    string newValue = string.Empty;

                    string value = content.Temp.Temp.LineText;
                    string[] lines = TextHelper.SplitText(value);
                    foreach (string ln in lines)
                    {
                        if (string.IsNullOrEmpty(ln)) continue;

                        if (char.IsWhiteSpace(ln.First()))
                        {

                            string newLn = ln.Substring(1);
                            newValue += newLn;
                        }
                        else
                        {
                            newValue += ln;
                        }
                        if (ln != lines.Last()) newValue += "\n";
                    }
                    content.Temp.SetText(newValue);
                    content.InitializeDisplay();
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_UnOrder_AllCheckAndMarking_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent content in this.UnOrderedTextContentList)
                {
                    Dictionary<int, string> tempDic = new Dictionary<int, string>();


                    string[] lines = TextHelper.SplitText(content.Temp.Temp.LineText);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string ln = lines[i];
                        if (TextHelper.IsNoText(ln)) continue;

                        tempDic.Add(i, ln);
                    }

                    int cnt = 0;

                    bool hasNoMark = tempDic.Values.Any(x => x.First() != '*');
                    while (hasNoMark)
                    {
                        cnt++;
                        bool hasTarget = tempDic.Values.Any(x => x.First() != '*' && !char.IsWhiteSpace(x.First())); 
                        while(!hasTarget)
                        {
                            foreach (int key in tempDic.Keys.ToList())
                            {
                                string ln = tempDic[key];
                                if(char.IsWhiteSpace(ln.First())) tempDic[key] = ln.Substring(1);
                            }
                            hasTarget = tempDic.Values.Any(x => x.First() != '*' && !char.IsWhiteSpace(x.First()));
                        }

                        foreach (int key in tempDic.Keys.ToList())
                        {
                            string ln = tempDic[key];
                            if (ln.First() == '*') continue;
                            if (char.IsWhiteSpace(ln.First())) continue;
                            if (TextHelper.IsNoText(ln)) continue;

                            if (TextHelper.IsFirstNumericListMark(ln)) ln = ln.Substring(3);
                            if (!char.IsLetterOrDigit(ln.First())) ln = ln.Substring(1);

                            string depth = string.Empty;
                            for (int i = 0; i < cnt; i++) depth += "*";

                            tempDic[key] = string.Format("{0} {1}", depth, ln);
                        }

                        hasNoMark = tempDic.Values.Any(x => x.First() != '*');
                    }

                    string result = string.Empty;
                    foreach (string item in tempDic.Values)
                    {
                        result += item;
                        if (item != tempDic.Values.Last()) result += "\n";
                    }
                    content.Temp.SetText(result);
                    content.InitializeDisplay();
                    content.SetBackground(true);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_Oreder_AllCheckAndMarking_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent content in this.OrderedTextContentList)
                {
                    Dictionary<int, string> tempDic = new Dictionary<int, string>();
                    string[] lines = TextHelper.SplitText(content.Temp.Temp.LineText);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string ln = TextHelper.CleansingForXML(lines[i]);
                        if (TextHelper.IsNoText(ln)) continue;

                        tempDic.Add(i, ln);
                    }

                    
                    int cnt = 0;
                    bool hasNoMark = tempDic.Values.Any(x => x.First() != '#' && x.First() != '*');
                    while (hasNoMark)
                    {
                        cnt++;
                        bool hasTarget = tempDic.Values.Any(x => x.First() != '#' && x.First() != '*' && !char.IsWhiteSpace(x.First()));
                        while (!hasTarget)
                        {
                            foreach (int key in tempDic.Keys.ToList())
                            {
                                string ln = tempDic[key];
                                if (char.IsWhiteSpace(ln.First())) tempDic[key] = ln.Substring(1);
                            }
                            hasTarget = tempDic.Values.Any(x => x.First() != '#' && x.First() != '*' && !char.IsWhiteSpace(x.First()));
                        }

                        foreach (int key in tempDic.Keys.ToList())
                        {
                            string ln = TextHelper.CleansingForXML(tempDic[key]);

                            if (TextHelper.IsNoText(ln)) continue;
                            if (ln.First() == '#' || ln.First() == '*') continue;
                            if (char.IsWhiteSpace(ln.First())) continue;
                            

                            string markingChar = string.Empty;
                            if (TextHelper.IsFirstNumericListMark(ln))
                            {
                                ln = ln.Substring(3);
                                markingChar = "#";
                            }
                            else
                            {
                                char first = ln.First();
                                if(char.IsDigit(first) || TextHelper.IsEnClosedNumbers(first))
                                {
                                    if(TextHelper.IsEnClosedNumbers(first)) ln = ln.Substring(1);
                                    markingChar = "#";
                                }
                                else
                                {
                                    if(!char.IsLetter(first)) ln = ln.Substring(1);
                                    markingChar = "*";
                                }
                            }

                            string mark = string.Empty;
                            for (int i = 0; i < cnt; i++) mark += markingChar;

                            tempDic[key] = string.Format("{0} {1}", mark, ln.Trim()).Trim();
                        }
                        
                        hasNoMark = tempDic.Values.Any(x => x.First() != '#' && x.First() != '*');
                    }

                    string result = string.Empty;
                    foreach (string item in tempDic.Values)
                    {
                        result += item;
                        if (item != tempDic.Values.Last()) result += "\n";
                    }
                    content.Temp.SetText(result);
                    content.InitializeDisplay();
                    content.SetBackground(true);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_Normal_AllCheckAndMarking_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent content in this.NormalTextContentList)
                {
                    string ln = content.Temp.Temp.LineText;
                    ln = ln.Trim();

                    if (TextHelper.IsFirstNumericListMark(ln)) ln = ln.Substring(3);

                    content.Temp.SetText(ln.Trim());
                    content.InitializeDisplay();
                    content.SetBackground(true);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_Classification_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent con in this.AllContentsList.ToList())
                {
                    switch (con.Temp.ItemType)
                    {
                        case eItemType.Text:
                            string value = con.Temp.Temp.LineText;
                            string[] lines = TextHelper.SplitText(value);
                            if(lines.Length ==1)
                            {
                                con.ContentType = eContentType.NormalText;
                                if (!this.NormalTextContentList.Contains(con)) this.NormalTextContentList.Add(con);
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

                                if(total / 2 < digitCnt)
                                {
                                    con.ContentType = eContentType.OrderList;
                                    if (!this.OrderedTextContentList.Contains(con)) this.OrderedTextContentList.Add(con);
                                }
                                else
                                {
                                    con.ContentType = eContentType.UnOrderList;
                                    if (!this.UnOrderedTextContentList.Contains(con)) this.UnOrderedTextContentList.Add(con);
                                }
                                
                                
                            }
                            if (this.AllContentsList.Contains(con)) this.AllContentsList.Remove(con);
                            break;
                        case eItemType.Image:
                            con.ContentType = eContentType.Image;
                            if (!this.ImageContentList.Contains(con)) this.ImageContentList.Add(con);
                            if (this.AllContentsList.Contains(con)) this.AllContentsList.Remove(con);
                            break;
                        case eItemType.Table:
                            con.ContentType = eContentType.Table;
                            if (!this.TableCotnentList.Contains(con)) this.TableCotnentList.Add(con);
                            if (this.AllContentsList.Contains(con)) this.AllContentsList.Remove(con);
                            break;
                    }
                }
                BindAllContents();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_AllImageCheck_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent item in this.ImageContentList)
                {
                    string lineString = item.Temp.Temp.LineText;
                    string fileName = TextHelper.GetImageFileNameFromMarkdown(lineString);
                    if (!fileName.ToLower().EndsWith(".png")) fileName += ".png";

                    string imagePath = Path.Combine(this.Material.DirectoryPath, fileName);
                    if(File.Exists(imagePath))
                    {



                        item.SetBackground(true);
                    }
                    else
                    {
                        item.SetBackground(false);
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_AllTableCheck_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent item in this.TableCotnentList)
                {

                    string lineString = item.Temp.Temp.LineText;
                    if (lineString.StartsWith("<table>")) continue;

                    bool isValid = TextHelper.IsTableMarkdownValid(lineString);
                    if(isValid)
                    {
                        string[] lines = TextHelper.SplitText(lineString);


                        StringBuilder tableHtml = new StringBuilder();
                        tableHtml.Append("<table>");
                        tableHtml.Append("<thead>");

                        int rowHeaderCnt = 0;
                        bool isHeader = true;
                        foreach (string ln in lines)
                        {
                            if(TextHelper.IsTableDivider(ln))
                            {
                                rowHeaderCnt = TextHelper.GetRowHeaderCount(ln);
                                isHeader = false;
                                tableHtml.Append("</thead>");
                                tableHtml.Append("<tbody>");
                            }
                            else
                            {
                                tableHtml.Append("<tr>");
                                string[] cells = TextHelper.GetCellValueInRowString(ln);
                                if (isHeader)
                                {
                                    foreach (string cell in cells)
                                    {
                                        string cellHtml = string.Format("<th><div>{0}</div></th>", cell.Trim());
                                        tableHtml.Append(cellHtml);
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < cells.Length; i++)
                                    {
                                        string cellValue = cells[i];
                                        if(cellValue.Contains("\\n")) cellValue = cellValue.Replace("\\n", "\n");
                                        string[] cellLines = TextHelper.SplitText(cellValue);    
                                        if(cellLines.Length != 1)
                                        {
                                            cellValue = "<ul>";

                                            foreach (string cellLine in cellLines)
                                            {
                                                string lineValue = cellLine;
                                                if(lineValue.First() == '-') lineValue = lineValue.Substring(1);
                                                cellValue += string.Format("<li>{0}</li>", lineValue.Trim());
                                            }


                                            cellValue += "</ul>";
                                        }



                                        string cellType = i < rowHeaderCnt ? "th" : "td";
                                        string cellHtml = string.Format("<{0}><div>{1}</div></{0}>", cellType, cellValue.Trim());
                                        tableHtml.Append(cellHtml);
                                    }
                                }
                                tableHtml.Append("</tr>");
                            }
                        }
                        
                        tableHtml.Append("</tbody>");
                        tableHtml.Append("</table>");

                        item.Temp.SetText(tableHtml.ToString());
                        item.InitializeDisplay();
                        item.SetBackground(true);
                    }
                    else
                    {
                        item.SetBackground(false);
                    }

                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }


        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var item in this.AllContentsList) item.InitializeDisplay();
                foreach (var item in this.NormalTextContentList) item.InitializeDisplay();
                foreach (var item in this.OrderedTextContentList) item.InitializeDisplay();
                foreach (var item in this.UnOrderedTextContentList) item.InitializeDisplay();
                foreach (var item in this.ImageContentList) item.InitializeDisplay();
                foreach (var item in this.TableCotnentList) item.InitializeDisplay();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
