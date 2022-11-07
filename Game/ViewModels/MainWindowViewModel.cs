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
using System.Net.Http;

namespace ChessDotNET.ViewModels.MainWindow
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

            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri(@"http://localhost:7002/")
            };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            webApiClientPlayersCommands = new WebApiClientPlayersCommands(httpClient);
            webApiClientInvitationsCommands = new WebApiClientInvitationsCommands(httpClient);
            webApiClientGamesCommands = new WebApiClientGamesCommands(httpClient);

            InitializeCommands();

            StartGame(false);
            //StartGameTestCastling(false);
            //StartGameTestCheckMate(false);

            //debugNoTurns = true;
        }
        #endregion Constuctors

        #region Fields
        private bool isRotated;
        private Canvas chessCanvas;
        private Image currentlyDraggedChessPieceImage;
        private int currentlyDraggedChessPieceOriginalCanvasLeft;
        private int currentlyDraggedChessPieceOriginalCanvasTop;
        private Point dragOverCanvasPosition;
        private Point dragOverChessPiecePosition;
        private bool isMouseMoving;
        private bool wasSideMenuOpen = false;
        private Coords promotePawnCoords;
        private List<Move> MoveList = new List<Move>();
        private readonly bool debugNoTurns = false;
        private bool isCheckMate = false;
        private static DataGrid dataGridLobbyAllPlayers = null;
        private static DataGrid dataGridLobbyInvitations = null;
        private bool hasLobbyOverlayPlayerNameTextBoxFocus;
        private Lobby lobby;
        private readonly WebApiClientPlayersCommands webApiClientPlayersCommands;
        private readonly WebApiClientInvitationsCommands webApiClientInvitationsCommands;
        private readonly WebApiClientGamesCommands webApiClientGamesCommands;
        private bool isOnlineGame = false;
        private bool isWaitingForMove = false;
        Game currentOnlineGame;
        #endregion Fields

        #region Property-Values
        private TileDictionary tileDict;
        private List<ImageSource> promotePawnList;
        private List<string> horizontalNotationList;
        private List<string> verticalNotationList;
        public ObservableCollection<Player> playerList;
        public ObservableCollection<Player> invitationList;
        private string sideMenuVisibility= "Hidden";
        private string sideMenuMainVisibility = "Visible";
        private string sideMenuGameModeVisibility = "Hidden";
        private string sideMenuLocalGameVisibility = "Hidden";
        private string sideMenuOnlineGameVisibility = "Hidden";
        private string sideMenuOnlineGameButtonVisibility = "Visible";
        private string sideMenuEndOnlineGameButtonVisibility = "Hidden";
        private string overlayPromotePawnVisibility = "Hidden";
        private string overlayOnlineGamePlayerQuitVisibility = "Hidden";
        private string chessCanvasRotationAngle = "0";
        private string chessCanvasRotationCenterX = "9";
        private string chessCanvasRotationCenterY = "-200";
        private string labelMoveInfo = "";
        private string textBoxLobbyOverLayPlayerName = "";
        private string isLobbyOverlayPlayerNameOkButtonEnabled = "False";
        private string isLobbyInviteButtonEnabled = "False";
        private string isLobbyAcceptInvitationButtonEnabled = "False";
        private string lobbyOverlayPlayerNameVisibility = "Hidden";
        private string lobbyOverlayOpponentAcceptedInvitationVisibility = "Hidden";
        private string lobbyOverlayOpponentCanceledInvitationVisibility = "Hidden";
        private string lobbyOverlayWaitingForInvitationAcceptedVisibility = "Hidden";
        private string labelPlayerNameConflict = "";
        private Player localPlayer = null;
        private Player opponent = null;
        #endregion Property-Values

        #region Bindable Properties
        public DataGrid DataGridLobbyAllPlayers
        {
            get => dataGridLobbyAllPlayers;
            set { dataGridLobbyAllPlayers = value; OnPropertyChanged(); }
        }
        public string LabelMoveInfo
        {
            get => labelMoveInfo;
            set { labelMoveInfo = value; OnPropertyChanged(); }
        }
        public TileDictionary TileDict
        {
            get => tileDict;
            set { tileDict = value; OnPropertyChanged(); }
        }
        public List<ImageSource> OverlayPromotePawnList
        {
            get => promotePawnList;
            set { promotePawnList = value; OnPropertyChanged(); }
        }
        public List<string> HorizontalNotationList
        {
            get => horizontalNotationList;
            set { horizontalNotationList = value; OnPropertyChanged(); }
        }
        public List<string> VerticalNotationList
        {
            get => verticalNotationList;
            set { verticalNotationList = value; OnPropertyChanged(); }
        }
        public string SideMenuVisibility
        {
            get => sideMenuVisibility;
            set { sideMenuVisibility = value; OnPropertyChanged(); }
        }
        public string SideMenuMainVisibility
        {
            get => sideMenuMainVisibility;
            set { sideMenuMainVisibility = value; OnPropertyChanged(); }
        }
        public string SideMenuGameModeVisibility
        {
            get => sideMenuGameModeVisibility;
            set { sideMenuGameModeVisibility = value; OnPropertyChanged(); }
        }
        public string SideMenuLocalGameVisibility
        {
            get => sideMenuLocalGameVisibility;
            set { sideMenuLocalGameVisibility = value; OnPropertyChanged(); }
        }
        public string SideMenuOnlineGameVisibility
        {
            get => sideMenuOnlineGameVisibility;
            set { sideMenuOnlineGameVisibility = value; OnPropertyChanged(); }
        }
        public string SideMenuOnlineGameButtonVisibility
        {
            get => sideMenuOnlineGameButtonVisibility;
            set { sideMenuOnlineGameButtonVisibility = value; OnPropertyChanged(); }
        }
        public string SideMenuEndOnlineGameButtonVisibility
        {
            get => sideMenuEndOnlineGameButtonVisibility;
            set { sideMenuEndOnlineGameButtonVisibility = value; OnPropertyChanged(); }
        }
        public string OverlayPromotePawnVisibility
        {
            get => overlayPromotePawnVisibility;
            set { overlayPromotePawnVisibility = value; OnPropertyChanged(); }
        }
        public string OverlayOnlineGamePlayerQuitVisibility
        {
            get => overlayOnlineGamePlayerQuitVisibility;
            set { overlayOnlineGamePlayerQuitVisibility = value; OnPropertyChanged(); }
        }
        public Player LocalPlayer
        {
            get => localPlayer;
            set { localPlayer = value; OnPropertyChanged(); }
        }
        public Player Opponent
        {
            get => opponent;
            set { opponent = value; OnPropertyChanged(); }
        }
        public string LobbyOverlayPlayerNameVisibility
        {
            get => lobbyOverlayPlayerNameVisibility;
            set { lobbyOverlayPlayerNameVisibility = value; OnPropertyChanged(); }
        }
        public string LobbyOverlayWaitingForInvitationAcceptedVisibility
        {
            get => lobbyOverlayWaitingForInvitationAcceptedVisibility;
            set { lobbyOverlayWaitingForInvitationAcceptedVisibility = value; OnPropertyChanged(); }
        }
        public string LobbyOverlayOpponentAcceptedInvitationVisibility
        {
            get => lobbyOverlayOpponentAcceptedInvitationVisibility;
            set { lobbyOverlayOpponentAcceptedInvitationVisibility = value; OnPropertyChanged(); }
        }
        public string LobbyOverlayOpponentCanceledInvitationVisibility
        {
            get => lobbyOverlayOpponentCanceledInvitationVisibility;
            set { lobbyOverlayOpponentCanceledInvitationVisibility = value; OnPropertyChanged(); }
        }
        public string ChessCanvasRotationAngle
        {
            get => chessCanvasRotationAngle;
            set { chessCanvasRotationAngle = value; OnPropertyChanged(); }
        }
        public string ChessCanvasRotationCenterX
        {
            get => chessCanvasRotationCenterX;
            set { chessCanvasRotationCenterX = value; OnPropertyChanged(); }
        }
        public string ChessCanvasRotationCenterY
        {
            get => chessCanvasRotationCenterY;
            set { chessCanvasRotationCenterY = value; OnPropertyChanged(); }
        }
        public string LabelPlayerNameConflict
        {
            get => labelPlayerNameConflict;
            set { labelPlayerNameConflict = value; OnPropertyChanged(); }
        }
        public ObservableCollection<Player> PlayerList
        {
            get => playerList;
            set
            {
                playerList = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Player> InvitationList
        {
            get => invitationList;
            set
            {
                invitationList = value;
                OnPropertyChanged();
            }
        }
        public string TextBoxPlayerName
        {
            get
            {
                return textBoxLobbyOverLayPlayerName;
            }
            set
            {
                textBoxLobbyOverLayPlayerName = value;
                LabelPlayerNameConflict = "";
                if (textBoxLobbyOverLayPlayerName != "")
                {
                    IsLobbyOverlayPlayerNameOkButtonEnabled = "True";
                }
                else
                {
                    IsLobbyOverlayPlayerNameOkButtonEnabled = "False";
                }
                OnPropertyChanged();
            }
        }
        public string IsLobbyOverlayPlayerNameOkButtonEnabled
        {
            get => isLobbyOverlayPlayerNameOkButtonEnabled;
            set { isLobbyOverlayPlayerNameOkButtonEnabled = value; OnPropertyChanged(); }
        }
        public string IsLobbyInviteButtonEnabled
        {
            get => isLobbyInviteButtonEnabled;
            set { isLobbyInviteButtonEnabled = value; OnPropertyChanged(); }
        }
        public string IsLobbyAcceptInvitationButtonEnabled
        {
            get => isLobbyAcceptInvitationButtonEnabled;
            set { isLobbyAcceptInvitationButtonEnabled = value; OnPropertyChanged(); }
        }
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
            if (lobby != null)
            {
                lobby.Close();
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
                bool isFirstTurn = MoveList.Count == 0;
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
                    ChessPieceColor lastMoveColor = MoveList[MoveList.Count - 1].ChessPieceColor;
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
        private async void OnMainWindowMouseLeftUpAction(object o, TileDictionary tileDict)
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
                            ChessPiece currentlyDraggedChessPiece = tileDict[oldCoords.String].ChessPiece;
                            ChessPieceColor currentlyDraggedChessPieceColor = currentlyDraggedChessPiece.ChessPieceColor;
                            ChessPieceType currentlyDraggedChessPieceType = currentlyDraggedChessPiece.ChessPieceType;

                            MoveValidationData moveValidationData = MoveValidationGameLogic.ValidateCurrentMove(
                                tileDict, oldCoords, newCoords);

                            bool isFirstMoveValid = true;
                            if (MoveList.Count == 0)
                            {
                                isFirstMoveValid = currentlyDraggedChessPieceColor == ChessPieceColor.White;
                            }

                            bool isTurnCorrectColor = true;
                            if (MoveList.Count > 0)
                            {
                                isTurnCorrectColor = MoveList[MoveList.Count - 1].ChessPieceColor != currentlyDraggedChessPieceColor;
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
                                    tileDict[tileDict.CoordsPawnMovedTwoTiles.String].ChessPiece = new ChessPiece();
                                    tileDict[tileDict.CoordsPawnMovedTwoTiles.String].IsOccupied = false;
                                }

                                // has a pawn moved two tiles at once? Store its coords for the next turn...
                                if (moveValidationData.MovedTwoTiles)
                                {
                                    tileDict.CoordsPawnMovedTwoTiles = moveValidationData.Coords[0];
                                }
                                else if (! isOnlineGame)
                                {
                                    tileDict.CoordsPawnMovedTwoTiles = null;
                                }

                                // promote your pawn if it is on the opposite of the field:
                                bool canPromote = PromotePawnGameLogic.CanPromote(tileDict, oldCoords, newCoords);
                                if (canPromote)
                                {
                                    tileDict.MoveChessPiece(oldCoords, newCoords, true);
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
                                    tileDict.MoveChessPiece(oldCoords, newCoords, true);
                                    Coords rookOldCoords = moveValidationData.Coords[0];
                                    Coords rookNewCoords = moveValidationData.Coords[1];
                                    tileDict.MoveChessPiece(rookOldCoords, rookNewCoords, true);
                                }
                                else
                                {
                                    tileDict.MoveChessPiece(oldCoords, newCoords, true);
                                }

                                string labelMoveInfoText = oldCoords.String + " -> " + newCoords.String;

                                if (ThreateningValidationGameLogic.IsTileThreatened(
                                    tileDict, ChessPieceColor.White, tileDict.WhiteKingCoords, false))
                                {
                                    labelMoveInfoText += ", White king is in check!";
                                }
                                else if (ThreateningValidationGameLogic.IsTileThreatened(
                                    tileDict, ChessPieceColor.Black, tileDict.BlackKingCoords, false))
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
                                    if (CheckMateValidationGameLogic.IsCheckMate(tileDict, tileDict.WhiteKingCoords))
                                    {
                                        labelMoveInfoText = oldCoords.String + " -> " + newCoords.String + ", White is check mate!";
                                        isCheckMate = true;
                                    }
                                }
                                else if (currentlyDraggedChessPieceColor == ChessPieceColor.White)
                                {
                                    if (CheckMateValidationGameLogic.IsCheckMate(tileDict, tileDict.BlackKingCoords))
                                    {
                                        labelMoveInfoText = oldCoords.String + " -> " + newCoords.String + ", Black is check mate!";
                                        isCheckMate = true;
                                    }
                                }

                                LabelMoveInfo = labelMoveInfoText;

                                MoveList.Add(new Move(oldCoords, newCoords, currentlyDraggedChessPieceColor, currentlyDraggedChessPieceType));

                                OnPropertyChangedByPropertyName("TileDict");

                                if (isOnlineGame)
                                {
                                    if (localPlayer.Color == "White")
                                    {
                                        currentOnlineGame.LastMoveStartWhite = oldCoords.String;
                                        currentOnlineGame.LastMoveEndWhite = newCoords.String;

                                        if (moveValidationData.CanCastle)
                                        {
                                            currentOnlineGame.LastMoveStartWhite += "C" + moveValidationData.Coords[0].String;
                                            currentOnlineGame.LastMoveEndWhite   += "C" + moveValidationData.Coords[1].String;
                                        }
                                        else if (moveValidationData.MovedTwoTiles)
                                        {
                                            currentOnlineGame.LastMoveStartWhite += "T" + tileDict.CoordsPawnMovedTwoTiles.String;
                                        }
                                        else if (moveValidationData.CanCaptureEnPassant)
                                        {
                                            currentOnlineGame.LastMoveStartWhite += "E" + tileDict.CoordsPawnMovedTwoTiles.String;
                                            tileDict.CoordsPawnMovedTwoTiles = null;
                                        }
                                        else if (canPromote)
                                        {
                                            currentOnlineGame.LastMoveStartWhite += "P";
                                        }
                                        currentOnlineGame.LastMoveStartBlack = null;
                                        currentOnlineGame.LastMoveEndBlack = null;
                                        currentOnlineGame.MoveInfo = LabelMoveInfo;
                                    }
                                    else
                                    {
                                        currentOnlineGame.LastMoveStartBlack = oldCoords.String;
                                        currentOnlineGame.LastMoveEndBlack = newCoords.String;

                                        if (moveValidationData.CanCastle)
                                        {
                                            currentOnlineGame.LastMoveStartBlack += "C" + moveValidationData.Coords[0].String;
                                            currentOnlineGame.LastMoveEndBlack   += "C" + moveValidationData.Coords[1].String;
                                        }
                                        else if (moveValidationData.MovedTwoTiles)
                                        {
                                            currentOnlineGame.LastMoveStartBlack += "T" + tileDict.CoordsPawnMovedTwoTiles.String;
                                        }
                                        else if (moveValidationData.CanCaptureEnPassant)
                                        {
                                            currentOnlineGame.LastMoveStartBlack += "E" + tileDict.CoordsPawnMovedTwoTiles.String;
                                            tileDict.CoordsPawnMovedTwoTiles = null;
                                        }
                                        else if (canPromote)
                                        {
                                            currentOnlineGame.LastMoveStartBlack += "P";
                                        }

                                        currentOnlineGame.LastMoveStartWhite = null;
                                        currentOnlineGame.LastMoveEndWhite = null;
                                        currentOnlineGame.MoveInfo = LabelMoveInfo;
                                    }

                                    if (!canPromote)
                                    {
                                        await webApiClientGamesCommands.PutCurrentGame(currentOnlineGame.Id, currentOnlineGame);

                                        isWaitingForMove = true;

                                        var keepCheckingForNextMoveStart = new ThreadStart(() => OnlineGameKeepCheckingForNextMove());
                                        var keepCheckingForNextMoveBackgroundThread = new Thread(keepCheckingForNextMoveStart)
                                        {
                                            IsBackground = true
                                        };
                                        keepCheckingForNextMoveBackgroundThread.Start();
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
            ChessPieceColor ownColor = tileDict[promotePawnCoords.String].ChessPiece.ChessPieceColor;
            ChessPiece chessPiece = null;

            if (chessPieceString == "Bishop")
                chessPiece = new ChessPiece(ownColor, ChessPieceType.Bishop, isRotated);
            else if (chessPieceString == "Knight")
                chessPiece = new ChessPiece(ownColor, ChessPieceType.Knight, isRotated);
            else if (chessPieceString == "Rook")
                chessPiece = new ChessPiece(ownColor, ChessPieceType.Rook, isRotated);
            else if (chessPieceString == "Queen")
                chessPiece = new ChessPiece(ownColor, ChessPieceType.Queen, isRotated);

            tileDict[promotePawnCoords.String].ChessPiece = chessPiece;
            promotePawnCoords = null;

            OverlayPromotePawnVisibility = "Hidden";

            OnPropertyChangedByPropertyName("TileDict");

            bool doPut = false;
            if (isOnlineGame)
            {
                if (currentOnlineGame.LastMoveStartWhite != null)
                {
                    if (currentOnlineGame.LastMoveStartWhite.Length > 2 && currentOnlineGame.LastMoveStartWhite[2] == 'P')
                    {
                        doPut = true;
                        currentOnlineGame.LastMoveStartWhite += chessPieceString;
                    }
                }
                else if (currentOnlineGame.LastMoveStartBlack != null)
                {
                    if (currentOnlineGame.LastMoveStartBlack.Length > 2 && currentOnlineGame.LastMoveStartBlack[2] == 'P')
                    {
                        doPut = true;
                        currentOnlineGame.LastMoveStartBlack += chessPieceString;
                    }
                }
                if (doPut)
                {
                    await webApiClientGamesCommands.PutCurrentGame(currentOnlineGame.Id, currentOnlineGame);

                    isWaitingForMove = true;

                    var keepCheckingForNextMoveStart = new ThreadStart(() => OnlineGameKeepCheckingForNextMove());
                    var keepCheckingForNextMoveBackgroundThread = new Thread(keepCheckingForNextMoveStart)
                    {
                        IsBackground = true
                    };
                    keepCheckingForNextMoveBackgroundThread.Start();
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
        private void OnlineGameKeepResettingWhiteInactiveCounter()
        {
            while (isOnlineGame)
            {
                Task.Run(async () =>
                {
                    if (currentOnlineGame != null)
                    {
                        try
                        {
                            await webApiClientGamesCommands.ResetWhiteInactiveCounterAsync(currentOnlineGame.Id);
                        }
                        catch
                        {
                            MessageBox.Show(lobby, "Cannot contact server...", "Error!");
                            lobby.Close();
                            lobby = null;
                        }
                    }
                });
                Thread.Sleep(1000);
            }
        }
        private void OnlineGameKeepResettingBlackInactiveCounter()
        {
            while (isOnlineGame)
            {
                Task.Run(async () =>
                {
                    if (currentOnlineGame != null)
                    {
                        try
                        {
                            await webApiClientGamesCommands.ResetBlackInactiveCounterAsync(currentOnlineGame.Id);
                        }
                        catch
                        {
                            MessageBox.Show(lobby, "Cannot contact server...", "Error!");
                            lobby.Close();
                            lobby = null;
                        }
                    }
                });
                Thread.Sleep(1000);
            }
        }
        private void OnlineGameKeepCheckingForNextMove()
        {
            bool isSuccess = false;
            while (! isSuccess && isOnlineGame)
            {
                DispatchService.Invoke(async () =>
                {
                    if (currentOnlineGame != null)
                    {
                        try
                        {
                            currentOnlineGame = await webApiClientGamesCommands.GetCurrentGame(currentOnlineGame.Id);

                            if (currentOnlineGame.HasPlayerQuit)
                            {
                                isSuccess = true;
                                isWaitingForMove = false;
                                isOnlineGame = false;
                                OverlayOnlineGamePlayerQuitVisibility = "Visible";
                            }
                            else if (localPlayer.Color == "White")
                            {
                                if (currentOnlineGame.LastMoveStartBlack != null && currentOnlineGame.LastMoveEndBlack != null)
                                {
                                    ChessPiece chessPiece = tileDict[currentOnlineGame.LastMoveStartBlack.Substring(0, 2)].ChessPiece;
                                    Coords oldCoords = Coords.StringToCoords(currentOnlineGame.LastMoveStartBlack.Substring(0, 2));
                                    Coords newCoords = Coords.StringToCoords(currentOnlineGame.LastMoveEndBlack.Substring(0, 2));

                                    tileDict.MoveChessPiece(oldCoords, newCoords, true);
                                    MoveList.Add(new Move(oldCoords, newCoords, chessPiece.ChessPieceColor, chessPiece.ChessPieceType));

                                    if (currentOnlineGame.LastMoveStartBlack.Length > 2)
                                    {
                                        if (currentOnlineGame.LastMoveStartBlack[2] == 'C')
                                        {
                                            Coords rookOldCoords = Coords.StringToCoords(currentOnlineGame.LastMoveStartBlack.Substring(3, 2));
                                            Coords rookNewCoords = Coords.StringToCoords(currentOnlineGame.LastMoveEndBlack.Substring(3, 2));
                                            tileDict.MoveChessPiece(rookOldCoords, rookNewCoords, true);
                                        }
                                        else if (currentOnlineGame.LastMoveStartBlack[2] == 'T')
                                        {
                                            tileDict.CoordsPawnMovedTwoTiles = Coords.StringToCoords(currentOnlineGame.LastMoveStartBlack.Substring(3, 2));
                                        }
                                        else if (currentOnlineGame.LastMoveStartBlack[2] == 'E')
                                        {
                                            Coords capturedCoords = Coords.StringToCoords(currentOnlineGame.LastMoveStartBlack.Substring(3, 2));
                                            tileDict[capturedCoords.String].ChessPiece = new ChessPiece();
                                            tileDict[capturedCoords.String].IsOccupied = false;
                                        }
                                        else if (currentOnlineGame.LastMoveStartBlack[2] == 'P')
                                        {
                                            string type = currentOnlineGame.LastMoveStartBlack.Remove(0, 3);
                                            ChessPieceColor color = ChessPieceColor.Black;
                                            if (type == "Bishop")
                                                chessPiece = new ChessPiece(color, ChessPieceType.Bishop, isRotated);
                                            else if (type == "Knight")
                                                chessPiece = new ChessPiece(color, ChessPieceType.Knight, isRotated);
                                            else if (type == "Rook")
                                                chessPiece = new ChessPiece(color, ChessPieceType.Rook, isRotated);
                                            else if (type == "Queen")
                                                chessPiece = new ChessPiece(color, ChessPieceType.Queen, isRotated);


                                            tileDict[currentOnlineGame.LastMoveEndBlack.Substring(0, 2)].ChessPiece = chessPiece;
                                        }
                                    }

                                    OnPropertyChangedByPropertyName("TileDict");
                                    isSuccess = true;
                                    isWaitingForMove = false;
                                    LabelMoveInfo = currentOnlineGame.MoveInfo;
                                }
                            }
                            else
                            {
                                if (currentOnlineGame.LastMoveStartWhite != null && currentOnlineGame.LastMoveEndWhite != null)
                                {
                                    ChessPiece chessPiece = tileDict[currentOnlineGame.LastMoveStartWhite.Substring(0, 2)].ChessPiece;
                                    Coords oldCoords = Coords.StringToCoords(currentOnlineGame.LastMoveStartWhite.Substring(0, 2));
                                    Coords newCoords = Coords.StringToCoords(currentOnlineGame.LastMoveEndWhite.Substring(0, 2));

                                    tileDict.MoveChessPiece(oldCoords, newCoords, true);
                                    MoveList.Add(new Move(oldCoords, newCoords, chessPiece.ChessPieceColor, chessPiece.ChessPieceType));

                                    if (currentOnlineGame.LastMoveStartWhite.Length > 2)
                                    {
                                        if (currentOnlineGame.LastMoveStartWhite[2] == 'C')
                                        {
                                            Coords rookOldCoords = Coords.StringToCoords(currentOnlineGame.LastMoveStartWhite.Substring(3, 2));
                                            Coords rookNewCoords = Coords.StringToCoords(currentOnlineGame.LastMoveEndWhite.Substring(3, 2));
                                            tileDict.MoveChessPiece(rookOldCoords, rookNewCoords, true);
                                        }
                                        else if (currentOnlineGame.LastMoveStartWhite[2] == 'T')
                                        {
                                            tileDict.CoordsPawnMovedTwoTiles = Coords.StringToCoords(currentOnlineGame.LastMoveStartWhite.Substring(3, 2));
                                        }
                                        else if (currentOnlineGame.LastMoveStartWhite[2] == 'E')
                                        {
                                            Coords capturedCoords = Coords.StringToCoords(currentOnlineGame.LastMoveStartWhite.Substring(3, 2));
                                            tileDict[capturedCoords.String].ChessPiece = new ChessPiece();
                                            tileDict[capturedCoords.String].IsOccupied = false;
                                        }
                                        else if (currentOnlineGame.LastMoveStartWhite[2] == 'P')
                                        {
                                            string type = currentOnlineGame.LastMoveStartWhite.Remove(0, 3);
                                            ChessPieceColor color = ChessPieceColor.White;
                                            if (type == "Bishop")
                                                chessPiece = new ChessPiece(color, ChessPieceType.Bishop, isRotated);
                                            else if (type == "Knight")
                                                chessPiece = new ChessPiece(color, ChessPieceType.Knight, isRotated);
                                            else if (type == "Rook")
                                                chessPiece = new ChessPiece(color, ChessPieceType.Rook, isRotated);
                                            else if (type == "Queen")
                                                chessPiece = new ChessPiece(color, ChessPieceType.Queen, isRotated);


                                            tileDict[currentOnlineGame.LastMoveEndWhite.Substring(0, 2)].ChessPiece = chessPiece;
                                        }
                                    }

                                    OnPropertyChangedByPropertyName("TileDict");
                                    isSuccess = true;
                                    isWaitingForMove = false;
                                    LabelMoveInfo = currentOnlineGame.MoveInfo;
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                });
                Thread.Sleep(1000);
            }
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

            if (lobby == null)
            {
                lobby = new Lobby
                {
                    DataContext = this
                };
                lobby.Show();

                PlayerList = new ObservableCollection<Player>();
                InvitationList = new ObservableCollection<Player>();

                LobbyOverlayPlayerNameVisibility = "Visible";


                if (localPlayer != null)
                {
                    TextBoxPlayerName = localPlayer.Name;
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
                lobby.Focus();
            }
        }
        private void SideMenuEndOnlineGameAction()
        {
            SideMenuEndOnlineGameButtonVisibility = "Hidden";
            SideMenuOnlineGameButtonVisibility = "Visible";
            isOnlineGame = false;
            isWaitingForMove = false;

            StartGame(false);
        }

        private void LobbyKeepResettingInactiveCounter()
        {
            while (lobby != null)
            {
                Task.Run(async () =>
                {
                    if (localPlayer != null)
                    {
                        try
                        {
                            await webApiClientPlayersCommands.ResetInactiveCounterAsync(LocalPlayer.Id);
                        }
                        catch
                        {
                            MessageBox.Show(lobby, "Cannot contact server...", "Error!");
                            lobby.Close();
                            lobby = null;
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
                var players = await webApiClientPlayersCommands.GetAllPlayersAsync();

                if (localPlayer != null)
                {
                    PlayerList = new ObservableCollection<Player>(players.Where(a => a.Name != localPlayer.Name).ToList());
                    InvitationList = await webApiClientInvitationsCommands.GetPlayerInvitationsAsync(localPlayer.Id);
                }
            }
        }
        private void LobbyCloseAction()
        {
            lobby.Close();
            lobby = null;
        }
        private async void OnLobbyClosingAction()
        {
            if (localPlayer != null)
            {
                await webApiClientPlayersCommands.DeletePlayerAsync(localPlayer.Id);

                if (LobbyOverlayWaitingForInvitationAcceptedVisibility == "Visible")
                {
                    LobbyOverlayWaitingForInvitationAcceptedVisibility = "Hidden";
                    await webApiClientInvitationsCommands.CancelInvitationAsync(Opponent.Id, LocalPlayer);
                    Opponent = null;
                }
            }
            //SideMenuEndOnlineGameButtonVisibility = "Hidden";
            //SideMenuOnlineGameButtonVisibility = "Visible";
            lobby = null;
        }
        private async void LobbyInviteAction()
        {
            var selectedInfo = dataGridLobbyAllPlayers.SelectedCells[0];
            string selectedPlayerName = ((TextBlock)selectedInfo.Column.GetCellContent(selectedInfo.Item)).Text;
            Opponent = playerList.Where(a => a.Name == selectedPlayerName).FirstOrDefault();

            OnPropertyChangedByPropertyName("Opponent");

            LobbyOverlayWaitingForInvitationAcceptedVisibility = "Visible";

            await webApiClientInvitationsCommands.InvitePlayerAsync(Opponent.Id, LocalPlayer);

            //System.Diagnostics.Debug.WriteLine(Opponent.Name);
            //System.Diagnostics.Debug.WriteLine(LocalPlayer.Name);

            currentOnlineGame = new Game();

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
            while (LobbyOverlayWaitingForInvitationAcceptedVisibility == "Visible" && currentOnlineGame.BlackId != localPlayer.Id)
            {
                Task.Run(() =>
                {
                    if (localPlayer != null)
                    {
                        try
                        {
                            DispatchService.Invoke(async () =>
                            {
                                currentOnlineGame = await webApiClientGamesCommands.GetNewGame(localPlayer.Id);
                            });
                        }
                        catch
                        {
                            MessageBox.Show(lobby, "Cannot contact server...", "Error!");
                            lobby.Close();
                            lobby = null;
                        }
                    }
                });
                Thread.Sleep(1000);
                counter++;
            }


            DispatchService.Invoke(() =>
            {
                SideMenuOnlineGameButtonVisibility = "Hidden";
                SideMenuEndOnlineGameButtonVisibility = "Visible";
                LobbyOverlayWaitingForInvitationAcceptedVisibility = "Hidden";

                if (lobby != null)
                {
                    lobby.Close();
                    lobby = null;

                    localPlayer.Color = "Black";
                    isOnlineGame = true;
                    isWaitingForMove = true;
                    StartGame(true);

                    var keepCheckingForNextMoveStart = new ThreadStart(() => OnlineGameKeepCheckingForNextMove());
                    var keepCheckingForNextMoveBackgroundThread = new Thread(keepCheckingForNextMoveStart)
                    {
                        IsBackground = true
                    };
                    keepCheckingForNextMoveBackgroundThread.Start();

                    var onlineGameKeepResettingInactiveCounterStart = new ThreadStart(() => OnlineGameKeepResettingWhiteInactiveCounter());
                    var onlineGameKeepResettingInactiveCounterBackgroundThread = new Thread(onlineGameKeepResettingInactiveCounterStart)
                    {
                        IsBackground = true
                    };
                    onlineGameKeepResettingInactiveCounterBackgroundThread.Start();
                }
            });
        }
        private async void LobbyAcceptInvitationAction()
        {
            SideMenuOnlineGameButtonVisibility = "Hidden";
            SideMenuEndOnlineGameButtonVisibility = "Visible";

            var selectedInfo = dataGridLobbyInvitations.SelectedCells[0];
            string selectedPlayerName = ((TextBlock)selectedInfo.Column.GetCellContent(selectedInfo.Item)).Text;
            var selectedPlayer = playerList.Where(a => a.Name == selectedPlayerName).FirstOrDefault();

            Opponent = selectedPlayer;
            OnPropertyChangedByPropertyName("Opponent");

            var invitations = await webApiClientInvitationsCommands.GetPlayerInvitationsAsync(localPlayer.Id);
            var opp = invitations.Where(a => a.Id == Opponent.Id).FirstOrDefault();

            if (opp != null)
            {
                currentOnlineGame = new Game
                {
                    BlackId = selectedPlayer.Id,
                    WhiteId = localPlayer.Id
                };

                currentOnlineGame = await webApiClientGamesCommands.StartNewGameAsync(currentOnlineGame);

                lobby.Close();
                lobby = null;

                localPlayer.Color = "White";
                isOnlineGame = true;
                LabelMoveInfo = "It's white's turn...";
                StartGame(false);

                var start = new ThreadStart(() => OnlineGameKeepResettingBlackInactiveCounter());
                var backgroundThread = new Thread(start)
                {
                    IsBackground = true
                };
                backgroundThread.Start();
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
                if (hasLobbyOverlayPlayerNameTextBoxFocus && TextBoxPlayerName != "")
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
                    IsLobbyInviteButtonEnabled = "True";
                }
                else
                {
                    IsLobbyInviteButtonEnabled = "False";
                }
            }
            else
            {
                IsLobbyInviteButtonEnabled = "False";
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
                    IsLobbyAcceptInvitationButtonEnabled = "True";
                }
                else
                {
                    IsLobbyAcceptInvitationButtonEnabled = "False";
                }
            }
            else
            {
                IsLobbyAcceptInvitationButtonEnabled = "False";
            }
        }
        private async void LobbyOverlayPlayerNameOkAction()
        {
            if (localPlayer == null)
            {
                LocalPlayer = new Player();
            }

            localPlayer.Name = TextBoxPlayerName;
            OnPropertyChangedByPropertyName("LocalPlayer");

            Player createPlayerResult = new Player();

            try
            {
                createPlayerResult = await webApiClientPlayersCommands.CreatePlayerAsync(localPlayer);
            }
            catch
            {
                MessageBox.Show(lobby, "Please try again later...", "Error!");
                lobby.Close();
                lobby = null;
            }

            if (createPlayerResult.Name == null)
            {
                LabelPlayerNameConflict = "This name is already taken!";
                localPlayer = null;
            }
            else
            {
                LabelPlayerNameConflict = "";
                LobbyOverlayPlayerNameVisibility = "Hidden";

                localPlayer = createPlayerResult;
            }
        }
        private void LobbyOverlayPlayerNameCancelAction()
        {
            TextBoxPlayerName = "";
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

            await webApiClientInvitationsCommands.CancelInvitationAsync(Opponent.Id, LocalPlayer);

            Opponent = null;
        }
        #endregion CommandActions

        #region Methods
        private void InitializeCommands()
        {
            OnMainWindowMouseMoveCommand = new RelayCommand<object>(o => OnMainWindowMouseMoveAction(o));
            OnMainWindowMouseLeftDownCommand = new RelayCommand<object>(o => OnMainWindowMouseLeftDownAction(o));
            OnMainWindowMouseLeftUpCommand = new RelayCommand<object>(o => OnMainWindowMouseLeftUpAction(o, tileDict));
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

            horizontalNotationList = Enumerable.Repeat("0", 8).ToList();
            verticalNotationList = Enumerable.Repeat("0", 8).ToList();

            if (isRotated)
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

            if (isRotated)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (i == 0) horizontalNotationList[i] = "H";
                    else if (i == 1) horizontalNotationList[i] = "G";
                    else if (i == 2) horizontalNotationList[i] = "F";
                    else if (i == 3) horizontalNotationList[i] = "E";
                    else if (i == 4) horizontalNotationList[i] = "D";
                    else if (i == 5) horizontalNotationList[i] = "C";
                    else if (i == 6) horizontalNotationList[i] = "B";
                    else if (i == 7) horizontalNotationList[i] = "A";

                }

                for (int i = 0; i < 8; i++)
                {
                    verticalNotationList[i] = (i + 1).ToString();
                }
            }

            else
            {
                for (int i = 0; i < 8; i++)
                {
                    if (i == 0) horizontalNotationList[i] = "A";
                    else if (i == 1) horizontalNotationList[i] = "B";
                    else if (i == 2) horizontalNotationList[i] = "C";
                    else if (i == 3) horizontalNotationList[i] = "D";
                    else if (i == 4) horizontalNotationList[i] = "E";
                    else if (i == 5) horizontalNotationList[i] = "F";
                    else if (i == 6) horizontalNotationList[i] = "G";
                    else if (i == 7) horizontalNotationList[i] = "H";

                }

                for (int i = 0; i < 8; i++)
                {
                    if (i == 0) verticalNotationList[i] = "8";
                    else if (i == 1) verticalNotationList[i] = "7";
                    else if (i == 2) verticalNotationList[i] = "6";
                    else if (i == 3) verticalNotationList[i] = "5";
                    else if (i == 4) verticalNotationList[i] = "4";
                    else if (i == 5) verticalNotationList[i] = "3";
                    else if (i == 6) verticalNotationList[i] = "2";
                    else if (i == 7) verticalNotationList[i] = "1";
                }
            }

            OnPropertyChangedByPropertyName("HorizontalNotationList");
            OnPropertyChangedByPropertyName("VerticalNotationList");
        }
        private void StartGame(bool doRotate)
        {
            isRotated = doRotate;
            tileDict = new TileDictionary();

            CreateNotation();

            for (int col = 1; col < 9; col++)
            {
                tileDict[Coords.IntsToCoordsString(col, 2)].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Pawn, doRotate);
                tileDict[Coords.IntsToCoordsString(col, 2)].IsOccupied = true;
            }
            tileDict["A1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Rook, doRotate);
            tileDict["B1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Knight, doRotate);
            tileDict["C1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Bishop, doRotate);
            tileDict["D1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Queen, doRotate);
            tileDict["E1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.King, doRotate);
            tileDict["F1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Bishop, doRotate);
            tileDict["G1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Knight, doRotate);
            tileDict["H1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Rook, doRotate);

            tileDict["A1"].IsOccupied = true;
            tileDict["B1"].IsOccupied = true;
            tileDict["C1"].IsOccupied = true;
            tileDict["D1"].IsOccupied = true;
            tileDict["E1"].IsOccupied = true;
            tileDict["F1"].IsOccupied = true;
            tileDict["G1"].IsOccupied = true;
            tileDict["H1"].IsOccupied = true;

            for (int col = 1; col < 9; col++)
            {
                tileDict[Coords.IntsToCoordsString(col, 7)].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Pawn, doRotate);
                tileDict[Coords.IntsToCoordsString(col, 7)].IsOccupied = true;
            }

            tileDict["A8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Rook, doRotate);
            tileDict["B8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Knight, doRotate);
            tileDict["C8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Bishop, doRotate);
            tileDict["D8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Queen, doRotate);
            tileDict["E8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.King, doRotate);
            tileDict["F8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Bishop, doRotate);
            tileDict["G8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Knight, doRotate);
            tileDict["H8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Rook, doRotate);

            tileDict["A8"].IsOccupied = true;
            tileDict["B8"].IsOccupied = true;
            tileDict["C8"].IsOccupied = true;
            tileDict["D8"].IsOccupied = true;
            tileDict["E8"].IsOccupied = true;
            tileDict["F8"].IsOccupied = true;
            tileDict["G8"].IsOccupied = true;
            tileDict["H8"].IsOccupied = true;

            OnPropertyChangedByPropertyName("TileDict");

            LabelMoveInfo = "It's white's turn...";
            MoveList = new List<Move>();
        }
        private bool IsInputAllowed()
        {
            if (
                isCheckMate
                || SideMenuVisibility == "Visible"
                || OverlayPromotePawnVisibility == "Visible"
                || isWaitingForMove)
            {
                return false;
            }

            return true;
        }
        #endregion Methods
    }
}
