using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using ChessDotNET.Models;
using ChessDotNET.Services;
using ChessDotNET.Views;


namespace ChessDotNET.ViewModels
{
    internal class SideMenuViewModel : ViewModelBase
    {
        public SideMenuViewModel()
        {
            globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();

            InitializeCommands();
            InitializeMessageHandlers();
        }

        #region Fields
        private Globals globals;
        #endregion

        #region Bindable Properties
        private string sideMenuMainVisibility = "Visible";
        public string SideMenuMainVisibility
        {
            get => sideMenuMainVisibility;
            set { sideMenuMainVisibility = value; OnPropertyChanged(); }
        }

        private string sideMenuGameModeVisibility = "Hidden";
        public string SideMenuGameModeVisibility
        {
            get => sideMenuGameModeVisibility;
            set { sideMenuGameModeVisibility = value; OnPropertyChanged(); }
        }

        private string sideMenuLocalGameVisibility = "Hidden";
        public string SideMenuLocalGameVisibility
        {
            get => sideMenuLocalGameVisibility;
            set { sideMenuLocalGameVisibility = value; OnPropertyChanged(); }
        }

        private string sideMenuOnlineGameVisibility = "Hidden";
        public string SideMenuOnlineGameVisibility
        {
            get => sideMenuOnlineGameVisibility;
            set { sideMenuOnlineGameVisibility = value; OnPropertyChanged(); }
        }

        private string sideMenuOnlineGameButtonVisibility = "Visible";
        public string SideMenuOnlineGameButtonVisibility
        {
            get => sideMenuOnlineGameButtonVisibility;
            set { sideMenuOnlineGameButtonVisibility = value; OnPropertyChanged(); }
        }

