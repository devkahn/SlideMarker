using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.TextFormatting;
using MDM.Models.DataModels;
using Newtonsoft.Json.Linq;

namespace MDM.Models.ViewModels
{
    public partial class vmShape
    {
        private mShape _Origin = null;


        public object _Display_Title = null;
        public object _Display_Text = null;
        private object _Display_Update = null;
        private Visibility _SameShape = Visibility.Hidden;


    }
    public partial class vmShape : vmViewModelbase
    {
        public vmShape(mShape origin)
        {
            this.Origin = origin;
        }

        private mShape Origin
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
                this.Temp = this.Origin.Copy<mShape>();
                InitializeDisplay();
            }
        }
        public mShape Temp { get; private set; } = null;
        public vmSlide Slide { get; set; } = null;
        private ObservableCollection<vmItem> OriginItems { get;  set; }


        public ReadOnlyObservableCollection<vmItem> Items => new ReadOnlyObservableCollection<vmItem>(this.OriginItems);
        public Visibility SameShape
        {
            get => _SameShape;
            private set
            {
                _SameShape = value;
                OnPropertyChanged(nameof(this.SameShape));
            }
        }
        public object Display_Title
        {
            get => _Display_Title;
            private set
            {
                _Display_Title = value;
                OnPropertyChanged(nameof(this.Display_Title));
            }
        }
        public object Display_Text
        {
            get => _Display_Text;
            private set
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
    }

    public partial class vmShape
    {
        internal void AddItem(vmItem item)
        {
            if (this.OriginItems.Contains(item)) return;
            this.OriginItems.Add(item);
        }
        internal void AddItem(vmItem item, int index)
        {
            if (this.OriginItems.Contains(item)) return;
            this.OriginItems.Insert(index, item);
        }
        public override void InitializeDisplay()
        {
            this.Display_Title = this.Temp.Title;
            this.Display_Text = this.Temp.Text;
            this.Display_UpdateDate = this.Temp.UpdateDate.HasValue ? this.Temp.UpdateDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null;

            this.OriginItems.Clear();
            foreach (mItem line in this.Temp.Items)
            {
                vmItem newLine = new vmItem(line);
                newLine.SetParentShape(this);
            }
            

            OnModifyStatusChanged();
        }
        internal void RemoveItem(vmItem item)
        {
            if (!this.OriginItems.Contains(item)) return;
            this.OriginItems.Remove(item);

            mItem sameItem = this.Temp.Items.Where(x=> x.Uid == item.Temp.Uid).FirstOrDefault();
            if(sameItem != null)  this.Temp.Items.Remove(sameItem);
        }
        public override void SetInitialData()
        {
            this.OriginItems = new ObservableCollection<vmItem>();    
        }
        public void SetSameShapeVisibility(bool isVisible)
        {
            this.SameShape = isVisible ? Visibility.Visible : Visibility.Hidden;
        }
        public void SetTitle(string title)
        {
            this.Display_Title = this.Temp.Title = title;
        }
        public void SetText(string text)
        {
            this.Display_Text = this.Temp.Text = text;
        }
        public void SetParentSlide(vmSlide parentSlide)
        {
            if (this.Slide != null ) this.Slide.RemoveShape(this);

            this.Slide = parentSlide;
            this.Temp.ParentSlideIdx = -1;
            this.Temp.ParentUid = string.Empty;

            if (this.Slide != null )
            {
                this.Slide.AddShape(this);
                this.Temp.ParentSlideIdx = parentSlide.Temp.Idx;
                this.Temp.ParentUid = parentSlide.Temp.Uid;
            }
        }
        public override object UpdateOriginData()
        {
            this.Origin.ParentSlideIdx = this.Temp.ParentSlideIdx = this.Temp.Idx;
            this.Origin.ParentUid = this.Temp.ParentUid;

            this.Origin.ShapeType = this.Temp.ShapeType;
            this.Origin.UpdateDate = this.Temp.UpdateDate = this.Origin.UpdateDate.HasValue ? DateTime.Now : this.Origin.CreateDate;

            return this.Origin;
        }
        public void UndoText()
        {
            SetTitle(this.Origin.Title);
            SetText(this.Origin.Text);
        }
    }
}
