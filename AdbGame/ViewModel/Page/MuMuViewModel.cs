using AdbGame.Common;
using AdbGame.Extension;
using AdbGame.Helper;
using AdbGame.Model;
using AdbGame.ViewModel.Page;
using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.DeviceCommands;
using AdvancedSharpAdbClient.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OpenCvSharp;
using Panuon.WPF.UI;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Local;
using Sdcb.PaddleOCR.Models.LocalV4;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using MessageBoxIcon = Panuon.WPF.UI.MessageBoxIcon;
using Point = System.Drawing.Point;
using Rect = OpenCvSharp.Rect;

namespace AdbGame.ViewModel
{
    public partial class MuMuViewModel : ObservableObject
    {
        private readonly Serilog.ILogger _log;
        [ObservableProperty] private string _title;
        [ObservableProperty] private int _serial;
        [ObservableProperty] private AdbClient _adb;
        [ObservableProperty] private DeviceData _adbdevice;
        [ObservableProperty] private GameHelper _gamehelper;
        [ObservableProperty] private ObservableCollection<MessageData> _messages;
        [ObservableProperty] private ObservableCollection<StepModel> _steps;
        [ObservableProperty] private ObservableCollection<GameMission> _gamemissions;
        [ObservableProperty] private bool _isRunning = false;
        [ObservableProperty] private bool _useJson = false;
        [ObservableProperty] private string _aDBPath;
        [ObservableProperty] private string _imagePath;
        CancellationTokenSource cts;

        public MuMuViewModel(string title, int serial)
        {
            _log = GlobalSLog.GetLogger(title);
            Title = title;
            Serial = serial;
            Gamehelper = new GameHelper();
            Messages = new ObservableCollection<MessageData>();
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
                    Serial = App.Current._host.Services.GetRequiredService<SettingsViewModel>().Mumus
                                        .Where(s => s.SubGameName == Title)
                                        .FirstOrDefault().Serial;
                    Adbdevice = adb_devices.Where(s => s.Serial == $"127.0.0.1:{Serial}").FirstOrDefault();
                    if (Adbdevice.Serial == null || Adbdevice.State != DeviceState.Online)
                    {
                        var result = adbServer.StartServer(ADBPath);
                        string res = Adb.Connect("127.0.0.1", Serial);
                        if (res.Contains("connected") && Adbdevice.State == DeviceState.Online)
                        {
                            ShowMessage($"连接成功，地址：【127.0.0.1:{Serial}】", MessageType.Debug);
                            GetGameMissions();
                        }
                        else
                        {
                            ShowMessage($"连接失败，地址：【127.0.0.1:{Serial}】", MessageType.Error);
                        }
                    }
                    else
                    {
                        string res = Adb.Connect("127.0.0.1", Serial);
                        if (res.Contains("connected") && Adbdevice.State == DeviceState.Online)
                        {
                            ShowMessage($"连接成功，地址：【127.0.0.1:{Serial}】", MessageType.Debug);
                            GetGameMissions();
                        }
                        else
                        {
                            ShowMessage($"连接失败，地址：【127.0.0.1:{Serial}】", MessageType.Error);
                        }
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
                ShowMessage($"{ex.Message}", MessageType.Error);
            }
        }

        public void GetGameMissions()
        {
            Gamemissions = new ObservableCollection<GameMission>();
            Framebuffer framebuffer = Adb.GetFrameBuffer(Adbdevice);
            using (Bitmap image = framebuffer.ToImage())
            {
                string gameName = App.Current._host.Services.GetRequiredService<SettingsViewModel>().Mumus
                                        .Where(s => s.SubGameName == Title)
                                        .FirstOrDefault().GameName;
                int width = image.Width > image.Height ? image.Width : image.Height;
                string missionPath = $"{Environment.CurrentDirectory}\\Assets\\Image\\{gameName}\\Mission";
                ImagePath = $"{Environment.CurrentDirectory}\\Assets\\Image\\{gameName}\\{width}";
                Steps = new ObservableCollection<StepModel>();
                string[] directories = Directory.GetDirectories(ImagePath);
                string[] missions = Directory.GetFiles(missionPath);

                if (Directory.Exists(missionPath))
                {
                    UseJson = true;
                    foreach (var task in missions)
                    {
                        Gamemissions.Add(new GameMission { GameMissionName = Path.GetFileName(task).Substring(0, Path.GetFileName(task).Length - 5), IsChecked = false });
                        ObservableCollection<StepModel> steps = JsonConvert.DeserializeObject<ObservableCollection<StepModel>>(File.ReadAllText(task));
                        Steps = new ObservableCollection<StepModel>(Steps.Concat(steps));

                    }

                }
                /*else
                {
                    Steps.Add(new StepModel() { GameName = Title, GameInstance = "主线", StepName = "", XShift = 0, YShift = 0 });
                    string json = JsonConvert.SerializeObject(Steps, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                    File.WriteAllText(jsonPath, json);
                }*/
                else
                {
                    foreach (var task in directories)
                    {
                        Gamemissions.Add(new GameMission { GameMissionName = Path.GetFileName(task), IsChecked = false });
                        string[] files = Directory.GetFiles(task);//获取文件路径
                        foreach (var item in files)
                        {
                            Steps.Add(new StepModel() { GameName = Title, GameInstance = Path.GetFileName(task), StepName = item });
                        }
                    }
                }
            }
        }

