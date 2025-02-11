using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MDM.Helpers;
using MDM.Models.DataModels;
using Microsoft.Office.Interop.Excel;
using static OfficeOpenXml.ExcelErrorValue;
using Button = System.Windows.Controls.Button;
using Path = System.IO.Path;
using UserControl = System.Windows.Controls.UserControl;

namespace MDM.Views.ExcelToDatabase.Pages
{
    /// <summary>
    /// ucExcelToDatabaseMain.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucExcelToDatabaseMain : UserControl
    {
        public Button AreaSelectButton => this.btn_AreaSelect;
        public Button SelectJsonPathButton => this.btn_SelectJSonPath;
        Range _SelectRange = null;
        public Range SelectRange
        {
            get => _SelectRange;
            set
            {
                _SelectRange = value;
                this.txtbox_Area_Textbox.Text = value == null ? string.Empty : value.Address;
            }
        }

        FileInfo _ExcelPath = null;
        public FileInfo ExcelPath
        {
            get => _ExcelPath;
            set
            {
                _ExcelPath = value;
                if(value == null)
                {
                    this.JsonDirectory = null;
                }
                else
                {
                    this.JsonDirectory = value.Directory;
                }
            }
        }

        DirectoryInfo _JsonDirectory = null;
        public DirectoryInfo JsonDirectory
        {
            get => _JsonDirectory;
            set
            {
                _JsonDirectory = value;
                this.txtbox_JsonPath_Textbox.Text = value == null ? string.Empty : value.FullName;
            }
        }




        public ucExcelToDatabaseMain()
        {
            InitializeComponent();
        }

        public DirectoryInfo GetDirectoryInfo()
        {
            DirectoryInfo output = null;

            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            DialogResult result = folderDialog.ShowDialog();

            // 사용자가 폴더를 선택한 경우
            if (result == DialogResult.OK)
            {
                string selectedFolderPath = folderDialog.SelectedPath;
                if (Directory.Exists(selectedFolderPath))
                {
                    output = new DirectoryInfo(selectedFolderPath);
                }
            }


            return output;
        }


        private void btn_AreaClear_Click(object sender, RoutedEventArgs e)
        {
            this.SelectRange = null;
        }

        private void btn_ConvertToJson_Click(object sender, RoutedEventArgs e)
        {

            int rowCnt = this.SelectRange.Rows.Count;
            int colCnt = this.SelectRange.Columns.Count;

            List<mContent> contents = new List<mContent>();
            for (int r = 1; r <= rowCnt; r++)
            {
                mContent newContents = new mContent();

                var value1 = (this.SelectRange[r, 1] as Range).Value2;
                newContents.SlideIdx = int.TryParse(value1.ToString(), out int slideNum) ? slideNum : -1;

                var level1Value = (this.SelectRange[r, 2] as Range).Value2;
                newContents.Heading1String = level1Value == null ? string.Empty : level1Value.ToString();
                var level2Value = (this.SelectRange[r, 3] as Range).Value2;
                newContents.Heading2String = level2Value == null ? string.Empty : level2Value.ToString();
                var level3Value = (this.SelectRange[r, 4] as Range).Value2;
                newContents.Heading3String = level3Value == null ? string.Empty : level3Value.ToString();
                var level4Value = (this.SelectRange[r, 5] as Range).Value2;
                newContents.Heading4String = level4Value == null ? string.Empty : level4Value.ToString();
                var level5Value = (this.SelectRange[r, 6] as Range).Value2;
                newContents.Heading5String = level5Value == null ? string.Empty : level5Value.ToString();
                var level6Value = (this.SelectRange[r, 7] as Range).Value2;
                newContents.Heading6String = level6Value == null ? string.Empty : level6Value.ToString();
                var level7Value = (this.SelectRange[r, 8] as Range).Value2;
                newContents.Heading7String = level7Value == null ? string.Empty : level7Value.ToString();
                var level8Value = (this.SelectRange[r, 9] as Range).Value2;
                newContents.Heading8String = level8Value == null ? string.Empty : level8Value.ToString();
                var level9Value = (this.SelectRange[r, 10] as Range).Value2;
                newContents.Heading9String = level9Value == null ? string.Empty : level9Value.ToString();
                var level10Value = (this.SelectRange[r, 11] as Range).Value2;
                newContents.Heading10String = level10Value == null ? string.Empty : level10Value.ToString();

                var contentTypeValue = (this.SelectRange[r, 12] as Range).Value2;
                newContents.ContentsType = contentTypeValue == null ? -1 : ConvertContentsTypeStringToCode(contentTypeValue.ToString());
                var contentValue = (this.SelectRange[r, 13] as Range).Value2;
                newContents.Contents = contentValue == null ? string.Empty : contentValue.ToString();

                var descriptionValue = (this.SelectRange[r, 14] as Range).Value2;
                newContents.Description = descriptionValue == null ? string.Empty : descriptionValue.ToString();
                var messageValue = (this.SelectRange[r, 15] as Range).Value2;
                newContents.Message = messageValue == null ? string.Empty : messageValue.ToString();


                contents.Add(newContents);
            }


            string jsonString = JsonHelper.ToJsonString(contents);
            string targetPath = Path.Combine(this.JsonDirectory.FullName, Path.GetFileNameWithoutExtension(this.ExcelPath.Name)+ ".json");
            if(File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }
            File.WriteAllText(targetPath, jsonString);

            this.txtblock_JsonStatus.Text = "Json 파일이 생성되었습니다.";
        }

        private int ConvertContentsTypeStringToCode(string value)
        {
            switch (value.Trim())
            {
                case "글":
                    return 221;
                case "이미지":
                case "그림":
                    return 222;
                case "표":
                    return 223;
                default: 
                    return -1;
             
            }
        }

        private void btn_SelectJsonSaveDirectory_Click(object sender, RoutedEventArgs e) => this.JsonDirectory = GetDirectoryInfo();

    }

  
}