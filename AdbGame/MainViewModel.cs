using AdbGame.View.Page;
using AdbGame.ViewModel;
using AdbGame.ViewModel.Page;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
