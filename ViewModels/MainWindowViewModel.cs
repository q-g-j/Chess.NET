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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ChessDotNET.Helpers;
using CommunityToolkit.Mvvm.Input;

namespace ChessDotNET.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Constructors
        public MainWindowViewModel()
        {
            chessPieces = new ChessPieces();
            cellImageList = new List<List<ImageSource>>();
            cellImageStringList = new List<List<string>>();
            for (int col = 0; col < 8; col++)
            {
                List<ImageSource> tempList = new List<ImageSource>();
                List<string> tempStringList = new List<string>();
                for (int row = 0; row < 8; row++)
                {
                    tempList.Add(chessPieces.Empty);
                    tempStringList.Add("");
                }
                cellImageList.Add(tempList);
                cellImageStringList.Add(tempStringList);
            }

            WindowMouseMoveCommand = new RelayCommand<object>(o => WindowMouseMoveAction(o));
            WindowMouseLeftUpCommand = new RelayCommand(WindowMouseLeftUpAction);
            ChessPieceMouseLeftDownCommand = new RelayCommand<object>(o => ChessPieceMouseleftDownAction(o));

            currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            currentlyDraggedChessPieceOriginalCanvasTop = -1000;

            StartGame("white");

            MoveChessPiece(7,8,1,2);
        }
        #endregion Constuctors

        #region Fields
        private readonly ChessPieces chessPieces;
        private Canvas canvas;
        private Image currentlyDraggedChessPiece;
        private int currentlyDraggedChessPieceOriginalCanvasLeft;
        private int currentlyDraggedChessPieceOriginalCanvasTop;
        private Point dragOverPosition;
        private bool isMouseMoving;
        #endregion Fields

        #region Property-Values
        private List<List<ImageSource>> cellImageList;
        private List<List<string>> cellImageStringList;
        private ImageSource cellImage;
        #endregion Property-Values

        #region Properties
        public List<List<ImageSource>> CellImageList
        {
            get
            {
                return cellImageList;
            }
            set
            {
                cellImageList = value;
                OnPropertyChanged();
            }
        }
        public List<List<string>> CellImageStringList
        {
            get
            {
                return cellImageStringList;
            }
            set
            {
                cellImageStringList = value;
            }
        }
        public ImageSource CellImage
        {
            get
            {
                return cellImage;
            }
            set
            {
                cellImage = value;
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

            dragOverPosition = e.GetPosition(canvas);
            if (currentlyDraggedChessPiece != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    isMouseMoving = true;
                    dragOverPosition = e.GetPosition(canvas);
                    currentlyDraggedChessPiece.SetValue(Panel.ZIndexProperty, 20);

                    double X = dragOverPosition.X - 25;
                    double Y = dragOverPosition.Y - 25;

                    Canvas.SetLeft(currentlyDraggedChessPiece, X);
                    Canvas.SetTop(currentlyDraggedChessPiece, Y);
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
                if (dragOverPosition.X < 0 || dragOverPosition.X > 400 || dragOverPosition.Y < 0 || dragOverPosition.Y > 400)
                {
                    Canvas.SetLeft(currentlyDraggedChessPiece, currentlyDraggedChessPieceOriginalCanvasLeft);
                    Canvas.SetTop(currentlyDraggedChessPiece, currentlyDraggedChessPieceOriginalCanvasTop);
                    currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
                    currentlyDraggedChessPieceOriginalCanvasTop = -1000;
                }
                else
                {
                    double X = dragOverPosition.X - dragOverPosition.X % 50;
                    double Y = dragOverPosition.Y - dragOverPosition.Y % 50;

                    Coords coords = CanvasPositionToCoords(dragOverPosition);

                    if (coords.Col >= 0 && coords.Col <= 7 && coords.Row >= 0 && coords.Row <= 7)
                    {
                        if (cellImageStringList[coords.Col][coords.Row] == "")
                        {
                            Canvas.SetLeft(currentlyDraggedChessPiece, X);
                            Canvas.SetTop(currentlyDraggedChessPiece, Y);
                            Point oldPoint = new Point(currentlyDraggedChessPieceOriginalCanvasLeft, currentlyDraggedChessPieceOriginalCanvasTop);
                            Coords oldCoords = CanvasPositionToCoords(oldPoint);
                            cellImageStringList[coords.Col][coords.Row] = currentlyDraggedChessPiece.ToString();
                            cellImageStringList[oldCoords.Col][oldCoords.Row] = "";
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
            if      (row == 1) row = 8;
            else if (row == 2) row = 7;
            else if (row == 3) row = 6;
            else if (row == 4) row = 5;
            else if (row == 5) row = 4;
            else if (row == 6) row = 3;
            else if (row == 7) row = 2;
            else if (row == 8) row = 1;

            cellImageList[col - 1][row - 1] = chessPiece;
            cellImageStringList[col - 1][row - 1] = chessPiece.ToString();
            CellImageList = CellImageList;
        }
        internal bool MoveChessPiece(int oldCol, int oldRow, int newCol, int newRow)
        {
            int row = -1;
            if      (oldRow == 1) row = 8;
            else if (oldRow == 2) row = 7;
            else if (oldRow == 3) row = 6;
            else if (oldRow == 4) row = 5;
            else if (oldRow == 5) row = 4;
            else if (oldRow == 6) row = 3;
            else if (oldRow == 7) row = 2;
            else if (oldRow == 8) row = 1;

            if (cellImageStringList[oldCol - 1][row - 1] == "")
            {
                return false;
            }
            else
            {
                PlaceChessPiece(newCol, newRow, cellImageList[oldCol - 1][row - 1]);
                PlaceChessPiece(oldCol, oldRow, chessPieces.Empty);
            }
            return true;
        }
        internal void StartGame(string colorAtBottom)
        {
            if (colorAtBottom == "white")
            {
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
