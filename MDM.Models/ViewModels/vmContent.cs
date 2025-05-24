using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MDM.Commons;
using MDM.Commons.Enum;
using MDM.Models.DataModels;
using MDM.Models.DataModels.ManualWorksXMLs;

namespace MDM.Models.ViewModels
{
    public partial class vmContent
    {
        private vmItem _Origin = null;

        private vmHeading _ParentHeading = null;

        private vmHeading _Heading1 = null;
        private vmHeading _Heading2 = null;
        private vmHeading _Heading3 = null;
        private vmHeading _Heading4 = null;
        private vmHeading _Heading5 = null;
        private vmHeading _Heading6 = null;
        private vmHeading _Heading7 = null;
        private vmHeading _Heading8 = null;
        private vmHeading _Heading9 = null;
        private vmHeading _Heading10 = null;

        private Brush _Display_Background = Brushes.White;

        private object _Display_SlideNum = null;
        private object _Display_Heading1 = null;
        private object _Display_Heading2 = null;
        private object _Display_Heading3 = null;
        private object _Display_Heading4 = null;
        private object _Display_Heading5 = null;
        private object _Display_Heading6 = null;
        private object _Display_Heading7 = null;
        private object _Display_Heading8 = null;
        private object _Display_Heading9 = null;
        private object _Display_Heading10 = null;
        private object _Display_ContentType = null;
        private object _Display_Content = null;
        private object _Display_Description = null;
        private object _Display_Message = null;

        
        private string _Temp_Content = string.Empty;
        private string _Temp_Title = string.Empty;
        private string _Temp_TableHtml = string.Empty;
        private string _ImagePath = string.Empty;
        private bool? _IsContentsValid = null;
        private bool _IsEnable = true;

    }
    public partial class vmContent :vmViewModelbase
    {
        public vmContent (vmItem origin)
        {
            this.Temp = origin;
        }
  

        public vmItem Temp
        {
            get => _Origin;
            private set
            {
                _Origin = value;
                if (value == null) return;
                InitializeDisplay();
            }
        }
        
        public vmMaterial Material { get; private set; } = null;
        public vmHeading ParentHeading
        {
            get => _ParentHeading;
            private set
            {
                _ParentHeading = value;
                OnPropertyChanged(nameof(this.ParentHeading));  
                if(value != null)
                {
                    switch (value.Temp.Level)
                    {
                        case 1: this.Heading1 = value; break;
                        case 2: this.Heading2 = value; break;
                        case 3: this.Heading3 = value; break;
                        case 4: this.Heading4 = value; break;
                        case 5: this.Heading5 = value; break;
                        case 6: this.Heading6 = value; break;
                        case 7: this.Heading7 = value; break;
                        case 8: this.Heading8 = value; break;
                        case 9: this.Heading9 = value; break;
                        case 10: this.Heading10 = value; break;
                        default: break;
                    }
                }
                else
                {
                    this.Heading1 = null;
                    this.Heading2 = null;
                    this.Heading3 = null;
                    this.Heading4 = null;
                    this.Heading5 = null;
                    this.Heading6 = null;
                    this.Heading7 = null;
                    this.Heading8 = null;
                    this.Heading9 = null;
                    this.Heading10 = null;
                }
            }
        }
        public eContentType ContentType { get; set; } = eContentType.None;
        public xmlElementConfig XmlConfig { get; set; }


        public string ImagePath
        {
            get => _ImagePath;
            set
            {
                _ImagePath = value;
                OnPropertyChanged(nameof(ImagePath));
            }
        }
        public bool? IsContentsValid
        {
            get => _IsContentsValid;
            set
            {
                _IsContentsValid = value;
                OnPropertyChanged(nameof(IsContentsValid));
                this.Display_Background = value.HasValue ? value.Value ? Brushes.LightGray : Brushes.LightPink : Brushes.White;
            }
        }
        public bool IsEnable
        {
            get => _IsEnable;
            set
            {
                _IsEnable = false;
                OnPropertyChanged(nameof(this.IsEnable));
            }
        }

