using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MDM.Commons.Enum;
using MDM.Models.DataModels;

namespace MDM.Models.ViewModels
{
    public partial class vmHeading
    {
        private mHeading _Origin = null;

        private bool _IsTreeExpanded = true;
        private bool _IsEnabled = true;

        private object _Display_Name = null;
        private eHeadingType _HeadingType = eHeadingType.Normal;
        private object _Display_StartNum = null;
        private object _Display_FinishNum = null;
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
        internal ObservableCollection<vmHeading> OriginChildren { get; set; }
        private ObservableCollection<vmContent> OriginContents { get; set; }

        public eHeadingType HeadingType
        {
            get => _HeadingType;
            private set
            {
                _HeadingType = value;
                OnPropertyChanged(nameof(this.HeadingType));
            }
        }

        public bool IsTreeExpanded
        {
            get => _IsTreeExpanded;
            set
            {
                _IsTreeExpanded = value;
                OnPropertyChanged(nameof(this.IsTreeExpanded), false);
            }
        }
        public bool IsEnabled
        {
            get => _IsEnabled;
            set
            {
                _IsEnabled = value;
                OnPropertyChanged(nameof(this.IsEnabled));
            }
        }

        public ReadOnlyObservableCollection<vmHeading> Children => new ReadOnlyObservableCollection<vmHeading>(this.OriginChildren);
        public ReadOnlyObservableCollection<vmContent> Contents => new ReadOnlyObservableCollection<vmContent>(new ObservableCollection<vmContent>(this.OriginContents.Where(x=> x.IsEnable)));


        public object Display_Name
        {
            get => _Display_Name;
            set
            {
                _Display_Name = value;
                OnPropertyChanged(nameof(this.Display_Name));
            }
        }
        public object Display_StartNum
        {
            get
            {
                if (this.Contents != null && this.Contents.Count() > 0)
                {
                    int min = this.Contents.Min(x => int.Parse(x.Display_SlideNum.ToString()));
                    return this._Display_StartNum = min;
                }
   
                return null;
            }
        }
        public object Display_FinishNum
        {
            get
            {
                if (this.Contents != null && this.Contents.Count() > 0)
                {
                    int max = this.Contents.Max(x => int.Parse(x.Display_SlideNum.ToString()));
                    int min = this.Contents.Min(x => int.Parse(x.Display_SlideNum.ToString()));
                    this._Display_FinishNum = max;
                    if (max != min) return this._Display_FinishNum;
                }
    
                return null;
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
            OnPropertyChanged(nameof(this.Contents));
        }
        public void TreeExpand(bool isExpanded)
        {
            this.IsTreeExpanded = isExpanded;
        }
        public override void InitializeDisplay()
        {
            this.HeadingType = (eHeadingType)this.Temp.HeadintTypeCode;
            this.Display_Name = this.Temp.Name;
        }
        public void Move(bool toFoward, int gap = 1)
        {
            var list = this.Parent == null ? this.ParentMaterial.OriginHeadings : this.Parent.OriginChildren;

            int cIndex = list.IndexOf(this);
            if (toFoward && cIndex < 1) return;
            if (!toFoward && cIndex == list.Count()-1) return;

            int tIndex = toFoward ? cIndex -1 : cIndex + 1;
            list.Move(cIndex, tIndex);
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
            OnPropertyChanged(nameof(this.Contents));
            OnPropertyChanged(nameof(this.Display_StartNum));
            OnPropertyChanged(nameof(this.Display_FinishNum));
        }
        public void SetChildrenLevel()
        {
            this.Temp.Level = this.Parent == null ? 1 : this.Parent.Temp.Level + 1;
            foreach (vmHeading child in this.Children)
            {
                child.SetChildrenLevel();
            }
        }
        public void SetName(string newName)
        {
            this.Display_Name = this.Temp.Name = newName;
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
            this.Temp.ParentUid = string.Empty;
            
            if (this.Parent == null)
            {
                this.ParentMaterial.AddHeading(this);
            }
            else
            {
                this.Temp.ParentUid = this.Parent.Temp.Uid;
                this.Parent.AddChild(this);
            }
            SetChildrenLevel();
            
        }

