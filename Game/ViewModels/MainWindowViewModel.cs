using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Reflection;

using ChessDotNET.GameLogic;
using ChessDotNET.WebApiClient;
using ChessDotNET.Models;


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

            InitializeCommands();
            InitializeMessageHandlers();

            StartGame(false);
            //debugNoTurns = true;
        }
        #endregion Constuctors

        #region Fields
        private Globals globals;
        private Canvas chessCanvas;
        private Image currentlyDraggedChessPieceImage;
        private Point dragOverCanvasPosition;
        private Point dragOverChessPiecePosition;
        private bool isMouseMoving;
        private bool wasSideMenuOpen = false;
        private Coords promotePawnCoords;
        private readonly bool debugNoTurns = false;
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
        #endregion Bindable Properties

        #region Commands
        public RelayCommand OpenSideMenuCommand { get; set; }
        public RelayCommand OnMainWindowClosingCommand { get; set; }
        public RelayCommand<object> OnMainWindowMouseMoveCommand { get; set; }
        public RelayCommand<object> OnMainWindowMouseLeftDownCommand { get; set; }
        public RelayCommand<object> OnMainWindowMouseLeftUpCommand { get; set; }
        public RelayCommand<object> OnChessPieceMouseLeftDownCommand { get; set; }
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
                    ChangePropertyStringValueSideMenuViewModel("SideMenuGameModeVisibility", "Hidden");
                    ChangePropertyStringValueSideMenuViewModel("SideMenuLocalGameVisibility", "Hidden");
                    ChangePropertyStringValueSideMenuViewModel("SideMenuOnlineGameVisibility", "Hidden");
                    ChangePropertyStringValueSideMenuViewModel("SideMenuMainVisibility", "Visible");

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
        private void OnChessPieceMouseleftDownAction(object param)
        {
            if (IsInputAllowed())
            {
                currentlyDraggedChessPieceImage = null;
                globals.CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
                globals.CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;
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

                    if (globals.CurrentlyDraggedChessPieceOriginalCanvasLeft < 0 && globals.CurrentlyDraggedChessPieceOriginalCanvasTop < 0)
                    {
                        globals.CurrentlyDraggedChessPieceOriginalCanvasLeft = int.Parse(
                            currentlyDraggedChessPieceImage.GetValue(Canvas.LeftProperty).ToString()
                            );
                        globals.CurrentlyDraggedChessPieceOriginalCanvasTop = int.Parse(
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
                        Canvas.SetLeft(currentlyDraggedChessPieceImage, globals.CurrentlyDraggedChessPieceOriginalCanvasLeft);
                        Canvas.SetTop(currentlyDraggedChessPieceImage, globals.CurrentlyDraggedChessPieceOriginalCanvasTop);
                        globals.CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
                        globals.CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;
                    }
                    else
                    {
                        Point oldPoint = new Point(globals.CurrentlyDraggedChessPieceOriginalCanvasLeft,
                            globals.CurrentlyDraggedChessPieceOriginalCanvasTop);
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
                                Canvas.SetLeft(currentlyDraggedChessPieceImage, globals.CurrentlyDraggedChessPieceOriginalCanvasLeft);
                                Canvas.SetTop(currentlyDraggedChessPieceImage, globals.CurrentlyDraggedChessPieceOriginalCanvasTop);

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

                                        Services.BackgroundThreads.OnlineGameKeepCheckingForNextMove();
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
                                Canvas.SetLeft(currentlyDraggedChessPieceImage, globals.CurrentlyDraggedChessPieceOriginalCanvasLeft);
                                Canvas.SetTop(currentlyDraggedChessPieceImage, globals.CurrentlyDraggedChessPieceOriginalCanvasTop);
                            }
                        }
                        else
                        {
                            Canvas.SetLeft(currentlyDraggedChessPieceImage, globals.CurrentlyDraggedChessPieceOriginalCanvasLeft);
                            Canvas.SetTop(currentlyDraggedChessPieceImage, globals.CurrentlyDraggedChessPieceOriginalCanvasTop);
                        }
                    }
                    globals.CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
                    globals.CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;
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

                    Services.BackgroundThreads.OnlineGameKeepCheckingForNextMove();
                }
            }
        }
        private void OverlayOnlineGamePlayerQuitOkAction()
        {
            OverlayOnlineGamePlayerQuitVisibility = "Hidden";

            ChangePropertyStringValueSideMenuViewModel("SideMenuEndOnlineGameButtonVisibility", "Hidden");
            ChangePropertyStringValueSideMenuViewModel("SideMenuOnlineGameButtonVisibility", "Visible");

            Opponent = null;
            StartGame(false);
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
        }
        private void InitializeMessageHandlers()
        {
            WeakReferenceMessenger.Default.Register<MainWindowViewModel, PropertyTileDictValueRequestMessage>(this, (r, m) =>
            {
                m.Reply(r.TileDict);
            });

            WeakReferenceMessenger.Default.Register<MainWindowViewModel, PropertyLocalPlayerValueRequestMessage>(this, (r, m) =>
            {
                m.Reply(r.LocalPlayer);
            });

            WeakReferenceMessenger.Default.Register<MainWindowViewModel, PropertyOpponentValueRequestMessage>(this, (r, m) =>
            {
                m.Reply(r.Opponent);
            });

            WeakReferenceMessenger.Default.Register<PropertyStringValueChangedMessage>(this, (r, m) =>
            {
                PropertyInfo propertyName = GetType().GetProperty(m.Value.Item1);
                propertyName.SetValue(this, m.Value.Item2);
            });

            WeakReferenceMessenger.Default.Register<PropertyPlayerValueChangedMessage>(this, (r, m) =>
            {
                PropertyInfo propertyName = GetType().GetProperty(m.Value.Item1);
                propertyName.SetValue(this, m.Value.Item2);
            });

            WeakReferenceMessenger.Default.Register<StartGameMessage>(this, (r, m) =>
            {
                StartGame(m.Value);
            });

            WeakReferenceMessenger.Default.Register<OnPropertyChangedMessage>(this, (r, m) =>
            {
                OnPropertyChangedByPropertyName(m.Value);
            });
        }
        private void CreateNotation()
        {
            globals.CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            globals.CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;

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
            globals.Reset();
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
        // RequestMessage Handlers:
        internal class GlobalsRequestMessage : RequestMessage<Globals> { }
        internal class PropertyTileDictValueRequestMessage : RequestMessage<TileDictionary> { }
        internal class PropertyLocalPlayerValueRequestMessage : RequestMessage<Player> { }
        internal class PropertyOpponentValueRequestMessage : RequestMessage<Player> { }

        // ValueChangedMessage Handlers:
        internal class PropertyStringValueChangedMessage : ValueChangedMessage<Tuple<string, string>>
        {
            internal PropertyStringValueChangedMessage(Tuple<string, string> tuple) : base(tuple) { }
        }
        internal class PropertyPlayerValueChangedMessage : ValueChangedMessage<Tuple<string, Player>>
        {
            internal PropertyPlayerValueChangedMessage(Tuple<string, Player> tuple) : base(tuple) { }
        }
        internal class OnPropertyChangedMessage : ValueChangedMessage<string>
        {
            internal OnPropertyChangedMessage(string propertyName) : base(propertyName) { }
        }
        internal class StartGameMessage : ValueChangedMessage<bool>
        {
            internal StartGameMessage(bool isRotated) : base(isRotated) { }
        }
        #endregion
    }
}
