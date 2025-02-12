using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using MDM.Models.DataModels;
using Microsoft.Office.Interop.PowerPoint;

namespace MDM.Models.ViewModels
{
    public partial class vmMaterial
    {
        private mMaterial _Origin = null;
        private vmSlide _CurrentSlide = null;
        private vmContent _CurrentContent = null;

        private object _Display_Name = null;
        private object _Display_Update = null;
    }
    public partial class vmMaterial : vmViewModelbase
    {
        public vmMaterial(mMaterial origin) : base()
        {
            this.Origin = origin;
        }

        private mMaterial Origin
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
                this.Temp = this.Origin.Copy<mMaterial>();
                InitializeDisplay();
            }
        }
        public mMaterial Temp { get; private set; }= null;
        public string DirectoryPath { get; set; } = string.Empty;
        public Presentation OriginPresentation { get; private set; } = null;
        public List<Shape> SelectedShapes { get; set; } = new List<Shape>();


        private ObservableCollection<vmSlide> OriginSlides { get; set; }
        public ReadOnlyObservableCollection<vmSlide> Slides => new ReadOnlyObservableCollection<vmSlide>(this.OriginSlides);
        public vmSlide CurrentSlide
        {
            get => _CurrentSlide;
            set
            {
                _CurrentSlide = value;
                OnPropertyChanged(nameof(CurrentSlide));
            }
        }

        private ObservableCollection<vmContent> OriginContents { get; set; }
        public ReadOnlyObservableCollection<vmContent> Contents => new ReadOnlyObservableCollection<vmContent>(this.OriginContents);
        public vmContent CurrentContent
        {
            get => _CurrentContent;
            set
            {
                _CurrentContent = value;
                OnPropertyChanged(nameof(CurrentContent));
            }
        }
        

        private ObservableCollection<vmHeading> OriginHeadings { get; set; }
        public ReadOnlyObservableCollection<vmHeading> Headings => new ReadOnlyObservableCollection<vmHeading>(this.OriginHeadings);


        public object Display_Name
        {
            get => _Display_Name;
            private set
            {
                _Display_Name = value;
                OnPropertyChanged(nameof(this.Display_Name));
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
    public partial class vmMaterial
    {
      public void SelectShape(Shape shape)
        {
            if (this.SelectedShapes.Contains(shape)) return;
            this.SelectedShapes.Add(shape);
        }

        public void AddSlide(vmSlide slide)
        {
            slide.SetParentMaterial(this);
            this.OriginSlides.Add(slide);
        }
        public void AddContent(vmContent content)
        {
            content.SetParentMaterial(this);
            this.OriginContents.Add(content);
        }
        public void AddHeading(vmHeading heading)
        {
            heading.SetParentMaterial(this);
            this.OriginHeadings.Add(heading);
        }
        public void ClearSlides() => this.OriginSlides.Clear();
        public override void InitializeDisplay()
        {
            this.Display_Name = this.Temp.Name;
            this.Display_UpdateDate = this.Temp.UpdateDate.HasValue ? this.Temp.UpdateDate.Value.ToString("yyyy-MM-dd HH:mm:ss") : null;

            OnModifyStatusChanged();
        }
        public void OrderSlides()
        {
            this.OriginSlides = new ObservableCollection<vmSlide>(this.OriginSlides.OrderBy(x=> x.Temp.Index)); 
        }
        private void OriginSlides_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Slides));
        }
        public void RemoveSlide(vmSlide slide) => this.OriginSlides.Remove(slide);
        public override void SetInitialData()
        {
            //this.SelectedShapes = { };

            this.OriginSlides = new ObservableCollection<vmSlide>();
            this.OriginSlides.CollectionChanged += OriginSlides_CollectionChanged;

            this.OriginContents = new ObservableCollection<vmContent>();

            this.OriginHeadings = new ObservableCollection<vmHeading>();
        }
        public void SetPresentation(Presentation ppt) => this.OriginPresentation = ppt; 

        public override object UpdateOriginData()
        {
            this.Origin.ParentUid = this.Temp.ParentUid;
            this.Origin.Name = this.Temp.Name;
            this.Origin.UpdateDate = this.Temp.UpdateDate = this.Origin.UpdateDate.HasValue ? DateTime.Now : this.Origin.CreateDate;

            return this.Origin;
        }
      
    }
}
