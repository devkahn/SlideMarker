using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MDM.Helpers;
using MDM.Models.DataModels;
using MDM.Models.ViewModels;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace MDM_Visual_Module.Views.Windows
{
    /// <summary>
    /// wndMainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class wndMainWindow : Window
    {
        public wndMainWindow()
        {
            InitializeComponent();
            string[] SubKeys = { "SOFTWARE", "Microsoft", "Office", "Powerpoint", "Addins", "ManualDataManager" };
            RegistryKey reg = Registry.CurrentUser;
            foreach (string key in SubKeys) reg = reg.GetSubKeyNames().Contains(key) ? reg.OpenSubKey(key, true) : reg.CreateSubKey(key, true);
            if (reg != null && reg.GetValueNames().Contains("Version")) this.datalabeling.Version.Text = reg.GetValue("Version").ToString() + "T";

        }
        
    }
}
