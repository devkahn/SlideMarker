using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManualDataManager.Commons
{
    public static class Defines
    {
        public static string NAME_DIRECTORY_PROGRAM = "AI_DATA_MANAGER";
        public static string PATH_DIRECOTRY_PROGRMA = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), NAME_DIRECTORY_PROGRAM);
        public static string PATH_DIRECOTRY_ERROR_LOG = Path.Combine(PATH_DIRECOTRY_PROGRMA, "ERROR LOG");
        
    }
}
