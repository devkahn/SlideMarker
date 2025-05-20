using System;
using System.Collections.Generic;
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
using MDM.Models.ViewModels;

namespace MDM.Views.DataLabeling.Pages
{
    /// <summary>
    /// ucDataLabelingSildes.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucDataLabelingSildes : UserControl
    {
        bool IsXMLOpen { get; set; } = false;
        bool IsSelectionChange { get; set; } = false;
        bool isFirstTry { get; set; } = true;
        ePageStatus StatusCode { get; set; } = ePageStatus.All;
        ePageStatus ChangeCode { get; set; } = ePageStatus.All;

        private vmMaterial _Material = null;
        private vmMaterial Material
        {
            get => _Material;
            set
            {
                _Material = value;
                this.DataContext = value;
                this.rbtn_PageFilter_All.IsChecked = true;
                BindPages();
            }
        }

        public ucDataLabelingSildes()
        {
            InitializeComponent();
        }

        public void SetMaterial(vmMaterial material, bool isMarkChecker = false)
        {
            if(isMarkChecker)
            {
                this._Material = material;
                this.DataContext = material;
                this.rbtn_PageFilter_All.IsChecked = true;
            }
            else
            {
                this.Material = material;
            }
        }
        private void MovePage()
        {
            bool isIndex = int.TryParse(this.txtbox_CurSlideIndex.Text, out int index);
            while (!isIndex)
            {
                vmSlide curSlide = this.listbox_Pages.SelectedItem as vmSlide;
                if (curSlide == null)
                {
                    return;
                }
                else
                {
                    this.txtbox_CurSlideIndex.Text = curSlide.Temp.Index.ToString();
                    index = curSlide.Temp.Index;
                }
                isIndex = true;
            }

            if (this.Material == null) return;

            vmSlide sameSlide = this.Material.Slides.Where(x => x.Temp.Index == index).FirstOrDefault();
            if (!this.IsXMLOpen && sameSlide == null)
            {
                
                string msg = string.Format("{0} 번 슬라이드가 없습니다.", index);
                MessageHelper.ShowErrorMessage("Slide 이동", msg);
                return;
            }
            else
            {
                this.listbox_Pages.SelectedItem = sameSlide;
                this.listbox_Pages.ScrollIntoView(this.listbox_Pages.SelectedItem);
                var scrollViewer = ControlHelper.FindVisualChild<ScrollViewer>(this.listbox_Pages);
                //scrollViewer.ScrollToBottom();
                //scrollViewer.ScrollToVerticalOffset(0);
            }
        }
        public void MovePage(int index)
        {
            if (this.IsSelectionChange)
            {
                this.IsSelectionChange = false;
                return;
            }

            this.txtbox_CurSlideIndex.Text = index.ToString();
            MovePage();
        }
        public void BindPages(List<vmSlide> slideList = null, bool isXMLOpen  = false)
        {
            if (slideList == null && this.Material == null) return;

            this.IsXMLOpen = isXMLOpen;

            List<vmSlide> pages = slideList == null ? this.Material.Slides.ToList() : slideList;

            if (this.StatusCode != ePageStatus.All) pages = pages.Where(x=> x.Status.Status == this.StatusCode).ToList();
            if (this.ChangeCode != ePageStatus.All)
            {
                if(this.ChangeCode == ePageStatus.Saved) pages = pages.Where(x => x.IsChanged == false).ToList();
                else if(this.ChangeCode == ePageStatus.Changed) pages = pages.Where(x=> x.IsChanged == true).ToList();
            }
            
            this.listbox_Pages.ItemsSource = null;
            this.listbox_Pages.ItemsSource  = pages;
            this.listbox_Pages.SelectedIndex = 0;

            if (this.isFirstTry)
            {
                this.toggle_PropertyFold.IsChecked = true;
                this.isFirstTry = false;
            }
        }
        public void MoveNext()
        {
            if (this.Material == null) return;

            List<vmSlide> cList = this.listbox_Pages.ItemsSource as List<vmSlide>;
            if (cList == null) cList = new List<vmSlide>();

            vmSlide selectedSlide = this.listbox_Pages.SelectedItem as vmSlide;
            if (selectedSlide == null) return;
            
            int order = cList.IndexOf(selectedSlide);
            this.txtbox_CurSlideIndex.Text = cList[++order].Temp.Index.ToString();
            MovePage();
        }


        private void btn_SlideMove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(this.Material == null) return;   
                MovePage();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_MovePre_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Material == null) return;

