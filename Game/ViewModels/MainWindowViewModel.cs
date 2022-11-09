using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http.Headers;
using System.Collections.ObjectModel;
using System.Threading;
using System.Data;

using ChessDotNET.GameLogic;
using ChessDotNET.WebApiClient;
using ChessDotNET.Models;
using ChessDotNET.ViewHelpers;
using ChessDotNET.Views;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ChessDotNET.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        #region Constructors
        public MainWindowViewModel()
        {
            promotePawnList = new List<ImageSource>()
            {
                ChessPieceImages.WhiteBishop,
                ChessPieceImages.WhiteKnight,
                ChessPieceImages.WhiteRook,
                ChessPieceImages.WhiteQueen
            };

            globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();

            globals.httpClient.DefaultRequestHeaders.Accept.Clear();
            globals.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            InitializeCommands();

            StartGame(false);
            //debugNoTurns = true;

            WeakReferenceMessenger.Default.Register<MainWindowViewModel, PropertyLocalPlayerValueRequestMessage>(this, (r, m) =>
            {
                m.Reply(r.LocalPlayer);
            });

            WeakReferenceMessenger.Default.Register<PropertyStringValueChangedMessage>(this, (r, m) =>
            {
                if (m.Value.Item1 == "OverlayOnlineGamePlayerQuitVisibility")
                {
                    OverlayOnlineGamePlayerQuitVisibility = m.Value.Item2;
                }
            });

            WeakReferenceMessenger.Default.Register<OnPropertyChangedMessage>(this, (r, m) =>
            {
                OnPropertyChangedByPropertyName(m.Value);
            });
        }
        #endregion Constuctors

        #region Fields
        private Globals globals;
        private Canvas chessCanvas;
        private Image currentlyDraggedChessPieceImage;
        private int currentlyDraggedChessPieceOriginalCanvasLeft;
        private int currentlyDraggedChessPieceOriginalCanvasTop;
        private Point dragOverCanvasPosition;
        private Point dragOverChessPiecePosition;
        private bool isMouseMoving;
        private bool wasSideMenuOpen = false;
        private Coords promotePawnCoords;
        private readonly bool debugNoTurns = false;
        private static DataGrid dataGridLobbyAllPlayers = null;
        private static DataGrid dataGridLobbyInvitations = null;
        private bool hasLobbyOverlayPlayerNameTextBoxFocus;
        #endregion Fields

        #region Bindable Properties

        private Player localPlayer = null;
        public Player LocalPlayer {
            get => localPlayer;
            set { localPlayer = value; OnPropertyChanged(); } }

        private Player opponent = null;
        public Player Opponent {
            get => opponent;
            set { opponent = value; OnPropertyChanged(); } }

        private TileDictionary tileDict;
        public TileDictionary TileDict {
            get => tileDict; 
            set { tileDict = value; OnPropertyChanged(); } }

        private List<ImageSource> promotePawnList;
        public List<ImageSource> OverlayPromotePawnList { 
            get => promotePawnList; 
            set { promotePawnList = value; OnPropertyChanged(); } }

        private List<string> horizontalNotationList;
        public List<string> HorizontalNotationList { 
            get => horizontalNotationList; 
            set { horizontalNotationList = value; OnPropertyChanged(); } }

        private List<string> verticalNotationList;
        public List<string> VerticalNotationList { 
            get => verticalNotationList; 
            set { verticalNotationList = value; OnPropertyChanged(); } }

        public ObservableCollection<Player> playerList;
        public ObservableCollection<Player> PlayerList {
            get => playerList;
            set { playerList = value; OnPropertyChanged(); } }

        public ObservableCollection<Player> invitationList;
        public ObservableCollection<Player> InvitationList {
            get => invitationList;
            set { invitationList = value; OnPropertyChanged(); } }

        private string labelMoveInfo = "";
        public string LabelMoveInfo {
            get => labelMoveInfo;
            set { labelMoveInfo = value; OnPropertyChanged(); } }

        private string overlayPromotePawnVisibility = "Hidden";
        public string OverlayPromotePawnVisibility {
            get => overlayPromotePawnVisibility;
            set { overlayPromotePawnVisibility = value; OnPropertyChanged(); } }

        private string overlayOnlineGamePlayerQuitVisibility = "Hidden";
        public string OverlayOnlineGamePlayerQuitVisibility {
            get => overlayOnlineGamePlayerQuitVisibility;
            set { overlayOnlineGamePlayerQuitVisibility = value; OnPropertyChanged(); } }

        private string chessCanvasRotationAngle = "0";
        public string ChessCanvasRotationAngle {
            get => chessCanvasRotationAngle;
            set { chessCanvasRotationAngle = value; OnPropertyChanged(); } }

        private string chessCanvasRotationCenterX = "9";
        public string ChessCanvasRotationCenterX {
            get => chessCanvasRotationCenterX;
            set { chessCanvasRotationCenterX = value; OnPropertyChanged(); } }

        private string chessCanvasRotationCenterY = "-200";
        public string ChessCanvasRotationCenterY {
            get => chessCanvasRotationCenterY;
            set { chessCanvasRotationCenterY = value; OnPropertyChanged(); } }

        // opens the side menu:
        private string sideMenuVisibility = "Hidden";
        public string SideMenuVisibility { 
            get => sideMenuVisibility; 
            set { sideMenuVisibility = value; OnPropertyChanged(); } }

        // TODO: move to SideMenuViewModel
        private string sideMenuMainVisibility = "Visible";
        public string SideMenuMainVisibility { 
            get => sideMenuMainVisibility; 
            set { sideMenuMainVisibility = value; OnPropertyChanged(); } }

        private string sideMenuGameModeVisibility = "Hidden";
        public string SideMenuGameModeVisibility { 
            get => sideMenuGameModeVisibility; 
            set { sideMenuGameModeVisibility = value; OnPropertyChanged(); } }
        
        private string sideMenuLocalGameVisibility = "Hidden";
        public string SideMenuLocalGameVisibility { 
            get => sideMenuLocalGameVisibility; 
            set { sideMenuLocalGameVisibility = value; OnPropertyChanged(); } }

        private string sideMenuOnlineGameVisibility = "Hidden";
        public string SideMenuOnlineGameVisibility { 
            get => sideMenuOnlineGameVisibility; 
            set { sideMenuOnlineGameVisibility = value; OnPropertyChanged(); } }

        private string sideMenuOnlineGameButtonVisibility = "Visible";
        public string SideMenuOnlineGameButtonVisibility { 
            get => sideMenuOnlineGameButtonVisibility; 
            set { sideMenuOnlineGameButtonVisibility = value; OnPropertyChanged(); } }

        private string sideMenuEndOnlineGameButtonVisibility = "Hidden";
        public string SideMenuEndOnlineGameButtonVisibility { 
            get => sideMenuEndOnlineGameButtonVisibility; 
            set { sideMenuEndOnlineGameButtonVisibility = value; OnPropertyChanged(); } }
        // ENDTODO

        // TODO: move to LobbyViewModel
        private string lobbyOverlayPlayerNameVisibility = "Hidden";
        public string LobbyOverlayPlayerNameVisibility { 
            get => lobbyOverlayPlayerNameVisibility; 
            set { lobbyOverlayPlayerNameVisibility = value; OnPropertyChanged(); } }

        private string lobbyOverlayWaitingForInvitationAcceptedVisibility = "Hidden";
        public string LobbyOverlayWaitingForInvitationAcceptedVisibility { 
            get => lobbyOverlayWaitingForInvitationAcceptedVisibility; 
            set { lobbyOverlayWaitingForInvitationAcceptedVisibility = value; OnPropertyChanged(); } }

        private string lobbyOverlayOpponentAcceptedInvitationVisibility = "Hidden";
        public string LobbyOverlayOpponentAcceptedInvitationVisibility { 
            get => lobbyOverlayOpponentAcceptedInvitationVisibility; 
            set { lobbyOverlayOpponentAcceptedInvitationVisibility = value; OnPropertyChanged(); } }

        private string lobbyOverlayOpponentCanceledInvitationVisibility = "Hidden";
        public string LobbyOverlayOpponentCanceledInvitationVisibility { 
            get => lobbyOverlayOpponentCanceledInvitationVisibility; 
            set { lobbyOverlayOpponentCanceledInvitationVisibility = value; OnPropertyChanged(); } }

        private string labelPlayerNameConflict = "";
        public string LabelPlayerNameConflict { 
            get => labelPlayerNameConflict; 
            set { labelPlayerNameConflict = value; OnPropertyChanged(); } }

        private string lobbyOverlayPlayerNameOkButtonIsEnabled = "False";
        public string LobbyOverlayPlayerNameOkButtonIsEnabled { 
            get => lobbyOverlayPlayerNameOkButtonIsEnabled; 
            set { lobbyOverlayPlayerNameOkButtonIsEnabled = value; OnPropertyChanged(); } }

        private string lobbyInviteButtonIsEnabled = "False";
        public string LobbyInviteButtonIsEnabled { 
            get => lobbyInviteButtonIsEnabled; 
            set { lobbyInviteButtonIsEnabled = value; OnPropertyChanged(); } }

        private string lobbyAcceptInvitationButtonIsEnabled = "False";
        public string LobbyAcceptInvitationButtonIsEnabled { 
            get => lobbyAcceptInvitationButtonIsEnabled; 
            set { lobbyAcceptInvitationButtonIsEnabled = value; OnPropertyChanged(); } }

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
        // ENDTODO
        #endregion Bindable Properties

        #region Commands
        public RelayCommand OpenSideMenuCommand { get; set; }
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
        public RelayCommand OnMainWindowClosingCommand { get; set; }
        public RelayCommand LobbyInviteCommand { get; set; }
        public RelayCommand LobbyAcceptInvitationCommand { get; set; }
        public RelayCommand LobbyRefreshCommand { get; set; }
        public RelayCommand LobbyCloseCommand { get; set; }
        public RelayCommand<object> LobbyKeyboardCommand { get; set; }
        public RelayCommand<object> OnLobbyDataGridAllPlayersLoadedCommand { get; set; }
        public RelayCommand<object> OnLobbyDataGridInvitationsLoadedCommand { get; set; }
        public RelayCommand OnLobbyDataGridAllPlayersSelectedCellsChangedCommand { get; set; }
        public RelayCommand OnLobbyDataGridInvitationsSelectedCellsChangedCommand { get; set; }
        public RelayCommand OnLobbyClosingCommand { get; set; }
        public RelayCommand LobbyOverlayPlayerNameOkCommand { get; set; }
        public RelayCommand LobbyOverlayPlayerNameCancelCommand { get; set; }
        public RelayCommand LobbyOverlayOpponentAcceptedInvitationStartGameCommand { get; set; }
        public RelayCommand LobbyOverlayOpponentCanceledInvitationCloseCommand { get; set; }
        public RelayCommand LobbyOverlayWaitingForInvitationAcceptionCancelCommand { get; set; }
        public RelayCommand<object> OnMainWindowMouseMoveCommand { get; set; }
        public RelayCommand<object> OnMainWindowMouseLeftDownCommand { get; set; }
        public RelayCommand<object> OnMainWindowMouseLeftUpCommand { get; set; }
        public RelayCommand<object> OnChessPieceMouseLeftDownCommand { get; set; }
        public RelayCommand<object> OnLobbyOverlayPlayerNameTextBoxFocusCommand { get; set; }
        public RelayCommand<object> OverlayPromotePawnSelectChessPieceCommand { get; set; }
        public RelayCommand OverlayOnlineGamePlayerQuitOkCommand { get; set; }
        #endregion Commands

        #region CommandActions
        private void OnMainWindowClosingAction()
        {
            if (globals.LobbyWindow != null)
            {
                globals.LobbyWindow.Close();
            }
        }
        private void OpenSideMenuAction()
        {
            if (!wasSideMenuOpen)
            {
                if (SideMenuVisibility != "Visible"
                    && OverlayPromotePawnVisibility == "Hidden")
                {
                    SideMenuGameModeVisibility = "Hidden";
                    SideMenuLocalGameVisibility = "Hidden";
                    SideMenuOnlineGameVisibility = "Hidden";
                    SideMenuMainVisibility = "Visible";
                    SideMenuVisibility = "Visible";
                }
                else SideMenuVisibility = "Hidden";
            }
            else
            {
                wasSideMenuOpen = false;
            }
        }
        private void OnMainWindowMouseMoveAction(object o)
        {
            MouseEventArgs e = o as MouseEventArgs;

            if (currentlyDraggedChessPieceImage != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {

                    if (!wasSideMenuOpen)
                    {
                        if (!isMouseMoving)
                        {
                            dragOverCanvasPosition = e.GetPosition(chessCanvas);
                            dragOverChessPiecePosition = e.GetPosition(currentlyDraggedChessPieceImage);
                        }
                        isMouseMoving = true;
                        dragOverCanvasPosition = e.GetPosition(chessCanvas);
                        currentlyDraggedChessPieceImage.SetValue(Panel.ZIndexProperty, 20);

                        Canvas.SetLeft(currentlyDraggedChessPieceImage, dragOverCanvasPosition.X - dragOverChessPiecePosition.X);
                        Canvas.SetTop(currentlyDraggedChessPieceImage, dragOverCanvasPosition.Y - dragOverChessPiecePosition.Y);
                    }
                }
            }
            e.Handled = true;
        }
        private void OnMainWindowMouseLeftDownAction(object o)
        {
            var e = o as MouseEventArgs;

            if (e.Source != null)
            {
                if (e.Source.ToString() != "ChessDotNET.Views.SideMenu")
                {
                    if (SideMenuVisibility == "Visible")
                    {
                        wasSideMenuOpen = true;
                        SideMenuVisibility = "Hidden";
                    }
                    else
                    {
                        wasSideMenuOpen = false;
                    }
                }
            }
        }
        private void OnChessPieceMouseleftDownAction(object o)
        {
            if (IsInputAllowed())
            {
                object param = ((CompositeCommandParameter)o).Parameter;
                MouseEventArgs e = ((CompositeCommandParameter)o).EventArgs as MouseEventArgs;
                currentlyDraggedChessPieceImage = null;
                currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
                currentlyDraggedChessPieceOriginalCanvasTop = -1000;
                currentlyDraggedChessPieceImage = param as Image;
                ChessPieceColor currentlyDraggedChessPieceColor = ChessPieceImages.GetImageColor(currentlyDraggedChessPieceImage.Source);
                bool isFirstTurn = globals.MoveList.Count == 0;
                bool isInputAllowed = true;

                if (isFirstTurn)
                {
                    if (currentlyDraggedChessPieceColor == ChessPieceColor.Black)
                    {
                        isInputAllowed = false;
                    }
                }
                else
                {
                    ChessPieceColor lastMoveColor = globals.MoveList[globals.MoveList.Count - 1].ChessPieceColor;
                    if (currentlyDraggedChessPieceColor == lastMoveColor)
                    {
                        isInputAllowed = false;
                    }
                }

                if (!ChessPieceImages.IsEmpty(currentlyDraggedChessPieceImage.Source)
                    && isInputAllowed)
                {
                    chessCanvas = VisualTreeHelper.GetParent(param as Image) as Canvas;

                    if (currentlyDraggedChessPieceOriginalCanvasLeft < 0 && currentlyDraggedChessPieceOriginalCanvasTop < 0)
                    {
                        currentlyDraggedChessPieceOriginalCanvasLeft = int.Parse(
                            currentlyDraggedChessPieceImage.GetValue(Canvas.LeftProperty).ToString()
                            );
                        currentlyDraggedChessPieceOriginalCanvasTop = int.Parse(
                            currentlyDraggedChessPieceImage.GetValue(Canvas.TopProperty).ToString()
                            );
                    }
                    currentlyDraggedChessPieceImage.CaptureMouse();
                }
                else
                {
                    currentlyDraggedChessPieceImage = null;                
                }

                wasSideMenuOpen = false;
                e.Handled = true;
            }
        }
        private async void OnMainWindowMouseLeftUpAction(object o)
        {
            if (IsInputAllowed())
            {
                MouseEventArgs e = o as MouseEventArgs;

                if (currentlyDraggedChessPieceImage == null) return;
                if (currentlyDraggedChessPieceImage.IsMouseCaptured) currentlyDraggedChessPieceImage.ReleaseMouseCapture();

                if (isMouseMoving)
                {
                    isMouseMoving = false;
                    if (dragOverCanvasPosition.X < 0
                        || dragOverCanvasPosition.X > 400
                        || dragOverCanvasPosition.Y < 0
                        || dragOverCanvasPosition.Y > 400)
                    {
                        Canvas.SetLeft(currentlyDraggedChessPieceImage, currentlyDraggedChessPieceOriginalCanvasLeft);
                        Canvas.SetTop(currentlyDraggedChessPieceImage, currentlyDraggedChessPieceOriginalCanvasTop);
                        currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
                        currentlyDraggedChessPieceOriginalCanvasTop = -1000;
                    }
                    else
                    {
                        Point oldPoint = new Point(currentlyDraggedChessPieceOriginalCanvasLeft,
                            currentlyDraggedChessPieceOriginalCanvasTop);
                        Coords oldCoords = Coords.CanvasPositionToCoords(oldPoint);
                        Coords newCoords = Coords.CanvasPositionToCoords(dragOverCanvasPosition);

                        if (newCoords.X >= 1 && newCoords.X <= 8 && newCoords.Y >= 1 && newCoords.Y <= 8
                            && !(newCoords.X == oldCoords.X && newCoords.Y == oldCoords.Y))
                        {
                            ChessPiece currentlyDraggedChessPiece = TileDict[oldCoords.String].ChessPiece;
                            ChessPieceColor currentlyDraggedChessPieceColor = currentlyDraggedChessPiece.ChessPieceColor;
                            ChessPieceType currentlyDraggedChessPieceType = currentlyDraggedChessPiece.ChessPieceType;

                            MoveValidationData moveValidationData = MoveValidationGameLogic.ValidateCurrentMove(
                                TileDict, oldCoords, newCoords);

                            bool isFirstMoveValid = true;
                            if (globals.MoveList.Count == 0)
                            {
                                isFirstMoveValid = currentlyDraggedChessPieceColor == ChessPieceColor.White;
                            }

                            bool isTurnCorrectColor = true;
                            if (globals.MoveList.Count > 0)
                            {
                                isTurnCorrectColor = globals.MoveList[globals.MoveList.Count - 1].ChessPieceColor != currentlyDraggedChessPieceColor;
                            }

                            if (debugNoTurns)
                            {
                                isTurnCorrectColor = true;
                            }

                            if (isFirstMoveValid
                                && isTurnCorrectColor
                                && moveValidationData.IsValid)
                            {
                                // reset the currently dragged image's position:
                                Canvas.SetLeft(currentlyDraggedChessPieceImage, currentlyDraggedChessPieceOriginalCanvasLeft);
                                Canvas.SetTop(currentlyDraggedChessPieceImage, currentlyDraggedChessPieceOriginalCanvasTop);

                                // can an opponent's pawn be captured en passant?
                                if (moveValidationData.CanCaptureEnPassant)
                                {
                                    TileDict[TileDict.CoordsPawnMovedTwoTiles.String].ChessPiece = new ChessPiece();
                                    TileDict[TileDict.CoordsPawnMovedTwoTiles.String].IsOccupied = false;
                                }

                                // has a pawn moved two tiles at once? Store its coords for the next turn...
                                if (moveValidationData.MovedTwoTiles)
                                {
                                    TileDict.CoordsPawnMovedTwoTiles = moveValidationData.Coords[0];
                                }
                                else if (! globals.IsOnlineGame)
                                {
                                    TileDict.CoordsPawnMovedTwoTiles = null;
                                }

                                // promote your pawn if it is on the opposite of the field:
                                bool canPromote = PromotePawnGameLogic.CanPromote(TileDict, oldCoords, newCoords);
                                if (canPromote)
                                {
                                    TileDict.MoveChessPiece(oldCoords, newCoords, true);
                                    promotePawnCoords = newCoords;

                                    if (currentlyDraggedChessPieceColor == ChessPieceColor.White)
                                    {
                                        OverlayPromotePawnList = new List<ImageSource>()
                                        {
                                            ChessPieceImages.WhiteBishop,
                                            ChessPieceImages.WhiteKnight,
                                            ChessPieceImages.WhiteRook,
                                            ChessPieceImages.WhiteQueen
                                        };
                                    }
                                    else
                                    {
                                        OverlayPromotePawnList = new List<ImageSource>()
                                        {
                                            ChessPieceImages.BlackBishop,
                                            ChessPieceImages.BlackKnight,
                                            ChessPieceImages.BlackRook,
                                            ChessPieceImages.BlackQueen
                                        };
                                    }

                                    OverlayPromotePawnVisibility = "Visible";
                                }

                                // check if a king tries to castle:
                                else if (moveValidationData.CanCastle)
                                {
                                    TileDict.MoveChessPiece(oldCoords, newCoords, true);
                                    Coords rookOldCoords = moveValidationData.Coords[0];
                                    Coords rookNewCoords = moveValidationData.Coords[1];
                                    TileDict.MoveChessPiece(rookOldCoords, rookNewCoords, true);
                                }
                                else
                                {
                                    TileDict.MoveChessPiece(oldCoords, newCoords, true);
                                }

                                OnPropertyChangedByPropertyName("TileDict");

                                string labelMoveInfoText = oldCoords.String + " -> " + newCoords.String;

                                if (ThreateningValidationGameLogic.IsTileThreatened(
                                    TileDict, ChessPieceColor.White, TileDict.WhiteKingCoords, false))
                                {
                                    labelMoveInfoText += ", White king is in check!";
                                }
                                else if (ThreateningValidationGameLogic.IsTileThreatened(
                                    TileDict, ChessPieceColor.Black, TileDict.BlackKingCoords, false))
                                {
                                    labelMoveInfoText += ", Black king is in check!";
                                }

                                if (currentlyDraggedChessPieceColor == ChessPieceColor.Black)
                                {
                                    labelMoveInfoText += " - It's white's turn...";
                                }
                                else
                                {
                                    labelMoveInfoText += " - It's black's turn...";
                                }

                                if (currentlyDraggedChessPieceColor == ChessPieceColor.Black)
                                {
                                    if (CheckMateValidationGameLogic.IsCheckMate(TileDict, TileDict.WhiteKingCoords))
                                    {
                                        labelMoveInfoText = oldCoords.String + " -> " + newCoords.String + ", White is check mate!";
                                        globals.IsCheckMate = true;
                                    }
                                }
                                else if (currentlyDraggedChessPieceColor == ChessPieceColor.White)
                                {
                                    if (CheckMateValidationGameLogic.IsCheckMate(TileDict, TileDict.BlackKingCoords))
                                    {
                                        labelMoveInfoText = oldCoords.String + " -> " + newCoords.String + ", Black is check mate!";
                                        globals.IsCheckMate = true;
                                    }
                                }

                                LabelMoveInfo = labelMoveInfoText;

                                globals.MoveList.Add(new Move(oldCoords, newCoords, currentlyDraggedChessPieceColor, currentlyDraggedChessPieceType));

                                if (globals.IsOnlineGame)
                                {
                                    if (LocalPlayer.Color == "White")
                                    {
                                        globals.CurrentOnlineGame.LastMoveStartWhite = oldCoords.String;
                                        globals.CurrentOnlineGame.LastMoveEndWhite = newCoords.String;

                                        if (moveValidationData.CanCastle)
                                        {
                                            globals.CurrentOnlineGame.LastMoveStartWhite += "C" + moveValidationData.Coords[0].String;
                                            globals.CurrentOnlineGame.LastMoveEndWhite   += "C" + moveValidationData.Coords[1].String;
                                        }
                                        else if (moveValidationData.MovedTwoTiles)
                                        {
                                            globals.CurrentOnlineGame.LastMoveStartWhite += "T" + TileDict.CoordsPawnMovedTwoTiles.String;
                                        }
                                        else if (moveValidationData.CanCaptureEnPassant)
                                        {
                                            globals.CurrentOnlineGame.LastMoveStartWhite += "E" + TileDict.CoordsPawnMovedTwoTiles.String;
                                            TileDict.CoordsPawnMovedTwoTiles = null;
                                        }
                                        else if (canPromote)
                                        {
                                            globals.CurrentOnlineGame.LastMoveStartWhite += "P";
                                        }
                                        globals.CurrentOnlineGame.LastMoveStartBlack = null;
                                        globals.CurrentOnlineGame.LastMoveEndBlack = null;
                                        globals.CurrentOnlineGame.MoveInfo = LabelMoveInfo;
                                    }
                                    else
                                    {
                                        globals.CurrentOnlineGame.LastMoveStartBlack = oldCoords.String;
                                        globals.CurrentOnlineGame.LastMoveEndBlack = newCoords.String;

                                        if (moveValidationData.CanCastle)
                                        {
                                            globals.CurrentOnlineGame.LastMoveStartBlack += "C" + moveValidationData.Coords[0].String;
                                            globals.CurrentOnlineGame.LastMoveEndBlack   += "C" + moveValidationData.Coords[1].String;
                                        }
                                        else if (moveValidationData.MovedTwoTiles)
                                        {
                                            globals.CurrentOnlineGame.LastMoveStartBlack += "T" + TileDict.CoordsPawnMovedTwoTiles.String;
                                        }
                                        else if (moveValidationData.CanCaptureEnPassant)
                                        {
                                            globals.CurrentOnlineGame.LastMoveStartBlack += "E" + TileDict.CoordsPawnMovedTwoTiles.String;
                                            TileDict.CoordsPawnMovedTwoTiles = null;
                                        }
                                        else if (canPromote)
                                        {
                                            globals.CurrentOnlineGame.LastMoveStartBlack += "P";
                                        }

                                        globals.CurrentOnlineGame.LastMoveStartWhite = null;
                                        globals.CurrentOnlineGame.LastMoveEndWhite = null;
                                        globals.CurrentOnlineGame.MoveInfo = LabelMoveInfo;
                                    }

                                    if (!canPromote)
                                    {
                                        await WebApiClientGamesCommands.PutCurrentGame(globals.CurrentOnlineGame.Id, globals.CurrentOnlineGame);

                                        Services.BackgroundThreads.OnlineGameKeepCheckingForNextMove(globals, TileDict);
                                    }
                                }

                                //// Debug: Print occupation state of all tiles:
                                //for (int i = 8; i > 0; i--)
                                //{
                                //    for (int j = 1; j < 9; j++)
                                //    {
                                //        Coords c = new Coords(j, i);
                                //        char oc = tileDict[c.String].IsOccupied ? 'O' : ' ';
                                //        System.Diagnostics.Debug.Write(c.String + ":" + oc + " ");
                                //    }
                                //    System.Diagnostics.Debug.WriteLine("");
                                //}
                            }
                            else
                            {
                                Canvas.SetLeft(currentlyDraggedChessPieceImage, currentlyDraggedChessPieceOriginalCanvasLeft);
                                Canvas.SetTop(currentlyDraggedChessPieceImage, currentlyDraggedChessPieceOriginalCanvasTop);
                            }
                        }
                        else
                        {
                            Canvas.SetLeft(currentlyDraggedChessPieceImage, currentlyDraggedChessPieceOriginalCanvasLeft);
                            Canvas.SetTop(currentlyDraggedChessPieceImage, currentlyDraggedChessPieceOriginalCanvasTop);
                        }
                    }
                    currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
                    currentlyDraggedChessPieceOriginalCanvasTop = -1000;
                    currentlyDraggedChessPieceImage.SetValue(Panel.ZIndexProperty, 10);
                }
                currentlyDraggedChessPieceImage = null;
                e.Handled = true;
            }
        }
        private async void OverlayPromotePawnSelectChessPieceAction(object o)
        {
            string chessPieceString = (string)o;
            ChessPieceColor ownColor = TileDict[promotePawnCoords.String].ChessPiece.ChessPieceColor;
            ChessPiece chessPiece = null;

            if (chessPieceString == "Bishop")
                chessPiece = new ChessPiece(ownColor, ChessPieceType.Bishop, globals.IsRotated);
            else if (chessPieceString == "Knight")
                chessPiece = new ChessPiece(ownColor, ChessPieceType.Knight, globals.IsRotated);
            else if (chessPieceString == "Rook")
                chessPiece = new ChessPiece(ownColor, ChessPieceType.Rook, globals.IsRotated);
            else if (chessPieceString == "Queen")
                chessPiece = new ChessPiece(ownColor, ChessPieceType.Queen, globals.IsRotated);

            TileDict[promotePawnCoords.String].ChessPiece = chessPiece;
            promotePawnCoords = null;

            OverlayPromotePawnVisibility = "Hidden";

            OnPropertyChangedByPropertyName("TileDict");

            bool doPutCurrentGame = false;
            if (globals.IsOnlineGame)
            {
                if (globals.CurrentOnlineGame.LastMoveStartWhite != null)
                {
                    if (globals.CurrentOnlineGame.LastMoveStartWhite.Length > 2 && globals.CurrentOnlineGame.LastMoveStartWhite[2] == 'P')
                    {
                        doPutCurrentGame = true;
                        globals.CurrentOnlineGame.LastMoveStartWhite += chessPieceString;
                    }
                }
                else if (globals.CurrentOnlineGame.LastMoveStartBlack != null)
                {
                    if (globals.CurrentOnlineGame.LastMoveStartBlack.Length > 2 && globals.CurrentOnlineGame.LastMoveStartBlack[2] == 'P')
                    {
                        doPutCurrentGame = true;
                        globals.CurrentOnlineGame.LastMoveStartBlack += chessPieceString;
                    }
                }
                if (doPutCurrentGame)
                {
                    await WebApiClientGamesCommands.PutCurrentGame(globals.CurrentOnlineGame.Id, globals.CurrentOnlineGame);

                    Services.BackgroundThreads.OnlineGameKeepCheckingForNextMove(globals, TileDict);
                }
            }
        }
        private void OverlayOnlineGamePlayerQuitOkAction()
        {
            OverlayOnlineGamePlayerQuitVisibility = "Hidden";
            SideMenuEndOnlineGameButtonVisibility = "Hidden";
            SideMenuOnlineGameButtonVisibility = "Visible";

            Opponent = null;
            StartGame(false);
        }
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
            currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            currentlyDraggedChessPieceOriginalCanvasTop = -1000;

            SideMenuVisibility = "Hidden";
            SideMenuMainVisibility = "Visible";
            SideMenuGameModeVisibility = "Hidden";

            StartGame(false);
        }
        private void SideMenuLocalGameAsBlackAction()
        {
            currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            currentlyDraggedChessPieceOriginalCanvasTop = -1000;

            SideMenuVisibility = "Hidden";
            SideMenuMainVisibility = "Visible";
            SideMenuGameModeVisibility = "Hidden";

            StartGame(true);
        }
        private void SideMenuOnlineGameEnterLobbyAction()
        {
            SideMenuOnlineGameVisibility = "Hidden";
            SideMenuGameModeVisibility = "Hidden";
            SideMenuVisibility = "Hidden";

            if (globals.LobbyWindow== null)
            {
                globals.LobbyWindow= new Lobby
                {
                    DataContext = this
                };
                globals.LobbyWindow.Show();

                PlayerList = new ObservableCollection<Player>();
                InvitationList = new ObservableCollection<Player>();

                LobbyOverlayPlayerNameVisibility = "Visible";


                if (LocalPlayer != null)
                {
                    LobbyOverLayPlayerNameTextBox = LocalPlayer.Name;
                }

                var keepResettingCounterThreadStart = new ThreadStart(() => LobbyKeepResettingInactiveCounter());
                var keepResettingCounterBackgroundThread = new Thread(keepResettingCounterThreadStart)
                {
                    IsBackground = true
                };
                keepResettingCounterBackgroundThread.Start();
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

            StartGame(false);
        }

        private void LobbyKeepResettingInactiveCounter()
        {
            while (globals.LobbyWindow!= null)
            {
                Task.Run(async () =>
                {
                    if (LocalPlayer != null)
                    {
                        try
                        {
                            await WebApiClientPlayersCommands.ResetInactiveCounterAsync(LocalPlayer.Id);
                        }
                        catch
                        {
                            MessageBox.Show(globals.LobbyWindow, "Cannot contact server...", "Error!");
                            globals.LobbyWindow.Close();
                            globals.LobbyWindow= null;
                        }
                    }
                });
                Thread.Sleep(1000);
            }
        }
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
            globals.LobbyWindow= null;
        }
        private async void OnLobbyClosingAction()
        {
            globals.LobbyWindow= null;
            if (LocalPlayer != null)
            {
                await WebApiClientPlayersCommands.DeletePlayerAsync(LocalPlayer.Id);

                if (LobbyOverlayWaitingForInvitationAcceptedVisibility == "Visible")
                {
                    SideMenuEndOnlineGameButtonVisibility = "Hidden";
                    SideMenuOnlineGameButtonVisibility = "Visible";
                    LobbyOverlayWaitingForInvitationAcceptedVisibility = "Hidden";
                    await WebApiClientInvitationsCommands.CancelInvitationAsync(Opponent.Id, LocalPlayer);
                    Opponent = null;
                }
            }
            if (!globals.IsOnlineGame)
            {
                SideMenuEndOnlineGameButtonVisibility = "Hidden";
                SideMenuOnlineGameButtonVisibility = "Visible";
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

            var keepCheckingForOpponentAcceptionStart = new ThreadStart(() => LobbyKeepCheckingForOpponentAcception());
            var keepCheckingForOpponentAcceptionBackgroundThread = new Thread(keepCheckingForOpponentAcceptionStart)
            {
                IsBackground = true
            };
            keepCheckingForOpponentAcceptionBackgroundThread.Start();
        }
        private void LobbyKeepCheckingForOpponentAcception()
        {
            int counter = 0;
            while (LobbyOverlayWaitingForInvitationAcceptedVisibility == "Visible" && globals.CurrentOnlineGame.BlackId != LocalPlayer.Id)
            {
                Task.Run(() =>
                {
                    if (LocalPlayer != null)
                    {
                        try
                        {
                            DispatchService.Invoke(async () =>
                            {
                                globals.CurrentOnlineGame = await WebApiClientGamesCommands.GetNewGame(LocalPlayer.Id);
                            });
                        }
                        catch
                        {
                            MessageBox.Show(globals.LobbyWindow, "Cannot contact server...", "Error!");
                            globals.LobbyWindow.Close();
                            globals.LobbyWindow= null;
                        }
                    }
                });
                Thread.Sleep(1000);
                counter++;
            }

            DispatchService.Invoke(() =>
            {
            if (globals.LobbyWindow!= null)
                {
                    SideMenuOnlineGameButtonVisibility = "Hidden";
                    SideMenuEndOnlineGameButtonVisibility = "Visible";
                    LobbyOverlayWaitingForInvitationAcceptedVisibility = "Hidden";

                    if (globals.LobbyWindow!= null)
                    {
                        globals.LobbyWindow.Close();
                        globals.LobbyWindow= null;

                        StartGame(true);

                        LocalPlayer.Color = "Black";
                        globals.IsOnlineGame = true;

                        Services.BackgroundThreads.OnlineGameKeepCheckingForNextMove(globals, TileDict);
                        Services.BackgroundThreads.OnlineGameKeepResettingWhiteInactiveCounter(globals);
                    }
                }
            });
        }
        private async void LobbyAcceptInvitationAction()
        {
            SideMenuOnlineGameButtonVisibility = "Hidden";
            SideMenuEndOnlineGameButtonVisibility = "Visible";

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
                globals.LobbyWindow= null;

                LocalPlayer.Color = "White";
                globals.IsOnlineGame = true;
                LabelMoveInfo = "It's white's turn...";
                StartGame(false);

                Services.BackgroundThreads.OnlineGameKeepResettingBlackInactiveCounter(globals);
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
                globals.LobbyWindow= null;
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
        private void OnLobbyOverlayPlayerNameTextBoxFocusAction(object o)
        {
            string hasFocusString = (string)o;
            hasLobbyOverlayPlayerNameTextBoxFocus = hasFocusString == "True";
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
        #endregion CommandActions

        #region Methods
        private void InitializeCommands()
        {
            OnMainWindowMouseMoveCommand = new RelayCommand<object>(o => OnMainWindowMouseMoveAction(o));
            OnMainWindowMouseLeftDownCommand = new RelayCommand<object>(o => OnMainWindowMouseLeftDownAction(o));
            OnMainWindowMouseLeftUpCommand = new RelayCommand<object>(o => OnMainWindowMouseLeftUpAction(o));
            OnMainWindowClosingCommand = new RelayCommand(OnMainWindowClosingAction);

            OverlayPromotePawnSelectChessPieceCommand = new RelayCommand<object>(o => OverlayPromotePawnSelectChessPieceAction(o));
            OverlayOnlineGamePlayerQuitOkCommand = new RelayCommand(OverlayOnlineGamePlayerQuitOkAction);

            OnChessPieceMouseLeftDownCommand = new RelayCommand<object>(o => OnChessPieceMouseleftDownAction(o));

            OpenSideMenuCommand = new RelayCommand(OpenSideMenuAction);

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
        private void CreateNotation()
        {
            currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            currentlyDraggedChessPieceOriginalCanvasTop = -1000;

            HorizontalNotationList = Enumerable.Repeat("0", 8).ToList();
            VerticalNotationList = Enumerable.Repeat("0", 8).ToList();

            if (globals.IsRotated)
            {
                ChessCanvasRotationAngle = "180";
                ChessCanvasRotationCenterX = "200";
                ChessCanvasRotationCenterY = "200";
            }
            else
            {
                ChessCanvasRotationAngle = "0";
                ChessCanvasRotationCenterX = "0";
                ChessCanvasRotationCenterY = " -200";
            }

            if (globals.IsRotated)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (i == 0) HorizontalNotationList[i] = "H";
                    else if (i == 1) HorizontalNotationList[i] = "G";
                    else if (i == 2) HorizontalNotationList[i] = "F";
                    else if (i == 3) HorizontalNotationList[i] = "E";
                    else if (i == 4) HorizontalNotationList[i] = "D";
                    else if (i == 5) HorizontalNotationList[i] = "C";
                    else if (i == 6) HorizontalNotationList[i] = "B";
                    else if (i == 7) HorizontalNotationList[i] = "A";

                }

                for (int i = 0; i < 8; i++)
                {
                    VerticalNotationList[i] = (i + 1).ToString();
                }
            }

            else
            {
                for (int i = 0; i < 8; i++)
                {
                    if (i == 0) HorizontalNotationList[i] = "A";
                    else if (i == 1) HorizontalNotationList[i] = "B";
                    else if (i == 2) HorizontalNotationList[i] = "C";
                    else if (i == 3) HorizontalNotationList[i] = "D";
                    else if (i == 4) HorizontalNotationList[i] = "E";
                    else if (i == 5) HorizontalNotationList[i] = "F";
                    else if (i == 6) HorizontalNotationList[i] = "G";
                    else if (i == 7) HorizontalNotationList[i] = "H";

                }

                for (int i = 0; i < 8; i++)
                {
                    if (i == 0) VerticalNotationList[i] = "8";
                    else if (i == 1) VerticalNotationList[i] = "7";
                    else if (i == 2) VerticalNotationList[i] = "6";
                    else if (i == 3) VerticalNotationList[i] = "5";
                    else if (i == 4) VerticalNotationList[i] = "4";
                    else if (i == 5) VerticalNotationList[i] = "3";
                    else if (i == 6) VerticalNotationList[i] = "2";
                    else if (i == 7) VerticalNotationList[i] = "1";
                }
            }

            OnPropertyChangedByPropertyName("HorizontalNotationList");
            OnPropertyChangedByPropertyName("VerticalNotationList");
        }
        private void StartGame(bool doRotate)
        {
            globals.IsRotated = doRotate;
            TileDict = new TileDictionary();

            CreateNotation();

            for (int col = 1; col < 9; col++)
            {
                TileDict[Coords.IntsToCoordsString(col, 2)].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Pawn, doRotate);
                TileDict[Coords.IntsToCoordsString(col, 2)].IsOccupied = true;
            }
            TileDict["A1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Rook, doRotate);
            TileDict["B1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Knight, doRotate);
            TileDict["C1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Bishop, doRotate);
            TileDict["D1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Queen, doRotate);
            TileDict["E1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.King, doRotate);
            TileDict["F1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Bishop, doRotate);
            TileDict["G1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Knight, doRotate);
            TileDict["H1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Rook, doRotate);

            TileDict["A1"].IsOccupied = true;
            TileDict["B1"].IsOccupied = true;
            TileDict["C1"].IsOccupied = true;
            TileDict["D1"].IsOccupied = true;
            TileDict["E1"].IsOccupied = true;
            TileDict["F1"].IsOccupied = true;
            TileDict["G1"].IsOccupied = true;
            TileDict["H1"].IsOccupied = true;

            for (int col = 1; col < 9; col++)
            {
                TileDict[Coords.IntsToCoordsString(col, 7)].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Pawn, doRotate);
                TileDict[Coords.IntsToCoordsString(col, 7)].IsOccupied = true;
            }

            TileDict["A8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Rook, doRotate);
            TileDict["B8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Knight, doRotate);
            TileDict["C8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Bishop, doRotate);
            TileDict["D8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Queen, doRotate);
            TileDict["E8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.King, doRotate);
            TileDict["F8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Bishop, doRotate);
            TileDict["G8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Knight, doRotate);
            TileDict["H8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Rook, doRotate);

            TileDict["A8"].IsOccupied = true;
            TileDict["B8"].IsOccupied = true;
            TileDict["C8"].IsOccupied = true;
            TileDict["D8"].IsOccupied = true;
            TileDict["E8"].IsOccupied = true;
            TileDict["F8"].IsOccupied = true;
            TileDict["G8"].IsOccupied = true;
            TileDict["H8"].IsOccupied = true;

            OnPropertyChangedByPropertyName("TileDict");

            LabelMoveInfo = "It's white's turn...";
            globals.MoveList = new List<Move>();
        }
        private bool IsInputAllowed()
        {
            if (
                globals.IsCheckMate
                || SideMenuVisibility == "Visible"
                || OverlayPromotePawnVisibility == "Visible"
                || globals.IsWaitingForMove)
            {
                return false;
            }

            return true;
        }
        #endregion Methods

        #region Message Handlers
        internal class GlobalsRequestMessage : RequestMessage<Globals>
        {
        }
        internal class PropertyLocalPlayerValueRequestMessage : RequestMessage<Player> { }
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
