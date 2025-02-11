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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Office.Interop.Excel;

namespace SheetConverter.Views.Pages
{
    /// <summary>
    /// ucDataToDbPanelMain.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ucDataToDbPanelMain : UserControl
    {
        public ucDataToDbPanelMain()
        {
            InitializeComponent();
            this.mainpage.AreaSelectButton.Click += btn_AreaSelect_Click;
            this.mainpage.SelectJsonPathButton.Click += btn_SelectJsonSaveDirectory_Click;
        }


        private void btn_AreaSelect_Click(object sender, RoutedEventArgs e)
        {
            Range rng = (Range)Globals.ThisAddIn.Application.Selection;
            this.mainpage.SelectRange = rng;
        }
        private void btn_SelectJsonSaveDirectory_Click(object sender, RoutedEventArgs e)
        {
            string path = Globals.ThisAddIn.Application.ActiveWorkbook.FullName;
            FileInfo fInfo = new FileInfo(path);
            this.mainpage.ExcelPath = fInfo;    
        }
    }
}
