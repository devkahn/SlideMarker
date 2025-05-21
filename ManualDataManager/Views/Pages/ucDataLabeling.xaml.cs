using System;
using System.Collections.Generic;
using System.Configuration;
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
            try
            {
                this.ParentWindow = parent;
                InitializeComponent();
                SetInitialize();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }



        private RegistryKey GetVersionRegistryKey()
        {
            string[] SubKeys = { "SOFTWARE", "Microsoft", "Office", "Powerpoint", "Addins", "SlideMarker" };
            RegistryKey reg = Registry.CurrentUser;
            foreach (string key in SubKeys) reg = reg.GetSubKeyNames().Contains(key) ? reg.OpenSubKey(key, true) : reg.CreateSubKey(key, true);

            return reg;
        }
        private void SetInitialize()
        {
            this.pgDataLabeling.OpenButton.IsEnabled = false;
            this.pgDataLabeling.LoadButton.Click += btn_LoadButton_Click;
            SetVersion();
        }
        private void SetVersion()
        {
            RegistryKey regKey = GetVersionRegistryKey();
            if (regKey == null) return;
            if (regKey != null && regKey.GetValueNames().Contains("Version")) this.pgDataLabeling.Version.Text = regKey.GetValue("Version").ToString();
        }



        private void btn_LoadButton_Click(object sender, RoutedEventArgs e)
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


                List<Slide> slides = new List<Slide>();
                foreach (Slide slide in ProgramValues.PowerPointApp.ActivePresentation.Slides)
                {
                    slides.Add(slide);
                }
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
                        if (!isAny) shapes.Add(shape);
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
                            mShape newTable = PowerpointHelper.GetTableShape(shape);
                            shapeInstances.Add(newTable);
                        }
                        else if (shape.Type == MsoShapeType.msoPicture)
                        {
                            mShape newImage = PowerpointHelper.GetImageShpe(shape);
                            shapeInstances.Add(newImage);
                        }
                        else if (shape.Type == MsoShapeType.msoAutoShape)
                        {
                            if (shape.HasTextFrame == MsoTriState.msoTrue && shape.TextFrame.HasText == MsoTriState.msoTrue)
                            {
                                mShape newText = PowerpointHelper.GetTextShape(shape);
                                shapeInstances.Add(newText);
                            }
                            else
                            {
                                mShape newImage = PowerpointHelper.GetImageShpe(shape);
                                shapeInstances.Add(newImage);
                            }
                        }
                        else if (shape.HasTextFrame == MsoTriState.msoTrue && shape.TextFrame.HasText == MsoTriState.msoTrue)
                        {
                            mShape newText = PowerpointHelper.GetTextShape(shape);
                            shapeInstances.Add(newText);
                        }
                    }

                    sl.Shapes = shapeInstances;
                    sl.Shapes = sl.Shapes.OrderByOriginPoint();   
                    sl.Origin = slide;

                    vmSlide newSlide = new vmSlide(sl);
                    newSlide.SetParentMaterial(newVm);
                    newSlide.OnModifyStatusChanged(true);
                }

                newVm.OrderSlides();
                this.pgDataLabeling.SetMaterial(newVm);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }

        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgramValues.PowerPointApp.SlideSelectionChanged += this.pgDataLabeling.PowerPointApp_SlideSelectionChanged;
               // ProgramValues.PowerPointApp.WindowSelectionChange += this.pgDataLabeling.PowerPointApp_WindowSelectionChange;
                
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
               // ProgramValues.PowerPointApp.WindowSelectionChange -= this.pgDataLabeling.PowerPointApp_WindowSelectionChange;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
