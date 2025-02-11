using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ManualDataManager.Commons;
using ManualDataManager.Helpers;
using ManualDataManager.Views.Windows;
using MDM.Commons;
using MDM.Commons.Enum;
using MDM.Helpers;
using MDM.Models.DataModels;
using MDM.Models.ViewModels;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Win32;
using Path = System.IO.Path;
using Shape = Microsoft.Office.Interop.PowerPoint.Shape;
using Table = Microsoft.Office.Interop.PowerPoint.Table;

namespace ManualDataManager.Views.Pages
{
    /// <summary>
    /// ucDataLabeling.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucDataLabeling : UserControl
    {
        public frm_DataLabelingBase ParentWindow { get; set; } = null;

        public ucDataLabeling(frm_DataLabelingBase parent)
        {
            this.ParentWindow = parent;
            InitializeComponent();
            this.pgDataLabeling.OpenButton.IsEnabled = false;
            this.pgDataLabeling.LoadButton.Click += LoadButton_Click;

            string[] SubKeys = { "SOFTWARE", "Microsoft", "Office", "Powerpoint", "Addins", "ManualDataManager" };
            RegistryKey reg = Registry.CurrentUser;
            foreach (string key in SubKeys) reg = reg.GetSubKeyNames().Contains(key) ? reg.OpenSubKey(key, true) : reg.CreateSubKey(key, true);
            if(reg != null && reg.GetValueNames().Contains("Version")) this.pgDataLabeling.Version.Text = reg.GetValue("Version").ToString();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgramValues.PowerPointApp.SlideSelectionChanged += this.pgDataLabeling.PowerPointApp_SlideSelectionChanged;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgramValues.PowerPointApp.SlideSelectionChanged -= this.pgDataLabeling.PowerPointApp_SlideSelectionChanged;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fullPath = ProgramValues.PowerPointApp.ActivePresentation.FullName;

                #region 유효한 PPT 여부 확인
                FileInfo fInfo = new FileInfo(fullPath);
                if (!fInfo.Exists)
                {
                    string eMsg = "문서를 저장하세요.";
                    MessageHelper.ShowErrorMessage("LOAD", eMsg);
                    return;
                }
                #endregion
                #region DB 파일 세팅
                string onlyFilename = Path.GetFileNameWithoutExtension(fInfo.Name);
                string subDirPath = Path.Combine(fInfo.DirectoryName, onlyFilename);
                if (!Directory.Exists(subDirPath)) Directory.CreateDirectory(subDirPath);

                string dbTargetPath = Path.Combine(subDirPath, onlyFilename + ".mdm");
                if (!File.Exists(dbTargetPath))
                {
                    string assDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    string dbOriginPath = Path.Combine(assDirPath, Defines.PATH_FILE_BASE_DB);
                    if (!File.Exists(dbOriginPath))
                    {
                        dbOriginPath = Path.Combine(Defines.PATH_DIRECOTRY_PROGRMA, Defines.PATH_FILE_BASE_DB);
                    }
                    File.Copy(dbOriginPath, dbTargetPath, true);
                }


                DBHelper.ConnectionSting = dbTargetPath;
                #endregion

                mMaterial newMaterial = DBHelper.Read();
                if (newMaterial == null)
                {
                    newMaterial = new mMaterial();
                    newMaterial.Name = Path.GetFileNameWithoutExtension(ProgramValues.PowerPointApp.ActivePresentation.Name);
                    bool isCreated = DBHelper.Create(newMaterial);
                    if (!isCreated)
                    {
                        string eMsg = "데이터를 로드할 수 없습니다.";
                        MessageHelper.ShowErrorMessage("데이터 로드", eMsg);
                        return;
                    }
                }

                vmMaterial newVm = new vmMaterial(newMaterial);
                newVm.LoadChildren();
                newVm.SetPresentation(ProgramValues.PowerPointApp.ActivePresentation);
                newVm.DirectoryPath = subDirPath;

                List<Slide> slides = PowerpointHelper.GetSlidesFromCurrentFile();
                foreach (Slide slide in slides)
                {
                    vmSlide sameSlide = newVm.Slides.Where(x => x.Temp.Index == slide.SlideIndex).FirstOrDefault();
                    if (sameSlide != null)
                    {
                        sameSlide.OnModifyStatusChanged(false);
                        continue;
                    }

                    mSlide sl = new mSlide();
                    #region Slide 데이터 객체
                    sl.SlideId = slide.SlideID;
                    sl.Index = slide.SlideIndex;
                    sl.Name = slide.Name;
                    sl.SlideNumber = slide.SlideNumber;
                    #endregion
                    List<Shape> shapes = new List<Shape>();
                    #region Slide Shape
                    foreach (Shape shape in slide.Shapes)
                    {
                        if (shape.Type == MsoShapeType.msoGroup)
                        {
                            shape.Ungroup();
                            continue;
                        }

                        bool isAny = shapes.Where(x => x != null && x.Id == shape.Id).Any();
                        if(!isAny) shapes.Add(shape);
                    }
                    var masterShapes = slide.Master.Shapes; 
                    foreach (Shape shape in masterShapes)
                    {
                        if (shape.Type == MsoShapeType.msoGroup) shape.Ungroup();

                        bool isAny = shapes.Where(x => x.Id == shape.Id).Any();
                        if (!isAny) shapes.Add(shape);
                    }
                    #endregion
                    List<mShape> shapeInstances = new List<mShape>();
                    foreach (Shape shape in shapes)
                    {
                        if (shape.HasTable == MsoTriState.msoTrue)
                        {
                            mShape newTable = GetTableShape(shape);
                            shapeInstances.Add(newTable);
                        }
                        else if (shape.Type == MsoShapeType.msoPicture)
                        {
                            mShape newImage = GetImageShpe(shape);
                            shapeInstances.Add(newImage);
                        }
                        else if(shape.Type == MsoShapeType.msoAutoShape)
                        {
                            if (shape.HasTextFrame == MsoTriState.msoTrue && shape.TextFrame.HasText == MsoTriState.msoTrue)
                            {
                                mShape newText = GetTextShape(shape);
                                shapeInstances.Add(newText);
                            }
                            else
                            {
                                mShape newImage = GetImageShpe(shape);
                                shapeInstances.Add(newImage);
                            }
                        }
                        else if (shape.HasTextFrame == MsoTriState.msoTrue && shape.TextFrame.HasText == MsoTriState.msoTrue)
                        {
                            mShape newText = GetTextShape(shape);
                            shapeInstances.Add(newText);
                        }
                    }

                    sl.Shapes = shapeInstances;
                    sl.Shapes = sl.Shapes.OrderBy(x => x.Top).ThenBy(x => x.DistanceFromOrigin).ToList();
                    sl.Origin = slide;

                    vmSlide newSlide = new vmSlide(sl);
                    newVm.AddSlide(newSlide);
                    newSlide.OnModifyStatusChanged(true);
                }

                newVm.OrderSlides();
                this.pgDataLabeling.SetMaterial(newVm);
            }
            catch (Exception  ee)
            {
                ErrorHelper.ShowError(ee);
            }
            
        }

