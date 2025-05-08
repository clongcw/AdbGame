using AdbGame.ViewModel.Page;
using Microsoft.Extensions.DependencyInjection;

namespace AdbGame.View.Page
{
    /// <summary>
    /// GameView.xaml 的交互逻辑
    /// </summary>
    public partial class GameView
    {
        public GameView()
        {
            InitializeComponent();
            this.DataContext = App.Current._host.Services.GetRequiredService<GameViewModel>();
        }
    }
}
