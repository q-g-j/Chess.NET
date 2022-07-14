using System.Threading.Tasks;
using ChessDotNET.CustomTypes;
using ChessDotNET.Settings;

namespace ChessDotNET.GUI.ViewModels.MainWindow
{
    internal class OverlayNewEmailGameCommandActions
    {
        private readonly MainWindowViewModel vm;
        private readonly AppSettings appSettings;

        public OverlayNewEmailGameCommandActions(MainWindowViewModel _mainWindowViewModel, AppSettings _appSettings)
        {
            vm = _mainWindowViewModel;
            appSettings = _appSettings;
        }

        internal async void OverlayNewEmailGameStartAction()
        {
            if (vm.PropertiesDict["OverlayNewEmailGameTextBoxOpponentEmail"] == "") return;
            AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();

            vm.PropertiesDict["SideMenuVisibility"] = "Hidden";
            vm.PropertiesDict["SideMenuMainMenuVisibility"] = "Visible";
            vm.PropertiesDict["SideMenuNewGameModeVisibility"] = "Hidden";
            vm.OnPropertyChangedByPropertyName("PropertiesDict");

            var testConnection = EmailChess.Test.TestSMTPConnection(appSettingsStruct.EmailServer);
            if (! await testConnection)
            {
                vm.PropertiesDict["OverlayNewEmailGameErrorLabelVisibility"] = "Visible";
            }
            else
            {
                vm.IsEmailGame = true;
                vm.PropertiesDict["OverlayNewEmailGameVisibility"] = "Hidden";

                if (vm.PropertiesDict["OverlayNewEmailGameRadioButtonWhiteIsChecked"] == "True")
                {
                    vm.EmailGameOwnColor = ChessPieceColor.White;
                    vm.StartGame(false);
                    TileDictionary tileDict = vm.TileDict;
                }
            }

            vm.OnPropertyChangedByPropertyName("PropertiesDict");
        }

        internal async void OverlayNewEmailGameStartActionOld()
        {
            if (vm.PropertiesDict["OverlayNewEmailGameTextBoxOpponentEmail"] == "") return;
            AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();

            vm.PropertiesDict["OverlayNewEmailGameVisibility"] = "Hidden";
            vm.PropertiesDict["SideMenuVisibility"] = "Hidden";
            vm.PropertiesDict["SideMenuMainMenuVisibility"] = "Visible";
            vm.PropertiesDict["SideMenuNewGameModeVisibility"] = "Hidden";
            vm.OnPropertyChangedByPropertyName("PropertiesDict");

            vm.IsEmailGame = true;

            if (vm.PropertiesDict["OverlayNewEmailGameRadioButtonWhiteIsChecked"] == "True")
            {
                vm.EmailGameOwnColor = ChessPieceColor.White;
                vm.StartGame(false);
                TileDictionary tileDict = vm.TileDict;

                bool isConnectionOK = false;

                while (!isConnectionOK)
                {
                    await Task.Run(async () =>
                    {
                        appSettingsStruct = appSettings.LoadSettings();
                        isConnectionOK = true;

                        try
                        {
                            vm.DeleteOldEmailsTask = EmailChess.Delete.DeleteOldEmails(appSettingsStruct.EmailServer, false);
                            await vm.DeleteOldEmailsTask;
                        }
                        catch
                        {
                            isConnectionOK = false;

                            if (appSettingsStruct.EmailServer["email_address"] != null)
                            {
                                vm.PropertiesDict["OverlaySettingsTextBoxEmailAddress"] = appSettingsStruct.EmailServer["email_address"];
                            }
                            if (appSettingsStruct.EmailServer["pop3_server"] != null)
                            {
                                vm.PropertiesDict["OverlaySettingsTextBoxEmailPop3Server"] = appSettingsStruct.EmailServer["pop3_server"];
                            }
                            if (appSettingsStruct.EmailServer["smtp_server"] != null)
                            {
                                vm.PropertiesDict["OverlaySettingsTextBoxEmailSMTPServer"] = appSettingsStruct.EmailServer["smtp_server"];
                            }

                            vm.PropertiesDict["OverlaySettingsVisibility"] = "Visible";
                            vm.OnPropertyChangedByPropertyName("PropertiesDict");

                            await Task.Run(() =>
                            {
                                vm.isSettingsSaved = false;
                                while (!vm.isSettingsSaved)
                                {
                                    System.Threading.Thread.Sleep(500);
                                }
                            });
                        }
                        System.Threading.Thread.Sleep(1000);
                    });
                }
            }
            else if (vm.PropertiesDict["OverlayNewEmailGameRadioButtonBlackIsChecked"] == "True")
            {
                vm.EmailGameOwnColor = ChessPieceColor.Black;
                vm.StartGame(true);
                TileDictionary tileDict = vm.TileDict;

                vm.DoWaitForEmail = true;

                bool isConnectionOK = false;

                while (!isConnectionOK)
                {
                    await Task.Run(async () =>
                    {
                        appSettingsStruct = appSettings.LoadSettings();
                        isConnectionOK = true;

                        try
                        {
                            vm.DeleteOldEmailsTask = EmailChess.Delete.DeleteOldEmails(appSettingsStruct.EmailServer, false);
                            await vm.DeleteOldEmailsTask;

                            Task waitForEmailWhiteMoveTask = EmailChess.Receive.WaitForEmailNextWhiteMoveTask(vm, tileDict, appSettings);
                            await waitForEmailWhiteMoveTask;
                        }
                        catch
                        {
                            isConnectionOK = false;

                            if (appSettingsStruct.EmailServer["email_address"] != null)
                            {
                                vm.PropertiesDict["OverlaySettingsTextBoxEmailAddress"] = appSettingsStruct.EmailServer["email_address"];
                            }
                            if (appSettingsStruct.EmailServer["pop3_server"] != null)
                            {
                                vm.PropertiesDict["OverlaySettingsTextBoxEmailPop3Server"] = appSettingsStruct.EmailServer["pop3_server"];
                            }
                            if (appSettingsStruct.EmailServer["smtp_server"] != null)
                            {
                                vm.PropertiesDict["OverlaySettingsTextBoxEmailSMTPServer"] = appSettingsStruct.EmailServer["smtp_server"];
                            }

                            vm.PropertiesDict["OverlaySettingsVisibility"] = "Visible";
                            vm.OnPropertyChangedByPropertyName("PropertiesDict");

                            await Task.Run(() =>
                            {
                                vm.isSettingsSaved = false;
                                while (!vm.isSettingsSaved)
                                {
                                    System.Threading.Thread.Sleep(500);
                                }
                            });
                        }
                        System.Threading.Thread.Sleep(1000);
                    });
                }
                vm.DoWaitForEmail = false;
            }
        }
        internal void OverlayNewEmailGameCancelAction()
        {
            vm.PropertiesDict["OverlayNewEmailGameVisibility"] = "Hidden";
            vm.OnPropertyChangedByPropertyName("PropertiesDict");
        }
    }
}