        private string sideMenuEndOnlineGameButtonVisibility = "Hidden";
        public string SideMenuEndOnlineGameButtonVisibility
        {
            get => sideMenuEndOnlineGameButtonVisibility;
            set { sideMenuEndOnlineGameButtonVisibility = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        public RelayCommand SideMenuNewGameCommand { get; set; }
        public RelayCommand SideMenuLocalGameCommand { get; set; }
        public RelayCommand SideMenuOnlineGameCommand { get; set; }
        public RelayCommand SideMenuGameModeGoBackCommand { get; set; }
        public RelayCommand SideMenuLocalGameAsWhiteCommand { get; set; }
        public RelayCommand SideMenuLocalGameAsBlackCommand { get; set; }
        public RelayCommand SideMenuLocalGameGoBackCommand { get; set; }
        public RelayCommand SideMenuOnlineGameEnterLobbyCommand { get; set; }
        public RelayCommand SideMenuEndOnlineGameCommand { get; set; }
        public RelayCommand SideMenuOnlineGameGoBackCommand { get; set; }
        public RelayCommand SideMenuQuitProgramCommand { get; set; }
        #endregion

        #region Command Actions
        private void SideMenuNewGameAction()
        {
            SideMenuMainVisibility = "Hidden";
            SideMenuGameModeVisibility = "Visible";
        }
        private void SideMenuQuitProgramAction()
        {
            Application.Current.Shutdown();
        }
        private void SideMenuGameModeGoBackAction()
        {
            SideMenuGameModeVisibility = "Hidden";
            SideMenuMainVisibility = "Visible";
        }
        private void SideMenuLocalGameAction()
        {
            SideMenuGameModeVisibility = "Hidden";
            SideMenuLocalGameVisibility = "Visible";
        }
        private void SideMenuOnlineGameAction()
        {
            SideMenuGameModeVisibility = "Hidden";
            SideMenuOnlineGameVisibility = "Visible";
        }
        private void SideMenuLocalGameGoBackAction()
        {
            SideMenuLocalGameVisibility = "Hidden";
            SideMenuGameModeVisibility = "Visible";
        }
        private void SideMenuOnlineGameGoBackAction()
        {
            SideMenuOnlineGameVisibility = "Hidden";
            SideMenuGameModeVisibility = "Visible";
        }
        private void SideMenuLocalGameAsWhiteAction()
        {
            globals.CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            globals.CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;

            ChangePropertyStringValueMainWindowViewModel("SideMenuVisibility", "Hidden");
            SideMenuMainVisibility = "Visible";
            SideMenuGameModeVisibility = "Hidden";

            WeakReferenceMessenger.Default.Send(
                new MainWindowViewModel.StartGameMessage(false));
        }
        private void SideMenuLocalGameAsBlackAction()
        {
            globals.CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            globals.CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;

            ChangePropertyStringValueMainWindowViewModel("SideMenuVisibility", "Hidden");
            SideMenuMainVisibility = "Visible";
            SideMenuGameModeVisibility = "Hidden";

            WeakReferenceMessenger.Default.Send(
                new MainWindowViewModel.StartGameMessage(true));
        }
        private void SideMenuOnlineGameEnterLobbyAction()
        {
            SideMenuOnlineGameVisibility = "Hidden";
            SideMenuGameModeVisibility = "Hidden";
            ChangePropertyStringValueMainWindowViewModel("SideMenuVisibility", "Hidden");

            if (globals.LobbyWindow == null)
            {
                globals.LobbyWindow = new Lobby
                {
                    DataContext = new LobbyViewModel()
                };
                globals.LobbyWindow.Show();

                WeakReferenceMessenger.Default.Send(
                    new LobbyViewModel.PropertyPlayerListValueChangedMessage(new ObservableCollection<Player>()));
                WeakReferenceMessenger.Default.Send(
                    new LobbyViewModel.PropertyInvitationListValueChangedMessage(new ObservableCollection<Player>()));

                ChangePropertyStringValueLobbyViewModel("LobbyOverlayPlayerNameVisibility", "Visible");

                Player localPlayer = WeakReferenceMessenger.Default.Send<MainWindowViewModel.PropertyLocalPlayerValueRequestMessage>();
                if (localPlayer != null)
                {
                    ChangePropertyStringValueLobbyViewModel("LobbyOverLayPlayerNameTextBox", localPlayer.Name);
                }

                BackgroundThreads.LobbyKeepResettingInactiveCounter();
            }
            else
            {
                globals.LobbyWindow.Focus();
            }
        }
        private void SideMenuEndOnlineGameAction()
        {
            SideMenuEndOnlineGameButtonVisibility = "Hidden";
            SideMenuOnlineGameButtonVisibility = "Visible";
            globals.IsOnlineGame = false;
            globals.IsWaitingForMove = false;

            WeakReferenceMessenger.Default.Send(
                new MainWindowViewModel.StartGameMessage(false));
        }
        #endregion

        #region Methods
        private void InitializeCommands()
        {
            SideMenuNewGameCommand = new RelayCommand(SideMenuNewGameAction);

            SideMenuLocalGameCommand = new RelayCommand(SideMenuLocalGameAction);
            SideMenuOnlineGameCommand = new RelayCommand(SideMenuOnlineGameAction);
            SideMenuGameModeGoBackCommand = new RelayCommand(SideMenuGameModeGoBackAction);

            SideMenuLocalGameAsWhiteCommand = new RelayCommand(SideMenuLocalGameAsWhiteAction);
            SideMenuLocalGameAsBlackCommand = new RelayCommand(SideMenuLocalGameAsBlackAction);
            SideMenuLocalGameGoBackCommand = new RelayCommand(SideMenuLocalGameGoBackAction);

            SideMenuOnlineGameEnterLobbyCommand = new RelayCommand(SideMenuOnlineGameEnterLobbyAction);
            SideMenuEndOnlineGameCommand = new RelayCommand(SideMenuEndOnlineGameAction);
            SideMenuOnlineGameGoBackCommand = new RelayCommand(SideMenuOnlineGameGoBackAction);

            SideMenuQuitProgramCommand = new RelayCommand(SideMenuQuitProgramAction);
        }
        private void InitializeMessageHandlers()
        {
            WeakReferenceMessenger.Default.Register<PropertyStringValueChangedMessage>(this, (r, m) =>
            {
                var propertyName = GetType().GetProperty(m.Value.Item1);
                propertyName.SetValue(this, m.Value.Item2);
            });

            WeakReferenceMessenger.Default.Register<OnPropertyChangedMessage>(this, (r, m) =>
            {
                OnPropertyChangedByPropertyName(m.Value);
            });
        }
        #endregion

        #region Message Handlers
        // ValueChangedMessage Handlers:
        internal class PropertyStringValueChangedMessage : ValueChangedMessage<Tuple<string, string>>
        {
            internal PropertyStringValueChangedMessage(Tuple<string, string> tuple) : base(tuple) { }
        }
        internal class OnPropertyChangedMessage : ValueChangedMessage<string>
        {
            internal OnPropertyChangedMessage(string propertyName) : base(propertyName) { }
        }
        #endregion
    }
}
