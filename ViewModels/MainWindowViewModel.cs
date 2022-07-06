﻿using System;
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
using OpenPop.Pop3;
using CommunityToolkit.Mvvm.Input;

using ChessDotNET.GameLogic;
using ChessDotNET.CustomTypes;
using ChessDotNET.ViewHelpers;
using ChessDotNET.Settings;
using OpenPop.Mime;

namespace ChessDotNET.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Constructors
        public MainWindowViewModel()
        {
            appSettingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Chess.NET");
            appSettings = new AppSettings(appSettingsFolder);

            WindowMouseMoveCommand = new RelayCommand<object>(o => WindowMouseMoveAction(o));
            WindowMouseLeftUpCommand = new RelayCommand<object>(o => WindowMouseLeftUpAction(o));
            ChessPieceMouseLeftDownCommand = new RelayCommand<object>(o => ChessPieceMouseleftDownAction(o));

            currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            currentlyDraggedChessPieceOriginalCanvasTop = -1000;

            tileDict = new TileDict();

            if (!Directory.Exists(appSettingsFolder))
            {
                Directory.CreateDirectory(appSettingsFolder);
            }

            AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();

            //Console.Write("Password: ");
            //string password = Console.ReadLine().Replace("\\", "\\\\");

            //AppSettingsStruct appSettingsStruct = new AppSettingsStruct()
            //{
            //    EmailServer = new Dictionary<string, string>()
            //    {
            //        ["email"] = "j.emken@gmx.net",
            //        ["server"] = "pop.gmx.net",
            //        ["pop3_port"] = "995",
            //        ["smtp_port"] = "587",
            //        ["username"] = "j.emken@gmx.net",
            //        ["password"] = password
            //    }
            //};

            //Dictionary<string, string> emailServer = appSettingsStruct.EmailServer;

            //Console.WriteLine(@emailServer["password"].Replace("\\\\", "\\"));

            //var client = new Pop3Client();
            //client.Connect(emailServer["server"], int.Parse(emailServer["pop3_port"]), true);
            //client.Authenticate(emailServer["username"], @emailServer["password"].Replace("\\\\", "\\"));
            //var count = client.GetMessageCount();
            //Message message = client.GetMessage(count);
            //Console.WriteLine(message.Headers.Subject);

            StartGame("black");
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
            if (currentlyDraggedChessPieceImage != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
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
        internal void StartGame(string color)
        {
            if (color == "white")
            {
                bottomColor = "white";
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
                bottomColor = "black";
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
