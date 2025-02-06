using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManualDataManager.Views.Windows;
using MDM.Models.DataModels;
using Microsoft.Office.Interop.PowerPoint;

namespace ManualDataManager.Commons
{
    public static partial class ProgramValues
    {
        private static frm_DataLabelingBase _DataLabelingWindow = null;
        
    }
    public static partial class ProgramValues
    {
        public static Application PowerPointApp { get; set; } = null;

        public static frm_DataLabelingBase DataLabelingWindow
        {
            get => _DataLabelingWindow;
            set
            {
                _DataLabelingWindow = value;
                if (_DataLabelingWindow != null) _DataLabelingWindow.Show();
            }
        }


        
    }
}
