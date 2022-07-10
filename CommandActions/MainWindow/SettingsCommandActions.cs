using ChessDotNET.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ChessDotNET.ViewModels.CommandActions.MainWindow
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

        internal void SettingsPasswordBoxAction(object o)
        {
            var e = o as RoutedEventArgs;
            var passwordBox = e.Source as PasswordBox;
            vm.EmailPassword = passwordBox.Password;
        }
        internal void SettingsSaveAction()
        {
            AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
            vm.SettingsVisibility = "Hidden";
            appSettingsStruct.EmailServer["email_address"] = vm.SettingsTextBoxEmailAddress;
            appSettingsStruct.EmailServer["password"] = vm.EmailPassword;
            appSettingsStruct.EmailServer["pop3_server"] = vm.SettingsTextBoxEmailPop3Server;
            appSettingsStruct.EmailServer["smtp_server"] = vm.SettingsTextBoxEmailSMTPServer;
            appSettings.ChangeEmailServer(appSettingsStruct.EmailServer);
        }
        internal void SettingsCancelAction()
        {
            vm.SettingsVisibility = "Hidden";
        }
    }
}
