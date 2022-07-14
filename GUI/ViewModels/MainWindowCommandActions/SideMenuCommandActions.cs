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
            vm.PropertiesDict["SideMenuMainMenuVisibility"] = "Hidden";
            vm.PropertiesDict["SideMenuNewGameModeVisibility"] = "Visible";
            vm.OnPropertyChangedByPropertyName("PropertiesDict");
        }
        internal void SideMenuNewGameModeLocalAction()
        {
            vm.PropertiesDict["SideMenuNewGameModeVisibility"] = "Hidden";
            vm.PropertiesDict["SideMenuButtonsNewGameLocalColorVisibility"] = "Visible";
            vm.OnPropertyChangedByPropertyName("PropertiesDict");
        }
        internal void SideMenuNewGameModeEmailAction()
        {
            vm.PropertiesDict["SideMenuVisibility"] = "Hidden";
            vm.PropertiesDict["SideMenuMainMenuVisibility"] = "Visible";
            vm.PropertiesDict["SideMenuNewGameModeVisibility"] = "Hidden";
            vm.PropertiesDict["OverlayNewEmailGameErrorLabelVisibility"] = "Hidden";
            vm.PropertiesDict["OverlayNewEmailGameVisibility"] = "Visible";
            vm.OnPropertyChangedByPropertyName("PropertiesDict");
        }
        internal void SideMenuNewGameLocalColorGoBackAction()
        {
            vm.PropertiesDict["SideMenuButtonsNewGameLocalColorVisibility"] = "Hidden";
            vm.PropertiesDict["SideMenuNewGameModeVisibility"] = "Visible";
            vm.OnPropertyChangedByPropertyName("PropertiesDict");
        }
        internal void SideMenuNewGameLocalAsWhiteAction()
        {
            vm.CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            vm.CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;

            vm.PropertiesDict["SideMenuVisibility"] = "Hidden";
            vm.PropertiesDict["SideMenuMainMenuVisibility"] = "Visible";
            vm.PropertiesDict["SideMenuNewGameModeVisibility"] = "Hidden";
            vm.OnPropertyChangedByPropertyName("PropertiesDict");

            vm.IsEmailGame = false;
            vm.StartGame(false);
        }
        internal void SideMenuNewGameLocalAsBlackAction()
        {
            vm.CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            vm.CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;

            vm.PropertiesDict["SideMenuVisibility"] = "Hidden";
            vm.PropertiesDict["SideMenuMainMenuVisibility"] = "Visible";
            vm.PropertiesDict["SideMenuNewGameModeVisibility"] = "Hidden";
            vm.OnPropertyChangedByPropertyName("PropertiesDict");

            vm.IsEmailGame = false;
            vm.StartGame(true);
        }
        internal void SideMenuNewGameModeGoBackAction()
        {
            vm.PropertiesDict["SideMenuMainMenuVisibility"] = "Visible";
            vm.PropertiesDict["SideMenuNewGameModeVisibility"] = "Hidden";
            vm.OnPropertyChangedByPropertyName("PropertiesDict");
        }
        internal void SideMenuSettingsAction()
        {
            AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
            vm.PropertiesDict["SideMenuVisibility"] = "Hidden";
            vm.PropertiesDict["OverlaySettingsVisibility"] = "Visible";

            vm.isSettingsSaved = false;
            if (appSettingsStruct.EmailServer["email_address"] != null) vm.PropertiesDict["OverlaySettingsTextBoxEmailAddress"] = appSettingsStruct.EmailServer["email_address"];
            if (appSettingsStruct.EmailServer["pop3_server"] != null) vm.PropertiesDict["OverlaySettingsTextBoxEmailPop3Server"] = appSettingsStruct.EmailServer["pop3_server"];
            if (appSettingsStruct.EmailServer["smtp_server"] != null) vm.PropertiesDict["OverlaySettingsTextBoxEmailSMTPServer"] = appSettingsStruct.EmailServer["smtp_server"];

            vm.OnPropertyChangedByPropertyName("PropertiesDict");
        }
        internal void SideMenuQuitProgramAction()
        {
            Application.Current.Shutdown();
        }

    }
}
