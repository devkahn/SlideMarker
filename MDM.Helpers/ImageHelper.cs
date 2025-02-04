using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MDM.Helpers
{
    public static class ImageHelper
    {

        public static Bitmap BitmapSourceToBitmap(BitmapSource bitmapSource)
        {
            // BitmapSource를 Bitmap으로 변환하는 과정
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // BitmapSource를 Png 형식으로 메모리 스트림에 저장
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(memoryStream);

                // 메모리 스트림에서 Bitmap 객체로 변환
                memoryStream.Seek(0, SeekOrigin.Begin);
                return new Bitmap(memoryStream);
            }
        }
    }
}
