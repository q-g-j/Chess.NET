using CommunityToolkit.Mvvm.Messaging;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace ChessDotNET.ViewModels
{
    internal class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        protected void OnPropertyChangedByPropertyName(string propertyName)
        {

            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        protected void ChangePropertyStringValueMainWindowViewModel(string propertyName, string propertyValue)
        {
            WeakReferenceMessenger.Default.Send(
                        new MainWindowViewModel.PropertyStringValueChangedMessage(
                            new Tuple<string, string>(propertyName, propertyValue)));
        }
        protected void ChangePropertyStringValueSideMenuViewModel(string propertyName, string propertyValue)
        {
            WeakReferenceMessenger.Default.Send(
                        new SideMenuViewModel.PropertyStringValueChangedMessage(
                            new Tuple<string, string>(propertyName, propertyValue)));
        }
        protected void ChangePropertyStringValueLobbyViewModel(string propertyName, string propertyValue)
        {
            WeakReferenceMessenger.Default.Send(
                        new LobbyViewModel.PropertyStringValueChangedMessage(
                            new Tuple<string, string>(propertyName, propertyValue)));
        }
    }
}
