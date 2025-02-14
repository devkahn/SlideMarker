using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
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
        public vmMaterial Material
        {
            get => _ParentMaterial;
            private set
            {
                _ParentMaterial = value;
                OnPropertyChanged(nameof(this.Material));
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
        private ObservableCollection<vmShape> OriginShapes { get; set; }
        private ObservableCollection<vmItem> OriginItems { get; set; }



        public ReadOnlyObservableCollection<vmShape> Shapes => new ReadOnlyObservableCollection<vmShape>(this.OriginShapes);
        public ReadOnlyObservableCollection<vmItem> Items => new ReadOnlyObservableCollection<vmItem>(this.OriginItems);

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
        internal void AddShape(vmShape shape)
        {
            if (this.OriginShapes.Contains(shape)) return;
            this.OriginShapes.Add(shape);
        }
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
        public void ClearShapes()
        {
            foreach (vmShape item in this.OriginShapes) item.SetParentSlide(null);
            this.OriginShapes.Clear();
        }
        public override void InitializeDisplay()
        {
            this.Status = new vmSlideStatus((ePageStatus)this.Temp.Status);

            this.Display_Index = this.Temp.Index;
            this.Display_UpdateDate = this.Temp.UpdateDate.HasValue ? this.Temp.UpdateDate.Value.ToString("yyyy-MM-dd hh:mm:ss") : "-";
            this.Display_Description = this.Temp.Description;

            this.OriginShapes.Clear();
            this.OriginItems.Clear();
            foreach (mShape item in this.Temp.Shapes)
            {
                vmShape newShape = new vmShape(item);
                newShape.Slide = this;
                newShape.SetParentSlide(this);
            }

            OnModifyStatusChanged();
        }
        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null) return;
            foreach (var item in e.NewItems)
            {
                vmItem data = item as vmItem;
                if(data != null) data.SetRowIndex();
            }
        }
        public vmSlide NextItem()
        {
            vmSlide output = null;

            int index = this.Material.Slides.IndexOf(this);
            if (index < this.Material.Slides.Count() - 2) output = this.Material.Slides.ElementAt(index + 1);

            return output;
        }
        public void OnPreviewItemsChanged() => OnPropertyChanged(nameof(this.PreviewItems));
        public vmSlide PrevItem()
        {
            vmSlide output = null;

            int index = this.Material.Slides.IndexOf(this);
            if (0 < index) output = this.Material.Slides.ElementAt(index - 1);

            return output;
        }
        internal void RemoveShape(vmShape shape)
        {
            if (!this.OriginShapes.Contains(shape)) return;
            this.OriginShapes.Remove(shape);

            mShape sameShape = this.Temp.Shapes.Where(x => x.Uid == shape.Temp.Uid).FirstOrDefault();
            if (sameShape != null) this.Temp.Shapes.Remove(sameShape);
        }
        internal void RemoveItem(vmItem item)
        {
            if (!this.OriginItems.Contains(item)) return;
            this.OriginItems.Remove(item);
        }
        public override void SetInitialData()
        {
            this.OriginShapes = new ObservableCollection<vmShape>();
            this.OriginItems = new ObservableCollection<vmItem>();
            this.OriginItems.CollectionChanged += Items_CollectionChanged;
        }
        public void SetStatus(ePageStatus status)
        {
            this.Status.Status = status;
            this.Temp.Status = this.Status.StatusCode;

            this.IsChanged = true;
            OnPropertyChanged(nameof(IsChanged));   
        }
        public void SetParentMaterial(vmMaterial parentMaterial)
        {
            if (this.Material != null) this.Material.RemoveSlide(this);

            this.Material = parentMaterial;
            this.Temp.MaterialId = -1;
            this.Temp.ParentUid = string.Empty;

            if (this.Material != null)
            {
                this.Material.AddSlide(this);
                this.Temp.MaterialId = parentMaterial.Temp.Idx;
                this.Temp.ParentUid = parentMaterial.Temp.Uid;
            }
        }
        public void SetDescription(string  description)
        {
            this.Display_Description = this.Temp.Description = description;   
        }
        public override object UpdateOriginData()
        {
            this.Origin.MaterialId = this.Temp.MaterialId;
            this.Origin.ParentUid = this.Temp.ParentUid;

            this.Origin.Status = this.Temp.Status;
            this.Origin.Description = this.Temp.Description;
            this.Origin.UpdateDate = this.Temp.UpdateDate = this.Origin.UpdateDate.HasValue ? DateTime.Now : this.Origin.CreateDate;

            this.Origin.Shapes.Clear();
            foreach (mShape shape in this.Temp.Shapes) this.Origin.Shapes.Add(shape);

            return this.Origin;
        }
    }

 
}
