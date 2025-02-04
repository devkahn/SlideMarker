using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDM.Helpers
{
    public static class MessageHelper
    {
        public static void ShowMessage(string caption, string message)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static void ShowErrorMessage(string caption, string message)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static DialogResult ShowYewNoMessage(string caption, string question, string yesMsg = "", string noMsg = "")
        {
            string msg = question;
            if(!TextHelper.IsNoText(yesMsg))
            {
                msg += "\n";
                msg += "예 : " + yesMsg;
            }
            if(!TextHelper.IsNoText(noMsg))
            {
                msg += "\n";
                msg += "아니오 : " + noMsg;
            }

            return MessageBox.Show(question, msg, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
        public static DialogResult ShowYewNoCancelMessage(string caption, string question, string yesMsg, string noMsg, string cancelMsg)
        {
            string msg = string.Empty;  
            msg += question;
            msg += "\n";
            msg += "\n";
            msg += "예(Yes) - " + yesMsg;
            msg += "\n";
            msg += "아니오(No) -" + noMsg;
            msg += "\n";
            msg += "취소(Cancel) - " + cancelMsg;

            return MessageBox.Show(msg, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }
    }
}
