using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using ChessDotNET.Services;

using ChessDotNET.Models;
using ChessDotNET.WebApiClient;


namespace ChessDotNET.ViewModels
{
    internal class LobbyViewModel : ViewModelBase
    {
        public LobbyViewModel()
        {
            globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();
            localPlayer = WeakReferenceMessenger.Default.Send<MainWindowViewModel.PropertyLocalPlayerValueRequestMessage>();
            opponent = WeakReferenceMessenger.Default.Send<MainWindowViewModel.PropertyOpponentValueRequestMessage>();

            InititalizeCommands();
            InitializeMessageHandlers();
        }

        #region Fields
        private Globals globals;
        Player localPlayer;
        Player opponent;
        private static DataGrid dataGridLobbyAllPlayers = null;
        private static DataGrid dataGridLobbyInvitations = null;
        private bool hasLobbyOverlayPlayerNameTextBoxFocus;
        #endregion

        #region Bindable Properties
        public Player LocalPlayer
        {
            get => localPlayer;
            set
            {
                localPlayer = value;

                WeakReferenceMessenger.Default.Send(
                new MainWindowViewModel.PropertyPlayerValueChangedMessage(
                    new Tuple<string, Player>("LocalPlayer", value)));

                OnPropertyChanged();
            }
        }
        public Player Opponent
        {
            get => opponent;
            set
            {
                opponent = value;

                WeakReferenceMessenger.Default.Send(
                new MainWindowViewModel.PropertyPlayerValueChangedMessage(
                    new Tuple<string, Player>("Opponent", value)));

                OnPropertyChanged();
            }
        }
        public ObservableCollection<Player> playerList;
        public ObservableCollection<Player> PlayerList
        {
            get => playerList;
            set { playerList = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Player> invitationList;
        public ObservableCollection<Player> InvitationList
        {
            get => invitationList;
            set { invitationList = value; OnPropertyChanged(); }
        }
        private string lobbyOverlayPlayerNameVisibility = "Hidden";
        public string LobbyOverlayPlayerNameVisibility
        {
            get => lobbyOverlayPlayerNameVisibility;
            set { lobbyOverlayPlayerNameVisibility = value; OnPropertyChanged(); }
        }

        private string lobbyOverlayWaitingForInvitationAcceptedVisibility = "Hidden";
        public string LobbyOverlayWaitingForInvitationAcceptedVisibility
        {
            get => lobbyOverlayWaitingForInvitationAcceptedVisibility;
            set { lobbyOverlayWaitingForInvitationAcceptedVisibility = value; OnPropertyChanged(); }
        }

        private string lobbyOverlayOpponentAcceptedInvitationVisibility = "Hidden";
        public string LobbyOverlayOpponentAcceptedInvitationVisibility
        {
            get => lobbyOverlayOpponentAcceptedInvitationVisibility;
            set { lobbyOverlayOpponentAcceptedInvitationVisibility = value; OnPropertyChanged(); }
        }

        private string lobbyOverlayOpponentCanceledInvitationVisibility = "Hidden";
        public string LobbyOverlayOpponentCanceledInvitationVisibility
        {
            get => lobbyOverlayOpponentCanceledInvitationVisibility;
            set { lobbyOverlayOpponentCanceledInvitationVisibility = value; OnPropertyChanged(); }
        }

        private string labelPlayerNameConflict = "";
        public string LabelPlayerNameConflict
        {
            get => labelPlayerNameConflict;
            set { labelPlayerNameConflict = value; OnPropertyChanged(); }
        }

        private string lobbyOverlayPlayerNameOkButtonIsEnabled = "False";
        public string LobbyOverlayPlayerNameOkButtonIsEnabled
        {
            get => lobbyOverlayPlayerNameOkButtonIsEnabled;
            set { lobbyOverlayPlayerNameOkButtonIsEnabled = value; OnPropertyChanged(); }
        }

        private string lobbyInviteButtonIsEnabled = "False";
        public string LobbyInviteButtonIsEnabled
        {
            get => lobbyInviteButtonIsEnabled;
            set { lobbyInviteButtonIsEnabled = value; OnPropertyChanged(); }
        }

        private string lobbyAcceptInvitationButtonIsEnabled = "False";
        public string LobbyAcceptInvitationButtonIsEnabled
        {
            get => lobbyAcceptInvitationButtonIsEnabled;
            set { lobbyAcceptInvitationButtonIsEnabled = value; OnPropertyChanged(); }
        }

        private string lobbyOverLayPlayerNameTextBox = "";

        public string LobbyOverLayPlayerNameTextBox
        {
            get => lobbyOverLayPlayerNameTextBox;
            set
            {
                lobbyOverLayPlayerNameTextBox = value;
                LabelPlayerNameConflict = "";
                if (value != "")
                {
                    LobbyOverlayPlayerNameOkButtonIsEnabled = "True";
                }
                else
                {
                    LobbyOverlayPlayerNameOkButtonIsEnabled = "False";
                }
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public RelayCommand LobbyInviteCommand { get; set; }
        public RelayCommand LobbyAcceptInvitationCommand { get; set; }
        public RelayCommand LobbyRefreshCommand { get; set; }
        public RelayCommand LobbyCloseCommand { get; set; }
        public RelayCommand OnLobbyClosingCommand { get; set; }
        public RelayCommand<object> LobbyKeyboardCommand { get; set; }
        public RelayCommand<object> OnLobbyDataGridAllPlayersLoadedCommand { get; set; }
        public RelayCommand<object> OnLobbyDataGridInvitationsLoadedCommand { get; set; }
        public RelayCommand OnLobbyDataGridAllPlayersSelectedCellsChangedCommand { get; set; }
        public RelayCommand OnLobbyDataGridInvitationsSelectedCellsChangedCommand { get; set; }
        public RelayCommand LobbyOverlayPlayerNameOkCommand { get; set; }
        public RelayCommand LobbyOverlayPlayerNameCancelCommand { get; set; }
        public RelayCommand<object> OnLobbyOverlayPlayerNameTextBoxFocusCommand { get; set; }
        public RelayCommand LobbyOverlayOpponentAcceptedInvitationStartGameCommand { get; set; }
        public RelayCommand LobbyOverlayOpponentCanceledInvitationCloseCommand { get; set; }
        public RelayCommand LobbyOverlayWaitingForInvitationAcceptionCancelCommand { get; set; }
        #endregion

        #region Command Actions
        private async void LobbyRefreshAction()
        {
            if (LobbyOverlayWaitingForInvitationAcceptedVisibility != "Visible")
            {
                var players = await WebApiClientPlayersCommands.GetAllPlayersAsync();

                if (LocalPlayer != null)
                {
                    PlayerList = new ObservableCollection<Player>(players.Where(a => a.Name != LocalPlayer.Name).ToList());
                    InvitationList = await WebApiClientInvitationsCommands.GetPlayerInvitationsAsync(LocalPlayer.Id);
                }
            }
        }
        private void LobbyCloseAction()
        {
            globals.LobbyWindow.Close();
            globals.LobbyWindow = null;
        }
        private async void OnLobbyClosingAction()
        {
            globals.LobbyWindow = null;
            if (LocalPlayer != null)
            {
                await WebApiClientPlayersCommands.DeletePlayerAsync(LocalPlayer.Id);

                if (LobbyOverlayWaitingForInvitationAcceptedVisibility == "Visible")
                {
                    ChangePropertyStringValueSideMenuViewModel("SideMenuEndOnlineGameButtonVisibility", "Hidden");
                    ChangePropertyStringValueSideMenuViewModel("SideMenuOnlineGameButtonVisibility", "Visible");

                    LobbyOverlayWaitingForInvitationAcceptedVisibility = "Hidden";

                    await WebApiClientInvitationsCommands.CancelInvitationAsync(Opponent.Id, LocalPlayer);

                    Opponent = null;
                }
            }
            if (!globals.IsOnlineGame)
            {
                ChangePropertyStringValueSideMenuViewModel("SideMenuEndOnlineGameButtonVisibility", "Hidden");
                ChangePropertyStringValueSideMenuViewModel("SideMenuOnlineGameButtonVisibility", "Visible");
            }
        }
        private async void LobbyInviteAction()
        {
            var selectedInfo = dataGridLobbyAllPlayers.SelectedCells[0];
            string selectedPlayerName = ((TextBlock)selectedInfo.Column.GetCellContent(selectedInfo.Item)).Text;
            Opponent = PlayerList.Where(a => a.Name == selectedPlayerName).FirstOrDefault();
            OnPropertyChangedByPropertyName("Opponent");

            LobbyOverlayWaitingForInvitationAcceptedVisibility = "Visible";

            await WebApiClientInvitationsCommands.InvitePlayerAsync(Opponent.Id, LocalPlayer);

            globals.CurrentOnlineGame = new Game();

            BackgroundThreads.LobbyKeepCheckingForOpponentAcception();
        }
        private async void LobbyAcceptInvitationAction()
        {
            ChangePropertyStringValueSideMenuViewModel("SideMenuEndOnlineGameButtonVisibility", "Visible");
            ChangePropertyStringValueSideMenuViewModel("SideMenuOnlineGameButtonVisibility", "Hidden");

            var selectedInfo = dataGridLobbyInvitations.SelectedCells[0];
            string selectedPlayerName = ((TextBlock)selectedInfo.Column.GetCellContent(selectedInfo.Item)).Text;
            var selectedPlayer = PlayerList.Where(a => a.Name == selectedPlayerName).FirstOrDefault();

            Opponent = selectedPlayer;
            OnPropertyChangedByPropertyName("Opponent");

            var invitations = await WebApiClientInvitationsCommands.GetPlayerInvitationsAsync(LocalPlayer.Id);
            var opp = invitations.Where(a => a.Id == Opponent.Id).FirstOrDefault();

            if (opp != null)
            {
                globals.CurrentOnlineGame = new Game
                {
                    BlackId = selectedPlayer.Id,
                    WhiteId = LocalPlayer.Id
                };

                globals.CurrentOnlineGame = await WebApiClientGamesCommands.StartNewGameAsync(globals.CurrentOnlineGame);

                globals.LobbyWindow.Close();
                globals.LobbyWindow = null;

                LocalPlayer.Color = "White";

                globals.IsOnlineGame = true;

                ChangePropertyStringValueMainWindowViewModel("LabelMoveInfo", "It's white's turn...");

                WeakReferenceMessenger.Default.Send(
                    new MainWindowViewModel.StartGameMessage(false));

                Services.BackgroundThreads.OnlineGameKeepResettingBlackInactiveCounter();
            }
            else
            {
                LobbyOverlayOpponentCanceledInvitationVisibility = "Visible";
            }
        }
        private void LobbyKeyboardAction(object o)
        {
            if ((string)o == "Enter")
            {
                if (hasLobbyOverlayPlayerNameTextBoxFocus && LobbyOverLayPlayerNameTextBox != "")
                {
                    LobbyOverlayPlayerNameOkAction();
                }
            }
        }
        private void OnLobbyDataGridAllPlayersLoadedAction(object o)
        {
            dataGridLobbyAllPlayers = (DataGrid)o;
        }
        private void OnLobbyDataGridInvitationsLoadedAction(object o)
        {
            dataGridLobbyInvitations = (DataGrid)o;
        }
        private void OnLobbyDataGridAllPlayersSelectedCellsChangedAction()
        {
            if (dataGridLobbyAllPlayers.SelectedCells.Count == 1)
            {
                var selectedInfo = dataGridLobbyAllPlayers.SelectedCells[0];
                var selectedCell = (TextBlock)selectedInfo.Column.GetCellContent(selectedInfo.Item);
                if (selectedCell != null)
                {
                    LobbyInviteButtonIsEnabled = "True";
                }
                else
                {
                    LobbyInviteButtonIsEnabled = "False";
                }
            }
            else
            {
                LobbyInviteButtonIsEnabled = "False";
            }
        }
        private void OnLobbyDataGridInvitationsSelectedCellsChangedAction()
        {
            if (dataGridLobbyInvitations.SelectedCells.Count == 1)
            {
                var selectedInfo = dataGridLobbyInvitations.SelectedCells[0];
                var selectedCell = (TextBlock)selectedInfo.Column.GetCellContent(selectedInfo.Item);
                if (selectedCell != null)
                {
                    LobbyAcceptInvitationButtonIsEnabled = "True";
                }
                else
                {
                    LobbyAcceptInvitationButtonIsEnabled = "False";
                }
            }
            else
            {
                LobbyAcceptInvitationButtonIsEnabled = "False";
            }
        }
        private void OnLobbyOverlayPlayerNameTextBoxFocusAction(object o)
        {
            string hasFocusString = (string)o;
            hasLobbyOverlayPlayerNameTextBoxFocus = hasFocusString == "True";
        }
        private async void LobbyOverlayPlayerNameOkAction()
        {
            if (LocalPlayer == null)
            {
                LocalPlayer = new Player();
            }

            LocalPlayer.Name = LobbyOverLayPlayerNameTextBox;
            OnPropertyChangedByPropertyName("LocalPlayer");

            Player createPlayerResult = new Player();

            try
            {
                createPlayerResult = await WebApiClientPlayersCommands.CreatePlayerAsync(LocalPlayer);
            }
            catch
            {
                MessageBox.Show(globals.LobbyWindow, "Please try again later...", "Error!");
                globals.LobbyWindow.Close();
                globals.LobbyWindow = null;
            }

            if (createPlayerResult.Name == null)
            {
                LabelPlayerNameConflict = "This name is already taken!";

                LocalPlayer = null;
            }
            else
            {
                LabelPlayerNameConflict = "";
                LobbyOverlayPlayerNameVisibility = "Hidden";

                LocalPlayer = createPlayerResult;
            }
        }
        private void LobbyOverlayPlayerNameCancelAction()
        {
            LobbyOverLayPlayerNameTextBox = "";
            LabelPlayerNameConflict = "";
            LobbyOverlayPlayerNameVisibility = "Hidden";
        }
        private void LobbyOverlayOpponentAcceptedInvitationStartGameAction()
        {
            LobbyOverlayOpponentAcceptedInvitationVisibility = "Hidden";
        }
        private void LobbyOverlayOpponentCanceledInvitationCloseAction()
        {
            LobbyOverlayOpponentCanceledInvitationVisibility = "Hidden";

            Opponent = null;
        }
        private async void LobbyOverlayWaitingForInvitationAcceptionCancelAction()
        {
            LobbyOverlayWaitingForInvitationAcceptedVisibility = "Hidden";

            await WebApiClientInvitationsCommands.CancelInvitationAsync(Opponent.Id, LocalPlayer);

            Opponent = null;
        }
        #endregion

        #region Methods
        private void InititalizeCommands()
        {
            LobbyRefreshCommand = new RelayCommand(LobbyRefreshAction);
            LobbyCloseCommand = new RelayCommand(LobbyCloseAction);
            LobbyInviteCommand = new RelayCommand(LobbyInviteAction);
            LobbyAcceptInvitationCommand = new RelayCommand(LobbyAcceptInvitationAction);
            LobbyKeyboardCommand = new RelayCommand<object>(o => LobbyKeyboardAction(o));

            OnLobbyDataGridAllPlayersLoadedCommand = new RelayCommand<object>(o => OnLobbyDataGridAllPlayersLoadedAction(o));
            OnLobbyDataGridInvitationsLoadedCommand = new RelayCommand<object>(o => OnLobbyDataGridInvitationsLoadedAction(o));
            OnLobbyDataGridAllPlayersSelectedCellsChangedCommand = new RelayCommand(OnLobbyDataGridAllPlayersSelectedCellsChangedAction);
            OnLobbyDataGridInvitationsSelectedCellsChangedCommand = new RelayCommand(OnLobbyDataGridInvitationsSelectedCellsChangedAction);
            OnLobbyClosingCommand = new RelayCommand(OnLobbyClosingAction);

            OnLobbyOverlayPlayerNameTextBoxFocusCommand = new RelayCommand<object>(o => OnLobbyOverlayPlayerNameTextBoxFocusAction(o));

            LobbyOverlayPlayerNameOkCommand = new RelayCommand(LobbyOverlayPlayerNameOkAction);
            LobbyOverlayPlayerNameCancelCommand = new RelayCommand(LobbyOverlayPlayerNameCancelAction);

            LobbyOverlayOpponentAcceptedInvitationStartGameCommand = new RelayCommand(LobbyOverlayOpponentAcceptedInvitationStartGameAction);
            LobbyOverlayOpponentCanceledInvitationCloseCommand = new RelayCommand(LobbyOverlayOpponentCanceledInvitationCloseAction);
            LobbyOverlayWaitingForInvitationAcceptionCancelCommand = new RelayCommand(LobbyOverlayWaitingForInvitationAcceptionCancelAction);
        }
        private void InitializeMessageHandlers()
        {
            //WeakReferenceMessenger.Default.Register<LobbyViewModel, LobbyOverlayWaitingForInvitationAcceptedVisibilityRequestMessage>(this, (r, m) =>
            //{
            //    m.Reply(r.LobbyOverlayWaitingForInvitationAcceptedVisibility);
            //});

            WeakReferenceMessenger.Default.Register<PropertyStringValueChangedMessage>(this, (r, m) =>
            {
                var propertyName = GetType().GetProperty(m.Value.Item1);
                propertyName.SetValue(this, m.Value.Item2);
            });

            WeakReferenceMessenger.Default.Register<PropertyPlayerListValueChangedMessage>(this, (r, m) =>
            {
                PlayerList = m.Value;
            });

            WeakReferenceMessenger.Default.Register<PropertyInvitationListValueChangedMessage>(this, (r, m) =>
            {
                InvitationList = m.Value;
            });
        }
        #endregion

        #region Message Handlers
        // RequestMessage Handlers:
        //internal class LobbyOverlayWaitingForInvitationAcceptedVisibilityRequestMessage : RequestMessage<string> { }

        // ValueChangedMessage Handlers:
        internal class PropertyStringValueChangedMessage : ValueChangedMessage<Tuple<string, string>>
        {
            internal PropertyStringValueChangedMessage(Tuple<string, string> tuple) : base(tuple) { }
        }
        internal class OnPropertyChangedMessage : ValueChangedMessage<string>
        {
            internal OnPropertyChangedMessage(string propertyName) : base(propertyName) { }
        }
        internal class PropertyPlayerListValueChangedMessage : ValueChangedMessage<ObservableCollection<Player>>
        {
            internal PropertyPlayerListValueChangedMessage(ObservableCollection<Player> playerList) : base(playerList) { }
        }
        internal class PropertyInvitationListValueChangedMessage : ValueChangedMessage<ObservableCollection<Player>>
        {
            internal PropertyInvitationListValueChangedMessage(ObservableCollection<Player> invitationList) : base(invitationList) { }
        }
        #endregion
    }
}