        public void SetPageNumber()
        {
            OnPropertyChanged(nameof(this.Display_StartNum));
            OnPropertyChanged(nameof(this.Display_FinishNum));
        }
        public int? GetStartNum()
        {
            int? min = null;

            if(this.Contents != null && this.Contents.Count >0)
            {
                return this.Contents.Min(x => int.Parse(x.Display_SlideNum.ToString()));
            }
            else if (this.Children != null && this.Children.Count > 0)
            {
                return this.Children.Min(x => x.GetStartNum());
            }

            return min;
        }
        public int? GetFinishNum()
        {
            int? max = null;

            if (this.Contents != null && this.Contents.Count > 0)
            {
                return this.Contents.Max(x => int.Parse(x.Display_SlideNum.ToString()));
            }
            else if (this.Children != null && this.Children.Count > 0)
            {
                return this.Children.Max(x => x.GetFinishNum());
            }

            return max;
        }

        public void SetHeadintType(eHeadingType type)
        {
            this.HeadingType = type;
            this.Temp.HeadintTypeCode = type.GetHashCode();
        }
        public override object UpdateOriginData()
        {
            this.Origin.Children.Clear();
            foreach (vmHeading child in this.Children)
            {
                mHeading childData = child.UpdateOriginData() as mHeading;
                if (childData != null) this.Origin.Children.Add(childData);
            }

            this.Origin.Contents.Clear();
            foreach (vmContent content in this.Contents)
            {
                mContent contentData = new mContent();
                contentData.MaterialIdx = content.Material.Temp.Idx;
                contentData.ParentUid = content.ParentHeading.Temp.Uid;
                contentData.Heading1Idx = content.Heading1 == null ? -1 : content.Heading1.Temp.Idx;
                contentData.Heading2Idx = content.Heading2 == null ? -1 : content.Heading2.Temp.Idx;
                contentData.Heading3Idx = content.Heading3 == null ? -1 : content.Heading3.Temp.Idx;
                contentData.Heading4Idx = content.Heading4 == null ? -1 : content.Heading4.Temp.Idx;
                contentData.Heading5Idx = content.Heading5 == null ? -1 : content.Heading5.Temp.Idx;
                contentData.Heading6Idx = content.Heading6 == null ? -1 : content.Heading6.Temp.Idx;
                contentData.Heading7Idx = content.Heading7 == null ? -1 : content.Heading7.Temp.Idx;
                contentData.Heading8Idx = content.Heading8 == null ? -1 : content.Heading8.Temp.Idx;
                contentData.Heading9Idx = content.Heading9 == null ? -1 : content.Heading9.Temp.Idx;
                contentData.Heading10Idx = content.Heading10 == null ? -1 : content.Heading10.Temp.Idx;

                contentData.SlideIdx = content.Temp.ParentShape.ParentSlide.Temp.Index;
                contentData.SlideUid = content.Temp.ParentShape.ParentSlide.Temp.Uid;
                contentData.HeadingUid_1 = content.Heading1 == null? string.Empty : content.Heading1.Temp.Uid;
                contentData.HeadingUid_2 = content.Heading2 == null? string.Empty : content.Heading2.Temp.Uid;
                contentData.HeadingUid_3 = content.Heading3 == null? string.Empty : content.Heading3.Temp.Uid;
                contentData.HeadingUid_4 = content.Heading4 == null? string.Empty : content.Heading4.Temp.Uid;
                contentData.HeadingUid_5 = content.Heading5 == null? string.Empty : content.Heading5.Temp.Uid;
                contentData.HeadingUid_6 = content.Heading6 == null? string.Empty : content.Heading6.Temp.Uid;
                contentData.HeadingUid_7 = content.Heading7 == null? string.Empty : content.Heading7.Temp.Uid;
                contentData.HeadingUid_8 = content.Heading8 == null? string.Empty : content.Heading8.Temp.Uid;
                contentData.HeadingUid_9 = content.Heading9 == null? string.Empty : content.Heading9.Temp.Uid;
                contentData.HeadingUid_10 = content.Heading10 == null ? string.Empty : content.Heading10.Temp.Uid;

                
                contentData.ContentsType = content.Temp.Temp.ItemType;
                contentData.Contents = content.Temp.Temp.LineText;

                this.Origin.Contents.Add(contentData);
            }

            this.Origin.MaterialIdx = this.Temp.MaterialIdx;
            this.Origin.ParentUid = this.Temp.ParentUid;

            this.Origin.HeadintTypeCode = this.Temp.HeadintTypeCode;
            this.Origin.Level = this.Temp.Level;
            this.Origin.Name = this.Temp.Name;
            return this.Origin;
        }
        public void ContentsOrderBy()
        {
            List<vmContent> orderContents = this.OriginContents.OrderBy(x => int.Parse(x.Display_SlideNum.ToString())).ToList();
            this.OriginContents.Clear();
            foreach (vmContent item in orderContents)
            {
                if (item.IsEnable == false) continue;

                this.OriginContents.Add(item);
            }
            SetPageNumber();

        }
    }
}
