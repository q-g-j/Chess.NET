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
using System.Net.Mail;
using OpenPop.Pop3;
using CommunityToolkit.Mvvm.Input;

using ChessDotNET.ViewModels.CommandActions.MainWindow;
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

            GeneralCommandActions generalCommandActions = new GeneralCommandActions(this);
            SideMenuCommandActions sideMenuCommandActions = new SideMenuCommandActions(this, appSettings);
            SettingsCommandActions settingsCommandActions = new SettingsCommandActions(this, appSettings);
            NewEmailGameCommandActions newEmailGameCommandActions = new NewEmailGameCommandActions(this, appSettings);

            WindowMouseMoveCommand = new RelayCommand<object>(o => generalCommandActions.WindowMouseMoveAction(o));
            WindowMouseLeftDownCommand = new RelayCommand<object>(o => generalCommandActions.WindowMouseLeftDownAction(o));
            WindowMouseLeftUpCommand = new RelayCommand<object>(o => generalCommandActions.WindowMouseLeftUpAction(o, tileDict, appSettings));
            ChessPieceMouseLeftDownCommand = new RelayCommand<object>(o => generalCommandActions.ChessPieceMouseleftDownAction(o));
            OpenSideMenuCommand = new RelayCommand(generalCommandActions.OpenSideMenuAction);

            SideMenuNewGameCommand = new RelayCommand(sideMenuCommandActions.SideMenuNewGameAction);
            SideMenuNewGameModeLocalCommand = new RelayCommand(sideMenuCommandActions.SideMenuNewGameModeLocalAction);
            SideMenuNewGameModeEmailCommand = new RelayCommand(sideMenuCommandActions.SideMenuNewGameModeEmailAction);
            SideMenuNewGameModeGoBackCommand = new RelayCommand(sideMenuCommandActions.SideMenuNewGameModeGoBackAction);
            SideMenuNewGameLocalAsWhiteCommand = new RelayCommand(sideMenuCommandActions.SideMenuNewGameLocalAsWhiteAction);
            SideMenuNewGameLocalAsBlackCommand = new RelayCommand(sideMenuCommandActions.SideMenuNewGameLocalAsBlackAction);
            SideMenuNewGameLocalColorGoBackCommand = new RelayCommand(sideMenuCommandActions.SideMenuNewGameLocalColorGoBackAction);
            SideMenuSettingsCommand = new RelayCommand(sideMenuCommandActions.SideMenuSettingsAction);
            SideMenuQuitProgramCommand = new RelayCommand(sideMenuCommandActions.SideMenuQuitProgramAction);

            SettingsPasswordBoxCommand = new RelayCommand<object>(o => settingsCommandActions.SettingsPasswordBoxAction(o));
            SettingsSaveCommand = new RelayCommand(settingsCommandActions.SettingsSaveAction);
            SettingsCancelCommand = new RelayCommand(settingsCommandActions.SettingsCancelAction);

            NewEmailGameStartCommand = new RelayCommand(() => newEmailGameCommandActions.NewEmailGameStartAction(tileDict));
            NewEmailGameCancelCommand = new RelayCommand(newEmailGameCommandActions.NewEmailGameCancelAction);

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

            WasSideMenuOpen = false;
            IsEmailGame = false;
            DoWaitForEmail = false;

            if (!Directory.Exists(appSettingsFolder))
            {
                Directory.CreateDirectory(appSettingsFolder);
            }

            StartGame(ChessPieceColor.White);
        }
        #endregion Constuctors

        #region Fields
        private readonly AppSettings appSettings;
        private readonly string appSettingsFolder;

        internal ChessPieceColor EmailGameOwnColor;
        internal ChessPieceColor BottomColor;
        internal Canvas ChessCanvas;
        internal Image CurrentlyDraggedChessPieceImage;
        internal int CurrentlyDraggedChessPieceOriginalCanvasLeft;
        internal int CurrentlyDraggedChessPieceOriginalCanvasTop;
        internal Point DragOverCanvasPosition;
        internal Point DragOverChessPiecePosition;
        internal bool IsMouseMoving;
        internal bool WasSideMenuOpen;
        internal bool IsEmailGame;
        internal bool DoWaitForEmail;
        internal Task DeleteOldEmailsTask;
        internal string EmailPassword;
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
        internal Dictionary<string, Tile> TileDictReadOnly
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
        public RelayCommand SideMenuQuitProgramCommand { get; }
        public RelayCommand<object> WindowMouseMoveCommand { get; }
        public RelayCommand<object> WindowMouseLeftDownCommand { get; }
        public RelayCommand<object> WindowMouseLeftUpCommand { get; }
        public RelayCommand<object> ChessPieceMouseLeftDownCommand { get; }
        #endregion Commands

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
            CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
            CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;
            
            tileDict = new TileDictionary();
            if (color == ChessPieceColor.White)
            {
                BottomColor = ChessPieceColor.White;
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
                BottomColor = ChessPieceColor.Black;
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
        internal bool IsInputAllowed()
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
