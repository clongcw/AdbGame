using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.System.Profile;

namespace AdbGame.Model
{
    public partial class MuMuModel : ObservableObject
    {
        [ObservableProperty] private string _gameName;
        [ObservableProperty] private int _serial;

        [JsonIgnore] public IRelayCommand<int> DeleteGameCommand { get; set; }
    }
}
