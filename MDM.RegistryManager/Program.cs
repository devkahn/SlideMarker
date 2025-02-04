using Microsoft.Win32;
using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace MDM.RegistryManager
{
    public class Program
    {
        const string Description = "Description";
        const string FriendlyName = "FriendlyName";
        const string LoadBehavior = "LoadBeHavior";
        const string Manifest = "Manifest";
        const string Comapany = "DL E&C";
        const string Developer = "devKahn";
        const string name = "ManualDataManager";
        static string[] SubKeys = { "Software", "Microsoft", "Office", "Powerpoint", "Addins" };

        static void Main(string[] args)
        {
            string dllPath = System.Environment.CurrentDirectory;


            try
            {
                RegistryKey reg = Registry.CurrentUser;
                foreach (string key in SubKeys)
                {
                    if (reg.GetSubKeyNames().Contains(key))
                    {
                        reg = reg.OpenSubKey(key, true);
                    }
                    else
                    {
                        reg = reg.CreateSubKey(key, true);
                    }
                }

                if (args[0] == "install")
                {
                    if (reg.GetSubKeyNames().Contains(name)) reg.DeleteSubKey(name);
                    RegistryKey esKey = reg.CreateSubKey(name);
                    esKey.SetValue(Description, name);
                    esKey.SetValue(FriendlyName, name);
                    esKey.SetValue(LoadBehavior, 3);
                    esKey.SetValue(nameof(Comapany), Comapany);
                    esKey.SetValue(nameof(Developer), Developer);
                    string filePath = @"file:///" + dllPath + "\\" + name + ".vsto|vstolocal";
                    esKey.SetValue(Manifest, filePath);

                    ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                    fileMap.ExeConfigFilename = dllPath + "\\MDM.config";

                    Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                    esKey.SetValue("Version", config.AppSettings.Settings["AppVersion"].Value);

                    

                    
                }
                else if (args[0] == "uninstall")
                {
                    if (reg.GetSubKeyNames().Contains(name)) reg.DeleteSubKey(name);
                }
            }
            catch (Exception ee)
            {
                string message = ee.Message + "\n\n\n" + ee.StackTrace;

                string logPath = dllPath + "\\ResgistryManager_Log\\";
                if (!Directory.Exists(logPath)) Directory.CreateDirectory(logPath);

                string filePath = logPath + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, message);
                }
                else
                {
                    StreamWriter sw = File.AppendText(filePath);
                    sw.WriteLine(message);
                }
            }
        }
    }
}
