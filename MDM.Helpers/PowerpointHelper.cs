using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MDM.Commons.Enum;
using MDM.Models.DataModels;
using Microsoft.Office.Interop.PowerPoint;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace MDM.Helpers
{
    public static class PowerpointHelper
    {
        public static FileInfo GetFileInfoFromPresentation(Presentation presentation)
        {
            FileInfo output = null;

            string filepath = presentation.FullName;
            output = new FileInfo(filepath);
            return output;
        }
        public static DirectoryInfo GetDirectoryInfoFromPresenction(Presentation presentation)
        {
            DirectoryInfo output = null;

            string filePath = presentation.FullName;
            string directoryName = Path.GetDirectoryName(filePath);
            output = new DirectoryInfo(directoryName);

            return output;
        }
        public static string GetFileNameFromPresentation(Presentation presentation)
        {
            string output = string.Empty;

            string filePath = presentation.FullName;
            output = Path.GetFileNameWithoutExtension(filePath);

            return output;
        }

        public static void SetShapeBaseData(mShape shape, Shape origin)
        {
            shape.ShapeId = origin.Id;
            shape.Name = origin.Name;
            shape.Left = origin.Left;
            shape.Top = origin.Top;
            shape.Width = origin.Width;
            shape.Height = origin.Height;
            shape.DistanceFromOrigin = Math.Sqrt(Math.Pow(shape.Left, 2) + Math.Pow(shape.Top, 2));
        }

        public static void DownloadImage(Shape origin, FileInfo fInfo)
        {
            var tempFile = Path.Combine(Path.GetTempPath(), "tempImage.png");

            origin.Copy();
            System.Windows.Forms.IDataObject dataObject = (System.Windows.Forms.IDataObject)Clipboard.GetDataObject();
            if (dataObject.GetDataPresent(System.Windows.Forms.DataFormats.Bitmap))
            {
                var bitmap = (System.Drawing.Bitmap)dataObject.GetData(System.Windows.Forms.DataFormats.Bitmap);
                bitmap.Save(tempFile);

                // 임시 파일을 지정된 경로로 이동
                File.Copy(tempFile, fInfo.FullName, true);
                File.Delete(tempFile);
            }
            Marshal.ReleaseComObject(origin);
        }

        public static mShape GetTextShape(Shape shape)
        {
            mShape newText = new mShape(eShapeType.Text);
            PowerpointHelper.SetShapeBaseData(newText, shape);
            newText.Text = shape.TextFrame.TextRange.Text.Trim();

            string[] lines = TextHelper.SplitText(newText.Text);
            foreach (string ln in lines)
            {
                mItem newItem = new mItem();
                newItem.Title = newText.Title;
                newItem.LineText = ln.Trim();
                newItem.ItemType = newText.ShapeType;
                newText.Lines.Add(newItem);
            }

            return newText;
        }
        public static mShape GetImageShpe(Shape shape)
        {
            mShape newImage = new mShape(eShapeType.Image);
            PowerpointHelper.SetShapeBaseData(newImage, shape);
            if (string.IsNullOrEmpty(newImage.Title)) newImage.Title = "NO TITLE";

            mItem newItem = new mItem();
            newItem.Title = newImage.Title;
            newItem.LineText = newItem.GenerateImageLineText(newImage);// string.Format("![{0}]({1}{2})", newImage.Title, newImage.Text, Defines.EXTENSION_IMAGE);
            newItem.ItemType = newImage.ShapeType;
            newImage.Lines.Add(newItem);

            return newImage;
        }
        public static mShape GetTableShape(Shape shape)
        {
            Table table = shape.Table;
            mShape newTable = new mShape(eShapeType.Table);
            PowerpointHelper.SetShapeBaseData(newTable, shape);
            System.Data.DataTable dt = new System.Data.DataTable(string.IsNullOrEmpty(table.Title) ? shape.Name : shape.Title);

            Row row = table.Rows[1];
            for (int colH = 1; colH <= row.Cells.Count; colH++)
            {
                string headerText = row.Cells[colH].Shape.TextFrame.TextRange.Text;
                string columnHeader = string.Format("Col_{0}_{1}", colH.ToString("000"), headerText);
                //columnHeader = headerText;
                dt.Columns.Add(columnHeader);
            }
            for (int rowNum = 2; rowNum <= table.Rows.Count; rowNum++)
            {
                DataRow addedRow = dt.NewRow();
                for (int col = 1; col <= table.Columns.Count; col++)
                {
                    string cellText = table.Rows[rowNum].Cells[col].Shape.TextFrame.TextRange.Text;
                    addedRow[col - 1] = cellText;
                }
                dt.Rows.Add(addedRow);
            }

            string tableString = string.Empty;

            string divText = "|";
            Dictionary<int, string> headerDic = new Dictionary<int, string>();
            for (int c = 0; c < dt.Columns.Count; c++)
            {
                string value = dt.Columns[c].ColumnName.Substring(8);
                string[] lines = TextHelper.SplitText(value);

                for (int r = 0; r < lines.Count(); r++)
                {
                    if (!headerDic.ContainsKey(r)) headerDic.Add(r, "|");
                    int barCount = headerDic[r].Count(x => x.Equals('|'));
                    for (int e = 0; e < (c) - barCount; e++) headerDic[r] += "\t|";

                    headerDic[r] += lines[r];
                    headerDic[r] += "|";
                }

                divText += " --- |";
            }

            foreach (string item in headerDic.Values)
            {
                tableString += item;
                tableString += "\n";
            }

            tableString += divText;
            tableString += "\n";


            string rowText = string.Empty;
            foreach (DataRow item in dt.Rows)
            {
                #region 이전 코드
                //Dictionary<int, string> rowDic = new Dictionary<int, string>();
                //int cNum = 1;
                //foreach (var cell in item.ItemArray)
                //{
                //    string value = cell.ToString();
                //    string[] lines = TextHelper.SplitText(value);

                //    for (int r = 0; r < lines.Count(); r++)
                //    {
                //        if (!rowDic.ContainsKey(r)) rowDic.Add(r, "|");
                //        int barCount = rowDic[r].Count(x => x.Equals('|'));
                //        for (int e = 0; e < (cNum) - barCount; e++) rowDic[r] += "\t|";

                //        rowDic[r] += lines[r];
                //        rowDic[r] += "|";
                //    }

                //    cNum++;
                //}
                //foreach (string rowstring in rowDic.Values)
                //{
                //    rowText += rowstring;
                //    if (rowDic.Values.LastOrDefault() != rowstring) rowText += "\n";
                //}
                #endregion
                rowText += "|";
                foreach (var cell in item.ItemArray)
                {
                    string value = cell.ToString();
                    if (TextHelper.IsNoText(value)) value = "{NULL}";
                    string[] lines = TextHelper.SplitText(value);

                    string cellString = string.Empty;
                    for (int r = 0; r < lines.Count(); r++)
                    {
                        cellString += lines[r];
                        if(r != lines.Count() - 1) cellString += "\n";  
                    }

                    rowText += cellString;
                    rowText += "\t|";
                }

                rowText += "\n";
            }
            tableString += rowText;


            newTable.Text = tableString.Trim();
            newTable.DataTable = JsonHelper.ToJsonString(dt);

            mItem newItem = new mItem();
            newItem.Title = newTable.Title;
            newItem.LineText = newTable.Text;
            newItem.ItemType = newTable.ShapeType;
            newTable.Lines.Add(newItem);

            return newTable;
        }
        public static mShape GetTableShapeInCludedMergeCell(Shape shape)
        {
            Table table = shape.Table;
            mShape newTable = new mShape(eShapeType.Table);
            PowerpointHelper.SetShapeBaseData(newTable, shape);
            List<mCell> cells = new List<mCell>();

            string tableString = string.Empty;
            for (int row = 1; row <= table.Rows.Count; row++)
            {
                for (int col = 1; col < table.Columns.Count; col++)
                {
                    Cell currentCell = table.Cell(row, col);
                    int columSpan = GetColumnSpan(currentCell, table, col);

                    float cellHeight = currentCell.Shape.Height;
                    int rowSpan = GetRowSpan(currentCell, table, row);

                    string cellText = currentCell.Shape.TextFrame.TextRange.Text;


                    
                }
            }





























            //Row row = table.Rows[1];
            //for (int colH = 1; colH <= row.Cells.Count; colH++)
            //{
            //    string headerText = row.Cells[colH].Shape.TextFrame.TextRange.Text;
            //    string columnHeader = string.Format("Col_{0}_{1}", colH.ToString("000"), headerText);
            //    //columnHeader = headerText;
            //    dt.Columns.Add(columnHeader);
            //}
            //for (int rowNum = 2; rowNum <= table.Rows.Count; rowNum++)
            //{
            //    DataRow addedRow = dt.NewRow();
            //    for (int col = 1; col <= table.Columns.Count; col++)
            //    {
            //        string cellText = table.Rows[rowNum].Cells[col].Shape.TextFrame.TextRange.Text;
            //        addedRow[col - 1] = cellText;
            //    }
            //    dt.Rows.Add(addedRow);
            //}

            //string tableString = string.Empty;

            //string divText = "|";
            //Dictionary<int, string> headerDic = new Dictionary<int, string>();
            //for (int c = 0; c < dt.Columns.Count; c++)
            //{
            //    string value = dt.Columns[c].ColumnName.Substring(8);
            //    string[] lines = TextHelper.SplitText(value);

            //    for (int r = 0; r < lines.Count(); r++)
            //    {
            //        if (!headerDic.ContainsKey(r)) headerDic.Add(r, "|");
            //        int barCount = headerDic[r].Count(x => x.Equals('|'));
            //        for (int e = 0; e < (c) - barCount; e++) headerDic[r] += "\t|";

            //        headerDic[r] += lines[r];
            //        headerDic[r] += "|";
            //    }

            //    divText += " --- |";
            //}

            //foreach (string item in headerDic.Values)
            //{
            //    tableString += item;
            //    tableString += "\n";
            //}

            //tableString += divText;
            //tableString += "\n";


            //string rowText = string.Empty;
            //foreach (DataRow item in dt.Rows)
            //{
            //    #region 이전 코드
            //    //Dictionary<int, string> rowDic = new Dictionary<int, string>();
            //    //int cNum = 1;
            //    //foreach (var cell in item.ItemArray)
            //    //{
            //    //    string value = cell.ToString();
            //    //    string[] lines = TextHelper.SplitText(value);

            //    //    for (int r = 0; r < lines.Count(); r++)
            //    //    {
            //    //        if (!rowDic.ContainsKey(r)) rowDic.Add(r, "|");
            //    //        int barCount = rowDic[r].Count(x => x.Equals('|'));
            //    //        for (int e = 0; e < (cNum) - barCount; e++) rowDic[r] += "\t|";

            //    //        rowDic[r] += lines[r];
            //    //        rowDic[r] += "|";
            //    //    }

            //    //    cNum++;
            //    //}
            //    //foreach (string rowstring in rowDic.Values)
            //    //{
            //    //    rowText += rowstring;
            //    //    if (rowDic.Values.LastOrDefault() != rowstring) rowText += "\n";
            //    //}
            //    #endregion
            //    rowText += "|";
            //    foreach (var cell in item.ItemArray)
            //    {
            //        string value = cell.ToString();
            //        if (TextHelper.IsNoText(value)) value = "{NULL}";
            //        string[] lines = TextHelper.SplitText(value);

            //        string cellString = string.Empty;
            //        for (int r = 0; r < lines.Count(); r++)
            //        {
            //            cellString += lines[r];
            //            if (r != lines.Count() - 1) cellString += "\n";
            //        }

            //        rowText += cellString;
            //        rowText += "\t|";
            //    }

            //    rowText += "\n";
            //}
            //tableString += rowText;


            //newTable.Text = tableString.Trim();
            //newTable.DataTable = JsonHelper.ToJsonString(dt);

            //mItem newItem = new mItem();
            //newItem.Title = newTable.Title;
            //newItem.LineText = newTable.Text;
            //newItem.ItemType = newTable.ShapeType;
            //newTable.Lines.Add(newItem);

            return newTable;
        }

        private static int GetColumnSpan(Cell currentCell, Table table, int col)
        {
            int output = 1;

            float cellWidth = currentCell.Shape.Width;
            float columnWidth = table.Columns[col].Width;
            while ( columnWidth < cellWidth)
            {
                columnWidth += table.Columns[col + output].Width;
                output++;
            }

            return output;
        }
        private static int GetRowSpan(Cell currentCell, Table table, int row)
        {
            int output = 1;
            float cellHeight = currentCell.Shape.Height;
            float rowHeight = table.Rows[row].Height;
            while (rowHeight < cellHeight)
            {
                rowHeight += table.Rows[row + output].Height;
                output++;
            }
            return output;
        }


        public static List<mShape> OrderByOriginPoint(this List<mShape> shapes)
        {
            List<mShape> output = shapes.OrderBy(x => x.Top).ThenBy(x => x.DistanceFromOrigin).ToList();
            return output;
        }
    }
}
