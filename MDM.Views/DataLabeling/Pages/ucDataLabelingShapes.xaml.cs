using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MDM.Commons;
using MDM.Commons.Enum;
using MDM.Helpers;
using MDM.Models.DataModels;
using MDM.Models.ViewModels;
using Microsoft.Office.Interop.PowerPoint;
using System.Threading;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using Microsoft.Office.Core;
using Shape = Microsoft.Office.Interop.PowerPoint.Shape;
using System.Text.RegularExpressions;
using System.Reflection;

namespace MDM.Views.DataLabeling.Pages
{
    /// <summary>
    /// ucDataLabelingShapes.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucDataLabelingShapes : UserControl
    {
        eItemType ItemFilterCode { get; set; } = eItemType.None;

        private vmMaterial _Material = null;
        private vmMaterial Material
        {
            get => _Material;
            set
            {
                _Material = value;
                this.DataContext = value;
                this.rBtn_AllItemn_Filter.IsChecked = true;
                BindItems();
            }
        }

        private ObservableCollection<vmItem> _BindingItems = new ObservableCollection<vmItem>();
        public ObservableCollection<vmItem> BindingItems
        {
            get => _BindingItems;
            set
            {
                _BindingItems = value;
                this.datagrid_Shapes.ItemsSource = null;
                this.datagrid_Shapes.ItemsSource = value;
                this.datagrid_Shapes.SelectedIndex = 0;
            }
        }
        

        public ucDataLabelingShapes()
        {
            InitializeComponent();
        }

        public void SetMaterial(vmMaterial material)
        {
            this.Material = material;
        }
        public void BindItems()
        {
            if (this.Material == null) return;
            if (this.Material.CurrentSlide == null) return;
            if (this.datagrid_Shapes == null) return;

            var items = this.Material.CurrentSlide.Items.OrderBy(x => x.Temp.Order);

            ObservableCollection<vmItem> output = new ObservableCollection<vmItem>();
            foreach (var item in items)
            {
                switch (this.ItemFilterCode)
                {
                    case eItemType.Content: 
                        if(item.ItemTypeCode %220 <10) output.Add(item); 
                        break;
                    case eItemType.Text: 
                        if (item.ItemType == this.ItemFilterCode || item.ItemType == eItemType.None) output.Add(item); 
                        break;
                    case eItemType.Header:
                    case eItemType.Image:
                    case eItemType.Table:
                        if(item.ItemType == this.ItemFilterCode) output.Add(item);
                        break;
                    case eItemType.None:
                    case eItemType.All:
                    default:
                        output.Add(item);
                        break;
                }
            }

            this.BindingItems = output;
        }


        private void btn_DownLevel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                vmItem data = btn.DataContext as vmItem;
                if (data == null) return;

                if (data.Temp.Level == 1) return;
                if (data.ParentItem == null) return;


                vmItem parent = data.ParentItem;
                if (parent != null)
                {
                    parent = parent.ParentItem;
                }

                data.SetLevel(false);
                data.SetParentItem(parent);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_UpLevel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                vmItem data = btn.DataContext as vmItem;
                if (data == null) return;

                if (data.Temp.Level == 10) return;

                vmItem parent = data.ParentItem;
                if (parent != null)
                {
                    var sameLevels = parent.Children.Where(x => x != data && x.IsHeader && x.Temp.Level == data.Temp.Level);
                    if (!sameLevels.Any()) return;

                    int index = this.Material.CurrentSlide.Items.IndexOf(data);
                    parent = sameLevels.Where(x => this.Material.CurrentSlide.Items.IndexOf(x) < index).LastOrDefault();
                    if (parent == null) parent = sameLevels.FirstOrDefault();
                }

                data.SetLevel(true);
                data.SetParentItem(parent);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_MergeLines_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string caption = "아이템 병합";

                //var selectedItems = this.Material.CurrentSlide.Items.Where(x => x.IsSelected);
                var selectedItems  = new List<vmItem>();
                foreach (var item in this.datagrid_Shapes.SelectedItems)
                {
                    selectedItems.Add(item as vmItem);
                }
                selectedItems = selectedItems.OrderBy(x=> x.RowIndex).ToList(); 
                
                int total = selectedItems.Count();
                if(total <= 1)
                {
                    string eMsg = "병합할 아이템을 2개 이상 선택하세요.";
                    MessageHelper.ShowErrorMessage(caption, eMsg);
                    return;
                }

                bool isAllText = selectedItems.Where(x => x.ItemType == eItemType.Text).Count() == total;
                if(isAllText)
                {
                    List<vmItem> items = selectedItems.ToList();
                    vmItem firstItem = items[0];
                    string temp = firstItem.Temp.LineText;
                    for (int i = 1; i < total; i++)
                    {
                        vmItem cItem = items[i];
                        temp += "\n";
                        temp += cItem.Temp.LineText;
                        this.Material.CurrentSlide.RemoveItem(cItem);
                        this.BindingItems.Remove(cItem);
                    }
                    firstItem.SetText(temp);
                    firstItem.InitializeDisplay();
                    firstItem.SetRowIndex(this.BindingItems);
                }
                else
                {
                    if (total == 2)
                    {
                        var texts = selectedItems.Where(x => x.ItemType == eItemType.Text);
                        bool hasTextOne = texts.Count() == 1;
                        var images = selectedItems.Where(x => x.ItemType == eItemType.Image);
                        bool hasImageOne = images.Count() == 1;
                        var tables = selectedItems.Where(x => x.ItemType == eItemType.Table);
                        bool hasTableOne = tables.Count() == 1;
                        if(hasTextOne && hasImageOne)
                        {
                            vmItem textItem = texts.First();
                            vmItem imageItem = images.First();

                            imageItem.SetTitle(textItem.Temp.LineText);
                            imageItem.SetImageText(imageItem.Temp.Title, imageItem.ParentShape.Temp.Text);
                            this.Material.CurrentSlide.RemoveItem(textItem);
                            imageItem.SetRowIndex(this.BindingItems);
                        }
                    }
                    else
                    {
                        string eMsg = "이종 타입은 병합 할 수 없습니다.";
                        MessageHelper.ShowErrorMessage("행 병합", eMsg);
                    }
                }
                
                
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_AlignLines_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int currentHeaderLevel = -1;
                foreach (vmItem item in this.BindingItems.OrderBy(x=> x.RowIndex))
                {
                    if(item.ItemType == eItemType.Header)
                    {
                        currentHeaderLevel = item.Temp.Level;
                    }
                    else
                    {
                        item.SetLevel(currentHeaderLevel + 1);
                    }
                } 
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string caption = "선택 아이템 삭제";

                var selectedItems = this.Material.CurrentSlide.Items.Where(x => x.IsSelected);
                int total = selectedItems.Count();
                if (total <= 0)
                {
                    string eMsg = "삭제할 아이템을 1개 이상 선택하세요.";
                    MessageHelper.ShowErrorMessage(caption, eMsg);
                    return;
                }

                string msg = string.Format("{0}개의 아이템을 삭제하시겠습니까?", total);
                System.Windows.Forms.DialogResult result = MessageHelper.ShowYewNoMessage(caption, msg);
                if (result != System.Windows.Forms.DialogResult.Yes) return;

                if (selectedItems == null) return;
                foreach (vmItem item in selectedItems.ToList())
                {
                    if (item == null) continue;
                    this.Material.CurrentSlide.RemoveItem(item);
                    this.BindingItems.Remove(item);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_AddItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string caption = "새로운 아이템 추가";

                var selectedItems = this.Material.CurrentSlide.Items.Where(x => x.IsSelected);
                vmItem lastItem = selectedItems.LastOrDefault();
                if (lastItem == null) lastItem = this.Material.CurrentSlide.Items.LastOrDefault();

                int index = 0;
                if (lastItem != null) index = this.Material.CurrentSlide.Items.IndexOf(lastItem) + 1;

                mItem newItem = new mItem();
                vmItem newVM = new vmItem(newItem);
                newVM.SetParentItem(lastItem.IsHeader ? lastItem : lastItem.ParentItem, false);
                this.Material.CurrentSlide.AddItem(newVM, index);// .Items.Insert(index, newVM);
                this.BindingItems.Insert(index, newVM);

                if (lastItem != null || selectedItems.Count() > 0) newVM.SetParent(lastItem.ParentShape);

                for (int i = index; i < this.BindingItems.Count; i++) this.BindingItems[i].SetRowIndex(this.BindingItems);
                

                this.datagrid_Shapes.SelectedItem = newVM;
                this.datagrid_Shapes.ScrollIntoView(this.datagrid_Shapes.SelectedItem);
                var scrollViewer = ControlHelper.FindVisualChild<ScrollViewer>(this.datagrid_Shapes);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_ClearLines_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var emptyItems = this.Material.CurrentSlide.Items.Where(x => TextHelper.IsNoText(x.Temp.LineText));
                foreach (vmItem item in emptyItems.ToList())
                {
                    this.Material.CurrentSlide.RemoveItem(item);
                    this.BindingItems.Remove(item);
                }
                foreach (vmItem item in this.BindingItems) item.SetRowIndex(this.BindingItems);
                
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_SplitLines_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string caption = "선택 아이템 행 분할";

                var selectedItems = this.Material.CurrentSlide.Items.Where(x => x.IsSelected);
                int total = selectedItems.Count();
                if (total <= 0) return;
             
                foreach (vmItem item in selectedItems.ToList())
                {
                    string[] lines = TextHelper.SplitText(item.Temp.LineText);
                    int cnt = lines.Count();
                    if (cnt <= 1) continue;

                    int index = this.BindingItems.IndexOf(item);

                    vmItem current = item;
                    while (current != null)
                    {
                        for (int i = 1; i < cnt; i++)
                        {
                            vmItem newItem = current.Duplicate();
                            newItem.SetText(lines[i]);
                            newItem.SetParentItem(current.ParentItem);
                            this.BindingItems.Insert(index + i, newItem);

                            current = i + 1 == cnt ? null : newItem;
                        }
                    }
                    item.SetText(lines[0]);

                    
                    for (int i = index; i < this.BindingItems.Count() ; i++) this.BindingItems[i].SetRowIndex(this.BindingItems);
                    
                }




            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_SplitCurrent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                TextBox tb = btn.Tag as TextBox;
                if (tb == null) return;

                vmItem currentItem = btn.DataContext as vmItem;
                if (currentItem == null) return;

                int caretIndex = tb.CaretIndex;
                string preText = tb.Text.Substring(0, caretIndex).Trim();
                string nextText = tb.Text.Substring(caretIndex).Trim();

                bool isPreTextEmpty = string.IsNullOrEmpty(preText) || string.IsNullOrWhiteSpace(preText);
                bool isNextTextEmpty = string.IsNullOrEmpty(nextText) || string.IsNullOrWhiteSpace(nextText);

                int index = currentItem.ParentShape.ParentSlide.Items.IndexOf(currentItem);
                if (!isPreTextEmpty && !isNextTextEmpty)
                {
                    currentItem.SetText(preText);
                    vmItem newItem = currentItem.Duplicate();
                    newItem.SetText(nextText);
                    newItem.SetParentItem(currentItem.ParentItem);
                    //  newItem.ParentShape.ParentSlide.AddItem(newItem, index+1);
                    this.BindingItems.Insert(index + 1, newItem);
                }

                for (int i = index; i < this.BindingItems.Count(); i++) this.BindingItems[i].SetRowIndex(this.BindingItems);

                this.datagrid_Shapes.CommitEdit();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_SelectItemType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                vmItem data = btn.DataContext as vmItem;
                if (data == null) return;

                switch (data.ItemType)
                {
                    case eItemType.Image:
                       if(data.ParentShape != null) data.ParentShape.UndoText();
                        data.UndoText();
                        data.SetItemType(eItemType.Table);
                        break;
                    case eItemType.Text:
                    case eItemType.None:
                        // data.ParentShape.SetTitle(data.ParentShape.Temp.Text);
                        // data.ParentShape.SetText(Guid.NewGuid().ToString());
                        data.SetImageText();// .SetText(data.Temp.GenerateImageLineText(data.ParentShape.Temp));
                        data.SetItemType(eItemType.Image);
                        break;
                    case eItemType.Table:
                    default:
                        if (data.ParentShape != null) data.ParentShape.UndoText();
                        data.UndoText();
                        data.SetItemType(eItemType.Text);
                        break;
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                vmItem data = btn.DataContext as vmItem;
                if (data == null) return;

                this.Material.CurrentSlide.RemoveItem(data);
                this.BindingItems.Remove(data);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_EditCompleted_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                TextBox tb = btn.Tag as TextBox;
                if (tb == null) return;

                vmItem item = btn.DataContext as vmItem;
                if (item == null) return;


                string value = tb.Text;
                switch (item.ItemType)
                {
                    case eItemType.Image:
                        Match match = TextHelper.IsImageMarkdown(value);
                        if (match.Success)
                        {
                            string title = match.Groups[1].Value;
                            string id = match.Groups[2].Value;

                            bool isIdValid = Guid.TryParse(id, out Guid result);
                            if (isIdValid)
                            {
                                item.SetTitle(title);
                                item.Temp.Uid = id;
                                item.SetImageText();
                                this.datagrid_Shapes.CommitEdit();
                                return;
                            }
                        }
                        string eMsg = "텍스트 형식이 맞지 않습니다.";
                        MessageHelper.ShowErrorMessage( "이미지 텍스트 변경", eMsg);
                        e.Handled = false;
                        return;
                    case eItemType.Table:
                    default:
                        item.SetText(tb.Text);
                        this.datagrid_Shapes.CommitEdit();
                        return;
                }
                

                
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_GoToUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vmItem selectedItem = this.datagrid_Shapes.SelectedItem as vmItem;
                if (selectedItem == null) return;

                var list = this.datagrid_Shapes.ItemsSource as ObservableCollection<vmItem>;
                if (list == null) return;

                int index = list.IndexOf(selectedItem);
                if (index <= 0) return;

                vmItem upperItem = list[index - 1] as vmItem;
                if (upperItem == null) return;

                list.Move(index, index - 1);
                selectedItem.SetRowIndex(this.BindingItems);
                upperItem.SetRowIndex(this.BindingItems);

                if (upperItem.IsHeader) selectedItem.SetParentItem(upperItem.ParentItem);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_GoToDown_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vmItem selectedItem = this.datagrid_Shapes.SelectedItem as vmItem;
                if (selectedItem == null) return;

                var list = this.datagrid_Shapes.ItemsSource as ObservableCollection<vmItem>;
                if (list == null) return;

                int index = list.IndexOf(selectedItem);
                if (index + 1 == list.Count) return;

                vmItem downItem = list[index + 1] as vmItem;
                if (downItem == null) return;

                list.Move(index, index + 1);
                selectedItem.SetRowIndex(this.BindingItems);
                downItem.SetRowIndex(this.BindingItems);

                if (downItem.IsHeader) selectedItem.SetParentItem(downItem);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_MergeShapes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vmItem selectedItem = this.datagrid_Shapes.SelectedItem as vmItem;
                if (selectedItem == null) return;

                vmShape parent = selectedItem.ParentShape;

                var list = parent.Items.ToList();
                int total = list.Count();
                vmItem firstItem = list[0];
                for (int i = 1; i < total; i++)
                {
                    vmItem cItem = list[i];
                    firstItem.Merge(cItem);
                    this.Material.CurrentSlide.RemoveItem(cItem);
                    this.BindingItems.Remove(cItem);
                }
                firstItem.InitializeDisplay();
                firstItem.SetRowIndex(this.BindingItems);

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_ImageDownload_Click(object sender, RoutedEventArgs e)
        {
            Bitmap originImage = null;
            Bitmap croppedImage = null;
            try
            {
                Button btn = sender as Button;
                if (btn == null) return;

                vmItem data = btn.DataContext as vmItem;
                if (data == null) return;

                mShape iShape = data.ParentShape.Temp;

                Slide currentSlide = (Slide)this.Material.OriginPresentation.Application.ActiveWindow.View.Slide;

                Slide firstSlide = this.Material.OriginPresentation.Slides[1];
                int slideIndex = firstSlide.SlideNumber == 0 ? currentSlide.SlideNumber + 1 : currentSlide.SlideNumber;

                string tempDirPath = Path.Combine(this.Material.DirectoryPath, "Temp");
                if (!Directory.Exists(tempDirPath)) Directory.CreateDirectory(tempDirPath);
                string pngFullPath = Path.Combine(tempDirPath, string.Format("page_{0}", slideIndex)+".png");
                if (!File.Exists(pngFullPath))
                {
                    string eMsg = "슬라이드 원본 이미지를 추가하세요.";
                    string caption = "이미지 다운로드";
                    MessageHelper.ShowErrorMessage(caption, eMsg);
                    return;
                }
                originImage = new Bitmap(pngFullPath);

           
                // 슬라이드 크기 가져오기
                float slideWidth = this.Material.OriginPresentation.PageSetup.SlideWidth;
                float slideHeight = this.Material.OriginPresentation.PageSetup.SlideHeight;

                // 크롭 속성 가져오기
                float cropLeft = (iShape.Left * originImage.Width) / slideWidth;
                float cropTop = (iShape.Top * originImage.Height) / slideHeight;
                // 크롭된 이미지의 크기 계산
                float croppedWidth = (iShape.Width * originImage.Width) / slideWidth;
                float croppedHeight = (iShape.Height * originImage.Height) / slideHeight;

                Rectangle cropArea = new Rectangle((int)cropLeft - 3, (int)cropTop - 3, (int)croppedWidth + 5, (int)croppedHeight + 5);
                croppedImage = originImage.Clone(cropArea, originImage.PixelFormat);
                string imagePath = Path.Combine(this.Material.DirectoryPath, data.Temp.Uid + Defines.EXTENSION_IMAGE);
                if (File.Exists(imagePath))
                {
                    data.SetPreviewItem(true);
                    File.Delete(imagePath);
                }
                croppedImage.Save(imagePath, ImageFormat.Png);
                croppedImage.Dispose();
                originImage.Dispose();
                //File.Delete(fullpath);

                data.OnImageFileExistChanged();
                data.SetPreviewItem();
            }
            catch (Exception ee)
            {
                if (originImage != null) originImage.Dispose();
                if (croppedImage != null) croppedImage.Dispose();
                ErrorHelper.ShowError(ee);
            }
        }
        
        private void btn_CompletedSaveNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                vmSlide current = this.Material.CurrentSlide;
                if (current == null) return;

                foreach (vmItem item in this.Material.CurrentSlide.Items) item.SetRowIndex();

                current.SetStatus(ePageStatus.Completed);
                
                current.Save();
                (this.Tag as ucDataLabeling).DataLabelingSlides.MoveNext();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
      
        private void btn_AllFold_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var headers = this.Material.CurrentSlide.Items.Where(x => x.IsHeader);
                foreach (var item in headers)
                {
                    vmItem header = item as vmItem;
                    if (header == null) return;

                    header.IsFolded = true;
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_AllUnFold_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var headers = this.Material.CurrentSlide.Items.Where(x => x.IsHeader);
                foreach (var item in headers)
                {
                    vmItem header = item as vmItem;
                    if (header == null) return;

                    header.IsFolded = false;
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void datagrid_Shapes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                foreach (vmItem item in e.RemovedItems)
                {
                    if (item == null) continue;
                    item.IsSelected = false;
                }
                foreach (vmItem item in e.AddedItems)
                {
                    if (item == null) continue;
                    item.IsSelected = true;

                    if (item.ParentShape == null) return;
                    if (this.Material.OriginPresentation != null)
                    {
                        Slide currentSlide = (Slide)this.Material.OriginPresentation.Application.ActiveWindow.View.Slide;
                        if (currentSlide == null) continue;

                        foreach (Shape sh in currentSlide.Shapes)
                        {
                            if (sh.Id == item.ParentShape.Temp.ShapeId) sh.Select();
                        }
                    }
                    
                }
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
                this.rBtn_AllItemn_Filter.IsChecked = true;
                BindItems();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }


        private void rBtn_LineFilter_Check(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton rBtn = sender as RadioButton;
                if (rBtn == null) return;

                bool isCodeValid = int.TryParse(rBtn.Uid, out int code);
                if (!isCodeValid) code = 0;

                this.ItemFilterCode = (eItemType)code;
                BindItems();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                TextBox tb = sender as TextBox;
                if (tb == null) return;

                if(e.Key == Key.Enter)
                {
                    int caretIndex = tb.CaretIndex;
                    tb.Text = tb.Text.Insert(caretIndex, "\n");
                    tb.CaretIndex = caretIndex + 1;
                }
                if (e.Key == Key.Tab)
                {
                    int caretIndex = tb.CaretIndex;
                    tb.Text = tb.Text.Insert(caretIndex, "\t");
                    tb.CaretIndex = caretIndex + 1;
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void txtbox_ItemContent_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox tb = sender as TextBox;
                if (tb == null) return;

                vmItem item = tb.DataContext as vmItem;
                if (item == null) return;

                tb.Text = item.Temp.LineText;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void toggle_IsHeading_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                ToggleButton tBtn = sender as ToggleButton;
                if (tBtn == null) return;

                vmItem data = tBtn.DataContext as vmItem;
                if (data == null) return;

                bool isHead = tBtn.IsChecked == true;
                data.SetItemType(isHead ? eItemType.Header : eItemType.Text);
                if (10 < data.Temp.Level) data.SetLevel(10);
                
                var items = this.Material.CurrentSlide.Items;
                int index = items.IndexOf(data);
                
                var overItems = items.Where(x => items.IndexOf(x) < index && x.ItemType == eItemType.Header);
                vmItem lastHeader = overItems.Where(x => x.Temp.Level + 1 == data.Temp.Level).LastOrDefault();
                data.SetParentItem(lastHeader);
                

                var underItems = items.Where(x => index < items.IndexOf(x));
                if (isHead)
                {
                    foreach (vmItem underItem in underItems)
                    {
                        if(underItem.ItemType != eItemType.Header)
                        {
                            underItem.SetParentItem(data);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    foreach (vmItem underItem in underItems) underItem.SetParentItem(data.ParentItem);
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void toggle_showhideChildren_Check(object sender, RoutedEventArgs e)
        {
            try
            {
                ToggleButton tBtn = sender as ToggleButton;
                if (tBtn == null) return;

                vmItem data = tBtn.DataContext as vmItem;
                if (data == null) return;

                data.IsFolded = tBtn.IsChecked == true;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void toggle_AllSelect_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                ToggleButton tBtn = sender as ToggleButton;
                if (tBtn == null) return;

                bool isChecked = tBtn.IsChecked == true;
                if (isChecked) this.datagrid_Shapes.SelectAll();
                else this.datagrid_Shapes.UnselectAll();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }



        
     
        private void btn_ReLoadOrigin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Material.CurrentSlide.ClearItems();
                this.Material.CurrentSlide.Shapes.Clear();

                List<Shape> shapes = new List<Shape>();

                #region Slide Shape
               // foreach (Shape shape in this.Material.CurrentSlide.slid.Shapes)
                //{
                //    if (shape.Type == MsoShapeType.msoGroup)
                //    {
                //        shape.Ungroup();
                //        continue;
                //    }

                //    bool isAny = shapes.Where(x => x != null && x.Id == shape.Id).Any();
                //    if (!isAny) shapes.Add(shape);
                //}
                //var masterShapes = slide.Master.Shapes;
                //foreach (Shape shape in masterShapes)
                //{
                //    if (shape.Type == MsoShapeType.msoGroup) shape.Ungroup();

                //    bool isAny = shapes.Where(x => x.Id == shape.Id).Any();
                //    if (!isAny) shapes.Add(shape);
                //}
                #endregion


                BindItems();

            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_ReLoadDb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Material.CurrentSlide.Temp.Idx == -1) return;

                this.Material.CurrentSlide.ClearItems();
                this.Material.CurrentSlide.Shapes.Clear();
                this.Material.CurrentSlide.LoadChildren();
                BindItems();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_AddNEwShape_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //#region 이전 코드
                //List<Shape> shapes = new List<Shape>();
                ////#region Slide Shape
                ////foreach (Shape shape in this.Material.SelectedShapes.ToList())
                ////{
                ////    if (shape.Type == MsoShapeType.msoGroup)
                ////    {
                ////        shape.Ungroup();
                ////        continue;
                ////    }

                ////    bool isAny = shapes.Where(x => x != null && x.Id == shape.Id).Any();
                ////    if (!isAny) shapes.Add(shape);
                ////}

                //#endregion
                //MessageBox.Show(shapes.Count.ToString(), "Add New Shape");

                //List<mShape> shapeInstances = new List<mShape>();
                //foreach (Shape shape in shapes)
                //{
                //    if (shape.HasTable == MsoTriState.msoTrue)
                //    {
                //        mShape newTable = PowerpointHelper.GetTableShape(shape);
                //        shapeInstances.Add(newTable);
                //    }
                //    else if (shape.Type == MsoShapeType.msoPicture)
                //    {
                //        mShape newImage = PowerpointHelper.GetImageShpe(shape);
                //        shapeInstances.Add(newImage);
                //    }
                //    else if (shape.Type == MsoShapeType.msoAutoShape)
                //    {
                //        if (shape.HasTextFrame == MsoTriState.msoTrue && shape.TextFrame.HasText == MsoTriState.msoTrue)
                //        {
                //            mShape newText = PowerpointHelper.GetTextShape(shape);
                //            shapeInstances.Add(newText);
                //        }
                //        else
                //        {
                //            mShape newImage = PowerpointHelper.GetImageShpe(shape);
                //            shapeInstances.Add(newImage);
                //        }
                //    }
                //    else if (shape.HasTextFrame == MsoTriState.msoTrue && shape.TextFrame.HasText == MsoTriState.msoTrue)
                //    {
                //        mShape newText = PowerpointHelper.GetTextShape(shape);
                //        shapeInstances.Add(newText);
                //    }
                //}
                //shapeInstances = shapeInstances.OrderByOriginPoint();
                //MessageBox.Show(shapeInstances.Count.ToString(), "Add New Shape");

                //foreach (mShape shape in shapeInstances)
                //{
                //    vmShape newShape = new vmShape(shape);
                //    newShape.ParentSlide = this.Material.CurrentSlide;
                //    foreach (vmItem ln in newShape.Items) this.Material.CurrentSlide.Items.Add(ln);
                //    this.Material.CurrentSlide.Shapes.Add(newShape);
                //}
                //#endregion



                //Slide slide = this.Material.OriginPresentation.Slides.FindBySlideID(this.Material.CurrentSlide.Temp.Index);
                //foreach (Shape item in slide.Shapes)
                //{

                //}
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_CheckQuality_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
