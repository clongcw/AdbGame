using CommunityToolkit.Mvvm.ComponentModel;

namespace AdbGame.Model
{
    public partial class GameMission : ObservableObject
    {
        [ObservableProperty] private string _gameMissionName;
        [ObservableProperty] private bool _isChecked;
    }
}
