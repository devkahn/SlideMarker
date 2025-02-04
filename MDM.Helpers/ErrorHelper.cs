using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MDM.Commons;

namespace MDM.Helpers
{
    public static class ErrorHelper
    {
        public static void ShowError(this Exception ee, bool isNotice = true)
        {
            SaveLog(ee);
            
            string caption = "알 수 없는 오류 발생";

            string eMsg = string.Empty;
            eMsg += "알 수 없는 오류가 발생하였습니다.";
            eMsg += "\n";
            eMsg += "===================================";
            eMsg += "\n";
            //eMsg += "Error Message : ";
            //eMsg += "\n";
            //eMsg += ee.Message;

            string yesMsg = "오류 폴더를 확인";
            string noMsg = "무시하기";


            if (!isNotice) return;
                    
            DialogResult result = MessageHelper.ShowYewNoMessage(caption, eMsg, yesMsg, noMsg);
            if (result == DialogResult.No) return;

            Process.Start(Defines.PATH_DIRECOTRY_ERROR_LOG);
        }


        private static void SaveLog(Exception ee)
        {
            string datetime = DateTime.Now.ToString("yyyy-MM-dd HHmmss");

            string log = string.Empty;
            log += string.Format("========== {0} ::: [Start Log] ::: ====================", datetime);
            log += "\n\n\n";
            log += "Message :";
            log += "\n";
            log += ee.Message;
            log += "\n\n";
            log += "Stack :";
            log += "\n";
            log += ee.StackTrace;
            log += "\n\n\n";
            log += string.Format("========== {0} ::: [End Log] ::: ====================", datetime);


            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string todayPath = Path.Combine(Defines.PATH_DIRECOTRY_ERROR_LOG, date);
            if(!Directory.Exists(todayPath))  Directory.CreateDirectory(todayPath);

            string filePath = Path.Combine(todayPath, datetime+".log");
            if (!File.Exists(filePath)) File.Create(filePath).Close();

            File.WriteAllText(filePath, log);
        }
    }
}
