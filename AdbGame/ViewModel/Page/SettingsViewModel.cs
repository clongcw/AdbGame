using AdbGame.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MessageBoxIcon = Panuon.WPF.UI.MessageBoxIcon;

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

            foreach (MuMuModel mumu in Mumus)
            {
                mumu.DeleteGameCommand = DeleteGameCommand;
            }
        }

        public void SetJson()
        {
            ObservableCollection<MuMuModel> mumus = new ObservableCollection<MuMuModel>();
            mumus.Add(new MuMuModel() { GameName = "少年西游记2", Serial = 16768});
            mumus.Add(new MuMuModel() { GameName = "笔绘西行", Serial = 16896});
            mumus.Add(new MuMuModel() { GameName = "河图寻仙记", Serial = 16864});


            string json = JsonConvert.SerializeObject(mumus, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            File.WriteAllText(Environment.CurrentDirectory + $"\\Assets\\Games.json", json);
        }

        [RelayCommand]
        public void AddGame()
        {
            Mumus.Add(new MuMuModel() { GameName = "新游戏", Serial = 65535, DeleteGameCommand = DeleteGameCommand });
        }

        [RelayCommand]
        public void DeleteGame(int serial)
        {
            Mumus.Remove(Mumus.Where(s => s.Serial == serial).FirstOrDefault());
        }

        [RelayCommand]
        public async Task SaveGame()
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };
            string json = JsonConvert.SerializeObject(Mumus, Newtonsoft.Json.Formatting.None, settings);

            File.WriteAllText(Environment.CurrentDirectory + $"\\Assets\\Games.json", json);
            await App.Current.Dispatcher.InvokeAsync(async () =>
            {
                MessageBoxX.Show("添加成功！", "提示", MessageBoxButton.OK, MessageBoxIcon.Success, DefaultButton.YesOK);
            });
        }
    }

}
