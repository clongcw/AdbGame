using AdbGame.Extension;
using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Panuon.WPF.UI;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using MessageBoxIcon = Panuon.WPF.UI.MessageBoxIcon;

namespace AdbGame.ViewModel.Page
{
    public partial class ScreenShotViewModel : ObservableObject
    {
        [ObservableProperty] private int _serial;
        [ObservableProperty] private AdbClient _adb;
        [ObservableProperty] private DeviceData _adbdevice;
        [ObservableProperty] private BitmapSource _srcImageSource;
        [ObservableProperty] private BitmapSource _dstImageSource;
        [ObservableProperty] private Bitmap _srcImage;
        [ObservableProperty] private Bitmap _dstImage;
        [ObservableProperty] private string _templateLocation;
        [ObservableProperty] private string _templateName;
        [ObservableProperty] private int _x;
        [ObservableProperty] private int _y;
        [ObservableProperty] private int _width;
        [ObservableProperty] private int _height;
        [ObservableProperty] private string _aDBPath;

        public ScreenShotViewModel()
        {
            X = 500;
            Y = 500;
            Width = 50;
            Height = 50;
            Serial = 17056;
            ADBPath = App.Current._host.Services.GetRequiredService<SettingsViewModel>().ADBPath;
        }

        [RelayCommand]
        public void Connect()
        {
            try
            {
                AdbServer adbServer = new AdbServer();
            ConnectAdb:
                AdbServerStatus adbServerStatus = adbServer.GetStatus();
                if (adbServerStatus.IsRunning)
                {
                    Adb = new AdbClient();
                    var adb_devices = Adb.GetDevices();
                    Adbdevice = adb_devices.Where(s => s.Serial == $"127.0.0.1:{Serial}").FirstOrDefault();
                    if (Adbdevice.Serial == null || Adbdevice.State != DeviceState.Online)
                    {
                        var result = adbServer.StartServer(ADBPath);
                        string res = Adb.Connect("127.0.0.1", Serial);
                        if (res.Contains("connected") && Adbdevice.State == DeviceState.Online)
                        {
                            App.Current.Dispatcher.InvokeAsync(async () =>
                            {
                                MessageBoxX.Show("连接成功！", "提示", MessageBoxButton.OK, MessageBoxIcon.Success, DefaultButton.YesOK);
                            });
                        }
                        else
                        {
                            App.Current.Dispatcher.InvokeAsync(async () =>
                            {
                                MessageBoxX.Show("连接失败！", "提示", MessageBoxButton.OK, MessageBoxIcon.Error, DefaultButton.YesOK);
                            });
                        }
                    }
                    else
                    {
                        string res = Adb.Connect("127.0.0.1", Serial);
                        App.Current.Dispatcher.InvokeAsync(async () =>
                        {
                            MessageBoxX.Show("连接成功！", "提示", MessageBoxButton.OK, MessageBoxIcon.Success, DefaultButton.YesOK);
                        });
                    }
                }
                else
                {
                    var result = adbServer.StartServer(ADBPath);
                    goto ConnectAdb;
                }
            }
            catch (Exception ex)
            {
                App.Current.Dispatcher.InvokeAsync(async () =>
                {
                    MessageBoxX.Show($"连接失败！{ex.Message}", "提示", MessageBoxButton.OK, MessageBoxIcon.Error, DefaultButton.YesOK);
                });
            }
        }

        [RelayCommand]
        public void ScreenShot()
        {
            try
            {
                Framebuffer framebuffer = Adb.GetFrameBuffer(Adbdevice);
                SrcImage = framebuffer.ToImage();
                SrcImageSource = SrcImage.ToBitmapImage();
            }
            catch (Exception ex)
            {
                App.Current.Dispatcher.InvokeAsync(async () =>
                {
                    MessageBoxX.Show($"{ex}", "提示", MessageBoxButton.OK, MessageBoxIcon.Error, DefaultButton.YesOK);
                });
            }
        }

        [RelayCommand]
        public void SelectLocation()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            // 显示选择文件夹对话框并获取结果
            DialogResult result = folderBrowserDialog.ShowDialog();

            // 处理结果
            if (result == DialogResult.OK)
            {
                // 用户选择了文件夹
                TemplateLocation = $"{folderBrowserDialog.SelectedPath}";
            }
        }

        [RelayCommand]
        public void SaveTemplate()
        {
            if (!string.IsNullOrEmpty(TemplateLocation) && !string.IsNullOrEmpty(TemplateName))
            {
                try
                {
                    // 指定要截取的区域
                    Rectangle cropArea = new Rectangle(X, Y, Width, Height);
                    DstImage = SrcImage.Clone(cropArea, SrcImage.PixelFormat);
                    DstImageSource = DstImage.ToBitmapImage();

                    string file = $"{TemplateLocation}\\{TemplateName}.png";
                    if (!File.Exists(file))
                    {
                        DstImage.Save(file);
                    }
                    else
                    {
                        App.Current.Dispatcher.InvokeAsync(async () =>
                        {
                            var result = MessageBoxX.Show($"{TemplateName}已存在，是否覆盖", "提示", MessageBoxButton.OKCancel, MessageBoxIcon.Question, DefaultButton.YesOK);
                            if (result == MessageBoxResult.OK)
                            {
                                DstImage.Save(file);
                            }
                        });
                    }
                }
                catch (Exception ex)
                {
                    App.Current.Dispatcher.InvokeAsync(async () =>
                    {
                        MessageBoxX.Show($"{ex.Message}", "提示", MessageBoxButton.OK, MessageBoxIcon.Error, DefaultButton.YesOK);
                    });
                }
            }
            else
            {
                App.Current.Dispatcher.InvokeAsync(async () =>
                {
                    MessageBoxX.Show($"模板为空", "提示", MessageBoxButton.OK, MessageBoxIcon.Error, DefaultButton.YesOK);
                });
            }
        }

        [RelayCommand]
        public void ShowTemplate()
        {
            try
            {
                // 指定要截取的区域
                Rectangle cropArea = new Rectangle(X, Y, Width, Height);
                DstImage = SrcImage.Clone(cropArea, SrcImage.PixelFormat);
                DstImageSource = DstImage.ToBitmapImage();
            }
            catch (Exception ex)
            {
                App.Current.Dispatcher.InvokeAsync(async () =>
                {
                    MessageBoxX.Show($"{ex.Message}", "提示", MessageBoxButton.OK, MessageBoxIcon.Error, DefaultButton.YesOK);
                });
            }
        }
    }
}
