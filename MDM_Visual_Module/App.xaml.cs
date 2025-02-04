using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MDM_Visual_Module.Views.Windows;
using Microsoft.Win32;

namespace MDM_Visual_Module
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            wndMainWindow wndMain = new wndMainWindow();
            wndMain.Show();
        }
    }
}
