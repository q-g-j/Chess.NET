using ChessDotNET.GUI.ViewModels.MainWindow;
using ChessDotNET.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDotNET.GUI.ViewModels.MainWindow
{
    internal class OverlayInvitationAcceptedCommandActions
    {
        public OverlayInvitationAcceptedCommandActions(MainWindowViewModel _mainWindowViewModel, AppSettings _appSettings)
        {
            vm = _mainWindowViewModel;
            appSettings = _appSettings;
        }

        private readonly MainWindowViewModel vm;
        private readonly AppSettings appSettings;

        internal void OverlayInvitationAcceptedStartGameAction()
        {

            vm.PropertiesDict["InvitationAcceptedOverlayVisibility"] = "Hidden";
            vm.PropertiesDict = vm.PropertiesDict;
        }
    }
}
