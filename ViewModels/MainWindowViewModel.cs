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
            SideMenuSettingsCommand = new RelayCommand(SideMenuSettingsAction);
            SettingsPasswordBoxCommand = new RelayCommand<object>(o => SettingsPasswordBoxAction(o));
            SettingsSaveCommand = new RelayCommand(SettingsSaveAction);
            SettingsCancelCommand = new RelayCommand(SettingsCancelAction);
            NewEmailGameStartCommand = new RelayCommand(NewEmailGameStartAction);
            NewEmailGameCancelCommand = new RelayCommand(NewEmailGameCancelAction);
            QuitProgramCommand = new RelayCommand(QuitProgramAction);
            WindowMouseMoveCommand = new RelayCommand<object>(o => WindowMouseMoveAction(o));
            WindowMouseLeftDownCommand = new RelayCommand<object>(o => WindowMouseLeftDownAction(o));
            WindowMouseLeftUpCommand = new RelayCommand<object>(o => WindowMouseLeftUpAction(o));
            ChessPieceMouseLeftDownCommand = new RelayCommand<object>(o => ChessPieceMouseleftDownAction(o));

            currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            currentlyDraggedChessPieceOriginalCanvasTop = -1000;

            newEmailGameRadioButtonWhiteIsChecked = "True";
            newEmailGameRadioButtonBlackIsChecked = "False";

            sideMenuVisibility = "Hidden";
            sideMenuMainMenuVisibility = "Visible";
            sideMenuNewGameModeVisibility = "Hidden";
            SideMenuButtonsNewGameLocalColorVisibility = "Hidden";
            newEmailGameVisibility = "Hidden";
            settingsVisibility = "Hidden";

            newEmailGameTextBoxOwnEmail = "";
            newEmailGameTextBoxOpponentEmail = "";

            wasSideMenuOpen = false;
            isEmailGame = false;
            doWaitForEmail = false;

            tileDict = new TileDictionary();

            if (!Directory.Exists(appSettingsFolder))
            {
                Directory.CreateDirectory(appSettingsFolder);
            }

            StartGame(ChessPieceColor.White);
        }
        #endregion Constuctors

        #region Fields
        private string appSettingsFolder;
        private string emailPassword;
        private ChessPieceColor emailGameOwnColor;
        private AppSettings appSettings;
        private Canvas canvas;
        private Image currentlyDraggedChessPieceImage;
        private int currentlyDraggedChessPieceOriginalCanvasLeft;
        private int currentlyDraggedChessPieceOriginalCanvasTop;
        private Point dragOverCanvasPosition;
        private Point dragOverChessPiecePosition;
        private bool isMouseMoving;
        private bool wasSideMenuOpen;
        private bool isEmailGame;
        private bool doWaitForEmail;
        private ChessPieceColor bottomColor;
        #endregion Fields

        #region Property-Values
        private TileDictionary tileDict;
        private string sideMenuVisibility;
        private string sideMenuMainMenuVisibility;
        private string sideMenuNewGameModeVisibility;
        private string sideMenuButtonsNewGameLocalColorVisibility;
        private string settingsVisibility;
        private string newEmailGameVisibility;

        private string settingsTextBoxEmailAddress;
        private string settingsTextBoxEmailPop3Server;
        private string settingsTextBoxEmailSMTPServer;

        private string newEmailGameTextBoxOwnEmail;
        private string newEmailGameTextBoxOpponentEmail;
        private string newEmailGameRadioButtonWhiteIsChecked;
        private string newEmailGameRadioButtonBlackIsChecked;
        #endregion Property-Values

        #region Properties
        public string SideMenuVisibility
        {
            get => sideMenuVisibility;
            set { sideMenuVisibility = value; OnPropertyChanged(); }
        }
        public string SideMenuMainMenuVisibility
        {
            get => sideMenuMainMenuVisibility;
            set { sideMenuMainMenuVisibility = value; OnPropertyChanged(); }
        }
        public string SideMenuNewGameModeVisibility
        {
            get => sideMenuNewGameModeVisibility;
            set { sideMenuNewGameModeVisibility = value; OnPropertyChanged(); }
        }
        public string SideMenuButtonsNewGameLocalColorVisibility
        {
            get => sideMenuButtonsNewGameLocalColorVisibility;
            set { sideMenuButtonsNewGameLocalColorVisibility = value; OnPropertyChanged(); }
        }
        public string SettingsVisibility
        {
            get => settingsVisibility;
            set { settingsVisibility = value; OnPropertyChanged(); }
        }
        public string NewEmailGameVisibility
        {
            get => newEmailGameVisibility;
            set { newEmailGameVisibility = value; OnPropertyChanged(); }
        }
        public string SettingsTextBoxEmailAddress
        {
            get => settingsTextBoxEmailAddress;
            set { settingsTextBoxEmailAddress = value; OnPropertyChanged(); }
        }
        public string SettingsTextBoxEmailPop3Server
        {
            get => settingsTextBoxEmailPop3Server;
            set { settingsTextBoxEmailPop3Server = value; OnPropertyChanged(); }
        }
        public string SettingsTextBoxEmailSMTPServer
        {
            get => settingsTextBoxEmailSMTPServer;
            set { settingsTextBoxEmailSMTPServer = value; OnPropertyChanged(); }
        }
        public string NewEmailGameTextBoxOwnEmail
        {
            get => newEmailGameTextBoxOwnEmail;
            set { newEmailGameTextBoxOwnEmail = value; OnPropertyChanged(); }
        }
        public string NewEmailGameTextBoxOpponentEmail
        {
            get => newEmailGameTextBoxOpponentEmail;
            set { newEmailGameTextBoxOpponentEmail = value; OnPropertyChanged(); }
        }
        public string NewEmailGameRadioButtonWhiteIsChecked
        {
            get => newEmailGameRadioButtonWhiteIsChecked;
            set { newEmailGameRadioButtonWhiteIsChecked = value; OnPropertyChanged(); }
        }
        public string NewEmailGameRadioButtonBlackIsChecked
        {
            get => newEmailGameRadioButtonBlackIsChecked;
            set { newEmailGameRadioButtonBlackIsChecked = value; OnPropertyChanged(); }
        }
        public TileDictionary TileDict
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
        public RelayCommand SideMenuSettingsCommand { get; }
        public RelayCommand<object> SettingsPasswordBoxCommand { get; }
        public RelayCommand SettingsSaveCommand { get; }
        public RelayCommand SettingsCancelCommand { get; }
        public RelayCommand NewEmailGameStartCommand { get; }
        public RelayCommand NewEmailGameCancelCommand { get; }
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
                if (sideMenuVisibility != "Visible" && settingsVisibility == "Hidden" && newEmailGameVisibility == "Hidden")
                {
                    SideMenuNewGameModeVisibility = "Hidden";
                    SideMenuButtonsNewGameLocalColorVisibility = "Hidden";
                    SideMenuMainMenuVisibility = "Visible";
                    SideMenuVisibility = "Visible";
                }
                else SideMenuVisibility = "Hidden";
            }
            else
            {
                wasSideMenuOpen = false;
            }
        }
        private void SideMenuNewGameAction()
        {
            SideMenuMainMenuVisibility = "Hidden";
            SideMenuNewGameModeVisibility = "Visible";
        }
        private void SideMenuNewGameModeLocalAction()
        {
            SideMenuNewGameModeVisibility = "Hidden";
            SideMenuButtonsNewGameLocalColorVisibility = "Visible";
        }
        private void SideMenuNewGameModeEmailAction()
        {
            SideMenuVisibility = "Hidden";
            SideMenuMainMenuVisibility = "Visible";
            SideMenuNewGameModeVisibility = "Hidden";
            NewEmailGameVisibility = "Visible";
        }
        private void SideMenuNewGameLocalColorGoBackAction()
        {
            SideMenuButtonsNewGameLocalColorVisibility = "Hidden";
            SideMenuNewGameModeVisibility = "Visible";
        }
        private void SideMenuNewGameLocalAsWhiteAction()
        {
            currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            currentlyDraggedChessPieceOriginalCanvasTop = -1000;

            tileDict = new TileDictionary();
            SideMenuVisibility = "Hidden";
            SideMenuMainMenuVisibility = "Visible";
            SideMenuNewGameModeVisibility = "Hidden";
            isEmailGame = false;
            StartGame(ChessPieceColor.White);
        }
        private void SideMenuNewGameLocalAsBlackAction()
        {
            currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            currentlyDraggedChessPieceOriginalCanvasTop = -1000;

            tileDict = new TileDictionary();
            SideMenuVisibility = "Hidden";
            SideMenuMainMenuVisibility = "Visible";
            SideMenuNewGameModeVisibility = "Hidden";
            isEmailGame = false;
            StartGame(ChessPieceColor.Black);
        }
        private void SideMenuNewGameModeGoBackAction()
        {
            SideMenuMainMenuVisibility = "Visible";
            SideMenuNewGameModeVisibility = "Hidden";
        }
        private void SideMenuSettingsAction()
        {
            AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
            SideMenuVisibility = "Hidden";
            SettingsVisibility = "Visible";
            if (appSettingsStruct.EmailServer["email_address"] != null) SettingsTextBoxEmailAddress = appSettingsStruct.EmailServer["email_address"];
            if (appSettingsStruct.EmailServer["pop3_server"] != null) SettingsTextBoxEmailPop3Server = appSettingsStruct.EmailServer["pop3_server"];
            if (appSettingsStruct.EmailServer["smtp_server"] != null) SettingsTextBoxEmailSMTPServer = appSettingsStruct.EmailServer["smtp_server"];
        }
        private void SettingsPasswordBoxAction(object o)
        {
            var e = o as RoutedEventArgs;
            var passwordBox = e.Source as PasswordBox;
            emailPassword = passwordBox.Password;
        }
        private void SettingsSaveAction()
        {
            AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
            SettingsVisibility = "Hidden";
            appSettingsStruct.EmailServer["email_address"] = settingsTextBoxEmailAddress;
            appSettingsStruct.EmailServer["password"] = emailPassword;
            appSettingsStruct.EmailServer["pop3_server"] = settingsTextBoxEmailPop3Server;
            appSettingsStruct.EmailServer["smtp_server"] = settingsTextBoxEmailSMTPServer;
            appSettings.ChangeEmailServer(appSettingsStruct.EmailServer);
        }
        private void SettingsCancelAction()
        {
            SettingsVisibility = "Hidden";
        }
        private async void NewEmailGameStartAction()
        {
            NewEmailGameVisibility = "Hidden";

            currentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            currentlyDraggedChessPieceOriginalCanvasTop = -1000;

            tileDict = new TileDictionary();
            SideMenuVisibility = "Hidden";
            SideMenuMainMenuVisibility = "Visible";
            SideMenuNewGameModeVisibility = "Hidden";
            isEmailGame = true;

            if (newEmailGameRadioButtonWhiteIsChecked == "True")
            {
                emailGameOwnColor = ChessPieceColor.White;
                StartGame(ChessPieceColor.White);

            }
            else if (newEmailGameRadioButtonBlackIsChecked == "True")
            {
                emailGameOwnColor = ChessPieceColor.Black;
                StartGame(ChessPieceColor.Black);
                doWaitForEmail = true;
                Task waitForEmailWhiteMoveTask = WaitForEmailNextWhiteMoveTask();
                await waitForEmailWhiteMoveTask;
                doWaitForEmail = false;
            }
        }
        private void NewEmailGameCancelAction()
        {
            NewEmailGameVisibility = "Hidden";
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
                        SideMenuVisibility = "Hidden";
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
                if (e.LeftButton == MouseButtonState.Pressed && !doWaitForEmail && IsInputAllowed())
                {
                    if (isEmailGame && emailGameOwnColor != ChessPieceImages.GetImageColor(currentlyDraggedChessPieceImage.Source)) return;
                    if (SideMenuVisibility == "Visible")
                    {
                        wasSideMenuOpen = true;
                        SideMenuVisibility = "Hidden";
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
            if (!doWaitForEmail && IsInputAllowed())
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
        }
        private async void WindowMouseLeftUpAction(object o)
        {
            if (!doWaitForEmail && IsInputAllowed())
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
                                ChessPieceColor oldColor = tileDict[oldCoords.ToString()].ChessPiece.ChessPieceColor;

                                Canvas.SetLeft(currentlyDraggedChessPieceImage, currentlyDraggedChessPieceOriginalCanvasLeft);
                                Canvas.SetTop(currentlyDraggedChessPieceImage, currentlyDraggedChessPieceOriginalCanvasTop);
                                //Console.WriteLine("Old Coords before: " + "Is occupied? " + tileDict[oldCoordsString.ToString()].IsOccupied.ToString() + "\t| Coords: " + oldCoordsString.ToString() + "\t| Color = " + tileDict[oldCoordsString.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[oldCoordsString.ToString()].ChessPiece.ChessPieceType.ToString());
                                //Console.WriteLine("New Coords before: " + "Is occupied? " + tileDict[newCoordsString.ToString()].IsOccupied.ToString() + "\t| Coords: " + newCoordsString.ToString() + "\t| Color = " + tileDict[newCoordsString.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[newCoordsString.ToString()].ChessPiece.ChessPieceType.ToString());

                                tileDict[newCoords.ToString()].ChessPiece = tileDict[oldCoords.ToString()].ChessPiece;
                                tileDict[oldCoords.ToString()].ChessPiece = new ChessPiece(ChessPieceImages.Empty, ChessPieceColor.Empty, ChessPieceType.Empty);
                                tileDict[oldCoords.ToString()].IsOccupied = false;
                                tileDict[newCoords.ToString()].IsOccupied = true;
                                tileDict[newCoords.ToString()].ChessPiece.MoveCount++;
                                TileDict = tileDict;

                                if (isEmailGame)
                                {
                                    if (oldColor == ChessPieceColor.White)
                                    {
                                        await Task.Run(() => SendEmailWhiteMoveTask(oldCoords, newCoords));
                                        doWaitForEmail = true;
                                        await Task.Run(() => WaitForEmailNextBlackMoveTask());
                                        doWaitForEmail = false;
                                    }
                                    else
                                    {
                                        await Task.Run(() => SendEmailBlackMoveTask(oldCoords, newCoords));
                                        doWaitForEmail = true;
                                        await Task.Run(() => WaitForEmailNextWhiteMoveTask());
                                        doWaitForEmail = false;
                                    }

                                }

                                // store a list of threatening tiles:
                                tileDict[newCoords.ToString()].ThreatenedByTileList = ThreatDetectionGameLogic.GetThreateningTilesList(tileDict, newCoords, bottomColor);

                                //Console.WriteLine("Old Coords after : " + "Is occupied? " + tileDict[oldCoordsString.ToString()].IsOccupied.ToString() + "\t| Coords: " + oldCoordsString.ToString() + "\t| Color = " + tileDict[oldCoordsString.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[oldCoordsString.ToString()].ChessPiece.ChessPieceType.ToString());
                                //Console.WriteLine("New Coords after : " + "Is occupied? " + tileDict[newCoordsString.ToString()].IsOccupied.ToString() + "\t| Coords: " + newCoordsString.ToString() + "\t| Color = " + tileDict[newCoordsString.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[newCoordsString.ToString()].ChessPiece.ChessPieceType.ToString());
                                //Console.WriteLine("MoveCount: " + tileDict[newCoordsString.ToString()].ChessPiece.MoveCount);
                                //Console.WriteLine();
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
            TileDict = tileDict;
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
        private async Task SendEmailWhiteMoveTask(Coords oldCoords, Coords newCoords)
        {
            AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
            Task sendCurrentMove = EmailChess.Send.SendCurrentWhiteMove(appSettingsStruct.EmailServer, newEmailGameTextBoxOpponentEmail, oldCoords, newCoords);
            await sendCurrentMove;
        }
        private async Task SendEmailBlackMoveTask(Coords oldCoords, Coords newCoords)
        {
            AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
            Task sendCurrentMove = EmailChess.Send.SendCurrentBlackMove(appSettingsStruct.EmailServer, newEmailGameTextBoxOpponentEmail, oldCoords, newCoords);
            await sendCurrentMove;
        }
        private async Task WaitForEmailNextWhiteMoveTask()
        {
            await Task.Run(() =>
            {
                AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
                string message;
                string oldCoordsString = "";
                string newCoordsString = "";
                bool hasReceived = false;
                while (!hasReceived)
                {
                    message = EmailChess.Receive.CheckForNextWhiteMove(appSettingsStruct.EmailServer, ChessPieceColor.White);
                    if (message == "")
                    {
                        System.Threading.Thread.Sleep(5000);
                    }
                    else
                    {
                        hasReceived = true;
                        Console.WriteLine(message);
                        oldCoordsString = message.Substring(0, 2);
                        newCoordsString = message.Substring(4, 2);
                    }
                }
                Console.WriteLine(oldCoordsString, newCoordsString);

                (oldCoordsString, newCoordsString) = InvertCoords(oldCoordsString, newCoordsString);

                tileDict[newCoordsString].ChessPiece = tileDict[oldCoordsString].ChessPiece;
                tileDict[oldCoordsString].ChessPiece = new ChessPiece(ChessPieceImages.Empty, ChessPieceColor.Empty, ChessPieceType.Empty);
                tileDict[oldCoordsString].IsOccupied = false;
                tileDict[newCoordsString].IsOccupied = true;
                tileDict[newCoordsString].ChessPiece.MoveCount++;
                TileDict = tileDict;
            });
        }
        private async Task WaitForEmailNextBlackMoveTask()
        {
            await Task.Run(() =>
            {

                AppSettingsStruct appSettingsStruct = appSettings.LoadSettings();
                string message;
                string oldCoordsString = "";
                string newCoordsString = "";
                bool hasReceived = false;
                while (!hasReceived)
                {
                    message = EmailChess.Receive.CheckForNextBlackMove(appSettingsStruct.EmailServer, ChessPieceColor.Black);
                    if (message == "")
                    {
                        System.Threading.Thread.Sleep(5000);
                    }
                    else
                    {
                        hasReceived = true;
                        Console.WriteLine(message);
                        oldCoordsString = message.Substring(0, 2);
                        newCoordsString = message.Substring(4, 2);
                    }
                }
                
                Console.WriteLine(oldCoordsString, newCoordsString);

                (oldCoordsString, newCoordsString) = InvertCoords(oldCoordsString, newCoordsString);
                
                tileDict[newCoordsString].ChessPiece = tileDict[oldCoordsString].ChessPiece;
                tileDict[oldCoordsString].ChessPiece = new ChessPiece(ChessPieceImages.Empty, ChessPieceColor.Empty, ChessPieceType.Empty);
                tileDict[oldCoordsString].IsOccupied = false;
                tileDict[newCoordsString].IsOccupied = true;
                tileDict[newCoordsString].ChessPiece.MoveCount++;
                TileDict = tileDict;
            });
        }
        private (string, string) InvertCoords(string oldCoordsString, string newCoordsString)
        {
            if (oldCoordsString[0] == 'A') oldCoordsString = 'H' + oldCoordsString[1].ToString();
            else if (oldCoordsString[0] == 'B') oldCoordsString = 'G' + oldCoordsString[1].ToString();
            else if (oldCoordsString[0] == 'C') oldCoordsString = 'F' + oldCoordsString[1].ToString();
            else if (oldCoordsString[0] == 'D') oldCoordsString = 'E' + oldCoordsString[1].ToString();
            else if (oldCoordsString[0] == 'E') oldCoordsString = 'D' + oldCoordsString[1].ToString();
            else if (oldCoordsString[0] == 'F') oldCoordsString = 'C' + oldCoordsString[1].ToString();
            else if (oldCoordsString[0] == 'G') oldCoordsString = 'B' + oldCoordsString[1].ToString();
            else if (oldCoordsString[0] == 'H') oldCoordsString = 'A' + oldCoordsString[1].ToString();

            if (newCoordsString[0] == 'A') newCoordsString = 'H' + newCoordsString[1].ToString();
            else if (newCoordsString[0] == 'B') newCoordsString = 'G' + newCoordsString[1].ToString();
            else if (newCoordsString[0] == 'C') newCoordsString = 'F' + newCoordsString[1].ToString();
            else if (newCoordsString[0] == 'D') newCoordsString = 'E' + newCoordsString[1].ToString();
            else if (newCoordsString[0] == 'E') newCoordsString = 'D' + newCoordsString[1].ToString();
            else if (newCoordsString[0] == 'F') newCoordsString = 'C' + newCoordsString[1].ToString();
            else if (newCoordsString[0] == 'G') newCoordsString = 'B' + newCoordsString[1].ToString();
            else if (newCoordsString[0] == 'H') newCoordsString = 'A' + newCoordsString[1].ToString();

            if (oldCoordsString[1] == '8') oldCoordsString = oldCoordsString[0].ToString() + '1';
            else if (oldCoordsString[1] == '7') oldCoordsString = oldCoordsString[0].ToString() + '2';
            else if (oldCoordsString[1] == '6') oldCoordsString = oldCoordsString[0].ToString() + '3';
            else if (oldCoordsString[1] == '5') oldCoordsString = oldCoordsString[0].ToString() + '4';
            else if (oldCoordsString[1] == '4') oldCoordsString = oldCoordsString[0].ToString() + '5';
            else if (oldCoordsString[1] == '3') oldCoordsString = oldCoordsString[0].ToString() + '6';
            else if (oldCoordsString[1] == '2') oldCoordsString = oldCoordsString[0].ToString() + '7';
            else if (oldCoordsString[1] == '1') oldCoordsString = oldCoordsString[0].ToString() + '8';

            if (newCoordsString[1] == '8') newCoordsString = newCoordsString[0].ToString() + '1';
            else if (newCoordsString[1] == '7') newCoordsString = newCoordsString[0].ToString() + '2';
            else if (newCoordsString[1] == '6') newCoordsString = newCoordsString[0].ToString() + '3';
            else if (newCoordsString[1] == '5') newCoordsString = newCoordsString[0].ToString() + '4';
            else if (newCoordsString[1] == '4') newCoordsString = newCoordsString[0].ToString() + '5';
            else if (newCoordsString[1] == '3') newCoordsString = newCoordsString[0].ToString() + '6';
            else if (newCoordsString[1] == '2') newCoordsString = newCoordsString[0].ToString() + '7';
            else if (newCoordsString[1] == '1') newCoordsString = newCoordsString[0].ToString() + '8';

            return (oldCoordsString, newCoordsString);
        }
        private bool IsInputAllowed()
        {
            if (sideMenuVisibility == "Visible") return false;
            if (settingsVisibility == "Visible") return false;
            if (newEmailGameVisibility == "Visible") return false;
            return true;
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
