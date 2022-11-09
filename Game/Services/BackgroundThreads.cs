using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using ChessDotNET.Models;
using ChessDotNET.ViewModels;
using ChessDotNET.WebApiClient;


namespace ChessDotNET.Services
{
    internal static class BackgroundThreads
    {
        internal static void LobbyKeepCheckingForOpponentAcception()
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();
            Player localPlayer = WeakReferenceMessenger.Default.Send<MainWindowViewModel.PropertyLocalPlayerValueRequestMessage>();

            var threadStart = new ThreadStart(() =>
            {
                //while (WeakReferenceMessenger.Default.Send<LobbyViewModel.LobbyOverlayWaitingForInvitationAcceptedVisibilityRequestMessage>() == "Visible"
                //&& globals.CurrentOnlineGame.BlackId != localPlayer.Id)

                while (globals.CurrentOnlineGame.BlackId != localPlayer.Id)
                {
                    Task.Run(() =>
                    {
                        localPlayer = WeakReferenceMessenger.Default.Send<MainWindowViewModel.PropertyLocalPlayerValueRequestMessage>();
                        if (localPlayer != null)
                        {
                            try
                            {
                                DispatchService.Invoke(async () =>
                                {
                                    globals.CurrentOnlineGame = await WebApiClientGamesCommands.GetNewGame(localPlayer.Id);
                                });
                            }
                            catch
                            {
                                MessageBox.Show(globals.LobbyWindow, "Cannot contact server...", "Error!");
                                globals.LobbyWindow.Close();
                                globals.LobbyWindow = null;
                            }
                        }
                    });
                    Thread.Sleep(1000);
                }

                DispatchService.Invoke(() =>
                {
                    if (globals.LobbyWindow != null)
                    {
                        WeakReferenceMessenger.Default.Send(
                                    new SideMenuViewModel.PropertyStringValueChangedMessage(
                                        new Tuple<string, string>("SideMenuOnlineGameButtonVisibility", "Hidden")));

                        WeakReferenceMessenger.Default.Send(
                                    new SideMenuViewModel.PropertyStringValueChangedMessage(
                                        new Tuple<string, string>("SideMenuEndOnlineGameButtonVisibility", "Visible")));

                        WeakReferenceMessenger.Default.Send(
                                    new LobbyViewModel.PropertyStringValueChangedMessage(
                                        new Tuple<string, string>("LobbyOverlayWaitingForInvitationAcceptedVisibility", "Hidden")));

                        if (globals.LobbyWindow != null)
                        {
                            globals.LobbyWindow.Close();
                            globals.LobbyWindow = null;

                            WeakReferenceMessenger.Default.Send(
                                new MainWindowViewModel.StartGameMessage(true));

                            localPlayer.Color = "Black";
                            WeakReferenceMessenger.Default.Send(
                                new MainWindowViewModel.OnPropertyChangedMessage("LocalPlayer"));

                            globals.IsOnlineGame = true;

                            OnlineGameKeepCheckingForNextMove();
                            OnlineGameKeepResettingWhiteInactiveCounter();
                        }
                    }
                });
            });
            var backgroundThread = new Thread(threadStart)
            {
                IsBackground = true
            };
            backgroundThread.Start();
        }
        internal static void LobbyKeepResettingInactiveCounter()
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();

            var threadStart = new ThreadStart(() =>
            {
                while (globals.LobbyWindow != null)
                {
                    Task.Run(async () =>
                    {
                        Player localPlayer = WeakReferenceMessenger.Default.Send<MainWindowViewModel.PropertyLocalPlayerValueRequestMessage>();

                        if (localPlayer != null)
                        {
                            try
                            {
                                await WebApiClientPlayersCommands.ResetInactiveCounterAsync(localPlayer.Id);
                            }
                            catch
                            {
                                MessageBox.Show(globals.LobbyWindow, "Cannot contact server...", "Error!");
                                globals.LobbyWindow.Close();
                                globals.LobbyWindow = null;
                            }
                        }
                    });
                    Thread.Sleep(1000);
                }
            });

