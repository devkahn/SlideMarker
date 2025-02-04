using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.PowerPoint;

namespace ManualDataManager.Helpers
{
    public class PowperPointHelper
    {
        public static void CropImage(Shape shape)
        {
            // 크롭 속성 가져오기
            float cropLeft = shape.PictureFormat.CropLeft;
            float cropTop = shape.PictureFormat.CropTop;
            float cropRight = shape.PictureFormat.CropRight;
            float cropBottom = shape.PictureFormat.CropBottom;

            // 이미지의 원본 크기 가져오기
            float originalWidth = shape.Width;
            float originalHeight = shape.Height;

            // 크롭된 이미지의 크기 계산
            float croppedWidth = originalWidth - cropLeft - cropRight;
            float croppedHeight = originalHeight - cropTop - cropBottom;

            // 크롭된 영역만 잘라내기
            Rectangle cropArea = new Rectangle((int)cropLeft,(int)cropTop, (int)croppedWidth, (int)croppedHeight);

        }

        public static void ExtractAndResizeCroppedImage(Shape shape, string outputPath)
        {
            // 크롭 속성 가져오기
            float cropLeft = shape.PictureFormat.CropLeft;
            float cropTop = shape.PictureFormat.CropTop;
            float cropRight = shape.PictureFormat.CropRight;
            float cropBottom = shape.PictureFormat.CropBottom;

            // 이미지의 원본 크기 가져오기
            float originalWidth = shape.Width;
            float originalHeight = shape.Height;

            // 크롭된 이미지의 크기 계산
            float croppedWidth = originalWidth - cropLeft - cropRight;
            float croppedHeight = originalHeight - cropTop - cropBottom;

            // 원본 이미지를 Bitmap으로 가져오기
            Bitmap originalImage = new Bitmap(shape.LinkFormat.SourceFullName); // 이미지 경로가 있는 경우

            // 크롭된 영역만 잘라내기
            Rectangle cropArea = new Rectangle((int)cropLeft, (int)cropTop, (int)croppedWidth, (int)croppedHeight);

            Bitmap croppedImage = originalImage.Clone(cropArea, originalImage.PixelFormat);

            // 크롭된 이미지의 비율을 유지하면서 가로 크기를 1080px로 조정
            int newWidth = 1080;
            int newHeight = (int)((croppedHeight / croppedWidth) * newWidth);

            Bitmap resizedImage = new Bitmap(croppedImage, new Size(newWidth, newHeight));

            // 크롭된 리사이즈된 이미지를 파일로 저장
            resizedImage.Save(outputPath, System.Drawing.Imaging.ImageFormat.Jpeg);

            // 리소스 해제
            originalImage.Dispose();
            croppedImage.Dispose();
            resizedImage.Dispose();

        }
    }
}