        [RelayCommand]
        public async void Start()
        {
            await Task.Run(async () =>
            {
                if (!IsRunning)
                {
                    IsRunning = true;
                    cts = new CancellationTokenSource();
                    while (IsRunning && !cts.IsCancellationRequested)
                    {
                        foreach (var item in Gamemissions)
                        {
                            if (item.IsChecked)
                            {
                                Framebuffer framebuffer = Adb.GetFrameBuffer(Adbdevice);
                                Bitmap image = framebuffer.ToImage();
                                byte[] imageData = image.BitmapToByteArray();
                                foreach (var step in Steps)
                                {
                                    if (item.GameMissionName == step.GameInstance)
                                    {
                                        if (!cts.IsCancellationRequested)
                                        {
                                            if (!UseJson)
                                            {
                                                Rect rect = Gamehelper.MatchTemplate(image, Gamehelper.LoadAssetImage(step.StepName));
                                                if (rect.X != 0 && rect.Y != 0)
                                                {
                                                    string stepName = Path.GetFileName(step.StepName).Substring(0, Path.GetFileName(step.StepName).Length - 4);
                                                    if (stepName != "任务结束")
                                                    {
                                                        //使用正态分布获取随机坐标
                                                        Point point = Gamehelper.GenerateRandomPoint(rect);
                                                        int x = point.X;
                                                        int y = point.Y;
                                                        Adb.Click(Adbdevice, point);
                                                        ShowMessage($"点击【{stepName}】成功，坐标：【{x}，{y}】");
                                                        await Task.Delay(1000);
                                                    }
                                                    else
                                                    {
                                                        await Stop();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Rect rect = default;
                                                string imagePath = $"{ImagePath}\\{step.GameInstance}\\{step.StepName}";
                                                if (step.MatchMode == "OCR")
                                                {
                                                    rect = Gamehelper.OCR(imageData, step.StepName);
                                                }
                                                else
                                                {
                                                    rect = Gamehelper.MatchTemplate(image, Gamehelper.LoadAssetImage(imagePath + ".png"));
                                                }
                                                if (rect.X != 0 && rect.Y != 0)
                                                {
                                                    if (step.StepName != "任务结束")
                                                    {
                                                        //使用正态分布获取随机坐标
                                                        Point point = Gamehelper.GenerateRandomPoint(rect);
                                                        point.X += step.XShift;
                                                        point.Y += step.YShift;
                                                        int x = point.X;
                                                        int y = point.Y;
                                                        Adb.Click(Adbdevice, point);
                                                        ShowMessage($"点击【{step.StepName}】成功，坐标：【{x}，{y}】");
                                                        await Task.Delay(1000);
                                                    }
                                                    else
                                                    {
                                                        await Stop();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    await Stop();
                }
            });
        }

        [RelayCommand]
        public async Task Stop()
        {
            if (IsRunning)
            {
                cts.Cancel();
                IsRunning = false;
                ShowMessage("任务完成！", MessageType.Debug);
            }
        }

        public void ShowMessage(string message, MessageType type = MessageType.Info)
        {
            try
            {
                if (type == MessageType.Info)
                {
                    _log.Info(message);
                }
                else
                {
                    _log.Error(message);
                }
                void action()
                {
                    Messages.Add(new MessageData($"{message}", DateTime.Now, Title, type));
                }
                Gamehelper.ExecuteFunBeginInvoke(action);

            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
    }
}