            var backgroundThread = new Thread(threadStart)
            {
                IsBackground = true
            };
            backgroundThread.Start();
        }
        internal static void OnlineGameKeepResettingWhiteInactiveCounter()
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();

            var threadStart = new ThreadStart(() =>
            {
                while (globals.IsOnlineGame)
                {
                    Task.Run(async () =>
                    {
                        if (globals.CurrentOnlineGame != null)
                        {
                            try
                            {
                                await WebApiClientGamesCommands.ResetWhiteInactiveCounterAsync(globals.CurrentOnlineGame.Id);
                            }
                            catch
                            {
                                MessageBox.Show(globals.LobbyWindow, "Cannot contact server...", "Error!");
                                globals.LobbyWindow.Close();
                                globals.LobbyWindow = null;
                            }
                        }
                    });
                    Thread.Sleep(1000);
                }
            });

            var backgroundThread = new Thread(threadStart)
            {
                IsBackground = true
            };
            backgroundThread.Start();
        }
        internal static void OnlineGameKeepResettingBlackInactiveCounter()
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();

            var threadStart = new ThreadStart(() =>
            {
                while (globals.IsOnlineGame)
                {
                    Task.Run(async () =>
                    {
                        if (globals.CurrentOnlineGame != null)
                        {
                            try
                            {
                                await WebApiClientGamesCommands.ResetBlackInactiveCounterAsync(globals.CurrentOnlineGame.Id);
                            }
                            catch
                            {
                                MessageBox.Show(globals.LobbyWindow, "Cannot contact server...", "Error!");
                                globals.LobbyWindow.Close();
                                globals.LobbyWindow = null;
                            }
                        }
                    });
                    Thread.Sleep(1000);
                }
            });

            var backgroundThread = new Thread(threadStart)
            {
                IsBackground = true
            };
            backgroundThread.Start();
        }
        internal static void OnlineGameKeepCheckingForNextMove()
        {
            Globals globals = WeakReferenceMessenger.Default.Send<App.GlobalsRequestMessage>();

            globals.IsWaitingForMove = true;
            TileDictionary tileDict = WeakReferenceMessenger.Default.Send<MainWindowViewModel.PropertyTileDictValueRequestMessage>();

            var threadStart = new ThreadStart(() =>
            {
                bool isSuccess = false;
                while (!isSuccess && globals.IsOnlineGame)
                {
                    DispatchService.Invoke(async () =>
                    {
                        if (globals.CurrentOnlineGame != null)
                        {
                            try
                            {
                                globals.CurrentOnlineGame = await WebApiClientGamesCommands.GetCurrentGame(globals.CurrentOnlineGame.Id);
                                Player localPlayer = WeakReferenceMessenger.Default.Send<MainWindowViewModel.PropertyLocalPlayerValueRequestMessage>();

                                if (globals.CurrentOnlineGame.HasPlayerQuit)
                                {
                                    isSuccess = true;
                                    globals.IsWaitingForMove = false;
                                    globals.IsOnlineGame = false;
                                    Tuple<string, string> overlayOnlineGamePlayerQuitVisibility = new Tuple<string, string>(
                                        "OverlayOnlineGamePlayerQuitVisibility", "Visible");
                                    WeakReferenceMessenger.Default.Send(
                                        new MainWindowViewModel.PropertyStringValueChangedMessage(overlayOnlineGamePlayerQuitVisibility));
                                }

                                else if (localPlayer.Color == "White")
                                {
                                    if (globals.CurrentOnlineGame.LastMoveStartBlack != null && globals.CurrentOnlineGame.LastMoveEndBlack != null)
                                    {
                                        ChessPiece chessPiece = tileDict[globals.CurrentOnlineGame.LastMoveStartBlack.Substring(0, 2)].ChessPiece;
                                        Coords oldCoords = Coords.StringToCoords(globals.CurrentOnlineGame.LastMoveStartBlack.Substring(0, 2));
                                        Coords newCoords = Coords.StringToCoords(globals.CurrentOnlineGame.LastMoveEndBlack.Substring(0, 2));

                                        tileDict.MoveChessPiece(oldCoords, newCoords, true);
                                        globals.MoveList.Add(new Move(oldCoords, newCoords, chessPiece.ChessPieceColor, chessPiece.ChessPieceType));

                                        if (globals.CurrentOnlineGame.LastMoveStartBlack.Length > 2)
                                        {
                                            if (globals.CurrentOnlineGame.LastMoveStartBlack[2] == 'C')
                                            {
                                                Coords rookOldCoords = Coords.StringToCoords(globals.CurrentOnlineGame.LastMoveStartBlack.Substring(3, 2));
                                                Coords rookNewCoords = Coords.StringToCoords(globals.CurrentOnlineGame.LastMoveEndBlack.Substring(3, 2));
                                                tileDict.MoveChessPiece(rookOldCoords, rookNewCoords, true);
                                            }
                                            else if (globals.CurrentOnlineGame.LastMoveStartBlack[2] == 'T')
                                            {
                                                tileDict.CoordsPawnMovedTwoTiles = Coords.StringToCoords(globals.CurrentOnlineGame.LastMoveStartBlack.Substring(3, 2));
                                            }
                                            else if (globals.CurrentOnlineGame.LastMoveStartBlack[2] == 'E')
                                            {
                                                Coords capturedCoords = Coords.StringToCoords(globals.CurrentOnlineGame.LastMoveStartBlack.Substring(3, 2));
                                                tileDict[capturedCoords.String].ChessPiece = new ChessPiece();
                                                tileDict[capturedCoords.String].IsOccupied = false;
                                            }
                                            else if (globals.CurrentOnlineGame.LastMoveStartBlack[2] == 'P')
                                            {
                                                string type = globals.CurrentOnlineGame.LastMoveStartBlack.Remove(0, 3);
                                                ChessPieceColor color = ChessPieceColor.Black;
                                                if (type == "Bishop")
                                                    chessPiece = new ChessPiece(color, ChessPieceType.Bishop, globals.IsRotated);
                                                else if (type == "Knight")
                                                    chessPiece = new ChessPiece(color, ChessPieceType.Knight, globals.IsRotated);
                                                else if (type == "Rook")
                                                    chessPiece = new ChessPiece(color, ChessPieceType.Rook, globals.IsRotated);
                                                else if (type == "Queen")
                                                    chessPiece = new ChessPiece(color, ChessPieceType.Queen, globals.IsRotated);


                                                tileDict[globals.CurrentOnlineGame.LastMoveEndBlack.Substring(0, 2)].ChessPiece = chessPiece;
                                            }
                                        }

                                        WeakReferenceMessenger.Default.Send(
                                            new MainWindowViewModel.OnPropertyChangedMessage("TileDict"));

                                        isSuccess = true;
                                        globals.IsWaitingForMove = false;

                                        Tuple<string, string> labelMoveInfo = new Tuple<string, string>(
                                            "LabelMoveInfo", globals.CurrentOnlineGame.MoveInfo);
                                        WeakReferenceMessenger.Default.Send(
                                            new MainWindowViewModel.PropertyStringValueChangedMessage(labelMoveInfo));
                                    }
                                }
                                else
                                {
                                    if (globals.CurrentOnlineGame.LastMoveStartWhite != null && globals.CurrentOnlineGame.LastMoveEndWhite != null)
                                    {
                                        ChessPiece chessPiece = tileDict[globals.CurrentOnlineGame.LastMoveStartWhite.Substring(0, 2)].ChessPiece;
                                        Coords oldCoords = Coords.StringToCoords(globals.CurrentOnlineGame.LastMoveStartWhite.Substring(0, 2));
                                        Coords newCoords = Coords.StringToCoords(globals.CurrentOnlineGame.LastMoveEndWhite.Substring(0, 2));

                                        tileDict.MoveChessPiece(oldCoords, newCoords, true);
                                        globals.MoveList.Add(new Move(oldCoords, newCoords, chessPiece.ChessPieceColor, chessPiece.ChessPieceType));

                                        if (globals.CurrentOnlineGame.LastMoveStartWhite.Length > 2)
                                        {
                                            if (globals.CurrentOnlineGame.LastMoveStartWhite[2] == 'C')
                                            {
                                                Coords rookOldCoords = Coords.StringToCoords(globals.CurrentOnlineGame.LastMoveStartWhite.Substring(3, 2));
                                                Coords rookNewCoords = Coords.StringToCoords(globals.CurrentOnlineGame.LastMoveEndWhite.Substring(3, 2));
                                                tileDict.MoveChessPiece(rookOldCoords, rookNewCoords, true);
                                            }
                                            else if (globals.CurrentOnlineGame.LastMoveStartWhite[2] == 'T')
                                            {
                                                tileDict.CoordsPawnMovedTwoTiles = Coords.StringToCoords(globals.CurrentOnlineGame.LastMoveStartWhite.Substring(3, 2));
                                            }
                                            else if (globals.CurrentOnlineGame.LastMoveStartWhite[2] == 'E')
                                            {
                                                Coords capturedCoords = Coords.StringToCoords(globals.CurrentOnlineGame.LastMoveStartWhite.Substring(3, 2));
                                                tileDict[capturedCoords.String].ChessPiece = new ChessPiece();
                                                tileDict[capturedCoords.String].IsOccupied = false;
                                            }
                                            else if (globals.CurrentOnlineGame.LastMoveStartWhite[2] == 'P')
                                            {
                                                string type = globals.CurrentOnlineGame.LastMoveStartWhite.Remove(0, 3);
                                                ChessPieceColor color = ChessPieceColor.White;
                                                if (type == "Bishop")
                                                    chessPiece = new ChessPiece(color, ChessPieceType.Bishop, globals.IsRotated);
                                                else if (type == "Knight")
                                                    chessPiece = new ChessPiece(color, ChessPieceType.Knight, globals.IsRotated);
                                                else if (type == "Rook")
                                                    chessPiece = new ChessPiece(color, ChessPieceType.Rook, globals.IsRotated);
                                                else if (type == "Queen")
                                                    chessPiece = new ChessPiece(color, ChessPieceType.Queen, globals.IsRotated);


                                                tileDict[globals.CurrentOnlineGame.LastMoveEndWhite.Substring(0, 2)].ChessPiece = chessPiece;
                                            }
                                        }

                                        WeakReferenceMessenger.Default.Send(
                                            new MainWindowViewModel.OnPropertyChangedMessage("TileDict"));

                                        isSuccess = true;
                                        globals.IsWaitingForMove = false;

                                        Tuple<string, string> labelMoveInfo = new Tuple<string, string>(
                                            "LabelMoveInfo", globals.CurrentOnlineGame.MoveInfo);
                                        WeakReferenceMessenger.Default.Send(
                                            new MainWindowViewModel.PropertyStringValueChangedMessage(labelMoveInfo));
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    });
                    Thread.Sleep(1000);
                }
            });

            var backgroundThread = new Thread(threadStart)
            {
                IsBackground = true
            };
            backgroundThread.Start();
        }
    }
}
