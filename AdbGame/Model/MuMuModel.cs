using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdbGame.Model
{
    public partial class MuMuModel : ObservableObject
    {
        [ObservableProperty] private string _gameName;
        [ObservableProperty] private int _serial;
    }
}
