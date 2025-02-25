using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MDM.Commons.Enum;
using MDM.Models.DataModels;

namespace MDM.Models.ViewModels
{
    public partial class vmSlide
    {
        private mSlide _Origin = null;
        private vmItem _CurrentItem = null;
        public vmMaterial _ParentMaterial { get; set; } = null;
        private vmSlideStatus _Status = new vmSlideStatus(ePageStatus.None);    


        private object _Display_Index = null;
        private object _Display_Update = null;
        private object _Display_Description = null;

    }

    public partial class vmSlide : vmViewModelbase
    {
        public vmSlide(mSlide origin)
        {
            this.Origin = origin;
        }

        private mSlide Origin
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
                this.Temp = this.Origin.Copy<mSlide>();
                InitializeDisplay();
            }
        }
        public mSlide Temp { get; private set; } = null;
        public vmMaterial ParentMaterial
        {
            get => _ParentMaterial;
            private set
            {
                _ParentMaterial = value;
                OnPropertyChanged(nameof(ParentMaterial));
            }
        }
        public vmSlideStatus Status
        {
            get => _Status;
            private set
            {
                _Status = value;
                OnPropertyChanged(nameof(Status));
            }
        }
        public vmItem CurrentItem
        {
            get => _CurrentItem;
            set
            {
                _CurrentItem = value;
                OnPropertyChanged(nameof(CurrentItem), false);
            }
        }


        public ObservableCollection<vmShape> Shapes { get; private set; }
        public ObservableCollection<vmItem> Items { get; private set; }
        public ReadOnlyObservableCollection<object> PreviewItems => new ReadOnlyObservableCollection<object>(new ObservableCollection<object>(this.Items.Select(x => x.Display_PreviewItem_Slide)));



        public object Display_Index
        {
            get => _Display_Index;
            private set
            {
                _Display_Index = value;
                OnPropertyChanged(nameof(Display_Index));  
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
        public object Display_Description
        {
            get => _Display_Description;
            private set
            {
                _Display_Description = value;
                OnPropertyChanged(nameof(this.Display_Description));
            }
        }
    }

    public partial class vmSlide
    {

        public override void InitializeDisplay()
        {
            this.Status = new vmSlideStatus((ePageStatus)this.Temp.Status);

            this.Display_Index = this.Temp.Index;
            this.Display_UpdateDate = this.Temp.UpdateDate.HasValue ? this.Temp.UpdateDate.Value.ToString("yyyy-MM-dd hh:mm:ss") : "-";
            this.Display_Description = this.Temp.Description;

            this.Shapes.Clear();
            this.Items.Clear();
            foreach (mShape item in this.Temp.Shapes)
            {
                vmShape newShape = new vmShape(item);
                newShape.ParentSlide = this;
                this.Shapes.Add(newShape);

                foreach (vmItem ln in newShape.Items) this.Items.Add(ln);
            }

            OnModifyStatusChanged();
        }
        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null) return;
            foreach (var item in e.NewItems)
            {
                vmItem data = item as vmItem;
                if(data == null) continue;

                data.SetRowIndex();
            }
        }
        public vmSlide NextItem()
        {
            vmSlide output = null;

            int index = this.ParentMaterial.Slides.IndexOf(this);
            if (index < this.ParentMaterial.Slides.Count() - 2) output = this.ParentMaterial.Slides.ElementAt(index + 1);

            return output;
        }
        public void OnPreviewItemsChanged() => OnPropertyChanged(nameof(this.PreviewItems));
        public vmSlide PrevItem()
        {
            vmSlide output = null;

            int index = this.ParentMaterial.Slides.IndexOf(this);
            if (0 < index) output = this.ParentMaterial.Slides.ElementAt(index - 1);

            return output;
        }
        public override void SetInitialData()
        {
            this.Shapes = new ObservableCollection<vmShape>();
            this.Items = new ObservableCollection<vmItem>();
            this.Items.CollectionChanged += Items_CollectionChanged;
        }
        public void SetParentMaterial(vmMaterial parentMaterial) => this.ParentMaterial = parentMaterial;
        
        public void SetStatus(ePageStatus status)
        {
            this.Status.Status = status;
            this.Temp.Status = this.Status.StatusCode;

            this.IsChanged = true;
            OnPropertyChanged(nameof(IsChanged));   
        }
        public void SetParent(vmMaterial parent)
        {
            if (this.ParentMaterial != null) this.ParentMaterial.RemoveSlide(this);
            this.ParentMaterial = parent;
            this.Origin.MaterialId = this.Temp.MaterialId = parent.Temp.Idx;
            this.Origin.ParentUid = this.Temp.ParentUid = parent.Temp.Uid;
            if(this.ParentMaterial != null) this.ParentMaterial.AddSlide(this);
        }
        public void SetDescription(string  description)
        {
            this.Display_Description = this.Temp.Description = description;   
        }
        public void ItemsOrderBy()
        {
            this.Items = new ObservableCollection<vmItem>(this.Items.OrderBy(x => x.Temp.Order));
            OnPropertyChanged(nameof(this.PreviewItems));
        }
        public override object UpdateOriginData()
        {
            this.Origin.MaterialId = this.Temp.MaterialId = this.ParentMaterial.Temp.Idx;
            this.Origin.Status = this.Temp.Status;
            this.Origin.Description = this.Temp.Description;
            this.Origin.UpdateDate = this.Temp.UpdateDate = this.Origin.UpdateDate.HasValue ? DateTime.Now : this.Origin.CreateDate;
            

            return this.Origin;
        }
        
        

        
    }

 
}
