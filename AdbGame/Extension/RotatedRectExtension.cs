using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AdbGame.Extension
{
    public static class RotatedRectExtension
    {
        public static Rect RotatedRectToRect(this RotatedRect rotatedRect)
        {
            int x = (int)rotatedRect.Center.X;
            int y = (int)rotatedRect.Center.Y;
            int width = (int)rotatedRect.Size.Height;
            int height = (int)rotatedRect.Size.Width;
            x = x - width / 2;
            y = y - height / 2;
            return new Rect(x, y, width, height);
        }
    }
}
