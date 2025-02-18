using System;
using System.Diagnostics;
using ManualDataManager.Commons;
using MDM.Helpers;

namespace ManualDataManager
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            try
            {
                FileHelper.InitailizeProgramDirectory();
                ProgramValues.PowerPointApp = this.Application;
            }
            catch (Exception ee)
            {
                ErrorHelper.ShowError(ee);
            }
            
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
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
            ProgramValues.PowerPointApp = null;
        }

        #region VSTO에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
