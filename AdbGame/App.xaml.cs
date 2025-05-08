using AdbGame.Common;
using AdbGame.View.Page;
using AdbGame.ViewModel;
using AdbGame.ViewModel.Page;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using Application = System.Windows.Application;

namespace AdbGame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public new static App? Current => Application.Current as App;

        public readonly IHost _host;
        public App()
        {
            _host = CreateHostBuilder().Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            GlobalSLog._log.Info("AdbGame启动");

            _host.Start();

            MainView mainView = _host.Services.GetRequiredService<MainView>();
            mainView.DataContext = _host.Services.GetRequiredService<MainViewModel>();
            mainView!.Show();
        }

        public static IHostBuilder CreateHostBuilder(string[] args = null)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton<MainView>();

                    services.AddSingleton<GameViewModel>();
                    services.AddSingleton<GameView>();

                    services.AddSingleton<MuMuViewModel>();
                    services.AddSingleton<MuMuView>();

                    services.AddSingleton<ScreenShotViewModel>();
                    services.AddSingleton<ScreenShotView>();

                    services.AddSingleton<SettingsViewModel>();
                    services.AddSingleton<SettingsView>();



                    services.AddSingleton<WeakReferenceMessenger>();
                    services.AddSingleton<IMessenger, WeakReferenceMessenger>(provider => provider.GetRequiredService<WeakReferenceMessenger>());
                });
        }
    }

}
