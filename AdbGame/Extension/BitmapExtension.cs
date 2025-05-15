using OpenCvSharp;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace AdbGame.Extension
{
    public static class BitmapExtension
    {
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            var ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            var image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        public static byte[] BitmapToByteArray(this Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }


        public static Scalar ToScalar(this Color color)
        {
            return new Scalar(color.R, color.G, color.B);
        }
    }
}
