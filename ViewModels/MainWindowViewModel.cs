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
using ChessDotNET.Helpers;
using static ChessDotNET.Helpers.Rows;

namespace ChessDotNET.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Constructors
        public MainWindowViewModel()
        {
            chessPieces = new ChessPieces();
            tileImageList = new List<List<ImageSource>>();
            tileImageStringList = new List<List<string>>();
            for (int col = 0; col < 8; col++)
            {
                List<ImageSource> tempList = new List<ImageSource>();
                List<string> tempStringList = new List<string>();
                for (int row = 0; row < 8; row++)
                {
                    tempList.Add(chessPieces.Empty);
                    tempStringList.Add("");
                }
                tileImageList.Add(tempList);
                tileImageStringList.Add(tempStringList);
            }

            WindowMouseMoveCommand = new RelayCommand<object>(o => WindowMouseMoveAction(o));
            WindowMouseLeftUpCommand = new RelayCommand(WindowMouseLeftUpAction);
            ChessPieceMouseLeftDownCommand = new RelayCommand<object>(o => ChessPieceMouseleftDownAction(o));

            currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            currentlyDraggedChessPieceOriginalCanvasTop = -1000;

            StartGame("white");

            //MoveChessPiece(G, 8, A, 2);
        }
        #endregion Constuctors

        #region Fields
        private readonly ChessPieces chessPieces;
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
        private List<List<ImageSource>> tileImageList;
        private List<List<string>> tileImageStringList;
        private ImageSource tileImage;
        #endregion Property-Values

        #region Properties
        public List<List<ImageSource>> TileImageList
        {
            get
            {
                return tileImageList;
            }
            set
            {
                tileImageList = value;
                OnPropertyChanged();
            }
        }
        public List<List<string>> TileImageStringList
        {
            get
            {
                return tileImageStringList;
            }
            set
            {
                tileImageStringList = value;
            }
        }
        public ImageSource TileImage
        {
            get
            {
                return tileImage;
            }
            set
            {
                tileImage = value;
                OnPropertyChanged();
            }
        }
        #endregion Properties

        #region Commands
        public RelayCommand<object> WindowMouseMoveCommand { get; }
        public RelayCommand WindowMouseLeftUpCommand { get; }
        public RelayCommand<object> ChessPieceMouseLeftDownCommand { get; }
        #endregion Commands

        #region Command-Actions
        private void WindowMouseMoveAction(object o)
        {
            MouseEventArgs e = o as MouseEventArgs;

            dragOverCanvasPosition = e.GetPosition(canvas);
            if (currentlyDraggedChessPiece != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (!isMouseMoving)
                    {
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

            currentlyDraggedChessPiece = param as Image;
            canvas = VisualTreeHelper.GetParent(param as Image) as Canvas;

            if (currentlyDraggedChessPieceOriginalCanvasLeft < 0 && currentlyDraggedChessPieceOriginalCanvasTop < 0)
            {
                currentlyDraggedChessPieceOriginalCanvasLeft = int.Parse(currentlyDraggedChessPiece.GetValue(Canvas.LeftProperty).ToString());
                currentlyDraggedChessPieceOriginalCanvasTop = int.Parse(currentlyDraggedChessPiece.GetValue(Canvas.TopProperty).ToString());
            }

            currentlyDraggedChessPiece.CaptureMouse();

            e.Handled = true;
        }
        private void WindowMouseLeftUpAction()
        {
            if (isMouseMoving && currentlyDraggedChessPiece != null)
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
                    double X = dragOverCanvasPosition.X - dragOverCanvasPosition.X % 50;
                    double Y = dragOverCanvasPosition.Y - dragOverCanvasPosition.Y % 50;

                    Coords newCoords = CanvasPositionToCoords(dragOverCanvasPosition);

                    if (newCoords.Col >= 0 && newCoords.Col <= 7 && newCoords.Row >= 0 && newCoords.Row <= 7)
                    {
                        Point oldPoint = new Point(currentlyDraggedChessPieceOriginalCanvasLeft, currentlyDraggedChessPieceOriginalCanvasTop);
                        Coords oldCoords = CanvasPositionToCoords(oldPoint);

                        bool isValidMove = MoveValidatorGameLogic.ValidateCurrentMove(tileImageStringList, currentlyDraggedChessPiece, bottomColor, oldCoords, newCoords);

                        if (isValidMove)
                        {
                            Canvas.SetLeft(currentlyDraggedChessPiece, X);
                            Canvas.SetTop(currentlyDraggedChessPiece, Y);
                            tileImageStringList[newCoords.Col][newCoords.Row] = currentlyDraggedChessPiece.ToString();
                            tileImageStringList[oldCoords.Col][oldCoords.Row] = "";
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
                    currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
                    currentlyDraggedChessPieceOriginalCanvasTop = -1000;
                }
                currentlyDraggedChessPiece.SetValue(Panel.ZIndexProperty, 10);
                currentlyDraggedChessPiece.ReleaseMouseCapture();
                currentlyDraggedChessPiece = null;
            }
        }
        #endregion Command-Actions

        #region Methods
        internal void PlaceChessPiece(int col, int row, ImageSource chessPiece)
        {
            tileImageList[col - 1][row - 1] = chessPiece;
            tileImageStringList[col - 1][row - 1] = chessPiece.ToString().Contains("png") ? chessPiece.ToString() : "";
            TileImageList = TileImageList;
        }
        internal bool MoveChessPiece(int oldCol, int oldRow, int newCol, int newRow)
        {
            if (tileImageStringList[oldCol - 1][oldRow - 1] == "")
            {
                return false;
            }
            else
            {
                PlaceChessPiece(newCol, newRow, tileImageList[oldCol - 1][oldRow - 1]);
                PlaceChessPiece(oldCol, oldRow, chessPieces.Empty);
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
                    PlaceChessPiece(col, 2, chessPieces.WhitePawn);
                }

                PlaceChessPiece(1, 1, chessPieces.WhiteRook);
                PlaceChessPiece(2, 1, chessPieces.WhiteKnight);
                PlaceChessPiece(3, 1, chessPieces.WhiteBishop);
                PlaceChessPiece(4, 1, chessPieces.WhiteQueen);
                PlaceChessPiece(5, 1, chessPieces.WhiteKing);
                PlaceChessPiece(6, 1, chessPieces.WhiteBishop);
                PlaceChessPiece(7, 1, chessPieces.WhiteKnight);
                PlaceChessPiece(8, 1, chessPieces.WhiteRook);

                for (int col = 1; col < 9; col++)
                {
                    PlaceChessPiece(col, 7, chessPieces.BlackPawn);
                }

                PlaceChessPiece(1, 8, chessPieces.BlackRook);
                PlaceChessPiece(2, 8, chessPieces.BlackKnight);
                PlaceChessPiece(3, 8, chessPieces.BlackBishop);
                PlaceChessPiece(4, 8, chessPieces.BlackQueen);
                PlaceChessPiece(5, 8, chessPieces.BlackKing);
                PlaceChessPiece(6, 8, chessPieces.BlackBishop);
                PlaceChessPiece(7, 8, chessPieces.BlackKnight);
                PlaceChessPiece(8, 8, chessPieces.BlackRook);
            }
            else
            {
                bottomColor = "black";
                for (int col = 1; col < 9; col++)
                {
                    PlaceChessPiece(col, 2, chessPieces.BlackPawn);
                }

                PlaceChessPiece(1, 1, chessPieces.BlackRook);
                PlaceChessPiece(2, 1, chessPieces.BlackKnight);
                PlaceChessPiece(3, 1, chessPieces.BlackBishop);
                PlaceChessPiece(4, 1, chessPieces.BlackKing);
                PlaceChessPiece(5, 1, chessPieces.BlackQueen);
                PlaceChessPiece(6, 1, chessPieces.BlackBishop);
                PlaceChessPiece(7, 1, chessPieces.BlackKnight);
                PlaceChessPiece(8, 1, chessPieces.BlackRook);

                for (int col = 1; col < 9; col++)
                {
                    PlaceChessPiece(col, 7, chessPieces.WhitePawn);
                }

                PlaceChessPiece(1, 8, chessPieces.WhiteRook);
                PlaceChessPiece(2, 8, chessPieces.WhiteKnight);
                PlaceChessPiece(3, 8, chessPieces.WhiteBishop);
                PlaceChessPiece(4, 8, chessPieces.WhiteKing);
                PlaceChessPiece(5, 8, chessPieces.WhiteQueen);
                PlaceChessPiece(6, 8, chessPieces.WhiteBishop);
                PlaceChessPiece(7, 8, chessPieces.WhiteKnight);
                PlaceChessPiece(8, 8, chessPieces.WhiteRook);
            }
        }
        internal Coords CanvasPositionToCoords(Point point)
        {
            int col = (int)((point.X - point.X % 50) / 50);
            int row = (int)((point.Y - point.Y % 50) / 50);
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
