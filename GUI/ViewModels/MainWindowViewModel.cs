using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using CommunityToolkit.Mvvm.Input;
using ChessDotNET.CustomTypes;
using ChessDotNET.Settings;
using System.Collections.Generic;
using System.Linq;

namespace ChessDotNET.GUI.ViewModels.MainWindow
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Constructors
        public MainWindowViewModel()
        {
            AppSettingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Chess.NET");
            appSettings = new AppSettings(AppSettingsFolder);

            GeneralCommandActions generalCommandActions = new GeneralCommandActions(this, appSettings);
            SideMenuCommandActions sideMenuCommandActions = new SideMenuCommandActions(this, appSettings);
            OverlayInvitationCommandActions overlayInvitationCommandActions = new OverlayInvitationCommandActions(this, appSettings);
            OverlayInvitationAcceptedCommandActions overlayInvitationAcceptedCommandActions = new OverlayInvitationAcceptedCommandActions(this, appSettings);
            OverlaySettingsCommandActions overlaySettingsCommandActions = new OverlaySettingsCommandActions(this, appSettings);
            OverlayNewEmailGameCommandActions overlayNewEmailGameCommandActions = new OverlayNewEmailGameCommandActions(this, appSettings);

            WindowMouseMoveCommand = new RelayCommand<object>(o => generalCommandActions.WindowMouseMoveAction(o));
            WindowMouseLeftDownCommand = new RelayCommand<object>(o => generalCommandActions.WindowMouseLeftDownAction(o));
            WindowMouseLeftUpCommand = new RelayCommand<object>(o => generalCommandActions.WindowMouseLeftUpAction(o, tileDict));
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

            OverlaySettingsPasswordBoxCommand = new RelayCommand<object>(o => overlaySettingsCommandActions.OverlaySettingsPasswordBoxAction(o));
            OverlaySettingsSaveCommand = new RelayCommand(overlaySettingsCommandActions.OverlaySettingsSaveAction);
            OverlaySettingsCancelCommand = new RelayCommand(overlaySettingsCommandActions.OverlaySettingsCancelAction);

            OverlayNewEmailGameStartCommand = new RelayCommand(overlayNewEmailGameCommandActions.OverlayNewEmailGameStartAction);
            OverlayNewEmailGameCancelCommand = new RelayCommand(overlayNewEmailGameCommandActions.OverlayNewEmailGameCancelAction);

            OverlayInvitationAcceptCommand = new RelayCommand(overlayInvitationCommandActions.OverlayInvitationAcceptAction);
            OverlayInvitationRejectCommand = new RelayCommand(overlayInvitationCommandActions.OverlayInvitationRejectAction);

            OverlayInvitationAcceptedCommand = new RelayCommand(overlayInvitationAcceptedCommandActions.OverlayInvitationAcceptedStartGameAction);

            propertiesDict = new Dictionary<string, string>()
            {
                ["SideMenuVisibility"] = "Hidden",
                ["SideMenuMainMenuVisibility"] = "Visible",
                ["SideMenuNewGameModeVisibility"] = "Hidden",
                ["SideMenuButtonsNewGameLocalColorVisibility"] = "Hidden",
                ["NewEmailGameOverlayVisibility"] = "Hidden",
                ["NewEmailGameOverlayErrorLabelVisibility"] = "Hidden",
                ["SettingsOverlayVisibility"] = "Hidden",
                ["InvitationOverlayVisibility"] = "Hidden",
                ["InvitationAcceptedOverlayVisibility"] = "Hidden",

                ["SettingsOverlayTextBoxEmailAddress"] = " ",
                ["SettingsOverlayTextBoxEmailPop3Server"] = " ",
                ["SettingsOverlayTextBoxEmailSMTPServer"] = " ",

                ["NewEmailGameOverlayTextBoxOwnEmail"] = " ",
                ["NewEmailGameOverlayTextBoxOpponentEmail"] = " ",

                ["NewEmailGameOverlayRadioButtonWhiteIsChecked"] = "True",
                ["NewEmailGameOverlayRadioButtonBlackIsChecked"] = "False",

                ["InvitationOverlayLabelSenderInfoText1"] = "user@server.com möchte eine Partie E-Mail-Schach spielen!",
                ["InvitationOverlayLabelSenderInfoText2"] = "Dein Herausforderer hat die Farbe weiß gewählt.",

                ["InvitationAcceptedOverlayLabelText"] = "user@server.com hat die Einladung angenommen! Du bist am Zug...",

                ["ChessCanvasRotationAngle"] = "0",
                ["ChessCanvasRotationCenterX"] = "0",
                ["ChessCanvasRotationCenterY"] = "-200",

                ["InvitationLabelSenderInfo1"] = " ",
                ["InvitationLabelSenderInfo2"] = " ",
            };

            WasSideMenuOpen = false;
            IsEmailGame = false;
            DoWaitForEmail = false;
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
        internal ChessPieceColor EmailGameOwnColor;
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
        internal bool isSettingsSaved;
        internal Task DeleteOldEmailsTask;
        internal string EmailPassword;
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
                HorizontalNotationList = horizontalNotationList;

                for (int i = 0; i < 8; i++)
                {
                    verticalNotationList[i] = (i + 1).ToString();
                }
                VerticalNotationList = verticalNotationList;
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
                HorizontalNotationList = horizontalNotationList;

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
                VerticalNotationList = verticalNotationList;
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

            PropertiesDict = PropertiesDict;
            TileDict = TileDict;
        }
        internal bool IsInputAllowed()
        {
            if (propertiesDict["SideMenuVisibility"] == "Visible") return false;
            if (propertiesDict["SettingsOverlayVisibility"] == "Visible") return false;
            if (propertiesDict["NewEmailGameOverlayVisibility"] == "Visible") return false;
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
