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

        internal async void NewEmailGameStartAction(TileDictionary tileDict)
        {
            if (vm.NewEmailGameTextBoxOpponentEmail == "") return;
            AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
            vm.DeleteOldEmailsTask = EmailChess.Delete.DeleteOldEmails(appSettingsStruct.EmailServer);

            vm.NewEmailGameVisibility = "Hidden";

            vm.CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            vm.CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;

            tileDict = new TileDictionary();
            vm.SideMenuVisibility = "Hidden";
            vm.SideMenuMainMenuVisibility = "Visible";
            vm.SideMenuNewGameModeVisibility = "Hidden";
            vm.IsEmailGame = true;

            if (vm.NewEmailGameRadioButtonWhiteIsChecked == "True")
            {
                vm.EmailGameOwnColor = ChessPieceColor.White;
                vm.StartGame(ChessPieceColor.White);

            }
            else if (vm.NewEmailGameRadioButtonBlackIsChecked == "True")
            {
                vm.EmailGameOwnColor = ChessPieceColor.Black;
                vm.StartGame(ChessPieceColor.Black);
                vm.DoWaitForEmail = true;
                Task waitForEmailWhiteMoveTask = EmailChess.Receive.WaitForEmailNextWhiteMoveTask(vm, tileDict, appSettings);
                await waitForEmailWhiteMoveTask;
                vm.DoWaitForEmail = false;
            }
        }
        internal void NewEmailGameCancelAction()
        {
            vm.NewEmailGameVisibility = "Hidden";
        }
    }
}
