using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MDM.Helpers;
using MDM.Models.DataModels;
using MDM.Models.ViewModels;
using MDM.Views.DataLabeling.Windows;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using Shape = Microsoft.Office.Interop.PowerPoint.Shape;


namespace MDM.Views.DataLabeling.Pages
{
    /// <summary>
    /// ucDataLabeling.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucDataLabeling : UserControl
    {
        private vmMaterial _Material = null;
        private vmMaterial Material
        {
            get => _Material;
            set
            {
                _Material = value;
                this.ucDataLabelingInfo.SetMaterial(value);
                this.ucDataLabelingSlides.SetMaterial(value);
                this.ucDataLabelingShapes.SetMaterial(value);
                this.ucDataLabelingPreview.SetMaterial(value);
            }
        }
        public TextBlock Version => this.txtblock_Version;
        public Button OpenButton => this.btn_FileOpen;
        public Button LoadButton => this.btn_FileLoad;
        public ucDataLabelingSildes DataLabelingSlides => this.ucDataLabelingSlides;
        public ucDataLabelingShapes DataLabelingShapes => this.ucDataLabelingShapes;
        public wndSlidePreview PreviewWindow { get; set; }



        public ucDataLabeling()
        {
            try
            {
                InitializeComponent();
                // this.LoadButton.Click += LoadButton_Click;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }


        public void SetMaterial(vmMaterial material)
        {
            this.Material = material;
        }


        private void btn_FileOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileInfo fInfo = FileHelper.GetOpenFileInfo();
                if (fInfo == null) return;

                string textValue = FileHelper.ReadTextFile(fInfo);
                if (string.IsNullOrEmpty(textValue)) return;

                DBHelper.ConnectionSting = fInfo.FullName;

                mMaterial newMaterial = DBHelper.Read();
                if (newMaterial == null)
                {
                    newMaterial = new mMaterial();
                    newMaterial.Name = "test_PROJECT";
                    bool isCreated = DBHelper.Create(newMaterial);
                    if (!isCreated)
                    {
                        string eMsg = "material 생성 오류";
                        MessageHelper.ShowErrorMessage("Materail Load", eMsg);
                        return;
                    }
                }

                vmMaterial material = new vmMaterial(newMaterial);
                material.LoadChildren();
                material.CurrentSlide = material.Slides.FirstOrDefault();
                material.OrderSlides();

                this.Material = material;
                this.Material.DirectoryPath = fInfo.DirectoryName;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
        private void bnt_MaterialSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Cursor = Cursors.Wait;
                this.Material.Save();
                this.Cursor = Cursors.Arrow;
            }
            catch (Exception ee)
            {
                this.Cursor = Cursors.Arrow;
                ErrorHelper.ShowError(ee);
            }
        }
        private void btn_LoadButton_Click(object sender, RoutedEventArgs e)
        {
            string assDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //string dbOriginPath = Path.Combine(assDirPath, Defines.PATH_FILE_BASE_DB);
            string dbTargetPath = Path.Combine(assDirPath, "test.mdm");
            //if (!File.Exists(dbTargetPath)) File.Copy(dbOriginPath, dbTargetPath, true);
            DBHelper.ConnectionSting = dbTargetPath;

            mMaterial newMaterial = DBHelper.Read();
            if (newMaterial == null)
            {
                newMaterial = new mMaterial();
                newMaterial.Name = Path.GetFileNameWithoutExtension(DateTime.Now.ToString("yyyy-mm-dd hhMMss"));
                bool isCreated = DBHelper.Create(newMaterial);
                if (!isCreated)
                {
                    string eMsg = "데이터를 로드할 수 없습니다.";
                    MessageHelper.ShowErrorMessage("데이터 로드", eMsg);
                    return;
                }
                else
                {
                    MessageHelper.ShowMessage("데이터 로드", "생성");
                }
            }

        }


        public void PowerPointApp_SlideSelectionChanged(SlideRange SldRange)
        {
            try
            {
                int slideIndex = SldRange.SlideIndex;
                this.ucDataLabelingSlides.MovePage(slideIndex);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        public void PowerPointApp_WindowSelectionChange(Selection selections)
        {
            try
            {
                if (this.Material == null) return;

                // 선택된 항목이 있을 경우
                if (selections.Type == PpSelectionType.ppSelectionShapes)
                {
                    List<Shape> shapes = new List<Shape>();
                    // 선택된 개체들의 타입 출력
                    foreach (Shape shape in selections.ShapeRange)
                    {
                        MessageBox.Show(shape.Name, "selected");
                        this.Material.SelectShape(shape);
                      
                    }
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_BackUp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_ImagesImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DirectoryInfo dInfo = FileHelper.GetOpenDirectoryInfo();

                string tempDirPath = Path.Combine(this.Material.DirectoryPath, "Temp");
                if (!Directory.Exists(tempDirPath)) Directory.CreateDirectory(tempDirPath);

                foreach (FileInfo file in dInfo.GetFiles())
                {
                    string targetFilePath = Path.Combine(tempDirPath, file.Name);
                    file.CopyTo(targetFilePath, true);
                }

                string msg = "이미지 파일 입력 완료하였습니다.";
                MessageHelper.ShowMessage("이미지 입력", msg);
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void btn_ShowPreview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.PreviewWindow == null)
                {
                    this.PreviewWindow = new wndSlidePreview(this);
                    this.PreviewWindow.Material = this.Material;
                    this.PreviewWindow.Show();
                }

                this.PreviewWindow.Activate();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }

        private void parent_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.PreviewWindow != null)
                {
                    this.PreviewWindow.Close();
                    this.PreviewWindow = null;
                }
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
        }
    }
}
