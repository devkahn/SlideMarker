using MDM.Commons.Enum;
using MDM.Helpers;
using MDM.Models.ViewModels;
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
using static OfficeOpenXml.ExcelErrorValue;

namespace MDM.Views.MarkChecker.Pages
{
    /// <summary>
    /// ucMarkCheckerContentsCheckingTable.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucMarkCheckerContentsCheckingTable : UserControl
    {
        private vmMaterial _Material = null;
        private string _SearchKeyword = string.Empty;
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
                        bool hasSame = this.Origin.Any(x => x.Temp.Temp.Uid == con.Temp.Temp.Uid);
                        if (hasSame) continue;

                        switch (con.Temp.ItemType)
                        {
                            case eItemType.Table:
                                con.ContentType = eContentType.Table;
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

        public bool? TableStatus { get; set; } = null;
        public string SearchKeyword
        {
            get => _SearchKeyword;
            set
            {
                _SearchKeyword = value;
                this.btn_RemoveSearchKeyword.Visibility = string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;
            }
        }


        public ucMarkCheckerContentsCheckingTable()
        {
            InitializeComponent();
        }

        public void SetOriginList()
        {
            this.Origin.Clear();
            if (this.Material != null)
            {
                foreach (vmContent con in this.Material
                    .Contents)
                {
                    bool hasSame = this.Origin.Any(x => x.Temp.Temp.Uid == con.Temp.Temp.Uid);
                    if (hasSame) continue;

                    switch (con.Temp.ItemType)
                    {
                        case eItemType.Table:
                            con.ContentType = eContentType.Table;
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

                if (this.TableStatus.HasValue)
                {
                    if (con.IsContentsValid != this.TableStatus.Value) continue;
                }



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

                if (string.IsNullOrEmpty(uid))
                {
                    this.TableStatus = null;

                }
                else
                {
                    this.TableStatus = uid == "1";
                }


                BindList();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent item in this.Origin)
                {
                    if (item.IsContentsValid == true) continue;

                    string lineString = item.Temp.Temp.LineText;
                    if (lineString.StartsWith("<table>")) continue;

                    bool isValid = TextHelper.IsTableMarkdownValid(lineString);
                    if (isValid)
                    {
                        string[] lines = TextHelper.SplitText(lineString);


                        StringBuilder tableHtml = new StringBuilder();
                        tableHtml.Append("<table>");
                        tableHtml.Append("<thead>");

                        int rowHeaderCnt = 0;
                        bool isHeader = true;
                        foreach (string ln in lines)
                        {
                            if (TextHelper.IsNoText(ln)) continue;

                            if (TextHelper.IsTableDivider(ln))
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
                                        if (cellValue.Contains("\\n")) cellValue = cellValue.Replace("\\n", "\n");
                                        string[] cellLines = TextHelper.SplitText(cellValue);
                                        if (cellLines.Length != 1)
                                        {
                                            cellValue = "<ul>";

                                            foreach (string cellLine in cellLines)
                                            {
                                                string lineValue = cellLine;
                                                if (lineValue.First() == '-') lineValue = lineValue.Substring(1);
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

                        //item.Temp.SetText(tableHtml.ToString());
                        item.Temp_TableHTML = tableHtml.ToString();
                        item.InitializeDisplay();
                        item.IsContentsValid = true;
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

        private void btn_LineControl_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_RemovewEmpty_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_RemoveFirst_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_RemoveLast_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_KeywordRemove_Click(object sender, RoutedEventArgs e)
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

        }

        private void btn_AllApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (vmContent item in this.listbox_headers.SelectedItems)
                {
                    item.Temp.SetText(item.Temp_Content);
                }
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

        private void btn_MoveToText_Clcik(object sender, RoutedEventArgs e)
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

        private void btn_MoveToImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                List<vmContent> seleectedList = new List<vmContent>();
                foreach (vmContent item in this.listbox_headers.SelectedItems) seleectedList.Add(item);

                foreach (vmContent item in seleectedList)
                {
                    item.Temp.SetItemType(eItemType.Image);

                    string[] lines = TextHelper.SplitText(item.Temp_Content);
                    if (lines.Length == 1) item.ContentType = eContentType.Image;

                    (this.Tag as ucMarkCheckerContentsChecking).ucMarkCheckerCheckingImage.SetOriginList();


                    this.Origin.Remove(item);
                    BindList();
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
