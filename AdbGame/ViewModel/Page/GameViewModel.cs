using AdbGame.View.Page;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AdbGame.ViewModel.Page
{
    public partial class GameViewModel : ObservableObject
    {
        [ObservableProperty] private string _title = string.Empty;
        [ObservableProperty] private ObservableCollection<TabItem> _games;

        public GameViewModel()
        {
            Games = new ObservableCollection<TabItem>();

            Games.Add(new TabItem()
            {
                Header = "少年西游记2",
                Content = new MuMuView(),
                DataContext = new MuMuViewModel("少年西游记2", 16768)
            });
            Games.Add(new TabItem()
            {
                Header = "笔绘西行",
                Content = new MuMuView(),
                DataContext = new MuMuViewModel("笔绘西行", 16896)
            });
            Games.Add(new TabItem()
            {
                Header = "河图寻仙记",
                Content = new MuMuView(),
                DataContext = new MuMuViewModel("河图寻仙记", 16864)
            });
        }
    }
}
