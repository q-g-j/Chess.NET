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
            AppSettingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Chess.NET");
            appSettings = new AppSettings(AppSettingsFolder);

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
            SideMenuSettingsCommand = new RelayCommand(SideMenuSettingsAction);
            SideMenuQuitProgramCommand = new RelayCommand(SideMenuQuitProgramAction);

            OverlaySettingsSaveCommand = new RelayCommand(OverlaySettingsSaveAction);
            OverlaySettingsCancelCommand = new RelayCommand(OverlaySettingsCancelAction);

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
            isSettingsSaved = false;

            if (!Directory.Exists(AppSettingsFolder))
            {
                Directory.CreateDirectory(AppSettingsFolder);
            }

            StartGame(false);
        }
        #endregion Constuctors

        #region Fields
        private readonly AppSettings appSettings;

        internal readonly string AppSettingsFolder;
        internal Canvas ChessCanvas;
        internal Image CurrentlyDraggedChessPieceImage;
        internal int CurrentlyDraggedChessPieceOriginalCanvasLeft;
        internal int CurrentlyDraggedChessPieceOriginalCanvasTop;
        internal Point DragOverCanvasPosition;
        internal Point DragOverChessPiecePosition;
        internal bool IsMouseMoving;
        internal bool WasSideMenuOpen;
        internal bool isSettingsSaved;
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
        internal void OpenSideMenuAction()
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
        internal void WindowMouseMoveAction(object o)
        {
            MouseEventArgs e = o as MouseEventArgs;

            if (CurrentlyDraggedChessPieceImage != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {

                    if (!WasSideMenuOpen)
                    {
                        if (!IsMouseMoving)
                        {
                            DragOverCanvasPosition = e.GetPosition(ChessCanvas);
                            DragOverChessPiecePosition = e.GetPosition(CurrentlyDraggedChessPieceImage);
                        }
                        IsMouseMoving = true;
                        DragOverCanvasPosition = e.GetPosition(ChessCanvas);
                        CurrentlyDraggedChessPieceImage.SetValue(Panel.ZIndexProperty, 20);

                        Canvas.SetLeft(CurrentlyDraggedChessPieceImage, DragOverCanvasPosition.X - DragOverChessPiecePosition.X);
                        Canvas.SetTop(CurrentlyDraggedChessPieceImage, DragOverCanvasPosition.Y - DragOverChessPiecePosition.Y);
                    }

                    OnPropertyChangedByPropertyName("PropertiesDict");
                }
            }
            e.Handled = true;
        }
        internal void WindowMouseLeftDownAction(object o)
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
        internal void WindowMouseLeftUpAction(object o, TileDictionary tileDict)
        {
            if (IsInputAllowed())
            {
                MouseEventArgs e = o as MouseEventArgs;

                if (CurrentlyDraggedChessPieceImage == null) return;
                if (CurrentlyDraggedChessPieceImage.IsMouseCaptured) CurrentlyDraggedChessPieceImage.ReleaseMouseCapture();

                if (IsMouseMoving)
                {
                    IsMouseMoving = false;
                    if (DragOverCanvasPosition.X < 0 || DragOverCanvasPosition.X > 400 || DragOverCanvasPosition.Y < 0 || DragOverCanvasPosition.Y > 400)
                    {
                        Canvas.SetLeft(CurrentlyDraggedChessPieceImage, CurrentlyDraggedChessPieceOriginalCanvasLeft);
                        Canvas.SetTop(CurrentlyDraggedChessPieceImage, CurrentlyDraggedChessPieceOriginalCanvasTop);
                        CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
                        CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;
                    }
                    else
                    {
                        Point oldPoint = new Point(CurrentlyDraggedChessPieceOriginalCanvasLeft, CurrentlyDraggedChessPieceOriginalCanvasTop);
                        Coords oldCoords = Coords.CanvasPositionToCoords(oldPoint);
                        Coords newCoords = Coords.CanvasPositionToCoords(DragOverCanvasPosition);

                        if (newCoords.Col >= 1 && newCoords.Col <= 8 && newCoords.Row >= 1 && newCoords.Row <= 8
                            && !(newCoords.Col == oldCoords.Col && newCoords.Row == oldCoords.Row))
                        {

                            bool isValidMove = MoveValidationGameLogic.ValidateCurrentMove(TileDictReadOnly, oldCoords, newCoords);

                            if (isValidMove)
                            {
                                Canvas.SetLeft(CurrentlyDraggedChessPieceImage, CurrentlyDraggedChessPieceOriginalCanvasLeft);
                                Canvas.SetTop(CurrentlyDraggedChessPieceImage, CurrentlyDraggedChessPieceOriginalCanvasTop);
                                //Console.WriteLine("Old Coords before: " + "Is occupied? " + tileDict[oldCoordsString.ToString()].IsOccupied.ToString() + "\t| Coords: " + oldCoordsString.ToString() + "\t| Color = " + tileDict[oldCoordsString.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[oldCoordsString.ToString()].ChessPiece.ChessPieceType.ToString());
                                //Console.WriteLine("New Coords before: " + "Is occupied? " + tileDict[newCoordsString.ToString()].IsOccupied.ToString() + "\t| Coords: " + newCoordsString.ToString() + "\t| Color = " + tileDict[newCoordsString.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[newCoordsString.ToString()].ChessPiece.ChessPieceType.ToString());

                                tileDict[newCoords.ToString()].SetChessPiece(tileDict[oldCoords.ToString()].ChessPiece.ChessPieceImage);
                                tileDict[oldCoords.ToString()].SetChessPiece(ChessPieceImages.Empty);
                                tileDict[newCoords.ToString()].ChessPiece.MoveCount++;


                                // Get a queen if your pawn is on opposite of the field
                                if (tileDict[newCoords.ToString()].ChessPiece.ChessPieceType == ChessPieceType.Pawn && newCoords.Row == 8)
                                {
                                    tileDict[newCoords.ToString()].ChessPiece.ChessPieceImage = ChessPieceImages.WhiteQueen;
                                    tileDict[newCoords.ToString()].ChessPiece.ChessPieceType = ChessPieceType.Queen;
                                }
                                if (tileDict[newCoords.ToString()].ChessPiece.ChessPieceType == ChessPieceType.Pawn && newCoords.Row == 1)
                                {
                                    tileDict[newCoords.ToString()].ChessPiece.ChessPieceImage = ChessPieceImages.BlackQueen;
                                    tileDict[newCoords.ToString()].ChessPiece.ChessPieceType = ChessPieceType.Queen;
                                }

                                // store a list of threatening tiles:
                                tileDict[newCoords.ToString()].ThreatenedByTileList = ThreatDetectionGameLogic.GetThreateningTilesList(tileDict, newCoords);

                                //Console.WriteLine("Old Coords after : " + "Is occupied? " + tileDict[oldCoordsString.ToString()].IsOccupied.ToString() + "\t| Coords: " + oldCoordsString.ToString() + "\t| Color = " + tileDict[oldCoordsString.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[oldCoordsString.ToString()].ChessPiece.ChessPieceType.ToString());
                                //Console.WriteLine("New Coords after : " + "Is occupied? " + tileDict[newCoordsString.ToString()].IsOccupied.ToString() + "\t| Coords: " + newCoordsString.ToString() + "\t| Color = " + tileDict[newCoordsString.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[newCoordsString.ToString()].ChessPiece.ChessPieceType.ToString());
                                //Console.WriteLine("MoveCount: " + tileDict[newCoordsString.ToString()].ChessPiece.MoveCount);
                                //Console.WriteLine();

                                OnPropertyChangedByPropertyName("TileDict");
                            }
                            else
                            {
                                Canvas.SetLeft(CurrentlyDraggedChessPieceImage, CurrentlyDraggedChessPieceOriginalCanvasLeft);
                                Canvas.SetTop(CurrentlyDraggedChessPieceImage, CurrentlyDraggedChessPieceOriginalCanvasTop);
                            }
                        }
                        else
                        {
                            Canvas.SetLeft(CurrentlyDraggedChessPieceImage, CurrentlyDraggedChessPieceOriginalCanvasLeft);
                            Canvas.SetTop(CurrentlyDraggedChessPieceImage, CurrentlyDraggedChessPieceOriginalCanvasTop);
                        }
                    }
                    CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
                    CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;
                    CurrentlyDraggedChessPieceImage.SetValue(Panel.ZIndexProperty, 10);
                }
                CurrentlyDraggedChessPieceImage = null;
                e.Handled = true;
            }
        }
        internal void ChessPieceMouseleftDownAction(object o)
        {
            if (IsInputAllowed())
            {
                object param = ((CompositeCommandParameter)o).Parameter;
                MouseEventArgs e = ((CompositeCommandParameter)o).EventArgs as MouseEventArgs;
                CurrentlyDraggedChessPieceImage = null;
                CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
                CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;
                CurrentlyDraggedChessPieceImage = param as Image;
                if (!ChessPieceImages.IsEmpty(CurrentlyDraggedChessPieceImage.Source))
                {
                    ChessCanvas = VisualTreeHelper.GetParent(param as Image) as Canvas;

                    if (CurrentlyDraggedChessPieceOriginalCanvasLeft < 0 && CurrentlyDraggedChessPieceOriginalCanvasTop < 0)
                    {
                        CurrentlyDraggedChessPieceOriginalCanvasLeft = int.Parse(CurrentlyDraggedChessPieceImage.GetValue(Canvas.LeftProperty).ToString());
                        CurrentlyDraggedChessPieceOriginalCanvasTop = int.Parse(CurrentlyDraggedChessPieceImage.GetValue(Canvas.TopProperty).ToString());
                    }
                    CurrentlyDraggedChessPieceImage.CaptureMouse();
                }
                WasSideMenuOpen = false;
                e.Handled = true;
            }
        }
        internal void SideMenuNewGameAction()
        {
            PropertiesDict["SideMenuMainMenuVisibility"] = "Hidden";
            PropertiesDict["SideMenuNewGameModeVisibility"] = "Visible";
            OnPropertyChangedByPropertyName("PropertiesDict");
        }
        internal void SideMenuNewGameModeLocalAction()
        {
            PropertiesDict["SideMenuNewGameModeVisibility"] = "Hidden";
            PropertiesDict["SideMenuButtonsNewGameLocalColorVisibility"] = "Visible";
            OnPropertyChangedByPropertyName("PropertiesDict");
        }
        internal void SideMenuNewGameModeEmailAction()
        {
            PropertiesDict["SideMenuVisibility"] = "Hidden";
            PropertiesDict["SideMenuMainMenuVisibility"] = "Visible";
            PropertiesDict["SideMenuNewGameModeVisibility"] = "Hidden";
            PropertiesDict["OverlayNewEmailGameErrorLabelVisibility"] = "Hidden";
            PropertiesDict["OverlayNewEmailGameVisibility"] = "Visible";
            OnPropertyChangedByPropertyName("PropertiesDict");
        }
        internal void SideMenuNewGameLocalColorGoBackAction()
        {
            PropertiesDict["SideMenuButtonsNewGameLocalColorVisibility"] = "Hidden";
            PropertiesDict["SideMenuNewGameModeVisibility"] = "Visible";
            OnPropertyChangedByPropertyName("PropertiesDict");
        }
        internal void SideMenuNewGameLocalAsWhiteAction()
        {
            CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;

            PropertiesDict["SideMenuVisibility"] = "Hidden";
            PropertiesDict["SideMenuMainMenuVisibility"] = "Visible";
            PropertiesDict["SideMenuNewGameModeVisibility"] = "Hidden";
            OnPropertyChangedByPropertyName("PropertiesDict");

            StartGame(false);
        }
        internal void SideMenuNewGameLocalAsBlackAction()
        {
            CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;

            PropertiesDict["SideMenuVisibility"] = "Hidden";
            PropertiesDict["SideMenuMainMenuVisibility"] = "Visible";
            PropertiesDict["SideMenuNewGameModeVisibility"] = "Hidden";
            OnPropertyChangedByPropertyName("PropertiesDict");

            StartGame(true);
        }
        internal void SideMenuNewGameModeGoBackAction()
        {
            PropertiesDict["SideMenuMainMenuVisibility"] = "Visible";
            PropertiesDict["SideMenuNewGameModeVisibility"] = "Hidden";
            OnPropertyChangedByPropertyName("PropertiesDict");
        }
        internal void SideMenuSettingsAction()
        {
            AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
            PropertiesDict["SideMenuVisibility"] = "Hidden";
            PropertiesDict["OverlaySettingsVisibility"] = "Visible";

            isSettingsSaved = false;

            OnPropertyChangedByPropertyName("PropertiesDict");
        }
        internal void SideMenuQuitProgramAction()
        {
            Application.Current.Shutdown();
        }
        internal void OverlaySettingsSaveAction()
        {
        }
        internal void OverlaySettingsCancelAction()
        {
            PropertiesDict["OverlaySettingsVisibility"] = "Hidden";
        }
        #endregion CommandActions

        #region Methods
        internal void StartGame(bool doRotate)
        {
            CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;

            horizontalNotationList = Enumerable.Repeat<string>("0", 8).ToList<string>();
            verticalNotationList = Enumerable.Repeat<string>("0", 8).ToList<string>();

            if (doRotate)
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

            if (doRotate)
            {
                for (int i = 0; i < 8; i++)
                {
                    if      (i == 0) horizontalNotationList[i] = "H";
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
                    if      (i == 0) horizontalNotationList[i] = "A";
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
                    if      (i == 0) verticalNotationList[i] = "8";
                    else if (i == 1) verticalNotationList[i] = "7";
                    else if (i == 2) verticalNotationList[i] = "6";
                    else if (i == 3) verticalNotationList[i] = "5";
                    else if (i == 4) verticalNotationList[i] = "4";
                    else if (i == 5) verticalNotationList[i] = "3";
                    else if (i == 6) verticalNotationList[i] = "2";
                    else if (i == 7) verticalNotationList[i] = "1";
                }
            }

            tileDict = new TileDictionary();

            for (int col = 1; col < 9; col++)
            {
                tileDict.PlaceChessPiece(new Coords(col, 2), ChessPieceColor.White, ChessPieceType.Pawn, doRotate);
            }

            tileDict.PlaceChessPiece(new Coords(1, 1), ChessPieceColor.White, ChessPieceType.Rook, doRotate);
            tileDict.PlaceChessPiece(new Coords(2, 1), ChessPieceColor.White, ChessPieceType.Knight, doRotate);
            tileDict.PlaceChessPiece(new Coords(3, 1), ChessPieceColor.White, ChessPieceType.Bishop, doRotate);
            tileDict.PlaceChessPiece(new Coords(4, 1), ChessPieceColor.White, ChessPieceType.Queen, doRotate);
            tileDict.PlaceChessPiece(new Coords(5, 1), ChessPieceColor.White, ChessPieceType.King, doRotate);
            tileDict.PlaceChessPiece(new Coords(6, 1), ChessPieceColor.White, ChessPieceType.Bishop, doRotate);
            tileDict.PlaceChessPiece(new Coords(7, 1), ChessPieceColor.White, ChessPieceType.Knight, doRotate);
            tileDict.PlaceChessPiece(new Coords(8, 1), ChessPieceColor.White, ChessPieceType.Rook, doRotate);

            for (int col = 1; col < 9; col++)
            {
                tileDict.PlaceChessPiece(new Coords(col, 7), ChessPieceColor.Black, ChessPieceType.Pawn, doRotate);
            }

            tileDict.PlaceChessPiece(new Coords(1, 8), ChessPieceColor.Black, ChessPieceType.Rook, doRotate);
            tileDict.PlaceChessPiece(new Coords(2, 8), ChessPieceColor.Black, ChessPieceType.Knight, doRotate);
            tileDict.PlaceChessPiece(new Coords(3, 8), ChessPieceColor.Black, ChessPieceType.Bishop, doRotate);
            tileDict.PlaceChessPiece(new Coords(4, 8), ChessPieceColor.Black, ChessPieceType.Queen, doRotate);
            tileDict.PlaceChessPiece(new Coords(5, 8), ChessPieceColor.Black, ChessPieceType.King, doRotate);
            tileDict.PlaceChessPiece(new Coords(6, 8), ChessPieceColor.Black, ChessPieceType.Bishop, doRotate);
            tileDict.PlaceChessPiece(new Coords(7, 8), ChessPieceColor.Black, ChessPieceType.Knight, doRotate);
            tileDict.PlaceChessPiece(new Coords(8, 8), ChessPieceColor.Black, ChessPieceType.Rook, doRotate);

            OnPropertyChangedByPropertyName("PropertiesDict");
            OnPropertyChangedByPropertyName("TileDict");
            OnPropertyChangedByPropertyName("HorizontalNotationList");
            OnPropertyChangedByPropertyName("VerticalNotationList");
        }
        internal bool IsInputAllowed()
        {
            if (propertiesDict["SideMenuVisibility"] == "Visible") return false;
            if (propertiesDict["OverlaySettingsVisibility"] == "Visible") return false;
            return true;
        }
        internal void OnPropertyChangedByPropertyName(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion Methods

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events
    }
}
