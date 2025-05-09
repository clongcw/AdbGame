using AdbGame.View.Page;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace AdbGame.ViewModel.Page
{
    public partial class GameViewModel : ObservableObject
    {
        [ObservableProperty] private string _title = string.Empty;
        [ObservableProperty] private ObservableCollection<TabItem> _games;
        [ObservableProperty] private TabItem _selectedItem;

        public GameViewModel()
        {
            Games = new ObservableCollection<TabItem>();

            foreach (var item in App.Current._host.Services.GetRequiredService<SettingsViewModel>().Mumus)
            {
                Games.Add(new TabItem()
                {
                    Header = item.SubGameName,
                    Content = new MuMuView(),
                    DataContext = new MuMuViewModel(item.SubGameName, item.Serial)
                });
            }
        }
    }
}
