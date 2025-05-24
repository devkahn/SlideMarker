using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public vmMaterial Material
        {
            get => _ParentMaterial;
            private set
            {
                _ParentMaterial = value;
                OnPropertyChanged(nameof(Material));
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



        public ReadOnlyObservableCollection<vmShape> Shapes => new ReadOnlyObservableCollection<vmShape>(OriginShapes);
        public ReadOnlyObservableCollection<vmItem> Items => new ReadOnlyObservableCollection<vmItem>(OriginItems);
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
        internal void AddShape (vmShape shape)
        {
            if (this.OriginShapes.Contains(shape)) return;
            this.OriginShapes.Add(shape);
        }
        internal void AddItem (vmItem item)
        {
            if(this.OriginItems.Contains(item)) return;
            this.OriginItems.Add(item);
        }
        public void AddItem(vmItem item, int index)
        {
            if (this.OriginItems.Contains(item)) return;
            this.OriginItems.Insert(index, item);
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
                newShape.ParentSlide = this;
                this.OriginShapes.Add(newShape);

                foreach (vmItem ln in newShape.Items) this.OriginItems.Add(ln);
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

            int index = this.Material.Slides.IndexOf(this);
            if (index < this.Material.Slides.Count() - 2) output = this.Material.Slides.ElementAt(index + 1);

            return output;
        }
        private void OriginShapes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => this.OnPropertyChanged(nameof(this.Shapes));
        private void OriginItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => this.OnPropertyChanged(nameof(this.Items));
        public vmSlide PrevItem()
        {
            vmSlide output = null;

            int index = this.Material.Slides.IndexOf(this);
            if (0 < index) output = this.Material.Slides.ElementAt(index - 1);

            return output;
        }
        public void RemoveShpae(vmShape shape)
        {
            foreach (vmItem item in shape.Items) this.RemoveItem(item);

            if (!this.OriginShapes.Contains(shape)) return;
            this.OriginShapes.Remove(shape);    
        }
        public void RemoveItem (vmItem item)
        {
            if (!this.OriginItems.Contains(item)) return;
            this.OriginItems.Remove(item);
        }
        public override void SetInitialData()
        {
            this.OriginShapes = new ObservableCollection<vmShape>();
            this.OriginShapes.CollectionChanged += OriginShapes_CollectionChanged;

            this.OriginItems = new ObservableCollection<vmItem>();
            this.OriginItems.CollectionChanged += OriginItems_CollectionChanged;
        }
        public void SetStatus(ePageStatus status)
        {
            this.Status.Status = status;
            this.Temp.Status = this.Status.StatusCode;

            this.IsChanged = true;
            OnPropertyChanged(nameof(IsChanged));   
        }
        public void SetParentMaterial(vmMaterial parent)
        {
            if (this.Material != null) this.Material.RemoveSlide(this);
            this.Material = parent;
            this.Temp.MaterialId = -1;
            this.Temp.ParentUid = string.Empty;
            if (this.Material != null)
            {
                this.Material.AddSlide(this);
                this.Temp.MaterialId = parent.Temp.Idx;
                this.Temp.ParentUid = parent.Temp.Uid;
            }
        }
        public void SetDescription(string  description)
        {
            this.Display_Description = this.Temp.Description = description;   
        }
        public void ConvertAndSetContents(List<mHeading> allHeadings, List<mContent> originContentList = null)
        {
            if (this.Material == null) return;


            vmHeading level1Header = null;
            vmHeading level2Header = null;
            vmHeading level3Header = null;
            vmHeading level4Header = null;
            vmHeading level5Header = null;
            vmHeading level6Header = null;
            vmHeading level7Header = null;
            vmHeading level8Header = null;
            vmHeading level9Header = null;
            vmHeading level10Header = null;


            foreach (vmItem item in Items)
            {
                string itemUId = item.Temp.Uid;

                if (item.ItemType == eItemType.Header)
                {
                    vmHeading parent = null;
                    switch (item.Temp.Level)
                    {
                        case 1: parent = null; break;
                        case 2: parent = level1Header; break;
                        case 3: parent = level2Header; break;
                        case 4: parent = level3Header; break;
                        case 5: parent = level4Header; break;
                        case 6: parent = level5Header; break;
                        case 7: parent = level6Header; break;
                        case 8: parent = level7Header; break;
                        case 9: parent = level8Header; break;
                        case 10: parent = level9Header; break;
                        default: break;
                    }
                    List<vmHeading> headingList = parent == null ? this.Material.Headings.ToList() : parent.Children.ToList();

                    vmHeading sameHeading = headingList.Where(x => x.Temp.Uid == itemUId).FirstOrDefault();
                    if(sameHeading == null)
                    {
                        mHeading heading = allHeadings.Where(x=> x.Uid == itemUId).FirstOrDefault();
                        if (heading == null)
                        {
                            heading = new mHeading();
                            heading.Level = item.Temp.Level;
                            heading.Name = item.Temp.LineText;
                        }
                        sameHeading = new vmHeading(heading);
                        sameHeading.SetParentMaterial(this.Material);
                        sameHeading.SetParent(parent);
                    }

                    switch (item.Temp.Level)
                    {
                        case 1: level1Header = sameHeading; break;
                        case 2: level2Header = sameHeading; break;
                        case 3: level3Header = sameHeading; break;
                        case 4: level4Header = sameHeading; break;
                        case 5: level5Header = sameHeading; break;
                        case 6: level6Header = sameHeading; break;
                        case 7: level7Header = sameHeading; break;
                        case 8: level8Header = sameHeading; break;
                        case 9: level9Header = sameHeading; break;
                        case 10: level10Header = sameHeading; break;
                        default: break;
                    }
                }
                else
                {
                    vmContent newContent = new vmContent(item);
                    newContent.SetParentMaterial(this.Material);

                    mContent sameContent = originContentList.Where(x => x.Uid == itemUId).FirstOrDefault();
                    if (sameContent != null)
                    {
                        

                    }
                    else
                    {

                    }

                    
                    switch (item.Temp.Level)
                    {
                        case 1: newContent.SetParentHeading(level1Header); break;
                        case 2: newContent.SetParentHeading(level1Header); break;
                        case 3: newContent.SetParentHeading(level2Header); break;
                        case 4: newContent.SetParentHeading(level3Header); break;
                        case 5: newContent.SetParentHeading(level4Header); break;
                        case 6: newContent.SetParentHeading(level5Header); break;
                        case 7: newContent.SetParentHeading(level6Header); break;
                        case 8: newContent.SetParentHeading(level7Header); break;
                        case 9: newContent.SetParentHeading(level8Header); break;
                        case 10: newContent.SetParentHeading(level9Header); break;
                        default: break;
                    }
                    
                    
                }
            }
        }
        public void ConvertAndSetContents()
        {
            if (this.Material == null) return;


            vmHeading level1Header = null;
            vmHeading level2Header = null;
            vmHeading level3Header = null;
            vmHeading level4Header = null;
            vmHeading level5Header = null;
            vmHeading level6Header = null;
            vmHeading level7Header = null;
            vmHeading level8Header = null;
            vmHeading level9Header = null;
            vmHeading level10Header = null;


            foreach (vmItem item in Items)
            {
                if (item.ItemType == eItemType.Header)
                {
                    vmHeading parent = null;
                    switch (item.Temp.Level)
                    {
                        case 1: parent = null; break;
                        case 2: parent = level1Header; break;
                        case 3: parent = level2Header; break;
                        case 4: parent = level3Header; break;
                        case 5: parent = level4Header; break;
                        case 6: parent = level5Header; break;
                        case 7: parent = level6Header; break;
                        case 8: parent = level7Header; break;
                        case 9: parent = level8Header; break;
                        case 10: parent = level9Header; break;
                        default: break;
                    }
                    List<vmHeading> headingList = parent == null ? this.Material.Headings.ToList() : parent.Children.ToList();

                    vmHeading sameHeading = headingList.Where(x => x.Temp.Level == item.Temp.Level && x.Temp.Name == item.Temp.LineText).FirstOrDefault();
                    if (sameHeading == null)
                    {
                        mHeading heading = new mHeading();
                        heading.Level = item.Temp.Level;
                        heading.Name = item.Temp.LineText;
                        sameHeading = new vmHeading(heading);
                        sameHeading.SetParentMaterial(this.Material);
                        sameHeading.SetParent(parent);
                    }

                    switch (item.Temp.Level)
                    {
                        case 1: level1Header = sameHeading; break;
                        case 2: level2Header = sameHeading; break;
                        case 3: level3Header = sameHeading; break;
                        case 4: level4Header = sameHeading; break;
                        case 5: level5Header = sameHeading; break;
                        case 6: level6Header = sameHeading; break;
                        case 7: level7Header = sameHeading; break;
                        case 8: level8Header = sameHeading; break;
                        case 9: level9Header = sameHeading; break;
                        case 10: level10Header = sameHeading; break;
                        default: break;
                    }
                }
                else
                {
                    vmContent newContent = new vmContent(item);
                    newContent.SetParentMaterial(this.Material);


                    vmHeading parentHeading = null;
                    if (level1Header != null) parentHeading = level1Header;
                    if (level2Header != null) parentHeading = level2Header;
                    if (level3Header != null) parentHeading = level3Header;
                    if (level4Header != null) parentHeading = level4Header;
                    if (level5Header != null) parentHeading = level5Header;
                    if (level6Header != null) parentHeading = level6Header;
                    if (level7Header != null) parentHeading = level7Header;
                    if (level8Header != null) parentHeading = level8Header;
                    if (level9Header != null) parentHeading = level9Header;
                    if (level10Header != null) parentHeading = level10Header;
                    

                    newContent.SetParentHeading(parentHeading);
                }
            }
        }
        public override object UpdateOriginData()
        {
            this.Origin.MaterialId = this.Temp.MaterialId;
            this.Origin.ParentUid = this.Temp.ParentUid;

            this.Origin.Status = this.Temp.Status;
            this.Origin.Description = this.Temp.Description;
            this.Origin.UpdateDate = this.Temp.UpdateDate = this.Origin.UpdateDate.HasValue ? DateTime.Now : this.Origin.CreateDate;
            

            return this.Origin;
        }
        
        

        
    }

 
}
