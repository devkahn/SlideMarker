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
    /// ucMarkCheckerContentsCheckingTable.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucMarkCheckerContentsCheckingTable : UserControl
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

                BindList();
                BindPageComboBox();
            }
        }
        public ObservableCollection<vmContent> Origin { get; set; } = new ObservableCollection<vmContent>();


        public ucMarkCheckerContentsCheckingTable()
        {
            InitializeComponent();
        }


        public void BindList(string keyword = "", int page = -1)
        {
            ObservableCollection<vmContent> list = new ObservableCollection<vmContent>();
            foreach (vmContent con in this.Origin)
            {
                //if (this.ImageStatus.HasValue)
                //{
                //    if (this.ImageStatus.Value) continue;
                //}


                ////if (this.TextType != eContentType.All)
                ////{
                ////    if (con.ContentType != this.TextType) continue;
                ////}

                ////if (!string.IsNullOrEmpty(keyword))
                ////{
                ////    if (!con.Temp.Temp.LineText.ToUpper().Contains(keyword.ToUpper())) continue;
                ////}

                ////if (page > 0)
                ////{
                ////    if (con.Display_SlideNum.ToString() != page.ToString()) continue;
                ////}


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

        private void txtbox_SearchKeyword_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btn_ConvertToXML_Click(object sender, RoutedEventArgs e)
        {

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

        }
    }
}
