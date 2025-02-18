using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManualDataManager.Commons;
using ManualDataManager.Views.Pages;

namespace ManualDataManager.Views.Windows
{
    public partial class frm_DataLabelingBase : Form
    {
        public frm_DataLabelingBase()
        {
            InitializeComponent();
            this.elementHost1.Child = new ucDataLabeling(this);
        }

        private void frm_DataLabeling_Closing(object sender, FormClosingEventArgs e)
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                try
                {
                    if (process.MainModule.FileName.Contains("SQL.interop.dll"))
                    {
                        //Console.WriteLine($"Killing process: {process.ProcessName}");
                        process.Kill();
                    }
                }
                catch (Exception ex)
                {

                }
            }


            ProgramValues.DataLabelingWindow = null;
        }
    }
}
