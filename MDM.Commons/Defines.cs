using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media;

namespace MDM.Commons
{
    public static class Defines
    {

        public static string NAME_DIRECTORY_PROGRAM = "SlideMarker";
        public static string PATH_DIRECOTRY_DLENC = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DLENC");
        public static string PATH_DIRECOTRY_PROGRMA = Path.Combine(PATH_DIRECOTRY_DLENC, NAME_DIRECTORY_PROGRAM);
        public static string PATH_DIRECOTRY_ERROR_LOG = Path.Combine(PATH_DIRECOTRY_PROGRMA, "ERROR LOG");
        public static string PATH_FILE_BASE_DB = Path.Combine("Resources", "Database" + "\\BaseDB.db");


        public static string EXTENSION_IMAGE = ".png";

        public static List<Brush> LIST_INDENT_COLOR = new List<Brush>()
        {
            Brushes.Yellow,
            Brushes.Green,
            Brushes.Magenta,
            Brushes.Blue,
            Brushes.Orange,
            Brushes.LimeGreen,
            Brushes.Red,
            Brushes.SkyBlue,
            Brushes.Purple,
            Brushes.Pink,
        };

        public static List<char> LIST_MARKER = new List<char>()
        {
            '-',
            '>',
            '*',
            '▣',
            '▷'
        };
    }
}
