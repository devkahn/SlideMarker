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
                this.Origin.Clear();
                if (value != null)
                {
                    foreach (vmContent con in value.Contents)
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



        public void BindList(string keyword = "", int page = -1)
        {
            ObservableCollection<vmContent> list = new ObservableCollection<vmContent>();
            foreach (vmContent con in this.Origin)
            {
                if(this.ImageStatus.HasValue)
                {
                   // if (this.ImageStatus.Value) continue;
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

            this.listbox_headers.ItemsSource = list.OrderBy(x => int.Parse(x.Display_SlideNum.ToString()));
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

        }

        private void btn_RemovewEmpty_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_RemoveFirst_Click(object sender, RoutedEventArgs e)
        {

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

        }

        private void TextBox_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void RadioButton_Single_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btn_RemoveLast_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_ApplySingle_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_ResetSingle_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {

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
    }
}
