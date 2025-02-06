using ManualDataManager.Commons;
using MDM.Commons.Enum;
using MDM.Models.DataModels;
using Microsoft.Office.Interop.PowerPoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Shapes;
using Path = System.IO.Path;
using Shape = Microsoft.Office.Interop.PowerPoint.Shape;

namespace ManualDataManager.Helpers
{
    public static class PowerpointHelper
    {
        public static List<Slide> GetSlidesFromCurrentFile()
        {
            List<Slide> output = new List<Slide>();

            foreach (Slide slide in ProgramValues.PowerPointApp.ActivePresentation.Slides)
            {
                output.Add(slide);
            }

            return output;
        }

        public static FileInfo GetFileInfoFromPresentation(Presentation presentation)
        {
            FileInfo output = null;

            string filepath = presentation.FullName;
            output = new FileInfo(filepath);
            return output;
        }
        public static DirectoryInfo GetDirectoryInfoFromPresenction(Presentation presentation)
        {
            DirectoryInfo output = null;

            string filePath = presentation.FullName;
            string directoryName = Path.GetDirectoryName(filePath);
            output = new DirectoryInfo(directoryName);

            return output;
        }
        public static string GetFileNameFromPresentation(Presentation presentation)
        {
            string output = string.Empty;

            string filePath = presentation.FullName;
            output = Path.GetFileNameWithoutExtension(filePath);

            return output;
        }

        public static void SetShapeBaseData(mShape shape, Shape origin)
        {
            shape.ShapeId = origin.Id;
            shape.Name = origin.Name;
            shape.Left = origin.Left;   
            shape.Top = origin.Top;
            shape.Width = origin.Width; 
            shape.Height = origin.Height;   
            shape.DistanceFromOrigin = Math.Sqrt(Math.Pow(shape.Left, 2) + Math.Pow(shape.Top, 2));
        }

        public static void DownloadImage(Shape origin, FileInfo fInfo)
        {
            var tempFile = Path.Combine(Path.GetTempPath(), "tempImage.png");

            origin.Copy();
            System.Windows.Forms.IDataObject dataObject = (System.Windows.Forms.IDataObject)Clipboard.GetDataObject();
            if (dataObject.GetDataPresent(System.Windows.Forms.DataFormats.Bitmap))
            {
                var bitmap = (System.Drawing.Bitmap)dataObject.GetData(System.Windows.Forms.DataFormats.Bitmap);
                bitmap.Save(tempFile);

                // 임시 파일을 지정된 경로로 이동
                File.Copy(tempFile, fInfo.FullName, true);
                File.Delete(tempFile);
            }
            Marshal.ReleaseComObject(origin);



            //// 크롭 속성 가져오기
            //float cropLeft = origin.PictureFormat.CropLeft;
            //float cropTop = origin.PictureFormat.CropTop;
            //float cropRight = origin.PictureFormat.CropRight;
            //float cropBottom = origin.PictureFormat.CropBottom;

            //// 이미지의 원본 크기 가져오기
            //float originalWidth = origin.Width;
            //float originalHeight = origin.Height;

            //// 크롭된 이미지의 크기 계산
            //float croppedWidth = originalWidth - cropLeft - cropRight;
            //float croppedHeight = originalHeight - cropTop - cropBottom;

            //// 크롭된 영역만 잘라내기
            //Rectangle cropArea = new Rectangle(
            //    (int)cropLeft,
            //    (int)cropTop,
            //    (int)croppedWidth,
            //    (int)croppedHeight
            //);
        }

    }
}
