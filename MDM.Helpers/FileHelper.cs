using System.IO;
using Microsoft.Win32;
using MDM.Commons;
using Microsoft.WindowsAPICodePack.Dialogs;
using MDM.Commons.Enum;
using System;

namespace MDM.Helpers
{
    public static class FileHelper
    {
        public static void InitailizeProgramDirectory()
        {
            if (!Directory.Exists(Defines.PATH_DIRECOTRY_DLENC)) Directory.CreateDirectory(Defines.PATH_DIRECOTRY_DLENC);
            if (!Directory.Exists(Defines.PATH_DIRECOTRY_PROGRMA)) Directory.CreateDirectory(Defines.PATH_DIRECOTRY_PROGRMA);
            if (!Directory.Exists(Defines.PATH_DIRECOTRY_ERROR_LOG)) Directory.CreateDirectory(Defines.PATH_DIRECOTRY_ERROR_LOG);
        }

        public static FileInfo GetOpenFileInfo(string caption = "파일 열기",eFILE_TYPE filter = eFILE_TYPE.None)
        {
            FileInfo output = null;

            CommonOpenFileDialog ofd = new CommonOpenFileDialog();
            ofd.RestoreDirectory = true;
            ofd.Title  = caption;
            ofd.EnsureValidNames = true;
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string basicName = EnumHelpers.GetDescription(eFILE_TYPE.None).Split('|')[0];
            string absicExtensions = EnumHelpers.GetDescription(eFILE_TYPE.None).Split('|')[1];
            ofd.Filters.Add(new CommonFileDialogFilter(basicName, absicExtensions));
            if(filter != eFILE_TYPE.None)
            {
                string name = EnumHelpers.GetDescription(filter).Split('|')[0];
                string extensions = EnumHelpers.GetDescription(filter).Split('|')[1];
                ofd.Filters.Add(new CommonFileDialogFilter(name, extensions));
            }

            
            if(ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                output = new FileInfo(ofd.FileName);
            }

            return output;
        }
        public static DirectoryInfo GetOpenDirectoryInfo()
        {
            DirectoryInfo output = null;

            CommonOpenFileDialog ofd = new CommonOpenFileDialog();
            ofd.IsFolderPicker  = true;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                output = new DirectoryInfo(ofd.FileName);
            }

            return output;
        }

        public static string ReadTextFile(FileInfo file)
        {
            string output = string.Empty;

            if(file.Exists)
            {
                output = File.ReadAllText(file.FullName);
            }

            return output;
        }
    }
}
