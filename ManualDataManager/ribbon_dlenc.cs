using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManualDataManager.Commons;
using ManualDataManager.Views.Windows;
using MDM.Helpers;
using MDM.Models.DataModels;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Tools.Ribbon;

namespace ManualDataManager
{
    public partial class ribbon_dlenc
    {
        private void ribbon_dlenc_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void button_DataLabeling_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                FileHelper.InitailizeProgramDirectory();


                if (ProgramValues.DataLabelingWindow == null) ProgramValues.DataLabelingWindow = new frm_DataLabelingBase();
                ProgramValues.DataLabelingWindow.Activate();
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);  
            }
        }
    }
}
