using AdbGame.ViewModel.Page;
using Microsoft.Extensions.DependencyInjection;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace AdbGame.View.Page
{
    /// <summary>
    /// ScreenShotView.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenShotView
    {
        ScreenShotViewModel screenShotViewModel = App.Current._host.Services.GetRequiredService<ScreenShotViewModel>();
        public ScreenShotView()
        {
            InitializeComponent();
            this.DataContext = screenShotViewModel;
        }

        private void ImageControl_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(imageControl); // 获取鼠标在 Image 控件上的坐标

            // 计算鼠标在 Bitmap 上的像素坐标
            double scaleX = screenShotViewModel.SrcImageSource.PixelWidth / imageControl.ActualWidth;
            double scaleY = screenShotViewModel.SrcImageSource.PixelHeight / imageControl.ActualHeight;

            int bitmapX = (int)(position.X * scaleX);
            int bitmapY = (int)(position.Y * scaleY);

            screenShotViewModel.X = bitmapX;
            screenShotViewModel.Y = bitmapY;

            screenShotViewModel.ShowTemplate();
        }
    }
}
