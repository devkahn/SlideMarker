﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MDM.Commons;
using MDM.Commons.Enum;
using MDM.Models.DataModels;
using Microsoft.Office.Interop.PowerPoint;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Windows.Controls.Image;

namespace MDM.Models.ViewModels
{
    public partial class vmItem 
    {
        private mItem _Origin = null;
        private vmItem _ParentItem = null;
        private bool _IsSelected = false;
        private bool _IsFolded = true;
        private eItemType _ItemType = eItemType.None;
        private Thickness _RowMargin = new Thickness(5);
        private Visibility _RowVisibility = Visibility.Visible;
        private int _RowIndex = -1;

        private object _Display_Level = null;
        private object _Display_Indent = null;
        private object _Display_Title = null;
        private object _Display_Text = null;
        private object _Display_Update = null;
        private object _Display_PreviewItem = null;
        private object _Display_PreviewItem_Slide = null;
    }

    public partial class vmItem : vmViewModelbase
    {
        public vmItem(mItem origin)
        {
            this.Origin = origin;
        }

        private mItem Origin
        {
            get => _Origin;
            set
            {
                this._Origin = value;
                if (value == null)
                {
                    this.Temp = null;
                    return;
                }

                this.Temp = this.Origin.Copy<mItem>();
                InitializeDisplay();
            }
        }
        public mItem Temp { get; private set; } = null;
        public vmShape ParentShape { get; private set; } = null;
        public vmItem ParentItem
        {
            get => _ParentItem;
            private set
            {
                _ParentItem = value;
                //if (_ParentItem != null) _ParentItem.RemoveChild(this);
                //_ParentItem = value;
                
                //int level = 1;
                //if (_ParentItem != null)
                //{
                //    this.Temp.ParentItemUid = value.Temp.Uid;
                //    _ParentItem.AddChild(this);
                //    level = _ParentItem.Temp.Level + 1;
                //}
                //SetLevel(level);
            }
        }
        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                _IsSelected = value;
                if(this.ParentShape != null) this.ParentShape.SetSameShapeVisibility(value);
                OnPropertyChanged(nameof(this.IsSelected));
            }
        }
        public bool IsHeader => this.ItemType == eItemType.Header;
        public bool IsFolded
        {
            get => _IsFolded;
            set
            {
                _IsFolded = value;
                foreach (vmItem child in this.Children)
                {
                    if (_IsFolded) child.Hide();
                    else child.Show();
                }
                OnPropertyChanged(nameof(this.IsFolded));
            }
        }
        public bool IsImageFileExist
        {
            get
            {
                if (this.ItemType != eItemType.Image) return false;
                if (this.ParentShape == null) return false;
                if (string.IsNullOrEmpty(this.ParentShape.ParentSlide.ParentMaterial.DirectoryPath)) return false;

                mShape iShape = this.ParentShape.Temp;
                if(iShape == null) return false;

                string path = Path.Combine(this.ParentShape.ParentSlide.ParentMaterial.DirectoryPath, this.Temp.Uid + Defines.EXTENSION_IMAGE);
                return File.Exists(path);
            }
        }
        public int ItemTypeCode => this.ItemType.GetHashCode();
        public bool IsUsed { get; set; } = false;


        public eItemType ItemType
        {
            get => _ItemType;
            private set
            {
                _ItemType = value;
                OnPropertyChanged(nameof(this.ItemType));
                OnPropertyChanged(nameof(this.IsHeader));
                OnPropertyChanged(nameof(this.ItemTypeCode));
            }
        }
        public Thickness RowMargin
        {
            get => _RowMargin;
            private set
            {
                _RowMargin = value;
                OnPropertyChanged(nameof(RowMargin));
            }
        }
        public Visibility RowVisibility
        {
            get => _RowVisibility;
            private set
            {
                _RowVisibility = value;
                OnPropertyChanged(nameof(this.RowVisibility));
            }
        }
        public int RowIndex
        {
            get => this._RowIndex;
            //{
            //    return  this.ParentShape == null? -1 : this.ParentShape.ParentSlide.Items.IndexOf(this);
            //}
            private set
            {
                this._RowIndex = value;
                OnPropertyChanged(nameof(this.RowIndex));
            }
        }


        public ReadOnlyObservableCollection<vmItem> Children => new ReadOnlyObservableCollection<vmItem>(this.OriginChildren);
        private ObservableCollection<vmItem> OriginChildren { get; set; }



        public object Display_Level
        {
            get => _Display_Level;
            set
            {
                _Display_Level = value;
                SetIndent();
                OnPropertyChanged(nameof(this.Display_Level));
            }
        }
        public object Display_Indent
        {
            get => _Display_Indent;
            set
            {
                _Display_Indent = value;
                OnPropertyChanged(nameof(this.Display_Indent));
            }
        }
        public object Display_Title
        {
            get => _Display_Title;
            private set
            {
                _Display_Title = value;
                OnPropertyChanged(nameof(Display_Title));
            }
        }
        public object Display_Text
        {
            get => _Display_Text;
            set
            {
                _Display_Text = value;
                OnPropertyChanged(nameof(this.Display_Text));
            }
        }
        public object Display_UpdateDate
        {
            get => _Display_Update;
            set
            {
                _Display_Update = value;
                OnPropertyChanged(nameof(this.Display_UpdateDate));
            }
        }
        public object Display_PreviewItem
        {
            get
            {
                return _Display_PreviewItem;
            }
            set
            {
                _Display_PreviewItem = value;
                OnPropertyChanged(nameof(this.Display_PreviewItem));
            }
        }
        public object Display_PreviewItem_Slide 
        {
            get
            {
                switch (this.ItemType)
                {
                    case eItemType.Header: return GenerateHeaderCell(this);
                        
                    case eItemType.Image: return GenerateImageCell(this);
                    case eItemType.Table: return GenerateTableCell(this);
                    default:return GenerateTextCell(this);
                }
            }
            
        }
    }

    public partial class vmItem 
    {
        public void AddChild(vmItem child)
        {
            if (HasChild(child)) return;

            this.OriginChildren.Add(child);
            OnPropertyChanged(nameof(this.Children));
        }
        public vmItem Duplicate()
        {
            mItem newItem = new mItem();
            newItem.ParentItemUid = this.Temp.ParentItemUid;
            newItem.ParentShapeIdx = this.Temp.ParentShapeIdx;
            newItem.ParentItemIdx = this.Temp.ParentItemIdx;
            newItem.ItemType = this.Temp.ItemType;
            newItem.Level = this.Temp.Level;
            newItem.LineText = this.Temp.LineText;
            newItem.Title = this.Temp.Title;


            int originIndex = this.ParentShape.Temp.Lines.IndexOf(this.Temp);
            this.ParentShape.Temp.Lines.Insert(originIndex + 1, newItem);

            vmItem output = new vmItem(newItem);
            output.ParentShape = this.ParentShape;
            output.ParentItem = this.ParentItem;

            int vmIndex = this.ParentShape.Items.IndexOf(this);
            this.ParentShape.Items.Insert(vmIndex + 1, output);
            int slideIndex = this.ParentShape.ParentSlide.Items.IndexOf(this);
            this.ParentShape.ParentSlide.AddItem(output, slideIndex + 1);// .Items.Insert(slideIndex + 1, output);

            return output;
        }
        internal void Remove()
        {
            this.IsUsed = this.Temp.IsUsed = false;
            
        }
        private UIElement GenerateTextCell(vmItem item)
        {
            TextBlock output = new TextBlock();
            output.Margin = new Thickness(5);
            output.Text = item.Temp.LineText;
            output.FontSize = 10;
            return output;
        }
        private UIElement GenerateTextCell(string tempValue)
        {
            TextBlock output = new TextBlock();
            output.Margin = new Thickness(5);
            output.Text = tempValue;
            output.FontSize = 10;
            return output;
        }
        private UIElement GenerateTableCell(vmItem item)
        {
            StackPanel output = new StackPanel();
            output.Margin = new Thickness(5);
            output.HorizontalAlignment = HorizontalAlignment.Left;

            string[] rows = Regex.Split(item.Temp.LineText, @"[\v\r\n]");

            Border boundary = new Border();
            boundary.Background = Brushes.White;
            boundary.BorderBrush = Brushes.LightGray;
            boundary.BorderThickness = new Thickness(1, 1, 0, 0);

            Grid table = new Grid();
            boundary.Child = table;
            for (int i = 0; i < rows.Count(); i++) table.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < rows[0].Split('|').Count(); i++) table.ColumnDefinitions.Add(new ColumnDefinition());

            bool isHeader = true;
            int rowHeaderCnt = 0;
            for (int r = 0; r < rows.Count(); r++)
            {
                
                string value = rows[r].Trim();
                if (value.Contains('+')) value = value.Replace("+", "|");

                List<string> cells = value.Split('|').ToList();
                if (cells.FirstOrDefault() == string.Empty) cells.RemoveAt(0);
                if(cells.LastOrDefault() == string.Empty) cells.RemoveAt(cells.Count- 1);

                #region Column HeaderRow Count
                var distictList = cells.Distinct();
                if (distictList.Count() == 1)
                {
                    string bar = distictList.First();
                    bar = bar.Trim();

                    int cnt = bar.Count(x => !x.Equals(' ') && !x.Equals('-'));
                    if (cnt == 0)
                    {
                        value = rows[r].Trim();
                        #region RowHeaderCount Column Count
                        if (value.Contains('+'))
                        {
                            foreach (char c in value)
                            {
                                if (c.Equals('|')) rowHeaderCnt++;
                                if (c.Equals('+'))
                                {
                                    value = value.Replace("+", "|");
                                    break;
                                }
                            }
                        }
                        #endregion
                        isHeader = false;
                        continue;
                    }
                }
                #endregion

                for (int c = 0; c < cells.Count(); c++)
                {
                    Border border = new Border();
                    border.Background = isHeader ? Brushes.Navy : c<rowHeaderCnt? Brushes.LightGray : Brushes.White;
                    border.BorderBrush = Brushes.LightGray;
                    border.BorderThickness = new Thickness(0,0,1,1);
                    table.Children.Add(border);
                    Grid.SetColumn(border, c);
                    Grid.SetRow(border, r);

                    string cellString = cells[c];

                    Match match = IsImageMarkdown(cellString);
                    if (match.Success)
                    {
                        string imagename = match.Groups[1].Value;
                        string filename = match.Groups[2].Value;

                        Image cellValue = new Image();
                        if (this.ParentShape != null)
                        {
                            string dir = this.ParentShape.ParentSlide.ParentMaterial.DirectoryPath;
                            string filePath = Path.Combine(dir, filename + Defines.EXTENSION_IMAGE);
                            if(File.Exists(filePath))
                            {
                                BitmapImage bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                Uri url = new Uri(filePath, UriKind.Absolute);
                                bitmap.UriSource = url; // 이미지 파일 경로
                                bitmap.EndInit();
                                cellValue.Source = bitmap;
                            }
                        }
                        border.Child = cellValue;
                    }
                    else
                    {
                        TextBlock cellValue = new TextBlock();
                        cellValue.Text = cellString;
                        cellValue.Foreground = isHeader ? Brushes.White : Brushes.Black;
                        cellValue.FontWeight = isHeader ? FontWeights.Bold : c < rowHeaderCnt ? FontWeights.Bold : FontWeights.Normal;
                        cellValue.HorizontalAlignment = HorizontalAlignment.Center;
                        cellValue.VerticalAlignment = VerticalAlignment.Center;
                        cellValue.Margin = string.IsNullOrEmpty(cellValue.Text) ? new Thickness(0) : new Thickness(10);
                        border.Child = cellValue;
                    }
                }
            }

            TextBlock title = new TextBlock();
            title.Text = string.Format("<{0}>", item.Temp.LineText);
            title.FontWeight = FontWeights.Bold;
            title.FontSize = 12;
            title.HorizontalAlignment = HorizontalAlignment.Center;
            title.Margin = new Thickness(0, 5, 0, 0);

            output.Children.Add(boundary);
            //output.Children.Add(title);
            return output;
        }
        private UIElement GenerateTableCell(string  tempValue)
        {
            StackPanel output = new StackPanel();
            output.Margin = new Thickness(5);
            output.HorizontalAlignment = HorizontalAlignment.Left;

            string[] rows = Regex.Split(tempValue, @"[\v\r\n]");

            Border boundary = new Border();
            boundary.Background = Brushes.White;
            boundary.BorderBrush = Brushes.LightGray;
            boundary.BorderThickness = new Thickness(1, 1, 0, 0);

            Grid table = new Grid();
            boundary.Child = table;
            for (int i = 0; i < rows.Count(); i++) table.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < rows[0].Split('|').Count(); i++) table.ColumnDefinitions.Add(new ColumnDefinition());

            bool isHeader = true;
            int rowHeaderCnt = 0;
            for (int r = 0; r < rows.Count(); r++)
            {

                string value = rows[r].Trim();
                if (value.Contains('+')) value = value.Replace("+", "|");

                List<string> cells = value.Split('|').ToList();
                if (cells.FirstOrDefault() == string.Empty) cells.RemoveAt(0);
                if (cells.LastOrDefault() == string.Empty) cells.RemoveAt(cells.Count - 1);

                #region Column HeaderRow Count
                var distictList = cells.Distinct();
                if (distictList.Count() == 1)
                {
                    string bar = distictList.First();
                    bar = bar.Trim();

                    int cnt = bar.Count(x => !x.Equals(' ') && !x.Equals('-'));
                    if (cnt == 0)
                    {
                        value = rows[r].Trim();
                        #region RowHeaderCount Column Count
                        if (value.Contains('+'))
                        {
                            foreach (char c in value)
                            {
                                if (c.Equals('|')) rowHeaderCnt++;
                                if (c.Equals('+'))
                                {
                                    value = value.Replace("+", "|");
                                    break;
                                }
                            }
                        }
                        #endregion
                        isHeader = false;
                        continue;
                    }
                }
                #endregion

                for (int c = 0; c < cells.Count(); c++)
                {
                    Border border = new Border();
                    border.Background = isHeader ? Brushes.Navy : c < rowHeaderCnt ? Brushes.LightGray : Brushes.White;
                    border.BorderBrush = Brushes.LightGray;
                    border.BorderThickness = new Thickness(0, 0, 1, 1);
                    table.Children.Add(border);
                    Grid.SetColumn(border, c);
                    Grid.SetRow(border, r);

                    string cellString = cells[c];

                    Match match = IsImageMarkdown(cellString);
                    if (match.Success)
                    {
                        string imagename = match.Groups[1].Value;
                        string filename = match.Groups[2].Value;

                        Image cellValue = new Image();
                        if (this.ParentShape != null)
                        {
                            string dir = this.ParentShape.ParentSlide.ParentMaterial.DirectoryPath;
                            string filePath = Path.Combine(dir, filename + Defines.EXTENSION_IMAGE);
                            if (File.Exists(filePath))
                            {
                                BitmapImage bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                Uri url = new Uri(filePath, UriKind.Absolute);
                                bitmap.UriSource = url; // 이미지 파일 경로
                                bitmap.EndInit();
                                cellValue.Source = bitmap;
                            }
                        }
                        border.Child = cellValue;
                    }
                    else
                    {
                        TextBlock cellValue = new TextBlock();
                        cellValue.Text = cellString;
                        cellValue.Foreground = isHeader ? Brushes.White : Brushes.Black;
                        cellValue.FontWeight = isHeader ? FontWeights.Bold : c < rowHeaderCnt ? FontWeights.Bold : FontWeights.Normal;
                        cellValue.HorizontalAlignment = HorizontalAlignment.Center;
                        cellValue.VerticalAlignment = VerticalAlignment.Center;
                        cellValue.Margin = string.IsNullOrEmpty(cellValue.Text) ? new Thickness(0) : new Thickness(10);
                        border.Child = cellValue;
                    }
                }
            }

            TextBlock title = new TextBlock();
            title.Text = string.Format("<{0}>", this.Temp.Title);
            title.FontWeight = FontWeights.Bold;
            title.FontSize = 12;
            title.HorizontalAlignment = HorizontalAlignment.Center;
            title.Margin = new Thickness(0, 5, 0, 0);

            output.Children.Add(title);
            output.Children.Add(boundary);
            return output;
        }
        private UIElement GenerateImageCell(vmItem item)
        {
            StackPanel output = new StackPanel();
            output.Margin = new Thickness(5);
            output.HorizontalAlignment = HorizontalAlignment.Left;

            string titleString = item.Temp.Title;

            Border image = new Border();
            if(this.ParentShape != null)
            {
                mShape iShape = this.ParentShape.Temp;
                if (this.IsImageFileExist && iShape != null)
                {
                    string dir = this.ParentShape.ParentSlide.ParentMaterial.DirectoryPath;
                    string filePath = Path.Combine(dir, item.Temp.Uid + Defines.EXTENSION_IMAGE);

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.CacheOption = BitmapCacheOption.None;
                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;

                    bitmap.BeginInit();
                    Uri url = new Uri(filePath, UriKind.Absolute);
                    bitmap.UriSource = url; // 이미지 파일 경로
                    bitmap.EndInit();

                    

                    Image img = new Image();
                    img.Source = bitmap;
                    image.Child = img;
                }
                else
                {
                    image.Width = image.Height = 200;
                    image.Background = Brushes.LightGray;

                    TextBlock noImage = new TextBlock();
                    noImage.Text = item.Temp.Title;
                    noImage.FontSize = 15;
                    noImage.Foreground = Brushes.DimGray;
                    noImage.HorizontalAlignment = HorizontalAlignment.Center;
                    noImage.VerticalAlignment = VerticalAlignment.Center;
                    image.Child = noImage;
                }
            }
            else
            {
                image.Width = image.Height = 200;
                image.Background = Brushes.LightGray;

                TextBlock noImage = new TextBlock();
                noImage.Text = "No Image";
                noImage.FontSize = 15;
                noImage.Foreground = Brushes.DimGray;
                noImage.HorizontalAlignment = HorizontalAlignment.Center;
                noImage.VerticalAlignment = VerticalAlignment.Center;
                image.Child = noImage;
            }

            TextBlock title = new TextBlock();
            title.Text = string.Format("<{0}>", titleString);
            title.FontWeight = FontWeights.Bold;
            title.FontSize = 12;
            title.HorizontalAlignment = HorizontalAlignment.Center;
            title.Margin = new Thickness(0, 5, 0, 0);


            output.Children.Add(image);
            output.Children.Add(title);
            return output;
        }
        private UIElement GenerateImageCell(string tempValue)
        {
            StackPanel output = new StackPanel();
            output.Margin = new Thickness(5);
            output.HorizontalAlignment = HorizontalAlignment.Left;

            string titleString = string.Empty;
            string id = string.Empty;

            string pattern = @"^!\[([^\]]+)\]\(([\w-]+)\.png\)";
            Match mat =  Regex.Match(tempValue, pattern);
            if (!mat.Success) return null;
            
            

            titleString = mat.Groups[1].Value;
            id = mat.Groups[2].Value;

            Border image = new Border();
            if (this.ParentShape != null)
            {
                mShape iShape = this.ParentShape.Temp;
                if (this.IsImageFileExist && iShape != null)
                {
                    string dir = this.ParentShape.ParentSlide.ParentMaterial.DirectoryPath;
                    string filePath = Path.Combine(dir, this.Temp.Uid + Defines.EXTENSION_IMAGE);

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.CacheOption = BitmapCacheOption.None;
                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;

                    bitmap.BeginInit();
                    Uri url = new Uri(filePath, UriKind.Absolute);
                    bitmap.UriSource = url; // 이미지 파일 경로
                    bitmap.EndInit();

                    Image img = new Image();
                    img.Source = bitmap;
                    image.Child = img;
                }
                else
                {
                    image.Width = image.Height = 200;
                    image.Background = Brushes.LightGray;

                    TextBlock noImage = new TextBlock();
                    noImage.Text = "No Image";
                    noImage.FontSize = 15;
                    noImage.Foreground = Brushes.DimGray;
                    noImage.HorizontalAlignment = HorizontalAlignment.Center;
                    noImage.VerticalAlignment = VerticalAlignment.Center;
                    image.Child = noImage;
                }
            }
            else
            {
                image.Width = image.Height = 200;
                image.Background = Brushes.LightGray;

                TextBlock noImage = new TextBlock();
                noImage.Text = "No Image";
                noImage.FontSize = 15;
                noImage.Foreground = Brushes.DimGray;
                noImage.HorizontalAlignment = HorizontalAlignment.Center;
                noImage.VerticalAlignment = VerticalAlignment.Center;
                image.Child = noImage;
            }

            TextBlock title = new TextBlock();
            title.Text = string.Format("<{0}>", titleString);
            title.FontWeight = FontWeights.Bold;
            title.FontSize = 12;
            title.HorizontalAlignment = HorizontalAlignment.Center;
            title.Margin = new Thickness(0, 5, 0, 0);


            output.Children.Add(image);
            output.Children.Add(title);
            return output;
        }
        private UIElement GenerateHeaderCell(vmItem item)
        {
            StackPanel output = new StackPanel();
            output.Margin = new Thickness(5);
            output.Orientation = Orientation.Horizontal;

            TextBlock mark = new TextBlock();
            TextBlock header = new TextBlock();
            mark.FontWeight = header.FontWeight = FontWeights.Bold;
            mark.FontSize = header.FontSize = 30 - (3 * (item.Temp.Level - 1));

            header.Margin = new Thickness(10, 0, 0, 0);
            header.Text = item.Temp.LineText;

            output.Children.Add(mark);
            output.Children.Add(header);

            return output;
        }
        private UIElement GenerateHeaderCell(string tempValue)
        {
            StackPanel output = new StackPanel();
            output.Margin = new Thickness(5);
            output.Orientation = Orientation.Horizontal;

            TextBlock mark = new TextBlock();
            TextBlock header = new TextBlock();
            mark.FontWeight = header.FontWeight = FontWeights.Bold;
            mark.FontSize = header.FontSize = 30 - (3 * (this.Temp.Level - 1));

            header.Margin = new Thickness(10, 0, 0, 0);
            header.Text = tempValue;

            output.Children.Add(mark);
            output.Children.Add(header);

            return output;
        }
        public string GetImageDirPath()
        {
            string output = string.Empty;
            try
            {
                output = this.ParentShape.ParentSlide.ParentMaterial.DirectoryPath;
                if (Directory.Exists(output) == false) return string.Empty;
            }
            catch (Exception)
            {
                
            }
            return output;
        }
        public void Hide()
        {
            this.RowVisibility = Visibility.Collapsed;
            foreach (vmItem item in this.Children) item.Hide();
        }
        public bool HasChild()
        {
            return this.OriginChildren.Any();
        }
        public bool HasChild(vmItem child)
        {
            return this.OriginChildren.Contains(child);
        }
        public override void InitializeDisplay()
        {
            this.Display_Level = this.Temp.Level;
            this.Display_Text = this.Temp.LineText;
            this.Display_Title = this.Temp.Title;
            this.Display_UpdateDate = this.Temp.UpdateDate.HasValue ? this.Temp.UpdateDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null;

            this.RowIndex = this.Temp.Order;
            this.ItemType = (eItemType)this.Temp.ItemType;
            this.IsUsed = this.Temp.IsUsed;
            SetPreviewItem();

            OnModifyStatusChanged();
        }
        public void Merge(vmItem mergedItem)
        {
            string text = mergedItem.Temp.LineText.Clone().ToString();
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text))
            {
                this.Temp.LineText += "\n";
                this.Temp.LineText += text;
            }
            this.Origin.LineText = this.Temp.LineText;

           // mergedItem.Delete();
        }
        public void OnImageFileExistChanged() => this.OnPropertyChanged(nameof(this.IsImageFileExist));
        public void RemoveChild(vmItem child)
        {
            if (!HasChild(child)) return;

            this.OriginChildren.Remove(child);
            OnPropertyChanged(nameof(this.Children));
        }
        public void SetLevel(int lv)
        {
            this.Display_Level = this.Temp.Level = lv;
            foreach (vmItem child in this.Children) child.SetLevel(this.Temp.Level + 1);
        }
        public void SetLevel(bool isIncrease)
        {
            this.Display_Level = isIncrease ? ++this.Temp.Level : --this.Temp.Level;
            foreach (vmItem child in this.Children) child.SetLevel(this.Temp.Level+1);
        }
        private void SetIndent()
        {
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;

            if (1 < this.Temp.Level)
            {
                for (int i = 1; i < this.Temp.Level; i++)
                {
                    Border newBorder = new Border();
                    newBorder.Background = Defines.LIST_INDENT_COLOR[i % 10];

                    sp.Children.Add(newBorder);
                }
            }

            this.Display_Indent = sp;
        }
        public override void SetInitialData()
        {
            this.OriginChildren = new ObservableCollection<vmItem>();
        }
        public void SetText(string text)
        {
            this.Display_Text = this.Temp.LineText = text;
            SetPreviewItem();
        }
        public void SetTitle(string title)
        {
            this.Display_Title = this.Temp.Title = title;
            if(this.ItemType == eItemType.Image)  SetImageText(this.Temp.Title, this.Temp.Uid);
            SetPreviewItem();
        }
        public void SetImageText()
        {
            string title = this.Temp.Title;
            if (string.IsNullOrEmpty(title) || string.IsNullOrWhiteSpace(title)) title = this.Temp.Title = "NO TITLE";
            this.Display_Text = this.Temp.LineText = string.Format("![{0}]({1}{2})", this.Temp.Title, this.Temp.Uid, Defines.EXTENSION_IMAGE);
            SetPreviewItem();
        }
        public void SetImageText(string title, string text)
        {
            this.Display_Text = this.Temp.LineText = string.Format("![{0}]({1}{2})", title, text, Defines.EXTENSION_IMAGE);
            SetPreviewItem();
        }
        public void SetRowIndex(ObservableCollection<vmItem> list = null)
        {
            if(list == null)
            {
                this.RowIndex = this.ParentShape == null ? -1 : this.ParentShape.ParentSlide.Items.IndexOf(this);
            }
            else
            {
                this.RowIndex = list.IndexOf(this);
            }
        }
        public void SetRowIndex(int index)
        {
            this.RowIndex = index;
        }
        public void SetItemType(eItemType type)
        {
            this.ItemType = type;
            this.Temp.ItemType = this.ItemType.GetHashCode();
            SetPreviewItem();
        }
        public void SetParent(vmShape parent)
        {
            if (this.ParentShape != null)
            {
                if(this.ParentShape.Items.Contains(this)) this.ParentShape.Items.Remove(this);
                if (this.ParentShape.ParentSlide != null && this.ParentShape.ParentSlide.Items.Contains(this)) this.ParentShape.ParentSlide.RemoveItem(this);// .Items.Remove(this);
            }
            this.ParentShape = parent;
            if (this.ParentShape == null) return;

            this.Origin.ParentShapeIdx = this.Temp.ParentShapeIdx = parent.Temp.Idx;
            this.Origin.ParentUid = this.Temp.ParentUid = parent.Temp.Uid;
            if (this.ParentShape != null && !this.ParentShape.Items.Contains(this))
            {
                if (!this.ParentShape.Items.Contains(this)) this.ParentShape.Items.Add(this);
                if (this.ParentShape.ParentSlide != null && !this.ParentShape.ParentSlide.Items.Contains(this)) this.ParentShape.ParentSlide.AddItem(this);//.Items.Add(this);
            }
        }
        public void SetParentItem(vmItem parent, bool isDbLoad = false)
        {
            //this.ParentItem= parent;

            if (this.ParentItem != null) this.ParentItem.RemoveChild(this);
            this.ParentItem = parent;

            if (isDbLoad) return;

            int level = 1;
            if (this.ParentItem != null)
            {
                this.Temp.ParentItemUid = this.ParentItem.Temp.Uid;
                _ParentItem.AddChild(this);
                level = _ParentItem.Temp.Level + 1;
            }
            SetLevel(level);
        }
        public void SetPreviewItem(bool isReset = false)
        {
            if(isReset)
            {
                this.Display_PreviewItem = null;
                return;
            }

            switch (this.ItemType)
            {
                case eItemType.Header:
                    this.Display_PreviewItem = GenerateHeaderCell(this);
                    break;
                case eItemType.Image: 
                    this.Display_PreviewItem = GenerateImageCell(this);
                    
                    break;
                case eItemType.Table: 
                    this.Display_PreviewItem = GenerateTableCell(this);
                    break;
                default: 
                    this.Display_PreviewItem = GenerateTextCell(this);
                    break;
            }
            OnPropertyChanged(nameof(this.Display_PreviewItem_Slide));
        }
        public void SetPreviewItem(string tempValue, bool isReset = false )
        {
            if (isReset)
            {
                this.Display_PreviewItem = null;
                return;
            }

            switch (this.ItemType)
            {
                case eItemType.Header:
                    this.Display_PreviewItem = GenerateHeaderCell(tempValue);
                    break;
                case eItemType.Image:
                    this.Display_PreviewItem = GenerateImageCell(tempValue);

                    break;
                case eItemType.Table:
                    this.Display_PreviewItem = GenerateTableCell(tempValue);
                    break;
                default:
                    this.Display_PreviewItem = GenerateTextCell(tempValue);
                    break;
            }
            OnPropertyChanged(nameof(this.Display_PreviewItem_Slide));
        }
        public void Show()
        {
            this.RowVisibility = Visibility.Visible;
            foreach (vmItem item in this.Children) item.Show();
        }
        public override object UpdateOriginData()
        {

            //SetRowIndex();
            if (this.ParentShape != null) this.Origin.ParentShapeIdx = this.Temp.ParentShapeIdx = this.ParentShape.Temp.Idx;
            if (this.ParentItem != null) this.Origin.ParentItemUid = this.Temp.ParentItemUid = this.ParentItem.Temp.Uid;
            this.Origin.IsUsed = this.Temp.IsUsed;
            this.Origin.Uid = this.Temp.Uid;
            this.Origin.Order = this.Temp.Order;
            this.Origin.ItemType = this.Temp.ItemType;
            this.Origin.Level = this.Temp.Level;
            this.Origin.LineText = this.Temp.LineText;
            this.Origin.UpdateDate = this.Temp.UpdateDate;
            this.Origin.UpdateDate = this.Temp.UpdateDate = this.Origin.UpdateDate.HasValue ? DateTime.Now : this.Origin.CreateDate;
            this.Origin.Order = this.Temp.Order = this.RowIndex;

            return this.Origin;
        }
        public void UndoText()
        {
            this.Display_Text = this.Temp.LineText = this.Origin.LineText;
        }

        private Match IsImageMarkdown(string input)
        {
            string pattern = @"^!\[([^\]]+)\]\(([^\)]+)\.png\)$";
            return Regex.Match(input, pattern);
        }
    }
}
