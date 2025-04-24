using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using MDM.Commons;
using MDM.Commons.Enum;
using MDM.Models.DataModels;
using MDM.Models.ViewModels;

namespace MDM.Helpers
{
    public static class DataHelper
    {
        public static void Save(this vmMaterial obj)
        {
            if (obj == null) return;

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
            obj.ParentSlide.RemoveShpae(obj);// Shapes.Remove(obj);
            obj.ParentSlide.Temp.Shapes.Remove(obj.Temp);
            obj.SetParent(null);

            mShape data = obj.UpdateOriginData() as mShape;
            if (data == null) return;

            bool isDeleted = data.Delete();
            if (!isDeleted) return;

            foreach (vmItem slide in obj.Items) slide.Delete();
        }
        public static void Delete(this vmItem obj)
        {
            obj.ParentShape.Items.Remove(obj);
            obj.ParentShape.Temp.Lines.Remove(obj.Temp);
            obj.ParentShape.ParentSlide.RemoveItem(obj);// .Items.Remove(obj);
            obj.SetParent(null);

            foreach (vmItem child in obj.Children.ToList()) child.SetParentItem(obj.ParentItem);
            obj.SetParentItem(null);

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

            foreach (mSlide slide in children.OrderBy(x=> x.Index))
            {
                vmSlide newSlide = new vmSlide(slide);
                newSlide.SetParentMaterial(obj);
                newSlide.LoadChildren();
                Debug.WriteLine(slide.Index);
            }
        }
        public static void LoadChildren(this vmSlide obj)
        {
            Thread.Sleep(1000);
            if (obj.Display_Index.ToString() == "270")
            {

            }
            mSlide parent = obj.Temp;
            List<mShape> children = DBHelper.Read(parent);
            if (children == null) return;

            foreach (mShape shape in children)
            {
                
                vmShape newShape = new vmShape(shape);
                newShape.SetParent(obj);
                newShape.LoadChildren();
            }

            foreach (vmItem item in obj.Items)
            {
                if (string.IsNullOrEmpty(item.Temp.ParentItemUid)) continue;
                vmItem parentItem = obj.Items.Where(x => x.Temp.Uid == item.Temp.ParentItemUid).FirstOrDefault();
                if(parentItem != null) item.SetParentItem(parentItem, true);
            }
        }
        public static void LoadChildren(this vmShape obj)
        {
            mShape parent = obj.Temp;
            List<mItem> children = DBHelper.Read(parent);
            if (children == null) return;

            foreach (mItem item in children)
            {
                vmItem newItem = new vmItem(item);
                newItem.SetParent(obj);
            }
        }

        public static string GenerateImageLineText(this mItem newItem, mShape imageShape)
        {
            return string.Format("![{0}]({1}{2})", imageShape.Title, newItem.Uid, Defines.EXTENSION_IMAGE);
        }


  

        public static void ClassifyContent(vmContent content)
        {
            switch (content.Temp.ItemType)
            {
                case Commons.Enum.eItemType.Image: content.ContentType = Commons.Enum.eContentType.Image; break;
                case Commons.Enum.eItemType.Table: content.ContentType = Commons.Enum.eContentType.Table; break;
                case Commons.Enum.eItemType.Text: 
                default:
                    ClassifyTextContet(content);
                    break;
            }
            
        }

