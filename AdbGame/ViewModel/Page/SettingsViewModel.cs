using AdbGame.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
            if (!File.Exists(Environment.CurrentDirectory + $"\\Assets\\Games.json"))
            {
                SetJson();
            }
            
            string json = File.ReadAllText(Environment.CurrentDirectory + $"\\Assets\\Games.json");

            Mumus = JsonConvert.DeserializeObject<ObservableCollection<MuMuModel>>(json);
        }

        public void SetJson()
        {
            ObservableCollection<MuMuModel> mumus = new ObservableCollection<MuMuModel>();
            mumus.Add(new MuMuModel() { GameName = "少年西游记2", Serial = 16768 });
            mumus.Add(new MuMuModel() { GameName = "笔绘西行", Serial = 16896 });
            mumus.Add(new MuMuModel() { GameName = "河图寻仙记", Serial = 16864 });


            string json = JsonConvert.SerializeObject(mumus, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            File.WriteAllText(Environment.CurrentDirectory + $"\\Assets\\Games.json", json);
        }
    }
}