        public vmHeading Heading1
        {
            get => _Heading1;
            private set
            {
                _Heading1 = value;
                OnPropertyChanged(nameof(this.Heading1));

            }
        }
        public vmHeading Heading2
        {
            get => _Heading2;
            private set
            {
                _Heading2 = value;
                OnPropertyChanged(nameof(this.Heading2));
                if (value != null) this.Heading1 = value.Parent;
            }
        }
        public vmHeading Heading3
        {
            get => _Heading3;
            private set
            {
                _Heading3 = value;
                OnPropertyChanged(nameof(this.Heading3));
                if (value != null) this.Heading2 = value.Parent;
            }
        }
        public vmHeading Heading4
        {
            get => _Heading4;
            private set
            {
                _Heading4 = value;
                OnPropertyChanged(nameof(this.Heading4));
                if (value != null) this.Heading3 = value.Parent;
            }
        }
        public vmHeading Heading5
        {
            get => _Heading5;
            private set
            {
                _Heading5 = value;
                OnPropertyChanged(nameof(this.Heading5));
                if (value != null) this.Heading4 = value.Parent;
            }
        }
        public vmHeading Heading6
        {
            get => _Heading6;
            private set
            {
                _Heading6 = value;
                OnPropertyChanged(nameof(this.Heading6));
                if (value != null) this.Heading5 = value.Parent;
            }
        }
        public vmHeading Heading7
        {
            get => _Heading7;
            private set
            {
                _Heading7 = value;
                OnPropertyChanged(nameof(this.Heading7));
                if (value != null) this.Heading6 = value.Parent;
            }
        }
        public vmHeading Heading8
        {
            get => _Heading8;
            private set
            {
                _Heading8 = value;
                OnPropertyChanged(nameof(this.Heading8));
                if (value != null) this.Heading7 = value.Parent;
            }
        }
        public vmHeading Heading9
        {
            get => _Heading9;
            private set
            {
                _Heading9 = value;
                OnPropertyChanged(nameof(this.Heading9));
                if (value != null) this.Heading8 = value.Parent;
            }
        }
        public vmHeading Heading10
        {
            get => _Heading10;
            private set
            {
                _Heading10 = value;
                OnPropertyChanged(nameof(this.Heading10));
                if (value != null) this.Heading9 = value.Parent;
            }
        }


        public Brush Display_Background
        {
            get => _Display_Background;
            private set
            {
                _Display_Background = value;
                OnPropertyChanged(nameof(this.Display_Background));
            }
        }


        public ContextMenu ControlMenu { get; set; } = new ContextMenu();
        

        public string Temp_Content
        {
            get => _Temp_Content;
            set
            {
                _Temp_Content = value;
                OnPropertyChanged(nameof(this.Temp_Content));
            }
        }
        public string Temp_Title
        {
            get => _Temp_Title;
            set
            {
                _Temp_Title = value;
                OnPropertyChanged(nameof(this.Temp_Title));
            }
        }
        public string Temp_TableHTML
        {
            get => _Temp_TableHtml;
            set
            {
                _Temp_TableHtml = value;
                OnPropertyChanged(nameof(this.Temp_TableHTML));
            }
        }



