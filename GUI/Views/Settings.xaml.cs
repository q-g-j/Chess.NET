using ChessDotNET.GUI.ViewModels.MainWindow;
using ChessDotNET.Settings;
using System.Windows.Controls;

namespace ChessDotNET.GUI.Views
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            MainWindowViewModel vm = (MainWindowViewModel)DataContext;
            AppSettings appSettings = new AppSettings(vm.AppSettingsFolder);
            PasswordBox.Password = appSettings.LoadSettings().EmailServer["password"];
        }
    }
}
