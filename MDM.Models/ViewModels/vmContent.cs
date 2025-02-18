using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MDM.Commons.Enum;
using MDM.Models.DataModels;

namespace MDM.Models.ViewModels
{
    public partial class vmContent
    {
        private mContent _Origin = null;

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
        public vmContent (mContent origin)
        {
            this.Origin = origin;
        }

        private mContent Origin
        {
            get => _Origin;
            set
            {
                _Origin = value;
                if (value == null)
                {
                    this.Temp = null;
                    return;
                }
                this.Temp = this.Origin.Copy<mContent>();
                InitializeDisplay();
            }
        }
        public mContent Temp { get; private set; } = null;
        public vmMaterial Material { get; private set; } = null;
        public vmHeading ParentHeading { get; private set; } = null;


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
            }
        }
        public vmHeading Heading3
        {
            get => _Heading3;
            private set
            {
                _Heading3 = value;
                OnPropertyChanged(nameof(this.Heading3));
            }
        }
        public vmHeading Heading4
        {
            get => _Heading4;
            private set
            {
                _Heading4 = value;
                OnPropertyChanged(nameof(this.Heading4));
            }
        }
        public vmHeading Heading5
        {
            get => _Heading5;
            private set
            {
                _Heading5 = value;
                OnPropertyChanged(nameof(this.Heading5));
            }
        }
        public vmHeading Heading6
        {
            get => _Heading6;
            private set
            {
                _Heading6 = value;
                OnPropertyChanged(nameof(this.Heading6));
            }
        }
        public vmHeading Heading7
        {
            get => _Heading7;
            private set
            {
                _Heading7 = value;
                OnPropertyChanged(nameof(this.Heading7));
            }
        }
        public vmHeading Heading8
        {
            get => _Heading8;
            private set
            {
                _Heading8 = value;
                OnPropertyChanged(nameof(this.Heading8));
            }
        }
        public vmHeading Heading9
        {
            get => _Heading9;
            private set
            {
                _Heading9 = value;
                OnPropertyChanged(nameof(this.Heading9));
            }
        }
        public vmHeading Heading10
        {
            get => _Heading10;
            private set
            {
                _Heading10 = value;
                OnPropertyChanged(nameof(this.Heading10));
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
            if(this.Origin == null)
            {
                this.Display_SlideNum = null;

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

                this.Display_ContentType = null;
                this.Display_Content = null;
                this.Display_Description = null;
                this.Display_Message = null;
                return;
            }
            this.Display_SlideNum = this.Origin.SlideIdx;

            SetHeading();

            this.Display_ContentType = ((eItemType)this.Origin.ContentsType).ToString();
            this.Display_Content = this.Origin.Contents;
            this.Display_Description = this.Origin.Description;
            this.Display_Message = this.Origin.Message;
        }

        public void SetHeading()
        {
           
        }
        private vmHeading GetHeading(int headingLevel)
        {
            vmHeading output = null;
            switch (headingLevel)
            {
                //case 1: output = this.ParentMaterial.Headings.Where(x=> x.Temp.Name == this.Origin.Heading1String.Trim()).FirstOrDefault(); break;
                //case 2: output = this.ParentMaterial.Headings.Where(x => x.Temp.Name == this.Origin.Heading2String.Trim()).FirstOrDefault(); break;
                //case 3: output = this.ParentMaterial.Headings.Where(x => x.Temp.Name == this.Origin.Heading3String.Trim()).FirstOrDefault(); break;
                //case 4: output = this.ParentMaterial.Headings.Where(x => x.Temp.Name == this.Origin.Heading4String.Trim()).FirstOrDefault(); break;
                //case 6: output = this.ParentMaterial.Headings.Where(x => x.Temp.Name == this.Origin.Heading6String.Trim()).FirstOrDefault(); break;
                //case 7: output = this.ParentMaterial.Headings.Where(x => x.Temp.Name == this.Origin.Heading7String.Trim()).FirstOrDefault(); break;
                //case 8: output = this.ParentMaterial.Headings.Where(x => x.Temp.Name == this.Origin.Heading8String.Trim()).FirstOrDefault(); break;
                //case 5: output = this.ParentMaterial.Headings.Where(x => x.Temp.Name == this.Origin.Heading5String.Trim()).FirstOrDefault(); break;
                //case 9: output = this.ParentMaterial.Headings.Where(x => x.Temp.Name == this.Origin.Heading9String.Trim()).FirstOrDefault(); break;
                //case 10: output = this.ParentMaterial.Headings.Where(x => x.Temp.Name == this.Origin.Heading10String.Trim()).FirstOrDefault(); break;
                default:break;
            }

            return output;
        }

        public override void SetInitialData()
        {
            
        }

        public void SetMaterial(vmMaterial parent)
        {
            if (this.Material != null && this.Material.Contents.Contains(this)) this.Material.RemoveContent(this);
            this.Material = parent;
            this.Origin.MaterialIdx = this.Temp.MaterialIdx = -1;
            if (this.Material != null && !this.Material.Contents.Contains(this))
            {
                this.Material.AddContent(this); 
                this.Origin.MaterialIdx = this.Temp.MaterialIdx = parent.Temp.Idx;
            }
        }
        public void SetParentHeading(vmHeading parentHeading)
        {
        }

        public override object UpdateOriginData()
        {
            return null;
        }
    }
}
