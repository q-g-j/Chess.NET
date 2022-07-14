using System.Windows;
using System.Windows.Controls;
using ChessDotNET.Settings;


namespace ChessDotNET.GUI.ViewModels.MainWindow
{
    internal class OverlaySettingsCommandActions
    {
        public OverlaySettingsCommandActions(MainWindowViewModel _mainWindowViewModel, AppSettings _appSettings)
        {
            vm = _mainWindowViewModel;
            appSettings = _appSettings;
        }

        private readonly MainWindowViewModel vm;
        private readonly AppSettings appSettings;
        private PasswordBox passwordBox;

        internal void OverlaySettingsPasswordBoxAction(object o)
        {
            var e = o as RoutedEventArgs;
            if (passwordBox == null) passwordBox = e.Source as PasswordBox;
            vm.EmailPassword = passwordBox.Password;
        }
        internal void OverlaySettingsSaveAction()
        {
            if (vm.EmailPassword != "")
            {
                AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
                vm.PropertiesDict["SettingsOverlayVisibility"] = "Hidden";
                vm.PropertiesDict = vm.PropertiesDict;

                appSettingsStruct.EmailServer["email_address"] = vm.PropertiesDict["SettingsOverlayTextBoxEmailAddress"];
                if (vm.EmailPassword != null) appSettingsStruct.EmailServer["password"] = vm.EmailPassword;
                appSettingsStruct.EmailServer["pop3_server"] = vm.PropertiesDict["SettingsOverlayTextBoxEmailPop3Server"];
                appSettingsStruct.EmailServer["smtp_server"] = vm.PropertiesDict["SettingsOverlayTextBoxEmailSMTPServer"];
                appSettings.ChangeEmailServer(appSettingsStruct.EmailServer);
                vm.isSettingsSaved = true;
                passwordBox = null;

                vm.PropertiesDict = vm.PropertiesDict;
            }
        }
        internal void OverlaySettingsCancelAction()
        {
            vm.PropertiesDict["SettingsOverlayVisibility"] = "Hidden";
            vm.PropertiesDict = vm.PropertiesDict;

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