                List<vmSlide> cList = this.listbox_Pages.ItemsSource as List<vmSlide>;
                if (cList == null) cList = new List<vmSlide>();

                vmSlide selectedSlide = this.listbox_Pages.SelectedItem as vmSlide;
                if (selectedSlide == null)
                {
                    string currentIndex = this.txtbox_CurSlideIndex.Text;
                    bool isPageDigit = int.TryParse(currentIndex, out int index);
                    this.listbox_Pages.SelectedItem = selectedSlide = cList.Where(x => x.Temp.Index == index).FirstOrDefault();
                }
                if (selectedSlide == null) selectedSlide = cList.FirstOrDefault();
                if (selectedSlide == null) return;

                int order = cList.IndexOf(selectedSlide);
                this.txtbox_CurSlideIndex.Text = cList[--order].Temp.Index.ToString();
                MovePage();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_MoveNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Material == null) return;

                List<vmSlide> cList = this.listbox_Pages.ItemsSource as List<vmSlide>;
                if (cList == null) cList = new List<vmSlide>();

                vmSlide selectedSlide = this.listbox_Pages.SelectedItem as vmSlide;
                if (selectedSlide == null)
                {
                    string currentIndex = this.txtbox_CurSlideIndex.Text;
                    bool isPageDigit = int.TryParse(currentIndex, out int index);
                    this.listbox_Pages.SelectedItem = selectedSlide = cList.Where(x => x.Temp.Index == index).FirstOrDefault();
                }
                if (selectedSlide == null) selectedSlide = cList.FirstOrDefault();
                if (selectedSlide == null) return;

                int order = cList.IndexOf(selectedSlide);
                this.txtbox_CurSlideIndex.Text = cList[++order].Temp.Index.ToString();
                MovePage();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_MoveFirst_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Material == null) return;

                List<vmSlide> cList = this.listbox_Pages.ItemsSource as List<vmSlide>;
                if (cList == null) cList = new List<vmSlide>();

                vmSlide firstSlide = cList.FirstOrDefault();
                if (firstSlide == null) return;

                this.txtbox_CurSlideIndex.Text = firstSlide.Temp.Index.ToString();
                MovePage();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_MoveLast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Material == null) return;

                List<vmSlide> cList = this.listbox_Pages.ItemsSource as List<vmSlide>;
                if (cList == null) cList = new List<vmSlide>();

                vmSlide firstSlide = cList.LastOrDefault();
                if (firstSlide == null) return;

                this.txtbox_CurSlideIndex.Text = firstSlide.Temp.Index.ToString();
                MovePage();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void txtbox_CurSlideIndex_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (this.Material == null) return;

                TextBox tb = sender as TextBox;
                if (tb == null) return;

                if (e.Key == Key.Enter) MovePage();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void rBtn_SlideFilter_Check(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton rBtn = sender as RadioButton;
                if (rBtn == null) return;

                bool isUidDigit = int.TryParse(rBtn.Uid, out int code);
                if (!isUidDigit) code = -1;

                this.StatusCode = (ePageStatus)code;

                BindPages();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void listbox_Pages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if(this.IsSelectionChange)
                {
                    this.IsSelectionChange = false;
                    return;
                }

                List<vmSlide> cList = this.listbox_Pages.ItemsSource as List<vmSlide>;
                if (cList == null) cList = new List<vmSlide>();

                ListBox currentListbox = sender as ListBox;
                if (currentListbox == null) return;

                vmSlide selectedSlide = currentListbox.SelectedItem as vmSlide;
                if (selectedSlide == null) return;

                if (this.txtbox_CurSlideIndex != null)
                {
                    this.txtbox_CurSlideIndex.Text = selectedSlide.Temp.Index.ToString();
                }

                this.txtbox_Description.Text = selectedSlide.Temp.Description;

                this.btn_MoveFirst.IsEnabled = cList.FirstOrDefault() != selectedSlide;
                this.btn_MovePre.IsEnabled = cList.FirstOrDefault() != selectedSlide;
                this.btn_MoveLast.IsEnabled = cList.LastOrDefault() != selectedSlide;
                this.btn_MoveNext.IsEnabled = cList.LastOrDefault() != selectedSlide;

                if (this.Material != null && this.Material.OriginPresentation != null)
                {
                    this.IsSelectionChange = true;
                    //MessageBox.Show($"슬라이드 이동{selectedSlide.Temp.SlideId}", "슬라이드 이동", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    this.Material.OriginPresentation.Slides.FindBySlideID(selectedSlide.Temp.SlideId).Select();
                    this.IsSelectionChange = false;
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(this.Material.CurrentSlide.Status.Status == ePageStatus.Hold || this.Material.CurrentSlide.Status.Status == ePageStatus.Exception)
                {
                    string description = this.Material.CurrentSlide.Temp.Description;
                    if(string.IsNullOrEmpty(description))
                    {
                        string caption = "슬라이드 저장하기";
                        string eMsg = "보류나 예외는 비고란에 사유를 입력하세요.";
                        MessageHelper.ShowErrorMessage(caption, eMsg);
                        return;
                    }
                }

                    this.Cursor = Cursors.Wait;
                this.Material.CurrentSlide.Save();
                this.Cursor = Cursors.Arrow;
                
            }
            catch (Exception ee)
            {
                this.Cursor = Cursors.Arrow;
                ErrorHelper.ShowError(ee);
            }
        }

        private void rBtn_Status_Check(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.IsSelectionChange) return;

                RadioButton rBtn = sender as RadioButton;
                if(rBtn == null) return;

                bool isCodeValid = int.TryParse(rBtn.Uid, out int code);
                if (!isCodeValid) code = -1;

                ePageStatus status = (ePageStatus)code;

                this.Material.CurrentSlide.SetStatus(status);

                if (status == ePageStatus.Exception) this.grid_descriptionButtons.Visibility = Visibility.Visible;
               
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_AllSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string caption = "모두 저장하기";

                List<vmSlide> pages = this.Material.Slides.ToList();

                System.Windows.Forms.DialogResult result = System.Windows.Forms.DialogResult.Cancel;
                bool hasOnGoingItem = pages.Any(x => x.Status.Status == ePageStatus.None);
                if(hasOnGoingItem)
                {
                    string eMsg = string.Empty;
                    eMsg += "미완료 항목이 존재합니다.";
                    eMsg += "계속 진행하시겠습니까?";

                    string yesMsg = "[미완료]항목 포함";
                    string noMsg = "[미완료]항목 제외";

                    result =  MessageHelper.ShowYewNoCancelMessage(caption, eMsg, yesMsg, noMsg, "돌아가기");
                }

                if (result == System.Windows.Forms.DialogResult.Cancel) return;


                if(result == System.Windows.Forms.DialogResult.No) pages = pages.Where(x=> x.Status.Status != ePageStatus.None).ToList();
                foreach (vmSlide page in pages) page.Save();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void Grid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                vmSlide newSlide = e.NewValue as vmSlide;
                if (newSlide == null) return;

                this.IsSelectionChange = true;
                switch (newSlide.Status.Status)
                {
                    case ePageStatus.Completed: this.rBtn_Completed.IsChecked = true; break;
                    case ePageStatus.Hold: this.rBtn_Hold.IsChecked = true; break;
                    case ePageStatus.Exception: this.rBtn_Exception.IsChecked = true; break;
                    default: this.rBtn_OnGoing.IsChecked = true; break;
                }
                this.IsSelectionChange =false;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void txtbox_Description_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox tb = sender as TextBox; 
                if(tb == null) return;  

                vmSlide data = tb.DataContext as vmSlide;
                if(data == null) return;

                if(tb.Text != data.Temp.Description) data.SetDescription(tb.Text);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void rBtn_SlideStatus_Check(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton rbtn = sender as RadioButton;
                if(rbtn == null) return;

                bool isCodeValid = int.TryParse(rbtn.Uid, out int code);
                if (!isCodeValid) code = 0;

                this.ChangeCode = (ePageStatus)code;
                BindPages();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void txtbox_Description_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox tb = sender as TextBox;
                if (tb == null) return;

                vmSlide data = tb.DataContext as vmSlide;
                if (data == null) return;

                if (tb.Text != data.Temp.Description) data.SetDescription(tb.Text);
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
                Button btn = sender as Button;
                if (btn == null) return;

                string contentString =btn.Content.ToString();
                this.txtbox_Description.Text = contentString;
                this.grid_descriptionButtons.Visibility = Visibility.Collapsed;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
