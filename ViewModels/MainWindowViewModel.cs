using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Net.Mail;
using OpenPop.Pop3;
using CommunityToolkit.Mvvm.Input;

using ChessDotNET.GameLogic;
using ChessDotNET.CustomTypes;
using ChessDotNET.ViewHelpers;
using ChessDotNET.Settings;
using OpenPop.Mime;
using System.Net;

namespace ChessDotNET.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Constructors
        public MainWindowViewModel()
        {
            appSettingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Chess.NET");
            appSettings = new AppSettings(appSettingsFolder);

            OpenSideMenuCommand = new RelayCommand(OpenSideMenuAction);
            SideMenuNewGameCommand = new RelayCommand(SideMenuNewGameAction);
            SideMenuNewGameModeLocalCommand = new RelayCommand(SideMenuNewGameModeLocalAction);
            SideMenuNewGameModeEmailCommand = new RelayCommand(SideMenuNewGameModeEmailAction);
            SideMenuNewGameModeGoBackCommand = new RelayCommand(SideMenuNewGameModeGoBackAction);
            SideMenuNewGameLocalAsWhiteCommand = new RelayCommand(SideMenuNewGameLocalAsWhiteAction);
            SideMenuNewGameLocalAsBlackCommand = new RelayCommand(SideMenuNewGameLocalAsBlackAction);
            SideMenuNewGameLocalColorGoBackCommand = new RelayCommand(SideMenuNewGameLocalColorGoBackAction);
            QuitProgramCommand = new RelayCommand(QuitProgramAction);
            WindowMouseMoveCommand = new RelayCommand<object>(o => WindowMouseMoveAction(o));
            WindowMouseLeftDownCommand = new RelayCommand<object>(o => WindowMouseLeftDownAction(o));
            WindowMouseLeftUpCommand = new RelayCommand<object>(o => WindowMouseLeftUpAction(o));
            ChessPieceMouseLeftDownCommand = new RelayCommand<object>(o => ChessPieceMouseleftDownAction(o));

            currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            currentlyDraggedChessPieceOriginalCanvasTop = -1000;

            wasSideMenuOpen = false;
            sideMenuVisibility = "Collapsed";

            tileDict = new TileDict();

            if (!Directory.Exists(appSettingsFolder))
            {
                Directory.CreateDirectory(appSettingsFolder);
            }

            StartGame(ChessPieceColor.Black);
        }
        #endregion Constuctors

        #region Fields
        private readonly string appSettingsFolder;
        private AppSettings appSettings;
        private Canvas canvas;
        private Image currentlyDraggedChessPieceImage;
        private int currentlyDraggedChessPieceOriginalCanvasLeft;
        private int currentlyDraggedChessPieceOriginalCanvasTop;
        private Point dragOverCanvasPosition;
        private Point dragOverChessPiecePosition;
        private bool isMouseMoving;
        private bool wasSideMenuOpen;
        private ChessPieceColor bottomColor;
        #endregion Fields

        #region Property-Values
        private Dictionary<string, Tile> tileDict;
        private string sideMenuVisibility;
        private string sideMenuMainMenuVisibility;
        private string sideMenuNewGameModeVisibility;
        private string sideMenuButtonsNewGameLocalColorVisibility;
        #endregion Property-Values

        #region Properties
        public string SideMenuVisibility
        {
            get
            {
                return sideMenuVisibility;
            }
            set
            {
                sideMenuVisibility = value;
                OnPropertyChanged();
            }
        }
        public string SideMenuMainMenuVisibility
        {
            get
            {
                return sideMenuMainMenuVisibility;
            }
            set
            {
                sideMenuMainMenuVisibility = value;
                OnPropertyChanged();
            }
        }
        public string SideMenuNewGameModeVisibility
        {
            get
            {
                return sideMenuNewGameModeVisibility;
            }
            set
            {
                sideMenuNewGameModeVisibility = value;
                OnPropertyChanged();
            }
        }
        public string SideMenuButtonsNewGameLocalColorVisibility
        {
            get
            {
                return sideMenuButtonsNewGameLocalColorVisibility;
            }
            set
            {
                sideMenuButtonsNewGameLocalColorVisibility = value;
                OnPropertyChanged();
            }
        }
        public Dictionary<string, Tile> TileDict
        {
            get
            {
                return tileDict;
            }
            set
            {
                tileDict = value;
                OnPropertyChanged();
            }
        }
        private Dictionary<string, Tile> TileDictReadOnly
        {
            get
            {
                return tileDict;
            }
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
        public RelayCommand QuitProgramCommand { get; }
        public RelayCommand<object> WindowMouseMoveCommand { get; }
        public RelayCommand<object> WindowMouseLeftDownCommand { get; }
        public RelayCommand<object> WindowMouseLeftUpCommand { get; }
        public RelayCommand<object> ChessPieceMouseLeftDownCommand { get; }
        #endregion Commands

        #region Command-Actions
        private void OpenSideMenuAction()
        {
            if (!wasSideMenuOpen)
            {
                if (sideMenuVisibility == "Collapsed")
                {
                    SideMenuNewGameModeVisibility = "Collapsed";
                    SideMenuButtonsNewGameLocalColorVisibility = "Collapsed";
                    SideMenuMainMenuVisibility = "Visible";
                    SideMenuVisibility = "Visible";
                }
                else SideMenuVisibility = "Collapsed";
            }
            else
            {
                wasSideMenuOpen = false;
            }
        }
        private void SideMenuNewGameAction()
        {
            SideMenuMainMenuVisibility = "Collapsed";
            SideMenuNewGameModeVisibility = "Visible";
        }
        private void SideMenuNewGameModeLocalAction()
        {
            SideMenuNewGameModeVisibility = "Collapsed";
            SideMenuButtonsNewGameLocalColorVisibility = "Visible";
        }
        private void SideMenuNewGameModeEmailAction()
        {
            SideMenuNewGameModeVisibility = "Collapsed";
            SideMenuButtonsNewGameLocalColorVisibility = "Visible";
        }
        private void SideMenuNewGameLocalColorGoBackAction()
        {
            SideMenuButtonsNewGameLocalColorVisibility = "Collapsed";
            SideMenuNewGameModeVisibility = "Visible";
        }
        private void SideMenuNewGameLocalAsWhiteAction()
        {
            currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            currentlyDraggedChessPieceOriginalCanvasTop = -1000;

            tileDict = new TileDict();
            SideMenuVisibility = "Collapsed";
            SideMenuMainMenuVisibility = "Visible";
            SideMenuNewGameModeVisibility = "Collapsed";
            StartGame(ChessPieceColor.White);
        }
        private void SideMenuNewGameLocalAsBlackAction()
        {
            currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            currentlyDraggedChessPieceOriginalCanvasTop = -1000;

            tileDict = new TileDict();
            SideMenuVisibility = "Collapsed";
            SideMenuMainMenuVisibility = "Visible";
            SideMenuNewGameModeVisibility = "Collapsed";
            StartGame(ChessPieceColor.Black);
        }
        private void SideMenuNewGameModeGoBackAction()
        {
            SideMenuMainMenuVisibility = "Visible";
            SideMenuNewGameModeVisibility = "Collapsed";
        }
        private void QuitProgramAction()
        {
            Application.Current.Shutdown();
        }
        private void WindowMouseLeftDownAction(object o)
        {
            var e = o as MouseEventArgs;

            if (e.Source != null)
            {
                if (e.Source.ToString() != "ChessDotNET.Views.SideMenu")
                {
                    if (SideMenuVisibility == "Visible")
                    {
                        wasSideMenuOpen = true;
                        SideMenuVisibility = "Collapsed";
                    }
                    else
                    {
                        wasSideMenuOpen = false;
                    }
                }
            }
        }
        private void WindowMouseMoveAction(object o)
        {
            MouseEventArgs e = o as MouseEventArgs;

            if (currentlyDraggedChessPieceImage != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (SideMenuVisibility == "Visible")
                    {
                        wasSideMenuOpen = true;
                        SideMenuVisibility = "Collapsed";
                    }
                    else if (!wasSideMenuOpen)
                    {
                        if (!isMouseMoving)
                        {
                            dragOverCanvasPosition = e.GetPosition(canvas);
                            dragOverChessPiecePosition = e.GetPosition(currentlyDraggedChessPieceImage);
                        }
                        isMouseMoving = true;
                        dragOverCanvasPosition = e.GetPosition(canvas);
                        currentlyDraggedChessPieceImage.SetValue(Panel.ZIndexProperty, 20);

                        Canvas.SetLeft(currentlyDraggedChessPieceImage, dragOverCanvasPosition.X - dragOverChessPiecePosition.X);
                        Canvas.SetTop(currentlyDraggedChessPieceImage, dragOverCanvasPosition.Y - dragOverChessPiecePosition.Y);
                    }
                }
            }
            e.Handled = true;
        }
        private void ChessPieceMouseleftDownAction(object o)
        {
            object param = ((CompositeCommandParameter)o).Parameter;
            MouseEventArgs e = ((CompositeCommandParameter)o).EventArgs as MouseEventArgs;
            currentlyDraggedChessPieceImage = null;
            currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            currentlyDraggedChessPieceOriginalCanvasTop = -1000;
            currentlyDraggedChessPieceImage = param as Image;
            if (!ChessPieceImages.IsEmpty(currentlyDraggedChessPieceImage.Source))
            {
                canvas = VisualTreeHelper.GetParent(param as Image) as Canvas;

                if (currentlyDraggedChessPieceOriginalCanvasLeft < 0 && currentlyDraggedChessPieceOriginalCanvasTop < 0)
                {
                    currentlyDraggedChessPieceOriginalCanvasLeft = int.Parse(currentlyDraggedChessPieceImage.GetValue(Canvas.LeftProperty).ToString());
                    currentlyDraggedChessPieceOriginalCanvasTop = int.Parse(currentlyDraggedChessPieceImage.GetValue(Canvas.TopProperty).ToString());
                }
                currentlyDraggedChessPieceImage.CaptureMouse();
            }
            wasSideMenuOpen = false;
            e.Handled = true;
        }
        private void WindowMouseLeftUpAction(object o)
        {
            MouseEventArgs e = o as MouseEventArgs;

            if (currentlyDraggedChessPieceImage == null) return;
            if (currentlyDraggedChessPieceImage.IsMouseCaptured) currentlyDraggedChessPieceImage.ReleaseMouseCapture();

            if (isMouseMoving)
            {
                isMouseMoving = false;
                if (dragOverCanvasPosition.X < 0 || dragOverCanvasPosition.X > 400 || dragOverCanvasPosition.Y < 0 || dragOverCanvasPosition.Y > 400)
                {
                    Canvas.SetLeft(currentlyDraggedChessPieceImage, currentlyDraggedChessPieceOriginalCanvasLeft);
                    Canvas.SetTop(currentlyDraggedChessPieceImage, currentlyDraggedChessPieceOriginalCanvasTop);
                    currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
                    currentlyDraggedChessPieceOriginalCanvasTop = -1000;
                }
                else
                {
                    Coords newCoords = CanvasPositionToCoords(dragOverCanvasPosition);
                    Point oldPoint = new Point(currentlyDraggedChessPieceOriginalCanvasLeft, currentlyDraggedChessPieceOriginalCanvasTop);
                    Coords oldCoords = CanvasPositionToCoords(oldPoint);

                    if (newCoords.Col >= 1 && newCoords.Col <= 8 && newCoords.Row >= 1 && newCoords.Row <= 8
                        && !(newCoords.Col == oldCoords.Col && newCoords.Row == oldCoords.Row))
                    {

                        bool isValidMove = MoveValidatorGameLogic.ValidateCurrentMove(TileDictReadOnly, bottomColor, oldCoords, newCoords);

                        if (isValidMove)
                        {
                            Canvas.SetLeft(currentlyDraggedChessPieceImage, currentlyDraggedChessPieceOriginalCanvasLeft);
                            Canvas.SetTop(currentlyDraggedChessPieceImage, currentlyDraggedChessPieceOriginalCanvasTop);
                            Console.WriteLine("Old Coords before: " + "Is occupied? " + tileDict[oldCoords.ToString()].IsOccupied.ToString() + "\t| Coords: " + oldCoords.ToString() + "\t| Color = " + tileDict[oldCoords.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[oldCoords.ToString()].ChessPiece.ChessPieceType.ToString());
                            Console.WriteLine("New Coords before: " + "Is occupied? " + tileDict[newCoords.ToString()].IsOccupied.ToString() + "\t| Coords: " + newCoords.ToString() + "\t| Color = " + tileDict[newCoords.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[newCoords.ToString()].ChessPiece.ChessPieceType.ToString());

                            tileDict[newCoords.ToString()].ChessPiece = tileDict[oldCoords.ToString()].ChessPiece;
                            tileDict[oldCoords.ToString()].ChessPiece = new ChessPiece(ChessPieceImages.Empty, ChessPieceColor.Empty, ChessPieceType.Empty);
                            tileDict[oldCoords.ToString()].IsOccupied = false;
                            tileDict[newCoords.ToString()].IsOccupied = true;
                            tileDict[newCoords.ToString()].ChessPiece.MoveCount++;
                            TileDict = tileDict;

                            // store a list of threatening tiles:
                            tileDict[newCoords.ToString()].ThreatenedByTileList = ThreatDetectionGameLogic.GetThreateningTilesList(tileDict, newCoords, bottomColor);

                            Console.WriteLine("Old Coords after : " + "Is occupied? " + tileDict[oldCoords.ToString()].IsOccupied.ToString() + "\t| Coords: " + oldCoords.ToString() + "\t| Color = " + tileDict[oldCoords.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[oldCoords.ToString()].ChessPiece.ChessPieceType.ToString());
                            Console.WriteLine("New Coords after : " + "Is occupied? " + tileDict[newCoords.ToString()].IsOccupied.ToString() + "\t| Coords: " + newCoords.ToString() + "\t| Color = " + tileDict[newCoords.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[newCoords.ToString()].ChessPiece.ChessPieceType.ToString());
                            Console.WriteLine("MoveCount: " + tileDict[newCoords.ToString()].ChessPiece.MoveCount);
                            Console.WriteLine();
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
        #endregion Command-Actions

        #region Methods
        internal void PlaceChessPiece(Coords coords, ChessPieceColor color, ChessPieceType type)
        {
            ImageSource image = ChessPieceImages.Empty;
            if (color == ChessPieceColor.White)
            {
                if      (type == ChessPieceType.Pawn)   image = ChessPieceImages.WhitePawn;
                else if (type == ChessPieceType.Rook)   image = ChessPieceImages.WhiteRook;
                else if (type == ChessPieceType.Knight) image = ChessPieceImages.WhiteKnight;
                else if (type == ChessPieceType.Bishop) image = ChessPieceImages.WhiteBishop;
                else if (type == ChessPieceType.Queen)  image = ChessPieceImages.WhiteQueen;
                else if (type == ChessPieceType.King)   image = ChessPieceImages.WhiteKing;
            }
            else
            {
                if      (type == ChessPieceType.Pawn)   image = ChessPieceImages.BlackPawn;
                else if (type == ChessPieceType.Rook)   image = ChessPieceImages.BlackRook;
                else if (type == ChessPieceType.Knight) image = ChessPieceImages.BlackKnight;
                else if (type == ChessPieceType.Bishop) image = ChessPieceImages.BlackBishop;
                else if (type == ChessPieceType.Queen)  image = ChessPieceImages.BlackQueen;
                else if (type == ChessPieceType.King)   image = ChessPieceImages.BlackKing;
            }
            tileDict[coords.ToString()].ChessPiece = new ChessPiece(image, color, type);
            tileDict[coords.ToString()].IsOccupied = true;
            TileDict = tileDict;
        }
        internal void StartGame(ChessPieceColor color)
        {
            if (color == ChessPieceColor.White)
            {
                bottomColor = ChessPieceColor.White;
                for (int col = 1; col < 9; col++)
                {
                    PlaceChessPiece(new Coords(col, 2), ChessPieceColor.White, ChessPieceType.Pawn);
                }

                PlaceChessPiece(new Coords(1, 1), ChessPieceColor.White, ChessPieceType.Rook);
                PlaceChessPiece(new Coords(2, 1), ChessPieceColor.White, ChessPieceType.Knight);
                PlaceChessPiece(new Coords(3, 1), ChessPieceColor.White, ChessPieceType.Bishop);
                PlaceChessPiece(new Coords(4, 1), ChessPieceColor.White, ChessPieceType.Queen);
                PlaceChessPiece(new Coords(5, 1), ChessPieceColor.White, ChessPieceType.King);
                PlaceChessPiece(new Coords(6, 1), ChessPieceColor.White, ChessPieceType.Bishop);
                PlaceChessPiece(new Coords(7, 1), ChessPieceColor.White, ChessPieceType.Knight);
                PlaceChessPiece(new Coords(8, 1), ChessPieceColor.White, ChessPieceType.Rook);

                for (int col = 1; col < 9; col++)
                {
                    PlaceChessPiece(new Coords(col, 7), ChessPieceColor.Black, ChessPieceType.Pawn);
                }

                PlaceChessPiece(new Coords(1, 8), ChessPieceColor.Black, ChessPieceType.Rook);
                PlaceChessPiece(new Coords(2, 8), ChessPieceColor.Black, ChessPieceType.Knight);
                PlaceChessPiece(new Coords(3, 8), ChessPieceColor.Black, ChessPieceType.Bishop);
                PlaceChessPiece(new Coords(4, 8), ChessPieceColor.Black, ChessPieceType.Queen);
                PlaceChessPiece(new Coords(5, 8), ChessPieceColor.Black, ChessPieceType.King);
                PlaceChessPiece(new Coords(6, 8), ChessPieceColor.Black, ChessPieceType.Bishop);
                PlaceChessPiece(new Coords(7, 8), ChessPieceColor.Black, ChessPieceType.Knight);
                PlaceChessPiece(new Coords(8, 8), ChessPieceColor.Black, ChessPieceType.Rook);
            }
            else
            {
                bottomColor = ChessPieceColor.Black;
                for (int col = 1; col < 9; col++)
                {
                    PlaceChessPiece(new Coords(col, 2), ChessPieceColor.Black, ChessPieceType.Pawn);
                }

                PlaceChessPiece(new Coords(1, 1), ChessPieceColor.Black, ChessPieceType.Rook);
                PlaceChessPiece(new Coords(2, 1), ChessPieceColor.Black, ChessPieceType.Knight);
                PlaceChessPiece(new Coords(3, 1), ChessPieceColor.Black, ChessPieceType.Bishop);
                PlaceChessPiece(new Coords(4, 1), ChessPieceColor.Black, ChessPieceType.King);
                PlaceChessPiece(new Coords(5, 1), ChessPieceColor.Black, ChessPieceType.Queen);
                PlaceChessPiece(new Coords(6, 1), ChessPieceColor.Black, ChessPieceType.Bishop);
                PlaceChessPiece(new Coords(7, 1), ChessPieceColor.Black, ChessPieceType.Knight);
                PlaceChessPiece(new Coords(8, 1), ChessPieceColor.Black, ChessPieceType.Rook);

                for (int col = 1; col < 9; col++)
                {
                    PlaceChessPiece(new Coords(col, 7), ChessPieceColor.White, ChessPieceType.Pawn);
                }

                PlaceChessPiece(new Coords(1, 8), ChessPieceColor.White, ChessPieceType.Rook);
                PlaceChessPiece(new Coords(2, 8), ChessPieceColor.White, ChessPieceType.Knight);
                PlaceChessPiece(new Coords(3, 8), ChessPieceColor.White, ChessPieceType.Bishop);
                PlaceChessPiece(new Coords(4, 8), ChessPieceColor.White, ChessPieceType.King);
                PlaceChessPiece(new Coords(5, 8), ChessPieceColor.White, ChessPieceType.Queen);
                PlaceChessPiece(new Coords(6, 8), ChessPieceColor.White, ChessPieceType.Bishop);
                PlaceChessPiece(new Coords(7, 8), ChessPieceColor.White, ChessPieceType.Knight);
                PlaceChessPiece(new Coords(8, 8), ChessPieceColor.White, ChessPieceType.Rook);
            }
        }
        internal Coords CanvasPositionToCoords(Point point)
        {
            int col = (int)((point.X - point.X % 50) / 50) + 1;
            int row = (int)((point.Y - point.Y % 50) / 50) + 1;

            if (row == 1) row = 8;
            else if (row == 2) row = 7;
            else if (row == 3) row = 6;
            else if (row == 4) row = 5;
            else if (row == 5) row = 4;
            else if (row == 6) row = 3;
            else if (row == 7) row = 2;
            else if (row == 8) row = 1;

            return new Coords(col, row);
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
