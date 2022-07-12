using System.Threading.Tasks;
using ChessDotNET.CustomTypes;
using ChessDotNET.Settings;

namespace ChessDotNET.GUI.ViewModels.MainWindow
{
    internal class NewEmailGameCommandActions
    {
        private readonly MainWindowViewModel vm;
        private readonly AppSettings appSettings;

        public NewEmailGameCommandActions(MainWindowViewModel _mainWindowViewModel, AppSettings _appSettings)
        {
            vm = _mainWindowViewModel;
            appSettings = _appSettings;
        }

        internal async void NewEmailGameStartAction()
        {
            if (vm.NewEmailGameTextBoxOpponentEmail == "") return;
            AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();

            vm.NewEmailGameVisibility = "Hidden";
            vm.SideMenuVisibility = "Hidden";
            vm.SideMenuMainMenuVisibility = "Visible";
            vm.SideMenuNewGameModeVisibility = "Hidden";
            vm.IsEmailGame = true;

            if (vm.NewEmailGameRadioButtonWhiteIsChecked == "True")
            {
                vm.EmailGameOwnColor = ChessPieceColor.White;
                vm.StartGame(ChessPieceColor.White);
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
                                vm.SettingsTextBoxEmailAddress = appSettingsStruct.EmailServer["email_address"];
                            }
                            if (appSettingsStruct.EmailServer["pop3_server"] != null)
                            {
                                vm.SettingsTextBoxEmailPop3Server = appSettingsStruct.EmailServer["pop3_server"];
                            }
                            if (appSettingsStruct.EmailServer["smtp_server"] != null)
                            {
                                vm.SettingsTextBoxEmailSMTPServer = appSettingsStruct.EmailServer["smtp_server"];
                            }

                            vm.SettingsVisibility = "Visible";

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
            else if (vm.NewEmailGameRadioButtonBlackIsChecked == "True")
            {
                vm.EmailGameOwnColor = ChessPieceColor.Black;
                vm.StartGame(ChessPieceColor.Black);
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
                                vm.SettingsTextBoxEmailAddress = appSettingsStruct.EmailServer["email_address"];
                            }
                            if (appSettingsStruct.EmailServer["pop3_server"] != null)
                            {
                                vm.SettingsTextBoxEmailPop3Server = appSettingsStruct.EmailServer["pop3_server"];
                            }
                            if (appSettingsStruct.EmailServer["smtp_server"] != null)
                            {
                                vm.SettingsTextBoxEmailSMTPServer = appSettingsStruct.EmailServer["smtp_server"];
                            }

                            vm.SettingsVisibility = "Visible";

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
        internal void NewEmailGameCancelAction()
        {
            vm.NewEmailGameVisibility = "Hidden";
        }
    }
}
