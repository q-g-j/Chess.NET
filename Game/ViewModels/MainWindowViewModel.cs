using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using ChessDotNET.WebClient;
using ChessDotNET.Models;
using ChessDotNET.ViewHelpers;
using ChessDotNET.Views;
using System.Collections.Generic;
using System.Linq;
using ChessDotNET.GameLogic;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using System.Net.Http.Headers;
using System.Collections.ObjectModel;
using System.Threading;

namespace ChessDotNET.ViewModels.MainWindow
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Constructors
        public MainWindowViewModel()
        {
            OnWindowMouseMoveCommand = new RelayCommand<object>(o => OnWindowMouseMoveAction(o));
            OnWindowMouseLeftDownCommand = new RelayCommand<object>(o => OnWindowMouseLeftDownAction(o));
            OnWindowMouseLeftUpCommand = new RelayCommand<object>(o => OnWindowMouseLeftUpAction(o, tileDict));
            OnChessPieceMouseLeftDownCommand = new RelayCommand<object>(o => OnChessPieceMouseleftDownAction(o));
            OnLobbyOverlayPlayerNameTextBoxFocusCommand = new RelayCommand<object>(o => OnLobbyOverlayPlayerNameTextBoxFocusAction(o));

            OpenSideMenuCommand = new RelayCommand(OpenSideMenuAction);

            SideMenuNewGameCommand = new RelayCommand(SideMenuNewGameAction);

            SideMenuLocalGameCommand = new RelayCommand(SideMenuLocalGameAction);
            SideMenuOnlineGameCommand = new RelayCommand(SideMenuOnlineGameAction);
            SideMenuGameModeGoBackCommand = new RelayCommand(SideMenuGameModeGoBackAction);

            SideMenuLocalGameAsWhiteCommand = new RelayCommand(SideMenuLocalGameAsWhiteAction);
            SideMenuLocalGameAsBlackCommand = new RelayCommand(SideMenuLocalGameAsBlackAction);
            SideMenuLocalGameGoBackCommand = new RelayCommand(SideMenuLocalGameGoBackAction);

            SideMenuOnlineGameEnterLobbyCommand = new RelayCommand(SideMenuOnlineGameEnterLobbyActionAsync);
            SideMenuOnlineGameGoBackCommand = new RelayCommand(SideMenuOnlineGameGoBackAction);

            SideMenuQuitProgramCommand = new RelayCommand(SideMenuQuitProgramAction);
            OnMainWindowClosingCommand = new RelayCommand(OnMainWindowClosingAction);

            WindowLobbyOkCommand = new RelayCommand<object>(o => WindowLobbyOkAction(o));
            WindowLobbyCancelCommand = new RelayCommand<object>(o => WindowLobbyCancelAction(o));
            WindowLobbyKeyboardCommand = new RelayCommand<object>(o => WindowLobbyKeyboardAction(o));

            WindowPlayerNameOkCommand = new RelayCommand(WindowPlayerNameOkActionAsync);
            WindowPlayerNameCancelCommand = new RelayCommand(WindowPlayerNameCancelAction);

            PromotePawnSelectChessPieceCommand = new RelayCommand<object>(PromotePawnSelectChessPieceAction);

            promotePawnList = new List<ImageSource>()
            {
                ChessPieceImages.WhiteBishop,
                ChessPieceImages.WhiteKnight,
                ChessPieceImages.WhiteRook,
                ChessPieceImages.WhiteQueen
            };

            WebClientCommands.client.BaseAddress = new Uri(@"http://qgj.ddns.net:7002/");
            WebClientCommands.client.DefaultRequestHeaders.Accept.Clear();
            WebClientCommands.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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
        private Player localPlayer = null;
        private bool hasWindowLobbyPlayerNameTextBoxFocus;

        WindowLobby windowLobby;
        #endregion Fields

        #region Property-Values
        private TileDictionary tileDict;
        private List<ImageSource> promotePawnList;
        private List<string> horizontalNotationList;
        private List<string> verticalNotationList;
        public ObservableCollection<Player> playerList;
        private string sideMenuVisibility= "Hidden";
        private string sideMenuMainVisibility = "Visible";
        private string sideMenuGameModeVisibility = "Hidden";
        private string sideMenuLocalGameVisibility = "Hidden";
        private string sideMenuOnlineGameVisibility = "Hidden";
        private string overlayPromotePawnVisibility = "Hidden";
        private string chessCanvasRotationAngle = "0";
        private string chessCanvasRotationCenterX = "9";
        private string chessCanvasRotationCenterY = "-200";
        private string labelMoveInfo = "";
        private string textBoxPlayerName = "";
        private string onWindowPlayerNameOkButtonEnabled = "False";
        private string lobbyOverlayPlayerNameVisibility;
        private string labelPlayerNameConflict = "";
        #endregion Property-Values

        #region Bindable Properties
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
        public string OverlayPromotePawnVisibility
        {
            get => overlayPromotePawnVisibility;
            set { overlayPromotePawnVisibility = value; OnPropertyChanged(); }
        }
        public string LobbyOverlayPlayerNameVisibility
        {
            get => lobbyOverlayPlayerNameVisibility;
            set { lobbyOverlayPlayerNameVisibility = value; OnPropertyChanged(); }
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
            set { playerList = value; OnPropertyChanged(); }
        }
        public string TextBoxPlayerName
        {
            get
            {
                return textBoxPlayerName;
            }
            set
            {
                textBoxPlayerName = value;
                LabelPlayerNameConflict = "";
                if (textBoxPlayerName != "")
                {
                    OnWindowPlayerNameOkButtonEnabled = "True";
                }
                else
                {
                    OnWindowPlayerNameOkButtonEnabled = "False";
                }
                OnPropertyChanged();
            }
        }
        public string OnWindowPlayerNameOkButtonEnabled
        {
            get => onWindowPlayerNameOkButtonEnabled;
            set { onWindowPlayerNameOkButtonEnabled = value; OnPropertyChanged(); }
        }
        #endregion Bindable Properties

        #region Commands
        public RelayCommand OpenSideMenuCommand { get; }
        public RelayCommand SideMenuNewGameCommand { get; }
        public RelayCommand SideMenuLocalGameCommand { get; }
        public RelayCommand SideMenuOnlineGameCommand { get; }
        public RelayCommand SideMenuGameModeGoBackCommand { get; }
        public RelayCommand SideMenuLocalGameAsWhiteCommand { get; }
        public RelayCommand SideMenuLocalGameAsBlackCommand { get; }
        public RelayCommand SideMenuLocalGameGoBackCommand { get; }
        public RelayCommand SideMenuOnlineGameEnterLobbyCommand { get; }
        public RelayCommand SideMenuOnlineGameGoBackCommand { get; }
        public RelayCommand SideMenuQuitProgramCommand { get; }
        public RelayCommand OnMainWindowClosingCommand { get; }
        public RelayCommand<object> WindowLobbyOkCommand { get; }
        public RelayCommand<object> WindowLobbyCancelCommand { get; }
        public RelayCommand<object> WindowLobbyKeyboardCommand { get; }
        public RelayCommand WindowPlayerNameOkCommand { get; }
        public RelayCommand WindowPlayerNameCancelCommand { get; }
        public RelayCommand<object> OnWindowMouseMoveCommand { get; }
        public RelayCommand<object> OnWindowMouseLeftDownCommand { get; }
        public RelayCommand<object> OnWindowMouseLeftUpCommand { get; }
        public RelayCommand<object> OnChessPieceMouseLeftDownCommand { get; }
        public RelayCommand<object> OnLobbyOverlayPlayerNameTextBoxFocusCommand { get; }
        public RelayCommand<object> PromotePawnSelectChessPieceCommand { get; }
        #endregion Commands

        #region CommandActions
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
        private void OnWindowMouseMoveAction(object o)
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
        private void OnWindowMouseLeftDownAction(object o)
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
        private void OnWindowMouseLeftUpAction(object o, TileDictionary tileDict)
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
                                    tileDict.CoordsPawnMovedTwoTiles = null;
                                }

                                // has a pawn moved two tiles at once? Store its coords for the next turn...
                                if (moveValidationData.MovedTwoTiles)
                                {
                                    tileDict.CoordsPawnMovedTwoTiles = moveValidationData.Coords[0];
                                }
                                else
                                {
                                    tileDict.CoordsPawnMovedTwoTiles = null;
                                }

                                // promote your pawn if it is on the opposite of the field:
                                if (PromotePawnGameLogic.CanPromote(tileDict, oldCoords, newCoords))
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
        private void OnLobbyOverlayPlayerNameTextBoxFocusAction(object o)
        {
            string hasFocusString = (string)o;
            hasWindowLobbyPlayerNameTextBoxFocus = hasFocusString == "True";
        }
        private void SideMenuNewGameAction()
        {
            SideMenuMainVisibility = "Hidden";
            SideMenuGameModeVisibility = "Visible";
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
        private async void SideMenuOnlineGameEnterLobbyActionAsync()
        {
            SideMenuOnlineGameVisibility = "Hidden";
            SideMenuGameModeVisibility = "Hidden";
            SideMenuVisibility = "Hidden";

            windowLobby = new WindowLobby
            {
                DataContext = this
            };
            windowLobby.Show();

            PlayerList = new ObservableCollection<Player>(await WebClientCommands.GetAllPlayersAsync());

            if (localPlayer == null)
            {
                LobbyOverlayPlayerNameVisibility = "Visible";
            }

            if (localPlayer != null)
            {
                var createPlayerResult = await WebClientCommands.CreatePlayerAsync(localPlayer);
            }

            var keepRefreshingLobby = new ThreadStart(() => KeepRefreshingLobby());
            var keepRefreshingLobbyBackgroundThread = new Thread(keepRefreshingLobby)
            {
                IsBackground = true
            };
            keepRefreshingLobbyBackgroundThread.Start();


            var keepResettingCounter = new ThreadStart(() => KeepResettingInactiveCounter());
            var keepResettingCounterBackgroundThread = new Thread(keepResettingCounter)
            {
                IsBackground = true
            };
            keepResettingCounterBackgroundThread.Start();
        }
        private void KeepRefreshingLobby()
        {
            while (windowLobby.IsVisible)
            {
                Task.Run(async () =>
                {
                    var allPlayers = await WebClientCommands.GetAllPlayersAsync();
                    PlayerList = new ObservableCollection<Player>(allPlayers);
                });
                Thread.Sleep(5000);
            }
        }
        private void KeepResettingInactiveCounter()
        {
            while (windowLobby.IsVisible)
            {
                Task.Run(async () =>
                {
                    await WebClientCommands.ResetInactiveCounterAsync(localPlayer);
                });
                Thread.Sleep(1000);
            }
        }
        private void WindowLobbyOkAction(object o)
        {
            windowLobby = (WindowLobby)o;
            windowLobby.Close();
        }
        private void WindowLobbyCancelAction(object o)
        {
            windowLobby = (WindowLobby)o;
            windowLobby.Close();
        }
        private void WindowLobbyKeyboardAction(object o)
        {
            if ((string)o == "Enter")
            {
                if (hasWindowLobbyPlayerNameTextBoxFocus && TextBoxPlayerName != "")
                {
                    WindowPlayerNameOkActionAsync();
                }
            }
        }
        private async void WindowPlayerNameOkActionAsync()
        {
            localPlayer = new Player()
            {
                Name = TextBoxPlayerName
            };
            var createPlayerResult = await WebClientCommands.CreatePlayerAsync(localPlayer);

            if (createPlayerResult.Name == null)
            {
                LabelPlayerNameConflict = "This name is already taken!";
                localPlayer = null;
            }
            else
            {
                LabelPlayerNameConflict = "";
                LobbyOverlayPlayerNameVisibility = "Hidden";

                var allPlayers = await WebClientCommands.GetAllPlayersAsync();
                PlayerList = new ObservableCollection<Player>(allPlayers);
            }
        }
        private void WindowPlayerNameCancelAction()
        {
            TextBoxPlayerName = "";
            LabelPlayerNameConflict = "";
            LobbyOverlayPlayerNameVisibility = "Hidden";
        }
        private void SideMenuQuitProgramAction()
        {
            Application.Current.Shutdown();
        }
        private void OnMainWindowClosingAction()
        {
            if (windowLobby != null)
            {
                windowLobby.Close();
            }
        }
        private void PromotePawnSelectChessPieceAction(object o)
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
        }
        #endregion CommandActions

        #region Methods
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

            LabelMoveInfo = "";
            MoveList = new List<Move>();
        }
        private void StartGameTestCastling(bool doRotate)
        {
            isRotated = doRotate;
            tileDict = new TileDictionary();

            CreateNotation();

            tileDict["B6"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Rook, doRotate);
            tileDict["C6"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Pawn, doRotate);
            tileDict["D6"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Knight, doRotate);
            tileDict["E6"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Bishop, doRotate);
            tileDict["F6"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Queen, doRotate);
            tileDict["G6"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.King, doRotate);

            tileDict["A1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Rook, doRotate);
            tileDict["E1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.King, doRotate);
            tileDict["H1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Rook, doRotate);

            tileDict["A8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Rook, doRotate);
            tileDict["E8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.King, doRotate);
            tileDict["H8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Rook, doRotate);


            tileDict["A3"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Pawn, doRotate);
            tileDict["B3"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Pawn, doRotate);
            tileDict["C3"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Pawn, doRotate);
            tileDict["D3"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Pawn, doRotate);
            tileDict["E3"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Pawn, doRotate);
            tileDict["F3"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Pawn, doRotate);
            tileDict["G3"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Pawn, doRotate);
            tileDict["H3"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Pawn, doRotate);
            tileDict["A3"].IsOccupied = true;
            tileDict["B3"].IsOccupied = true;
            tileDict["C3"].IsOccupied = true;
            tileDict["D3"].IsOccupied = true;
            tileDict["E3"].IsOccupied = true;
            tileDict["F3"].IsOccupied = true;
            tileDict["G3"].IsOccupied = true;
            tileDict["H3"].IsOccupied = true;
            tileDict["B6"].IsOccupied = true;
            tileDict["C6"].IsOccupied = true;
            tileDict["D6"].IsOccupied = true;
            tileDict["E6"].IsOccupied = true;
            tileDict["F6"].IsOccupied = true;
            tileDict["G6"].IsOccupied = true;

            tileDict["A1"].IsOccupied = true;
            tileDict["E1"].IsOccupied = true;
            tileDict["H1"].IsOccupied = true;

            tileDict["A8"].IsOccupied = true;
            tileDict["E8"].IsOccupied = true;
            tileDict["H8"].IsOccupied = true;

            OnPropertyChangedByPropertyName("TileDict");

            LabelMoveInfo = "";
            MoveList = new List<Move>();
        }
        private void StartGameTestCheckMate(bool doRotate)
        {
            isRotated = doRotate;
            tileDict = new TileDictionary();

            CreateNotation();

            tileDict["H7"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.King, doRotate);
            tileDict["H7"].IsOccupied = true;
            tileDict.BlackKingCoords = new Coords(Columns.H, 7);

            tileDict["E1"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.King, doRotate);
            tileDict["E1"].IsOccupied = true;
            tileDict.WhiteKingCoords = new Coords(Columns.E, 1);

            tileDict["D2"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Queen, doRotate);
            tileDict["D2"].IsOccupied = true;

            tileDict["C3"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Bishop, doRotate);
            tileDict["C3"].IsOccupied = true;

            tileDict["A6"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Rook, doRotate);
            tileDict["A6"].IsOccupied = true;

            tileDict["A8"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Rook, doRotate);
            tileDict["A8"].IsOccupied = true;

            tileDict["H8"].ChessPiece = new ChessPiece(ChessPieceColor.Black, ChessPieceType.Knight, doRotate);
            tileDict["H8"].IsOccupied = true;

            tileDict["G4"].ChessPiece = new ChessPiece(ChessPieceColor.White, ChessPieceType.Pawn, doRotate);
            tileDict["G4"].IsOccupied = true;


            OnPropertyChangedByPropertyName("TileDict");

            LabelMoveInfo = "";
            MoveList = new List<Move>();
        }
        private bool IsInputAllowed()
        {
            if (
                isCheckMate
                || SideMenuVisibility == "Visible"
                || OverlayPromotePawnVisibility == "Visible")
            {
                return false;
            }

            return true;
        }
        private void OnPropertyChangedByPropertyName(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion Methods

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events
    }
}