        private static void ClassifyTextContet(vmContent content)
        {
            string origin = content.Temp.Temp.LineText;
            origin = TextHelper.RemoveNoTextLine(origin);

            if(TextHelper.IsImageMarkdown(origin).Success)
            {
                content.Temp.SetItemType(eItemType.Image);
                content.ContentType = eContentType.Image;
                return;
            }

            Dictionary<int, string> newLineDict = new Dictionary<int, string>();
            Dictionary<int, string> lineDict = TextHelper.SplitText(origin).ToDictionary();
            if (lineDict.Count() == 1)
            {
                string line = lineDict[0];
                Commons.Enum.eContentType  contType = GetContentType(line, out string output);
                content.ContentType = contType;
                content.IsContentsValid = true;

                if(content.ContentType == eContentType.UnOrderList)
                {
                    int index = TextHelper.markers.IndexOf(line.Trim().First());
                    if (index >= 0)
                    {
                        string mark = string.Empty;
                        for (int i = 0; i < index; i++) mark += "  ";
                        mark += "-";
                        line = line.Trim().Substring(1).Trim();
                        string newLine = string.Format("{0} {1}", mark, line);
                        newLineDict.Add(0, newLine);
                    }
                }
                else
                {
                    newLineDict.Add(0, output);
                }

                
            }
            else
            {
                int level = 0;
                while (lineDict.Count != 0)
                {
                    List<int> targetKeys = new List<int>();
                    while (!targetKeys.Any())
                    {
                        foreach (int key in lineDict.Keys.ToList())
                        {
                            string line = lineDict[key];
                            if (char.IsWhiteSpace(line.First()))
                            {
                                lineDict[key] = line.Substring(1);
                                continue;
                            }

                            targetKeys.Add(key);
                        }
                    }

                    string mark = string.Empty;
                    for (int i = 0; i < level; i++) mark += "  ";
                    
                    level++;
                    foreach (int key in targetKeys)
                    {
                        string line = lineDict[key];
                        if (char.IsWhiteSpace(line.First())) continue;

               

                        if (TextHelper.IsFirstNumericListMark(line.Trim()))
                        {
                            newLineDict.Add(key, string.Format("{0}{1}", mark, line.Trim()));
                            lineDict.Remove(key);
                        }
                        else
                        {
                            char firstChar = line.Trim().First();
                            int index = TextHelper.markers.IndexOf(firstChar);
                            if (index >= 0)
                            {
                                string rawLine = line.Trim().Substring(1).Trim();
                                bool isRawOrdered = TextHelper.IsFirstNumericListMark(rawLine);

                                if (isRawOrdered && char.IsDigit(rawLine.First()))
                                {
                                    char second = rawLine[1];
                                    if (!TextHelper.NumberDividerMarks.Contains(second)) isRawOrdered = false;
                                }

                                if (isRawOrdered)
                                {
                                    newLineDict.Add(key, string.Format("{0}{1}", mark, rawLine));
                                }
                                else
                                {
                                    if(mark.LastOrDefault() != '-') mark += "-";
                                    newLineDict.Add(key, string.Format("{0} {1}", mark, line.Trim().Substring(1).Trim()));
                                }

                                lineDict.Remove(key);
                            }
                            else
                            {
                                bool isLineBreak = TextHelper.linebreakMarkers.Contains(firstChar);
                                if (isLineBreak)
                                {
                                    if (key == 0)
                                    {
                                        content.ContentType = eContentType.None;
                                        content.IsContentsValid = false;
                                        return;
                                    }

                                    mark = string.Empty;
                                    string preLine = newLineDict[key - 1];
                                    mark += TextHelper.GetEmptyCharFromHead(preLine);
                                    if (firstChar != '+') mark += '+';

                                    newLineDict.Add(key, string.Format("{0} {1}", mark, line.Trim()));
                                    lineDict.Remove(key);
                                }
                                else
                                {
                                    content.ContentType = eContentType.None;
                                    content.IsContentsValid = false;
                                    return;
                                }
                            }
                        }

                
                    }
            

                }
                content.ContentType = eContentType.UnOrderList;
                content.IsContentsValid = true;
            }

            string newText = string.Empty;
            int preLevel = 0;
            foreach (int key in newLineDict.Keys.OrderBy(x=> x).ToList())
            {
                string line = newLineDict[key];
                int level = TextHelper.GetLineLevel(line);
                if(preLevel < level)
                {
                    if(level - preLevel > 1)
                    {
                        string mark = string.Empty;
                        for (int i = 0; i < preLevel + 1; i++) mark += "  ";
                        line = string.Format("{0}{1}", mark, line.Trim());
                    }
                }
                else
                {
                    preLevel = level;
                }

                newText += line  + "\n";
            }

            string[] templines = TextHelper.SplitText(newText);

            content.Temp.SetText(newText.Trim());
            content.InitializeDisplay();
        }

        private static eContentType GetContentType(string line, out string output)
        {
            output = line;
            bool isOrdered = TextHelper.IsFirstNumericListMark(line.Trim());
            if (isOrdered) return eContentType.OrderList;

            bool isUnOrdered = TextHelper.IsFirstCharUnorderMark(line.Trim());
  
            if (isUnOrdered)
            {
                string rawLine = line.Trim().Substring(1).Trim();
                bool isRawOrdered = TextHelper.IsFirstNumericListMark(rawLine);

                if (isRawOrdered && char.IsDigit(rawLine.First()))
                {
                    char second = rawLine[1];
                    if (!TextHelper.NumberDividerMarks.Contains(second)) isRawOrdered = false;
                }

                if (isRawOrdered)
                {
                    string mark = TextHelper.GetEmptyCharFromHead(line);
                    output = string.Format("{0}{1}", mark, rawLine);
                    return eContentType.OrderList;
                }

                return eContentType.UnOrderList;
            }


            return eContentType.NormalText;
        }
        
    }
}
