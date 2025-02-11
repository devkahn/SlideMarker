using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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


        public ObservableCollection<vmHeading> Children { get; private set; } = null;
        public vmContent Content { get; private set; } = null;
        public vmMaterial ParentMaterial { get; private set; } = null;
        public vmHeading Parent { get; private set; } = null;

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
        public void AddChild(vmHeading child)
        {
            this.Children.Add(child);
            child.SetParent(this);
        }
        public override void InitializeDisplay()
        {
            this.Display_Name = this.Temp.Name;
        }

        public override void SetInitialData()
        {
            this.Children = new ObservableCollection<vmHeading>();
        }
        internal void SetParentMaterial(vmMaterial parent)
        {
            this.ParentMaterial = parent;
            this.Origin.MaterialIdx = this.Temp.MaterialIdx = parent.Temp.Idx;
        }
        internal void SetParent(vmHeading parent)
        {
            this.Parent = parent;
        }
        internal void SetContent(vmContent item)
        {
            this.Content = item;
        }
        public override object UpdateOriginData()
        {
            return null;
        }
    }
}
