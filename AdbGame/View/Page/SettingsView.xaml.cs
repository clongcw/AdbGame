using AdbGame.ViewModel.Page;
using Microsoft.Extensions.DependencyInjection;

namespace AdbGame.View.Page
{
    /// <summary>
    /// SettingsView.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsView
    {
        public SettingsView()
        {
            InitializeComponent();
            this.DataContext = App.Current._host.Services.GetRequiredService<SettingsViewModel>();
        }
    }
}
