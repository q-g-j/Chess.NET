using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using ChessDotNET.CustomTypes;
using ChessDotNET.GUI.ViewHelpers;
using ChessDotNET.Settings;
using System.Collections.Generic;
using System.Linq;
using ChessDotNET.GameLogic;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Input;

namespace ChessDotNET.GUI.ViewModels.MainWindow
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Constructors
        public MainWindowViewModel()
        {
            //appSettingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Chess.NET");
            //appSettings = new AppSettings(appSettingsFolder);

            WindowMouseMoveCommand = new RelayCommand<object>(o => WindowMouseMoveAction(o));
            WindowMouseLeftDownCommand = new RelayCommand<object>(o => WindowMouseLeftDownAction(o));
            WindowMouseLeftUpCommand = new RelayCommand<object>(o => WindowMouseLeftUpAction(o, tileDict));
            ChessPieceMouseLeftDownCommand = new RelayCommand<object>(o => ChessPieceMouseleftDownAction(o));
            OpenSideMenuCommand = new RelayCommand(OpenSideMenuAction);

            SideMenuNewGameCommand = new RelayCommand(SideMenuNewGameAction);
            SideMenuNewGameModeLocalCommand = new RelayCommand(SideMenuNewGameModeLocalAction);
            SideMenuNewGameModeGoBackCommand = new RelayCommand(SideMenuNewGameModeGoBackAction);
            SideMenuNewGameLocalAsWhiteCommand = new RelayCommand(SideMenuNewGameLocalAsWhiteAction);
            SideMenuNewGameLocalAsBlackCommand = new RelayCommand(SideMenuNewGameLocalAsBlackAction);
            SideMenuNewGameLocalColorGoBackCommand = new RelayCommand(SideMenuNewGameLocalColorGoBackAction);
            //SideMenuSettingsCommand = new RelayCommand(SideMenuSettingsAction);
            SideMenuQuitProgramCommand = new RelayCommand(SideMenuQuitProgramAction);

            //OverlaySettingsSaveCommand = new RelayCommand(OverlaySettingsSaveAction);
            //OverlaySettingsCancelCommand = new RelayCommand(OverlaySettingsCancelAction);

            propertiesDict = new Dictionary<string, string>()
            {
                ["SideMenuVisibility"] = "Hidden",
                ["SideMenuMainMenuVisibility"] = "Visible",
                ["SideMenuNewGameModeVisibility"] = "Hidden",
                ["SideMenuButtonsNewGameLocalColorVisibility"] = "Hidden",

                ["OverlaySettingsVisibility"] = "Hidden",

                ["ChessCanvasRotationAngle"] = "0",
                ["ChessCanvasRotationCenterX"] = "0",
                ["ChessCanvasRotationCenterY"] = "-200",
            };

            WasSideMenuOpen = false;

            //if (!Directory.Exists(appSettingsFolder))
            //{
            //    Directory.CreateDirectory(appSettingsFolder);
            //}

            StartGame(false);
            //StartGameTestCastling(false);
        }
        #endregion Constuctors

        #region Fields
        //private readonly AppSettings appSettings;
        //private readonly string appSettingsFolder;
        private bool isRotated;
        private Canvas chessCanvas;
        private Image currentlyDraggedChessPieceImage;
        private int currentlyDraggedChessPieceOriginalCanvasLeft;
        private int currentlyDraggedChessPieceOriginalCanvasTop;
        private Point DragOverCanvasPosition;
        private Point DragOverChessPiecePosition;
        private bool IsMouseMoving;
        private bool WasSideMenuOpen;
        #endregion Fields

        #region Property-Values
        private TileDictionary tileDict;
        private Dictionary<string, string> propertiesDict;
        private List<string> horizontalNotationList;
        private List<string> verticalNotationList;
        #endregion Property-Values

        #region Properties
        public TileDictionary TileDict
        {
            get => tileDict;
            set { tileDict = value; OnPropertyChanged(); }
        }
        internal TileDictionary TileDictReadOnly
        {
            get => TileDict;
        }
        public Dictionary<string, string> PropertiesDict
        {
            get => propertiesDict;
            set { propertiesDict = value; OnPropertyChanged(); }
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
        #endregion Properties

        #region Commands
        public RelayCommand OpenSideMenuCommand { get; }
        public RelayCommand SideMenuNewGameCommand { get; }
        public RelayCommand SideMenuNewGameModeLocalCommand { get; }
        public RelayCommand SideMenuNewGameModeEmailCommand { get; }
        public RelayCommand SideMenuNewGameModeGoBackCommand { get; }
        public RelayCommand SideMenuNewGameLocalAsWhiteCommand { get; }
        public RelayCommand SideMenuNewGameLocalAsBlackCommand { get; }
        public RelayCommand SideMenuNewGameLocalColorGoBackCommand { get; }
        public RelayCommand SideMenuSettingsCommand { get; }
        public RelayCommand<object> OverlaySettingsPasswordBoxCommand { get; }
        public RelayCommand OverlaySettingsSaveCommand { get; }
        public RelayCommand OverlaySettingsCancelCommand { get; }
        public RelayCommand OverlayNewEmailGameStartCommand { get; }
        public RelayCommand OverlayNewEmailGameCancelCommand { get; }
        public RelayCommand OverlayInvitationAcceptCommand { get; }
        public RelayCommand OverlayInvitationAcceptedCommand { get; }
        public RelayCommand OverlayInvitationRejectCommand { get; }
        public RelayCommand SideMenuQuitProgramCommand { get; }
        public RelayCommand<object> WindowMouseMoveCommand { get; }
        public RelayCommand<object> WindowMouseLeftDownCommand { get; }
        public RelayCommand<object> WindowMouseLeftUpCommand { get; }
        public RelayCommand<object> ChessPieceMouseLeftDownCommand { get; }
        #endregion Commands

        #region CommandActions
        private void OpenSideMenuAction()
        {
            if (!WasSideMenuOpen)
            {
                if (PropertiesDict["SideMenuVisibility"] != "Visible"
                    && PropertiesDict["OverlaySettingsVisibility"] == "Hidden")
                {
                    PropertiesDict["SideMenuNewGameModeVisibility"] = "Hidden";
                    PropertiesDict["SideMenuButtonsNewGameLocalColorVisibility"] = "Hidden";
                    PropertiesDict["SideMenuMainMenuVisibility"] = "Visible";
                    PropertiesDict["SideMenuVisibility"] = "Visible";
                }
                else PropertiesDict["SideMenuVisibility"] = "Hidden";
            }
            else
            {
                WasSideMenuOpen = false;
            }

            OnPropertyChangedByPropertyName("PropertiesDict");
        }
        private void WindowMouseMoveAction(object o)
        {
            MouseEventArgs e = o as MouseEventArgs;

            if (currentlyDraggedChessPieceImage != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {

                    if (!WasSideMenuOpen)
                    {
                        if (!IsMouseMoving)
                        {
                            DragOverCanvasPosition = e.GetPosition(chessCanvas);
                            DragOverChessPiecePosition = e.GetPosition(currentlyDraggedChessPieceImage);
                        }
                        IsMouseMoving = true;
                        DragOverCanvasPosition = e.GetPosition(chessCanvas);
                        currentlyDraggedChessPieceImage.SetValue(Panel.ZIndexProperty, 20);

                        Canvas.SetLeft(currentlyDraggedChessPieceImage, DragOverCanvasPosition.X - DragOverChessPiecePosition.X);
                        Canvas.SetTop(currentlyDraggedChessPieceImage, DragOverCanvasPosition.Y - DragOverChessPiecePosition.Y);
                    }

                    OnPropertyChangedByPropertyName("PropertiesDict");
                }
            }
            e.Handled = true;
        }
        private void WindowMouseLeftDownAction(object o)
        {
            var e = o as MouseEventArgs;

            if (e.Source != null)
            {
                if (e.Source.ToString() != "ChessDotNET.GUI.Views.SideMenu")
                {
                    if (PropertiesDict["SideMenuVisibility"] == "Visible")
                    {
                        WasSideMenuOpen = true;
                        PropertiesDict["SideMenuVisibility"] = "Hidden";
                    }
                    else
                    {
                        WasSideMenuOpen = false;
                    }

                    OnPropertyChangedByPropertyName("PropertiesDict");
                }
            }
        }
        private void WindowMouseLeftUpAction(object o, TileDictionary tileDict)
        {
            if (IsInputAllowed())
            {
                MouseEventArgs e = o as MouseEventArgs;

                if (currentlyDraggedChessPieceImage == null) return;
                if (currentlyDraggedChessPieceImage.IsMouseCaptured) currentlyDraggedChessPieceImage.ReleaseMouseCapture();

                if (IsMouseMoving)
                {
                    IsMouseMoving = false;
                    if (DragOverCanvasPosition.X < 0
                        || DragOverCanvasPosition.X > 400
                        || DragOverCanvasPosition.Y < 0
                        || DragOverCanvasPosition.Y > 400)
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
                        Coords newCoords = Coords.CanvasPositionToCoords(DragOverCanvasPosition);

                        if (newCoords.X >= 1 && newCoords.X <= 8 && newCoords.Y >= 1 && newCoords.Y <= 8
                            && !(newCoords.X == oldCoords.X && newCoords.Y == oldCoords.Y))
                        {
                            MoveValidationData moveValidationData = MoveValidationGameLogic.ValidateCurrentMove(
                                TileDictReadOnly, oldCoords, newCoords
                                );

                            if (moveValidationData.IsValid)
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

                                // get a queen if your pawn is on opposite of the field:
                                if (SwapPawnGameLogic.CanSwap(tileDict, oldCoords, newCoords))
                                {
                                    tileDict[newCoords.String].ChessPiece = new ChessPiece(
                                        tileDict[oldCoords.String].ChessPiece.ChessPieceColor, ChessPieceType.Queen, isRotated
                                        );
                                    tileDict[oldCoords.String].ChessPiece = new ChessPiece();
                                    tileDict[newCoords.String].IsOccupied = true;
                                    tileDict[oldCoords.String].IsOccupied = false;
                                }

                                // check if a king tries to castle:
                                else if (moveValidationData.CanCastle)
                                {
                                    tileDict.MoveChessPiece(oldCoords, newCoords);
                                    Coords rookOldCoords = moveValidationData.Coords[0];
                                    Coords rookNewCoords = moveValidationData.Coords[1];
                                    tileDict.MoveChessPiece(rookOldCoords, rookNewCoords);
                                }
                                else
                                {
                                    tileDict.MoveChessPiece(oldCoords, newCoords);
                                }

                                OnPropertyChangedByPropertyName("TileDict");

                                // Debug: Print occupation state of all tiles:
                                for (int i = 8; i > 0; i--)
                                {
                                    for (int j = 1; j < 9; j++)
                                    {
                                        Coords c = new Coords(j, i);
                                        char oc = tileDict[c.String].IsOccupied ? 'O' : ' ';
                                        System.Diagnostics.Debug.Write(c.String + ":" + oc + " ");
                                        if (j == 8)
                                        {
                                            System.Diagnostics.Debug.WriteLine("");
                                        }
                                    }
                                }
                                System.Diagnostics.Debug.WriteLine("");
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
        private void ChessPieceMouseleftDownAction(object o)
        {
            if (IsInputAllowed())
            {
                object param = ((CompositeCommandParameter)o).Parameter;
                MouseEventArgs e = ((CompositeCommandParameter)o).EventArgs as MouseEventArgs;
                currentlyDraggedChessPieceImage = null;
                currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
                currentlyDraggedChessPieceOriginalCanvasTop = -1000;
                currentlyDraggedChessPieceImage = param as Image;
                if (! ChessPieceImages.IsEmpty(currentlyDraggedChessPieceImage.Source))
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
                WasSideMenuOpen = false;
                e.Handled = true;
            }
        }
        private void SideMenuNewGameAction()
        {
            PropertiesDict["SideMenuMainMenuVisibility"] = "Hidden";
            PropertiesDict["SideMenuNewGameModeVisibility"] = "Visible";
            OnPropertyChangedByPropertyName("PropertiesDict");
        }
        private void SideMenuNewGameModeLocalAction()
        {
            PropertiesDict["SideMenuNewGameModeVisibility"] = "Hidden";
            PropertiesDict["SideMenuButtonsNewGameLocalColorVisibility"] = "Visible";
            OnPropertyChangedByPropertyName("PropertiesDict");
        }
        private void SideMenuNewGameLocalColorGoBackAction()
        {
            PropertiesDict["SideMenuButtonsNewGameLocalColorVisibility"] = "Hidden";
            PropertiesDict["SideMenuNewGameModeVisibility"] = "Visible";
            OnPropertyChangedByPropertyName("PropertiesDict");
        }
        private void SideMenuNewGameLocalAsWhiteAction()
        {
            currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            currentlyDraggedChessPieceOriginalCanvasTop = -1000;

            PropertiesDict["SideMenuVisibility"] = "Hidden";
            PropertiesDict["SideMenuMainMenuVisibility"] = "Visible";
            PropertiesDict["SideMenuNewGameModeVisibility"] = "Hidden";
            OnPropertyChangedByPropertyName("PropertiesDict");

            StartGame(false);
        }
        private void SideMenuNewGameLocalAsBlackAction()
        {
            currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            currentlyDraggedChessPieceOriginalCanvasTop = -1000;

            PropertiesDict["SideMenuVisibility"] = "Hidden";
            PropertiesDict["SideMenuMainMenuVisibility"] = "Visible";
            PropertiesDict["SideMenuNewGameModeVisibility"] = "Hidden";
            OnPropertyChangedByPropertyName("PropertiesDict");

            StartGame(true);
        }
        private void SideMenuNewGameModeGoBackAction()
        {
            PropertiesDict["SideMenuMainMenuVisibility"] = "Visible";
            PropertiesDict["SideMenuNewGameModeVisibility"] = "Hidden";
            OnPropertyChangedByPropertyName("PropertiesDict");
        }
        //private void SideMenuSettingsAction()
        //{
        //    AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
        //    PropertiesDict["SideMenuVisibility"] = "Hidden";
        //    PropertiesDict["OverlaySettingsVisibility"] = "Visible";

        //    OnPropertyChangedByPropertyName("PropertiesDict");
        //}
        //private void OverlaySettingsSaveAction()
        //{
        //}
        //private void OverlaySettingsCancelAction()
        //{
        //    PropertiesDict["OverlaySettingsVisibility"] = "Hidden";
        //}
        private void SideMenuQuitProgramAction()
        {
            Application.Current.Shutdown();
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
                propertiesDict["ChessCanvasRotationAngle"] = "180";
                propertiesDict["ChessCanvasRotationCenterX"] = "200";
                propertiesDict["ChessCanvasRotationCenterY"] = "200";
            }
            else
            {
                propertiesDict["ChessCanvasRotationAngle"] = "0";
                propertiesDict["ChessCanvasRotationCenterX"] = "0";
                propertiesDict["ChessCanvasRotationCenterY"] = " -200";
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

            OnPropertyChangedByPropertyName("PropertiesDict");
            OnPropertyChangedByPropertyName("TileDict");
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

            tileDict["B6"]. IsOccupied = true;
            tileDict["C6"]. IsOccupied = true;
            tileDict["D6"]. IsOccupied = true;
            tileDict["E6"]. IsOccupied = true;
            tileDict["F6"]. IsOccupied = true;
            tileDict["G6"]. IsOccupied = true;

            tileDict["A1"]. IsOccupied = true;
            tileDict["E1"]. IsOccupied = true;
            tileDict["H1"]. IsOccupied = true;

            tileDict["A8"]. IsOccupied = true;
            tileDict["E8"]. IsOccupied = true;
            tileDict["H8"].IsOccupied = true;

            OnPropertyChangedByPropertyName("PropertiesDict");
            OnPropertyChangedByPropertyName("TileDict");
        }
        private bool IsInputAllowed()
        {
            if (propertiesDict["SideMenuVisibility"] == "Visible") return false;
            if (propertiesDict["OverlaySettingsVisibility"] == "Visible") return false;
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
