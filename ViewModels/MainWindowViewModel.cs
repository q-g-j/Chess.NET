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
using CommunityToolkit.Mvvm.Input;

using ChessDotNET.GameLogic;
using ChessDotNET.CustomTypes;
using static ChessDotNET.CustomTypes.Columns;
using ChessDotNET.ViewHelpers;
using System.IO;

namespace ChessDotNET.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Constructors
        public MainWindowViewModel()
        {
            folderSettings = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Chess.NET");
            WindowMouseMoveCommand = new RelayCommand<object>(o => WindowMouseMoveAction(o));
            WindowMouseLeftUpCommand = new RelayCommand<object>(o => WindowMouseLeftUpAction(o));
            ChessPieceMouseLeftDownCommand = new RelayCommand<object>(o => ChessPieceMouseleftDownAction(o));

            currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            currentlyDraggedChessPieceOriginalCanvasTop = -1000;

            tileDict = new TileDict();

            StartGame("white");
        }
        #endregion Constuctors

        #region Fields
        private readonly string folderSettings;
        private Canvas canvas;
        private Image currentlyDraggedChessPiece;
        private int currentlyDraggedChessPieceOriginalCanvasLeft;
        private int currentlyDraggedChessPieceOriginalCanvasTop;
        private Point dragOverCanvasPosition;
        private Point dragOverChessPiecePosition;
        private bool isMouseMoving;
        private string bottomColor;
        #endregion Fields

        #region Property-Values
        private Dictionary<string, Tile> tileDict;
        #endregion Property-Values

        #region Properties
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
        public RelayCommand<object> WindowMouseMoveCommand { get; }
        public RelayCommand<object> WindowMouseLeftUpCommand { get; }
        public RelayCommand<object> ChessPieceMouseLeftDownCommand { get; }
        #endregion Commands

        #region Command-Actions
        private void WindowMouseMoveAction(object o)
        {
            MouseEventArgs e = o as MouseEventArgs;
            if (currentlyDraggedChessPiece != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (!isMouseMoving)
                    {
                        dragOverCanvasPosition = e.GetPosition(canvas);
                        dragOverChessPiecePosition = e.GetPosition(currentlyDraggedChessPiece);
                    }
                    isMouseMoving = true;
                    dragOverCanvasPosition = e.GetPosition(canvas);
                    currentlyDraggedChessPiece.SetValue(Panel.ZIndexProperty, 20);

                    Canvas.SetLeft(currentlyDraggedChessPiece, dragOverCanvasPosition.X - dragOverChessPiecePosition.X);
                    Canvas.SetTop(currentlyDraggedChessPiece, dragOverCanvasPosition.Y - dragOverChessPiecePosition.Y);
                }
            }

            e.Handled = true;
        }
        private void ChessPieceMouseleftDownAction(object o)
        {
            object param = ((CompositeCommandParameter)o).Parameter;
            MouseEventArgs e = ((CompositeCommandParameter)o).EventArgs as MouseEventArgs;
            currentlyDraggedChessPiece = null;
            currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            currentlyDraggedChessPieceOriginalCanvasTop = -1000;
            currentlyDraggedChessPiece = param as Image;
            if (!ChessPieceImages.IsEmpty(currentlyDraggedChessPiece.Source))
            {
                canvas = VisualTreeHelper.GetParent(param as Image) as Canvas;

                if (currentlyDraggedChessPieceOriginalCanvasLeft < 0 && currentlyDraggedChessPieceOriginalCanvasTop < 0)
                {
                    currentlyDraggedChessPieceOriginalCanvasLeft = int.Parse(currentlyDraggedChessPiece.GetValue(Canvas.LeftProperty).ToString());
                    currentlyDraggedChessPieceOriginalCanvasTop = int.Parse(currentlyDraggedChessPiece.GetValue(Canvas.TopProperty).ToString());
                }
                currentlyDraggedChessPiece.CaptureMouse();
            }
            e.Handled = true;
        }
        private void WindowMouseLeftUpAction(object o)
        {
            MouseEventArgs e = o as MouseEventArgs;
            if (currentlyDraggedChessPiece == null) return;
            if (currentlyDraggedChessPiece.IsMouseCaptured) currentlyDraggedChessPiece.ReleaseMouseCapture();

            if (isMouseMoving)
            {
                isMouseMoving = false;
                if (dragOverCanvasPosition.X < 0 || dragOverCanvasPosition.X > 400 || dragOverCanvasPosition.Y < 0 || dragOverCanvasPosition.Y > 400)
                {
                    Canvas.SetLeft(currentlyDraggedChessPiece, currentlyDraggedChessPieceOriginalCanvasLeft);
                    Canvas.SetTop(currentlyDraggedChessPiece, currentlyDraggedChessPieceOriginalCanvasTop);
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
                            Canvas.SetLeft(currentlyDraggedChessPiece, currentlyDraggedChessPieceOriginalCanvasLeft);
                            Canvas.SetTop(currentlyDraggedChessPiece, currentlyDraggedChessPieceOriginalCanvasTop);
                            Console.WriteLine("Old Coords before: " + "Is occupied? " + tileDict[oldCoords.ToString()].IsOccupied.ToString() + "\t| Coords: " + oldCoords.ToString() + "\t| Color = " + tileDict[oldCoords.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[oldCoords.ToString()].ChessPiece.ChessPieceType.ToString());
                            Console.WriteLine("New Coords before: " + "Is occupied? " + tileDict[newCoords.ToString()].IsOccupied.ToString() + "\t| Coords: " + newCoords.ToString() + "\t| Color = " + tileDict[newCoords.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[newCoords.ToString()].ChessPiece.ChessPieceType.ToString());

                            tileDict[oldCoords.ToString()].SetChessPiece(ChessPieceImages.Empty);
                            tileDict[newCoords.ToString()].SetChessPiece(currentlyDraggedChessPiece.Source);
                            TileDict = tileDict;

                            Console.WriteLine("Old Coords after : " + "Is occupied? " + tileDict[oldCoords.ToString()].IsOccupied.ToString() + "\t| Coords: " + oldCoords.ToString() + "\t| Color = " + tileDict[oldCoords.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[oldCoords.ToString()].ChessPiece.ChessPieceType.ToString());
                            Console.WriteLine("New Coords after : " + "Is occupied? " + tileDict[newCoords.ToString()].IsOccupied.ToString() + "\t| Coords: " + newCoords.ToString() + "\t| Color = " + tileDict[newCoords.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[newCoords.ToString()].ChessPiece.ChessPieceType.ToString());
                            Console.WriteLine();
                        }
                        else
                        {
                            Canvas.SetLeft(currentlyDraggedChessPiece, currentlyDraggedChessPieceOriginalCanvasLeft);
                            Canvas.SetTop(currentlyDraggedChessPiece, currentlyDraggedChessPieceOriginalCanvasTop);
                        }
                    }
                    else
                    {
                        Canvas.SetLeft(currentlyDraggedChessPiece, currentlyDraggedChessPieceOriginalCanvasLeft);
                        Canvas.SetTop(currentlyDraggedChessPiece, currentlyDraggedChessPieceOriginalCanvasTop);
                    }
                }
                currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
                currentlyDraggedChessPieceOriginalCanvasTop = -1000;
                currentlyDraggedChessPiece.SetValue(Panel.ZIndexProperty, 10);
            }
            currentlyDraggedChessPiece = null;
            e.Handled = true;
        }
        #endregion Command-Actions

        #region Methods
        internal void PlaceChessPiece(Coords coords, ImageSource chessPieceImage)
        {
            tileDict[coords.ToString()].ChessPiece.ChessPieceImage = chessPieceImage;
            tileDict[coords.ToString()].SetChessPiece(chessPieceImage);

            TileDict = tileDict;
        }
        internal bool MoveChessPiece(Coords oldCoords, Coords newCoords)
        {
            ImageSource oldCoordsImage = tileDict[oldCoords.ToString()].ChessPiece.ChessPieceImage;

            if (!ChessPieceImages.IsEmpty(oldCoordsImage))
            {
                return false;
            }
            else
            {
                PlaceChessPiece(newCoords, oldCoordsImage);
                PlaceChessPiece(oldCoords, ChessPieceImages.Empty);
                tileDict[oldCoords.ToString()].IsOccupied = false;
                tileDict[newCoords.ToString()].IsOccupied = true;
            }
            return true;
        }
        internal void StartGame(string color)
        {
            if (color == "white")
            {
                bottomColor = "white";
                for (int col = 1; col < 9; col++)
                {
                    PlaceChessPiece(new Coords(col, 2), ChessPieceImages.WhitePawn);
                }

                PlaceChessPiece(new Coords(1, 1), ChessPieceImages.WhiteRook);
                PlaceChessPiece(new Coords(2, 1), ChessPieceImages.WhiteKnight);
                PlaceChessPiece(new Coords(3, 1), ChessPieceImages.WhiteBishop);
                PlaceChessPiece(new Coords(4, 1), ChessPieceImages.WhiteQueen);
                PlaceChessPiece(new Coords(5, 1), ChessPieceImages.WhiteKing);
                PlaceChessPiece(new Coords(6, 1), ChessPieceImages.WhiteBishop);
                PlaceChessPiece(new Coords(7, 1), ChessPieceImages.WhiteKnight);
                PlaceChessPiece(new Coords(8, 1), ChessPieceImages.WhiteRook);

                for (int col = 1; col < 9; col++)
                {
                    PlaceChessPiece(new Coords(col, 7), ChessPieceImages.BlackPawn);
                }

                PlaceChessPiece(new Coords(1, 8), ChessPieceImages.BlackRook);
                PlaceChessPiece(new Coords(2, 8), ChessPieceImages.BlackKnight);
                PlaceChessPiece(new Coords(3, 8), ChessPieceImages.BlackBishop);
                PlaceChessPiece(new Coords(4, 8), ChessPieceImages.BlackQueen);
                PlaceChessPiece(new Coords(5, 8), ChessPieceImages.BlackKing);
                PlaceChessPiece(new Coords(6, 8), ChessPieceImages.BlackBishop);
                PlaceChessPiece(new Coords(7, 8), ChessPieceImages.BlackKnight);
                PlaceChessPiece(new Coords(8, 8), ChessPieceImages.BlackRook);
            }
            else
            {
                bottomColor = "black";
                for (int col = 1; col < 9; col++)
                {
                    PlaceChessPiece(new Coords(col, 2), ChessPieceImages.BlackPawn);
                }

                PlaceChessPiece(new Coords(1, 1), ChessPieceImages.BlackRook);
                PlaceChessPiece(new Coords(2, 1), ChessPieceImages.BlackKnight);
                PlaceChessPiece(new Coords(3, 1), ChessPieceImages.BlackBishop);
                PlaceChessPiece(new Coords(4, 1), ChessPieceImages.BlackKing);
                PlaceChessPiece(new Coords(5, 1), ChessPieceImages.BlackQueen);
                PlaceChessPiece(new Coords(6, 1), ChessPieceImages.BlackBishop);
                PlaceChessPiece(new Coords(7, 1), ChessPieceImages.BlackKnight);
                PlaceChessPiece(new Coords(8, 1), ChessPieceImages.BlackRook);

                for (int col = 1; col < 9; col++)
                {
                    {
                        PlaceChessPiece(new Coords(col, 7), ChessPieceImages.WhitePawn);
                    }

                    PlaceChessPiece(new Coords(1, 8), ChessPieceImages.WhiteRook);
                    PlaceChessPiece(new Coords(2, 8), ChessPieceImages.WhiteKnight);
                    PlaceChessPiece(new Coords(3, 8), ChessPieceImages.WhiteBishop);
                    PlaceChessPiece(new Coords(4, 8), ChessPieceImages.WhiteKing);
                    PlaceChessPiece(new Coords(5, 8), ChessPieceImages.WhiteQueen);
                    PlaceChessPiece(new Coords(6, 8), ChessPieceImages.WhiteBishop);
                    PlaceChessPiece(new Coords(7, 8), ChessPieceImages.WhiteKnight);
                    PlaceChessPiece(new Coords(8, 8), ChessPieceImages.WhiteRook);
                }
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
