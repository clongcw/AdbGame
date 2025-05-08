using AdbGame.View.Page;
using Panuon.WPF.UI;
using Wpf.Ui.Controls;

namespace AdbGame
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : WindowX
    {
        public MainView()
        {
            InitializeComponent();

            Loaded += (_, _) => NavigationView.Navigate(typeof(GameView));


        }

    }
}
