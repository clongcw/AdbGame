using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;

namespace AdbGame.Model
{
    public partial class MuMuModel : ObservableObject
    {
        [ObservableProperty] private string _gameName;
        [ObservableProperty] private int _serial;

        [JsonIgnore] public IRelayCommand<int> DeleteGameCommand { get; set; }
    }
}
