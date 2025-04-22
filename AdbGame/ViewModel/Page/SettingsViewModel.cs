using AdbGame.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdbGame.ViewModel.Page
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<MuMuModel> _mumus;

        public SettingsViewModel()
        {
            Mumus = new ObservableCollection<MuMuModel>();
            Mumus.Add(new MuMuModel() { GameName = "少年西游记2", Serial = 16768 });
            Mumus.Add(new MuMuModel() { GameName = "笔绘西行", Serial = 16896 });
            Mumus.Add(new MuMuModel() { GameName = "河图寻仙记", Serial = 16864 });
        }
    }
}
