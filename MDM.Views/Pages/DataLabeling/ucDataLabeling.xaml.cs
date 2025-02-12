﻿using System;
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
using Microsoft.Office.Interop.PowerPoint;

namespace MDM.Views.Pages.DataLabeling
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

        private void btn_FileOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileInfo fInfo = FileHelper.GetOpenFileInfo();
                if (fInfo == null) return;

                string textValue = FileHelper.ReadTextFile(fInfo);
                if (string.IsNullOrEmpty(textValue)) return;

                string onlyFilename = Path.GetFileNameWithoutExtension(fInfo.Name);
                string subDirPath = Path.Combine(fInfo.DirectoryName, onlyFilename);
                if (!Directory.Exists(subDirPath)) Directory.CreateDirectory(subDirPath);

                string assDirPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string dbOriginPath = Path.Combine(assDirPath, "Resources\\Database", "BaseDB.db");
                string dbTargetPath = Path.Combine(subDirPath, onlyFilename + ".mdm");
                if (!File.Exists(dbTargetPath)) File.Copy(dbOriginPath, dbTargetPath, true);
                DBHelper.ConnectionSting = dbTargetPath;

                mMaterial newMaterial = DBHelper.Read();
                if(newMaterial == null)
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

                List<mSlide> slides = JsonHelper.ToObject<List<mSlide>>(textValue);
                foreach (mSlide slide in slides)
                {
                    vmSlide sameSlide = material.Slides.Where(x => x.Temp.Index == slide.Index).FirstOrDefault();
                    if(sameSlide != null)
                    {
                        sameSlide.OnModifyStatusChanged(false);
                        continue;
                    }
                    

                    foreach (mShape shape in slide.Shapes)
                    {
                        string[] lines = TextHelper.SplitText(shape.Text);
                        foreach (string line in lines)
                        {
                            if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line)) continue;

                            mItem newLine = new mItem();
                            newLine.LineText = line;
                            shape.Lines.Add(newLine);
                        }

                    }

                    vmSlide newSlide = new vmSlide(slide);
                    newSlide.SetParent(material);
                }
                material.CurrentSlide = material.Slides.FirstOrDefault();
                material.OrderSlides();

                this.Material = material;
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

        private void LoadButton_Click(object sender, RoutedEventArgs e)
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
    }
}