        private mShape GetTextShape(Shape shape)
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

        private mShape GetImageShpe(Shape shape)
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

        private mShape GetTableShape(Shape shape)
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
                    if(!headerDic.ContainsKey(r)) headerDic.Add(r, "|");
                    int barCount = headerDic[r].Count(x => x.Equals('|'));
                    for (int e = 0; e < (c ) - barCount; e++) headerDic[r] += "\t|";

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
                Dictionary<int, string> rowDic = new Dictionary<int, string>();

                int cNum = 1;
                foreach (var cell in item.ItemArray)
                {
                    string value = cell.ToString();
                    string[] lines = TextHelper.SplitText(value);

                    for (int r = 0; r < lines.Count(); r++)
                    {
                        if (!rowDic.ContainsKey(r)) rowDic.Add(r, "|");
                        int barCount = rowDic[r].Count(x => x.Equals('|'));
                        for (int e = 0; e < (cNum) - barCount; e++) rowDic[r] += "\t|";

                        rowDic[r] += lines[r];
                        rowDic[r] += "|";
                    }

                    cNum++;
                }

                foreach (string rowstring in rowDic.Values)
                {
                    rowText += rowstring;
                    if(rowDic.Values.LastOrDefault() != rowstring) rowText += "\n";
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
    }
}
