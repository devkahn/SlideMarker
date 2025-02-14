﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MDM.Commons;
using MDM.Models.DataModels;
using MDM.Models.ViewModels;

namespace MDM.Helpers
{
    public static class DataHelper
    {
        public static void Save(this vmMaterial obj)
        {
            mMaterial data = obj.UpdateOriginData() as mMaterial;
            if (data == null) return;

            if(data.Idx <0)
            {
                bool isCreated = data.Create();
                if (!isCreated) return;
                obj.Temp.Idx = data.Idx;  
            }
            else
            {
                bool isUpdate = data.Update();
                if(!isUpdate) return;
            }

            
            foreach (vmSlide slide in obj.Slides)
            {
                if(slide.IsChanged) slide.Save();
            }

            obj.IsChanged = false;
            obj.OnModifyStatusChanged();
        }
        public static void Save(this vmSlide obj)
        {
            mSlide data = obj.UpdateOriginData() as mSlide;
            if (data == null) return;

            if (data.Idx < 0)
            {
                bool isCreated = data.Create();
                if (!isCreated) return;
                obj.Temp.Idx = data.Idx;
            }
            else
            {
                bool isUpdate = data.Update();
                if (!isUpdate) return;
            }
            foreach (vmShape shape in obj.Shapes)
            {
                shape.Save();
            }

            obj.IsChanged = false;
            obj.OnModifyStatusChanged();
        }
        public static void Save(this vmShape obj)
        {
            mShape data = obj.UpdateOriginData() as mShape;
            if (data == null) return;

            if (data.Idx < 0)
            {
                bool isCreated = data.Create();
                if (!isCreated) return;
                obj.Temp.Idx = data.Idx;
            }
            else
            {
                bool isUpdate = data.Update();
                if (!isUpdate) return;
            }
            foreach (vmItem item in obj.Items)
            {
                item.Save();
            }

            obj.IsChanged = false;
            obj.OnModifyStatusChanged();
        }
        public static void Save(this vmItem obj)
        {
            mItem data = obj.UpdateOriginData() as mItem;
            if (data == null) return;

            if (data.Idx < 0)
            {
                bool isCreated = data.Create();
                if (!isCreated) return;
                obj.Temp.Idx = data.Idx;
            }
            else
            {
                bool isUpdate = data.Update();
                if (!isUpdate) return;
            }
            obj.IsChanged = false;
            obj.OnModifyStatusChanged();
        }

        public static void Delete(this vmMaterial obj)
        {
            mMaterial data = obj.UpdateOriginData() as mMaterial;
            if(data == null) return;

            bool isDeleted = data.Delete();
            if (!isDeleted) return;

            foreach (vmSlide slide in obj.Slides) slide.Delete();
        }
        public static void Delete(this vmSlide obj)
        {
            mSlide data = obj.UpdateOriginData() as mSlide;
            if (data == null) return;

            bool isDeleted = data.Delete();
            if (!isDeleted) return;

            foreach (vmShape slide in obj.Shapes) slide.Delete();
        }
        public static void Delete(this vmShape obj)
        {
            obj.SetParentSlide(null);

            mShape data = obj.UpdateOriginData() as mShape;
            if (data == null) return;

            bool isDeleted = data.Delete();
            if (!isDeleted) return;

            foreach (vmItem slide in obj.Items) slide.Delete();
        }
        public static void Delete(this vmItem obj)
        {
            obj.Shape.Temp.Items.Remove(obj.Temp);

            foreach (vmItem child in obj.Children.ToList()) child.SetParent(obj.Parent);
            obj.SetParentSlide(null);
            obj.SetParentShape(null);
            obj.SetParent(null);

            mItem data = obj.UpdateOriginData() as mItem;
            if (data == null) return;

            bool isDeleted = data.Delete();
            if (!isDeleted) return;
        }

        public static void LoadChildren(this vmMaterial obj)
        {
            mMaterial parent = obj.Temp;
            List<mSlide> children = DBHelper.Read(parent);
            if (children == null) return;

            foreach (mSlide slide in children)
            {
                vmSlide newSlide = new vmSlide(slide);
                newSlide.SetParentMaterial(obj);
                newSlide.LoadChildren();
            }
        }
        public static void LoadChildren(this vmSlide obj)
        {
            mSlide parent = obj.Temp;
            List<mShape> children = DBHelper.Read(parent);
            if (children == null) return;

            foreach (mShape shape in children)
            {
                vmShape newShape = new vmShape(shape);
                newShape.SetParentSlide(obj);
                newShape.LoadChildren();
            }

            foreach (vmItem item in obj.Items)
            {
                if (string.IsNullOrEmpty(item.Temp.ParentItemUid)) continue;
                vmItem parentItem = obj.Items.Where(x => x.Temp.Uid == item.Temp.ParentItemUid).FirstOrDefault();
                if(parentItem != null) item.SetParent(parentItem, true);
            }
        }
        public static void LoadChildren(this vmShape obj)
        {
            mShape parent = obj.Temp;
            List<mItem> children = DBHelper.Read(parent);
            if (children == null) return;

            foreach (mItem item in children.OrderBy(x=> x.Order))
            {
                vmItem newItem = new vmItem(item);
                newItem.SetParentShape(obj);
            }
        }

        public static string GenerateImageLineText(this mItem newItem, mShape imageShape)
        {
            return string.Format("![{0}]({1}{2})", imageShape.Title, newItem.Uid, Defines.EXTENSION_IMAGE);
        }
    }
}
