using System.Windows;
using ChessDotNET.CustomTypes;
using ChessDotNET.Settings;


namespace ChessDotNET.GUI.ViewModels.MainWindow
{
    internal class SideMenuCommandActions
    {
        internal SideMenuCommandActions(MainWindowViewModel _mainWindowViewModel, AppSettings _appSettings)
        {
            vm = _mainWindowViewModel;
            appSettings = _appSettings;
        }
        private readonly MainWindowViewModel vm;
        private readonly AppSettings appSettings;

        internal void SideMenuNewGameAction()
        {
            vm.SideMenuMainMenuVisibility = "Hidden";
            vm.SideMenuNewGameModeVisibility = "Visible";
        }
        internal void SideMenuNewGameModeLocalAction()
        {
            vm.SideMenuNewGameModeVisibility = "Hidden";
            vm.SideMenuButtonsNewGameLocalColorVisibility = "Visible";
        }
        internal void SideMenuNewGameModeEmailAction()
        {
            vm.SideMenuVisibility = "Hidden";
            vm.SideMenuMainMenuVisibility = "Visible";
            vm.SideMenuNewGameModeVisibility = "Hidden";
            vm.NewEmailGameVisibility = "Visible";
        }
        internal void SideMenuNewGameLocalColorGoBackAction()
        {
            vm.SideMenuButtonsNewGameLocalColorVisibility = "Hidden";
            vm.SideMenuNewGameModeVisibility = "Visible";
        }
        internal void SideMenuNewGameLocalAsWhiteAction()
        {
            vm.CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            vm.CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;

            vm.SideMenuVisibility = "Hidden";
            vm.SideMenuMainMenuVisibility = "Visible";
            vm.SideMenuNewGameModeVisibility = "Hidden";
            vm.IsEmailGame = false;
            vm.StartGame(ChessPieceColor.White);
        }
        internal void SideMenuNewGameLocalAsBlackAction()
        {
            vm.CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            vm.CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;

            vm.SideMenuVisibility = "Hidden";
            vm.SideMenuMainMenuVisibility = "Visible";
            vm.SideMenuNewGameModeVisibility = "Hidden";
            vm.IsEmailGame = false;
            vm.StartGame(ChessPieceColor.Black);
        }
        internal void SideMenuNewGameModeGoBackAction()
        {
            vm.SideMenuMainMenuVisibility = "Visible";
            vm.SideMenuNewGameModeVisibility = "Hidden";
        }
        internal void SideMenuSettingsAction()
        {
            AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
            vm.SideMenuVisibility = "Hidden";
            vm.SettingsVisibility = "Visible";
            vm.isSettingsSaved = false;
            if (appSettingsStruct.EmailServer["email_address"] != null) vm.SettingsTextBoxEmailAddress = appSettingsStruct.EmailServer["email_address"];
            if (appSettingsStruct.EmailServer["pop3_server"] != null) vm.SettingsTextBoxEmailPop3Server = appSettingsStruct.EmailServer["pop3_server"];
            if (appSettingsStruct.EmailServer["smtp_server"] != null) vm.SettingsTextBoxEmailSMTPServer = appSettingsStruct.EmailServer["smtp_server"];
        }
        internal void SideMenuQuitProgramAction()
        {
            Application.Current.Shutdown();
        }
    }
}
