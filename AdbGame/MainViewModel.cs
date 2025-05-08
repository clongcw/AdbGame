using AdbGame.View.Page;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace AdbGame
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty] private string _applicationTitle = string.Empty;
        [ObservableProperty] private object _content;

        [ObservableProperty] private ObservableCollection<object> _navigationFooter = [];

        [ObservableProperty]
        private ObservableCollection<object> _navigationItems;

        public MainViewModel()
        {
            ApplicationTitle = "AdbGame";

            NavigationItems = new ObservableCollection<object>()
            {
                new NavigationViewItem()
                {
                    Content = "游戏",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Games48 },
                    TargetPageType = typeof(GameView),
                },
                new NavigationViewItem()
                {
                    Content = "截屏",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Screenshot24 },
                    TargetPageType = typeof(ScreenShotView),
                }
            };

            NavigationFooter =
            [
                new NavigationViewItem()
                {
                    Content = "设置",
                    Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                    TargetPageType = typeof(SettingsView),
                },
            ];

        }
    }
}
