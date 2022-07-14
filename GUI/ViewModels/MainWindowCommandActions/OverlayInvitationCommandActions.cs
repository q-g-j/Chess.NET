using System.Windows;
using System.Windows.Controls;
using ChessDotNET.GUI.ViewModels.MainWindow;
using ChessDotNET.Settings;


namespace ChessDotNET.GUI.ViewModels.MainWindow
{
    internal class OverlayInvitationCommandActions
    {
        public OverlayInvitationCommandActions(MainWindowViewModel _mainWindowViewModel, AppSettings _appSettings)
        {
            vm = _mainWindowViewModel;
            appSettings = _appSettings;
        }

        private readonly MainWindowViewModel vm;
        private readonly AppSettings appSettings;

        internal void OverlayInvitationAcceptAction()
        {

            vm.PropertiesDict["InvitationOverlayVisibility"] = "Hidden";
            vm.PropertiesDict = vm.PropertiesDict;
        }
        internal void OverlayInvitationRejectAction()
        {
            vm.PropertiesDict["InvitationOverlayVisibility"] = "Hidden";
            vm.PropertiesDict = vm.PropertiesDict;
        }
    }
}
