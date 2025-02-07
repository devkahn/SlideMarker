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
                    mSlide newSlide = new mSlide();
                    newSlide.SlideId = slide.SlideID;
                    newSlide.Index = slide.SlideIndex;
                    newSlide.Name = slide.Name;
                    newSlide.SlideNumber = slide.SlideNumber;

                    vmSlide sameSlide = newVm.Slides.Where(x => x.Temp.Index == newSlide.Index).FirstOrDefault();
                    if (sameSlide != null)
                    {
                        sameSlide.OnModifyStatusChanged(false);
                        continue;
                    }

                    List<Shape> shapes = new List<Shape>();
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



                    newSlide.Shapes = new List<mShape>();
                    foreach (Shape shape in shapes)
                    {
                        if (shape.HasTable == MsoTriState.msoTrue)
                        {
                            Table table = shape.Table;

                            mShape newTable = new mShape(eShapeType.Table);
                            PowerpointHelper.SetShapeBaseData(newTable, shape);
                            System.Data.DataTable dt = new System.Data.DataTable(string.IsNullOrEmpty(table.Title) ? shape.Name : shape.Title);

                            for (int col = 1; col <= table.Columns.Count; col++)
                            {
                                if (dt.Columns.Count < table.Columns.Count) dt.Columns.Add("Col_" + col);
                            }

                            foreach (Row row in table.Rows)
                            {
                                newTable.Text += "|";
                                DataRow dataRow = dt.NewRow();
                                for (int col = 1; col <= table.Columns.Count; col++)
                                {
                                    string cellText = row.Cells[col].Shape.TextFrame.TextRange.Text;
                                    dataRow[col - 1] = cellText;
                                    newTable.Text += cellText;
                                    newTable.Text += "|";
                                }
                                dt.Rows.Add(dataRow);
                                newTable.Text += "\n";
                            }
                            newTable.Text = newTable.Text.Trim();
                            newTable.DataTable = JsonHelper.ToJsonString(dt);

                            mItem newItem = new mItem();
                            newItem.Title = newTable.Title;
                            newItem.LineText = newTable.Text;
                            newItem.ItemType = newTable.ShapeType;
                            newTable.Lines.Add(newItem);

                            newSlide.Shapes.Add(newTable);
                        }
                        else if (shape.Type == MsoShapeType.msoPicture)
                        {
                            mShape newImage = new mShape(eShapeType.Image);
                            PowerpointHelper.SetShapeBaseData(newImage, shape);
                            if (string.IsNullOrEmpty(newImage.Title)) newImage.Title = "NO TITLE";

                            mItem newItem = new mItem();
                            newItem.Title = newImage.Title;
                            newItem.LineText =  string.Format("![{0}]({1}{2})", newImage.Title, newItem.Uid, Defines.EXTENSION_IMAGE);
                            newItem.LineText = newItem.GenerateImageLineText(newImage);// string.Format("![{0}]({1}{2})", newImage.Title, newImage.Text, Defines.EXTENSION_IMAGE);
                            newItem.ItemType = newImage.ShapeType;
                            newImage.Lines.Add(newItem);

                            newSlide.Shapes.Add(newImage);
                        }
                        else if(shape.Type == MsoShapeType.msoAutoShape)
                        {
                            if (shape.HasTextFrame == MsoTriState.msoTrue && shape.TextFrame.HasText == MsoTriState.msoTrue)
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

                                newSlide.Shapes.Add(newText);
                            }
                            else
                            {
                                mShape newImage = new mShape(eShapeType.Image);
                                PowerpointHelper.SetShapeBaseData(newImage, shape);
                                if (string.IsNullOrEmpty(newImage.Title)) newImage.Title = "NO TITLE";

                                mItem newItem = new mItem();
                                newItem.Title = newImage.Title; 
                                newItem.LineText = string.Format("![{0}]({1}{2})", newImage.Title, newImage.Text, Defines.EXTENSION_IMAGE);
                                newItem.ItemType = newImage.ShapeType;
                                newImage.Lines.Add(newItem);

                                newSlide.Shapes.Add(newImage);
                            }
                        }
                        else if (shape.HasTextFrame == MsoTriState.msoTrue && shape.TextFrame.HasText == MsoTriState.msoTrue)
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

                            newSlide.Shapes.Add(newText);
                        }
                    }

                 







                    newSlide.Shapes = newSlide.Shapes.OrderBy(x => x.Top).ThenBy(x => x.DistanceFromOrigin).ToList();
                    newSlide.Origin = slide;
                    newVm.AddSlide(new vmSlide(newSlide));
                }

                newVm.OrderSlides();
                this.pgDataLabeling.SetMaterial(newVm);
            }
            catch (Exception  ee)
            {
                ErrorHelper.ShowError(ee);
            }
            
        }
    }
}
