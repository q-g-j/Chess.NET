using System.Windows;
using System.Windows.Controls;
using ChessDotNET.Settings;


namespace ChessDotNET.GUI.ViewModels.MainWindow
{
    internal class SettingsCommandActions
    {
        public SettingsCommandActions(MainWindowViewModel _mainWindowViewModel, AppSettings _appSettings)
        {
            vm = _mainWindowViewModel;
            appSettings = _appSettings;
        }

        private readonly MainWindowViewModel vm;
        private readonly AppSettings appSettings;
        private PasswordBox passwordBox;

        internal void SettingsPasswordBoxAction(object o)
        {
            var e = o as RoutedEventArgs;
            if (passwordBox == null) passwordBox = e.Source as PasswordBox;
            vm.EmailPassword = passwordBox.Password;
        }
        internal void SettingsSaveAction()
        {
            if (vm.EmailPassword != "")
            {
                AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
                vm.SettingsVisibility = "Hidden";
                appSettingsStruct.EmailServer["email_address"] = vm.SettingsTextBoxEmailAddress;
                if (vm.EmailPassword != null) appSettingsStruct.EmailServer["password"] = vm.EmailPassword;
                appSettingsStruct.EmailServer["pop3_server"] = vm.SettingsTextBoxEmailPop3Server;
                appSettingsStruct.EmailServer["smtp_server"] = vm.SettingsTextBoxEmailSMTPServer;
                appSettings.ChangeEmailServer(appSettingsStruct.EmailServer);
                passwordBox = null;
            }
        }
        internal void SettingsCancelAction()
        {
            vm.SettingsVisibility = "Hidden";
            if (passwordBox != null)
            {
                AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
                if (appSettingsStruct.EmailServer["password"] != null)
                {
                    passwordBox.Password = appSettingsStruct.EmailServer["password"];
                }
                passwordBox = null;
            }
        }
    }
}
