using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdbGame.Model
{
    public partial class GameMission : ObservableObject
    {
        [ObservableProperty] private string _gameMissionName;
        [ObservableProperty] private bool _isChecked;
    }
}
