using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AdbGame.Extension
{
    public static class BitmapSourceExtension
    {
        public static byte[] ConvertBitmapSource2Bytes(this BitmapSource bitmapSource, BitmapEncoder encoder)
        {
            // 将BitmapSource转换为byte[]
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

            using var stream = new MemoryStream();
            encoder.Save(stream);
            return stream.ToArray();
        }
    }
}
