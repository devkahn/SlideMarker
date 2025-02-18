using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MDM.Models.DataModels;

namespace MDM.Models.ViewModels
{
    public partial class vmHeading
    {
        private mHeading _Origin = null;

        private object _Display_Name = null;


    }

    public partial class vmHeading : vmViewModelbase
    {
        public vmHeading(mHeading origin)
        {
            this.Origin = origin;
        }

        private mHeading Origin
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
                this.Temp = this.Origin.Copy<mHeading>();
                InitializeDisplay();
            }
        }
        public mHeading Temp { get; private set; } = null;
        public vmMaterial ParentMaterial { get; private set; } = null;
        public vmHeading Parent { get; private set; } = null;

        private ObservableCollection<vmHeading> OriginChildren { get; set; }
        private ObservableCollection<vmContent> OriginContents { get; set; }


        public ReadOnlyObservableCollection<vmHeading> Children => new ReadOnlyObservableCollection<vmHeading>(this.OriginChildren);
        public ReadOnlyObservableCollection<vmContent> Contents => new ReadOnlyObservableCollection<vmContent>(this.OriginContents);


        public object Display_Name
        {
            get => _Display_Name;
            set
            {
                _Display_Name = value;
                OnPropertyChanged(nameof(this.Display_Name));
            }
        }
    }
    public partial class vmHeading 
    {
        internal void AddChild(vmHeading child)
        {
            this.OriginChildren.Add(child);
           
        }
        internal void AddContent(vmContent content)
        {
            if (this.OriginContents.Contains(content)) return;
            this.OriginContents.Add(content);
        }
        public override void InitializeDisplay()
        {
            this.Display_Name = this.Temp.Name;
        }
        private void OriginChildren_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => OnPropertyChanged(nameof(this.Children));
        private void OriginContents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => OnPropertyChanged(nameof(OriginContents));
        public void RemoveChild(vmHeading heading)
        {
            if (!this.OriginChildren.Contains(heading)) return; ;
            this.OriginChildren.Remove(heading);
        }
        public void RemoveContent(vmContent content)
        {
            if(!this.OriginContents.Contains(content)) return;
            this.OriginContents.Remove(content);
        }
        public override void SetInitialData()
        {
            this.OriginChildren = new ObservableCollection<vmHeading>();
            this.OriginChildren.CollectionChanged += OriginChildren_CollectionChanged;

            this.OriginContents = new ObservableCollection<vmContent>();
            this.OriginContents.CollectionChanged += OriginContents_CollectionChanged;
        }
        public void SetParentMaterial(vmMaterial parent)
        {
            if (this.ParentMaterial != null) this.ParentMaterial.RemoveHeading(this);
            this.ParentMaterial = parent;
            this.Temp.MaterialIdx = -1;
            if(this.ParentMaterial != null)
            {
                this.ParentMaterial.AddHeading(this);
                this.Temp.MaterialIdx = this.ParentMaterial.Temp.Idx;
            }
        }
        public void SetParent(vmHeading parent)
        {
            if (this.Parent != null) this.Parent.RemoveChild(this);

            this.Parent = parent;
            if (this.Parent == null)
            {
                this.ParentMaterial.AddHeading(this);
            }
            else
            {
                this.Parent.AddChild(this);
            }
        }
        public override object UpdateOriginData()
        {
            return null;
        }
    }
}
