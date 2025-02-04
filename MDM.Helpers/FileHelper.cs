using System.IO;
using Microsoft.Win32;
using MDM.Commons;
using Microsoft.WindowsAPICodePack.Dialogs;



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

        public static FileInfo GetOpenFileInfo()
        {
            FileInfo output = null;

            CommonOpenFileDialog ofd = new CommonOpenFileDialog();
            ofd.RestoreDirectory = true;
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
