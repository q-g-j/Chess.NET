using System.Windows;

namespace ChessDotNET.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GridSideMenu.Children.Add(new SideMenu());
            GridOverlayPromotePawn.Children.Add(new OverlayPromotePawn());
            GridOverlayOnlineGamePlayerQuit.Children.Add(new OverlayOnlineGamePlayerQuit());
        }
    }
}
