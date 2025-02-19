using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MDM.Commons.Enum;
using MDM.Models.DataModels;

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
            this.Display_SlideNum = this.Temp.ParentShape.ParentSlide.Temp.SlideNumber;

            this.Display_ContentType = this.Temp.ItemType.ToString();
            this.Display_Content = this.Temp.Display_Text;
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
            }
        }

        public override object UpdateOriginData()
        {
            return this.Temp;
        }
    }
}
