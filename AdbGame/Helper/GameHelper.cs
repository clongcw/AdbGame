using OpenCvSharp;
using OpenCvSharp.Extensions;
using Sdcb.PaddleOCR.Models.Local;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR;
using System.IO;
using Mat = OpenCvSharp.Mat;
using Point = System.Drawing.Point;
using Size = OpenCvSharp.Size;
using AdbGame.Extension;
using Sdcb.PaddleInference;

namespace AdbGame.Helper
{
    public class GameHelper
    {
        #region 正态分布
        Random random = new Random();

        // Box-Muller变换生成正态分布随机数
        public double GenerateGaussianRandom(double mean, double stdDev)
        {
            double u1 = random.NextDouble(); // Uniform(0,1)随机数
            double u2 = random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); // 标准正态分布
            return mean + stdDev * randStdNormal; // 调整为指定均值和标准差
        }

        // 在指定Rect区域生成随机点
        public Point GenerateRandomPoint(Rect rect)
        {
            double centerX = rect.X + rect.Width / 2.0; // 矩形中心X
            double centerY = rect.Y + rect.Height / 2.0; // 矩形中心Y
            double stdDevX = rect.Width / 6.0; // X方向标准差（范围可调整）
            double stdDevY = rect.Height / 6.0; // Y方向标准差（范围可调整）

            // 生成符合正态分布的坐标
            int randomX = Math.Clamp((int)GenerateGaussianRandom(centerX, stdDevX), rect.X, rect.X + rect.Width - 1);
            int randomY = Math.Clamp((int)GenerateGaussianRandom(centerY, stdDevY), rect.Y, rect.Y + rect.Height - 1);

            return new Point(randomX, randomY);
        }
        #endregion

        public Rect OCR(byte[] imageData, string dstText)
        {
            Rect rotatedRect = default;
            //FullOcrModel model = LocalFullModels.ChineseV4;

            try
            {
                //using (PaddleOcrAll all = new(model)
                //{
                //    AllowRotateDetection = true,
                //    Enable180Classification = false,
                //})
                {
                    using (PaddleOcrAll all = new PaddleOcrAll(LocalFullModels.ChineseV4, PaddleDevice.Mkldnn()))
                    using (Mat src = Cv2.ImDecode(imageData, ImreadModes.Color))
                    {
                        PaddleOcrResult result = all.Run(src);
                        foreach (PaddleOcrResultRegion region in result.Regions)
                        {
                            if (region.Text.Contains(dstText))
                            {
                                rotatedRect = region.Rect.RotatedRectToRect();
                                all.Dispose();
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return rotatedRect;
        }

        public Rect MatchTemplate(Bitmap srcBitmap, Mat dstMat)
        {
            if (srcBitmap.Width < srcBitmap.Height)
            {
                Rect rect = new Rect(0, 0, srcBitmap.Width, srcBitmap.Height);
                using (Mat srcMat = new(BitmapConverter.ToMat(srcBitmap), rect))
                {
                    // 调用核心匹配逻辑
                    return MatchTemplate(srcMat, dstMat, TemplateMatchModes.CCoeffNormed);
                }
            }
            else
            {
                // 使用 using 自动释放 srcMat
                using (Mat srcMat = BitmapConverter.ToMat(srcBitmap))
                {
                    // 颜色转换（原地操作，无需新建 Mat）
                    Cv2.CvtColor(srcMat, srcMat, ColorConversionCodes.BGRA2BGR);

                    // 调用核心匹配逻辑
                    return MatchTemplate(srcMat, dstMat, TemplateMatchModes.CCoeffNormed);
                }
            }
        }

        /// <summary>
        /// 模板匹配
        /// </summary>
        /// <param name="srcMat">原图像</param>
        /// <param name="dstMat">模板</param>
        /// <param name="matchMode">匹配方式</param>
        /// <param name="maskMat">遮罩</param>
        /// <param name="threshold">阈值</param>
        /// <returns>匹配区域的矩形范围，未匹配时返回空矩形</returns>
        public Rect MatchTemplate(Mat srcMat, Mat dstMat, TemplateMatchModes matchMode, Mat? maskMat = null, double threshold = 0.8)
        {
            Cv2.CvtColor(srcMat, srcMat, ColorConversionCodes.BGR2GRAY);
            Cv2.CvtColor(dstMat, dstMat, ColorConversionCodes.BGR2GRAY);

            try
            {
                using var result = new Mat();
                Cv2.MatchTemplate(srcMat, dstMat, result, matchMode);

                if (matchMode is TemplateMatchModes.SqDiff or TemplateMatchModes.CCoeff or TemplateMatchModes.CCorr)
                {
                    Cv2.Normalize(result, result, 0, 1, NormTypes.MinMax);
                }

                Cv2.MinMaxLoc(result, out var minValue, out var maxValue, out var minLoc, out var maxLoc);

                // 获取模板尺寸
                var templateSize = new Size(dstMat.Width, dstMat.Height);

                if (matchMode is TemplateMatchModes.SqDiff or TemplateMatchModes.SqDiffNormed)
                {
                    if (minValue <= 1 - threshold)
                    {
                        return new Rect(minLoc, templateSize); // 返回左上角坐标 + 模板尺寸
                    }
                }
                else
                {
                    if (maxValue >= threshold)
                    {
                        return new Rect(maxLoc, templateSize); // 返回左上角坐标 + 模板尺寸
                    }
                }

                return default; // 未匹配返回空矩形
            }
            catch (Exception ex)
            {
                return default;
            }
        }


        public Mat LoadAssetImage(string filePath, ImreadModes flags = ImreadModes.Color)
        {
            try
            {
                var mat = Mat.FromStream(File.OpenRead(filePath), flags);


                return mat;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }


        #region 刷新消息相关
        public void ExecuteFunBeginInvoke(Action action)
        {
            Action action2 = action;
            if (IsUIThread())
            {
                action2();
                return;
            }

            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                action2();
            });
        }

        public bool IsUIThread()
        {
            int managedThreadId = System.Windows.Application.Current.Dispatcher.Thread.ManagedThreadId;
            int currentManagedThreadId = Environment.CurrentManagedThreadId;
            if (managedThreadId.Equals(currentManagedThreadId))
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}
