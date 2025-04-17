using MarkChecker.Defines;
using MarkChecker_Visual_Module;
using Microsoft.Office.Tools.Ribbon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkChecker
{
    public partial class ribbon_dlenc
    {
        private void ribbon_dlenc_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.MainPage.PowerPointApp = ProgramValues.PowerPointApp;
            main.Show();
        }
    }
}