        public object Display_SlideNum
        {
            get => _Display_SlideNum;
            private set
            {
                _Display_SlideNum = value;
                OnPropertyChanged(nameof(this.Display_SlideNum));
            }
        }
        public object Display_ContentType
        {
            get => _Display_ContentType;
            private set
            {
                _Display_ContentType = value;
                OnPropertyChanged(nameof(this.Display_ContentType));
            }
        }
        public object Display_Content
        {
            get => _Display_Content;
            private set
            {
                _Display_Content = value;
                OnPropertyChanged(nameof(this.Display_Content));
            }
        }
        public object Display_Description
        {
            get => _Display_Description;
            private set
            {
                _Display_Description = value;
                OnPropertyChanged(nameof(this.Display_Description));
            }
        }
        public object Display_Message
        {
            get => _Display_Message;
            private set
            {
                _Display_Message = value;
                OnPropertyChanged(nameof(this.Display_Message));
            }
        }

    }
    public partial class vmContent
    {
        public override void InitializeDisplay()
        {
            this.Display_SlideNum = -1;
            if(this.Temp.ParentShape != null && this.Temp.ParentShape.ParentSlide != null) this.Display_SlideNum = this.Temp.ParentShape.ParentSlide.Temp.SlideNumber;

            switch (this.Temp.ItemType)
            {
                case eItemType.Text:
                    this.Temp.SetItemType(eItemType.Text);
                    break;
                case eItemType.Image:
                    this.Display_ContentType = eContentType.Image.ToString();
                    this.ContentType = eContentType.Image;
                    break;
                case eItemType.Table:
                    this.Display_ContentType = eContentType.Table.ToString();
                    this.ContentType = eContentType.Table;
                    break;
                default:
                    switch (this.Temp.Temp.ItemType)
                    {
                        case 2212:
                            this.Temp.SetItemType(eItemType.Text);
                            this.Display_ContentType = eContentType.OrderList.ToString();
                            this.ContentType = eContentType.OrderList;
                            break;
                        case 2213:
                            this.Temp.SetItemType(eItemType.Text);
                            this.Display_ContentType = eContentType.UnOrderList.ToString();
                            this.ContentType = eContentType.UnOrderList;
                            break;
                        default:
                            this.Temp.SetItemType(eItemType.Text);
                            this.Display_ContentType = eContentType.NormalText.ToString();
                            this.ContentType = eContentType.NormalText;
                            break;
                    }
                    break;
            }

            this.Display_Content = this.Temp.Display_Text;
            this.Temp_Content = this.Temp.Display_Text.ToString();
            this.Temp_Title = this.Temp.Display_Title == null ? string.Empty : this.Temp.Display_Title.ToString();
        }
        public override void SetInitialData()
        {
            
        }
        public void SetParentMaterial(vmMaterial parent)
        {
            if (this.Material != null) this.Material.RemoveContent(this);
            this.Material = parent;
            if(this.Material != null)
            {
                this.Material.AddContent(this);
            }
        }
        public void SetParentHeading(vmHeading heading)
        {
            if (this.ParentHeading != null) heading.RemoveContent(this);

            this.ParentHeading = heading;
            if (this.ParentHeading != null)
            {
                heading.AddContent(this);
                heading.SetPageNumber();
            }
        }
        public void SetSlideNum(int number)
        {
            //this.Display_SlideNum = this.Temp.ParentShape.ParentSlide.Temp.SlideNumber = number;
            this.Display_SlideNum = number;
        }
        public void SetBackground(Brush color)
        {
            this.Display_Background = color;
        }
        public void SetBackground(bool? isTrue)
        {
            if(isTrue.HasValue)
            {
                this.Display_Background = isTrue.Value ? Brushes.Cyan : Brushes.LightPink;
            }
            else
            {
                this.Display_Background = Brushes.White;
            }
        }
        

      

        public void SetNewContent()
        {
            if(this.ContentType == eContentType.Image)
            {
                this.Temp.SetTitle(this.Temp_Title);
            }
            else
            {
                this.Temp.SetText(this.Temp_Content);
            }
            
            InitializeDisplay();
        }
        public void SetNewContent(string newContent)
        {
            this.Temp.SetText(newContent);
            InitializeDisplay();
        }
        public void SetNewImageContent(string title, string fileName)
        {
            this.Temp_Title = title;
            this.Temp.SetTitle(title);
            this.Temp.SetImageText(title, fileName);

            InitializeDisplay();
        }
        public void SetContentType(eContentType type)
        {
            this.ContentType = type;
            OnPropertyChanged(nameof(this.ContentType));
        }
        public override object UpdateOriginData()
        {
            return this.Temp;
        }
    }
}
