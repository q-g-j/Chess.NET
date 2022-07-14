using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ChessDotNET.CustomTypes;
using ChessDotNET.GameLogic;
using ChessDotNET.Settings;
using ChessDotNET.GUI.ViewHelpers;


namespace ChessDotNET.GUI.ViewModels.MainWindow
{
    internal class GeneralCommandActions
    {
        public GeneralCommandActions(MainWindowViewModel _mainWindowViewModel, AppSettings _appSettings)
        {
            vm = _mainWindowViewModel;
            appSettings = _appSettings;
        }

        private readonly MainWindowViewModel vm;
        private readonly AppSettings appSettings;

        internal void OpenSideMenuAction()
        {
            if (!vm.WasSideMenuOpen)
            {
                if (vm.PropertiesDict["SideMenuVisibility"] != "Visible" 
                    && vm.PropertiesDict["SettingsOverlayVisibility"] == "Hidden" 
                    && vm.PropertiesDict["NewEmailGameOverlayVisibility"] == "Hidden")
                {
                    vm.PropertiesDict["SideMenuNewGameModeVisibility"] = "Hidden";
                    vm.PropertiesDict["SideMenuButtonsNewGameLocalColorVisibility"] = "Hidden";
                    vm.PropertiesDict["SideMenuMainMenuVisibility"] = "Visible";
                    vm.PropertiesDict["SideMenuVisibility"] = "Visible";
                }
                else vm.PropertiesDict["SideMenuVisibility"] = "Hidden";
            }
            else
            {
                vm.WasSideMenuOpen = false;
            }
            vm.PropertiesDict = vm.PropertiesDict;
        }
        internal void WindowMouseMoveAction(object o)
        {
            MouseEventArgs e = o as MouseEventArgs;

            if (vm.CurrentlyDraggedChessPieceImage != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed && !vm.DoWaitForEmail && vm.IsInputAllowed())
                {
                    if (vm.IsEmailGame && vm.EmailGameOwnColor != ChessPieceImages.GetImageColor(vm.CurrentlyDraggedChessPieceImage.Source)) return;
                    if (vm.PropertiesDict["SideMenuVisibility"] == "Visible")
                    {
                        vm.WasSideMenuOpen = true;
                        vm.PropertiesDict["SideMenuVisibility"] = "Hidden";
                    }
                    else if (!vm.WasSideMenuOpen)
                    {
                        if (!vm.IsMouseMoving)
                        {
                            vm.DragOverCanvasPosition = e.GetPosition(vm.ChessCanvas);
                            vm.DragOverChessPiecePosition = e.GetPosition(vm.CurrentlyDraggedChessPieceImage);
                        }
                        vm.IsMouseMoving = true;
                        vm.DragOverCanvasPosition = e.GetPosition(vm.ChessCanvas);
                        vm.CurrentlyDraggedChessPieceImage.SetValue(Panel.ZIndexProperty, 20);

                        Canvas.SetLeft(vm.CurrentlyDraggedChessPieceImage, vm.DragOverCanvasPosition.X - vm.DragOverChessPiecePosition.X);
                        Canvas.SetTop(vm.CurrentlyDraggedChessPieceImage, vm.DragOverCanvasPosition.Y - vm.DragOverChessPiecePosition.Y);
                    }
                    vm.PropertiesDict = vm.PropertiesDict;
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
                    if (vm.PropertiesDict["SideMenuVisibility"] == "Visible")
                    {
                        vm.WasSideMenuOpen = true;
                        vm.PropertiesDict["SideMenuVisibility"] = "Hidden";
                    }
                    else
                    {
                        vm.WasSideMenuOpen = false;
                    }

                    vm.PropertiesDict = vm.PropertiesDict;
                }
            }
        }
        internal async void WindowMouseLeftUpAction(object o, TileDictionary tileDict)
        {
            if (!vm.DoWaitForEmail && vm.IsInputAllowed())
            {
                MouseEventArgs e = o as MouseEventArgs;

                if (vm.CurrentlyDraggedChessPieceImage == null) return;
                if (vm.CurrentlyDraggedChessPieceImage.IsMouseCaptured) vm.CurrentlyDraggedChessPieceImage.ReleaseMouseCapture();

                if (vm.IsMouseMoving)
                {
                    vm.IsMouseMoving = false;
                    if (vm.DragOverCanvasPosition.X < 0 || vm.DragOverCanvasPosition.X > 400 || vm.DragOverCanvasPosition.Y < 0 || vm.DragOverCanvasPosition.Y > 400)
                    {
                        Canvas.SetLeft(vm.CurrentlyDraggedChessPieceImage, vm.CurrentlyDraggedChessPieceOriginalCanvasLeft);
                        Canvas.SetTop(vm.CurrentlyDraggedChessPieceImage, vm.CurrentlyDraggedChessPieceOriginalCanvasTop);
                        vm.CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
                        vm.CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;
                    }
                    else
                    {
                        Point oldPoint = new Point(vm.CurrentlyDraggedChessPieceOriginalCanvasLeft, vm.CurrentlyDraggedChessPieceOriginalCanvasTop);
                        Coords oldCoords = Coords.CanvasPositionToCoords(oldPoint);
                        Coords newCoords = Coords.CanvasPositionToCoords(vm.DragOverCanvasPosition);

                        if (newCoords.Col >= 1 && newCoords.Col <= 8 && newCoords.Row >= 1 && newCoords.Row <= 8
                            && !(newCoords.Col == oldCoords.Col && newCoords.Row == oldCoords.Row))
                        {

                            bool isValidMove = MoveValidatorGameLogic.ValidateCurrentMove(vm.TileDictReadOnly, oldCoords, newCoords);

                            if (isValidMove)
                            {
                                ChessPieceColor oldColor = tileDict[oldCoords.ToString()].ChessPiece.ChessPieceColor;

                                Canvas.SetLeft(vm.CurrentlyDraggedChessPieceImage, vm.CurrentlyDraggedChessPieceOriginalCanvasLeft);
                                Canvas.SetTop(vm.CurrentlyDraggedChessPieceImage, vm.CurrentlyDraggedChessPieceOriginalCanvasTop);
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

                                vm.TileDict = vm.TileDict;

                                if (vm.IsEmailGame)
                                {
                                    if (oldColor == ChessPieceColor.White)
                                    {
                                        if (vm.DeleteOldEmailsTask != null) await vm.DeleteOldEmailsTask;
                                        await Task.Run(() => EmailChess.Send.SendEmailWhiteMoveTask(oldCoords, newCoords, appSettings, vm.PropertiesDict["NewEmailGameOverlayTextBoxOpponentEmail"]));
                                        vm.DoWaitForEmail = true;
                                        await Task.Run(() => EmailChess.Receive.WaitForEmailNextBlackMoveTask(vm, tileDict, appSettings));
                                        vm.DoWaitForEmail = false;
                                    }
                                    else
                                    {
                                        if (vm.DeleteOldEmailsTask != null) await vm.DeleteOldEmailsTask;
                                        await Task.Run(() => EmailChess.Send.SendEmailBlackMoveTask(oldCoords, newCoords, appSettings, vm.PropertiesDict["NewEmailGameOverlayTextBoxOpponentEmail"]));
                                        vm.DoWaitForEmail = true;
                                        await Task.Run(() => EmailChess.Receive.WaitForEmailNextWhiteMoveTask(vm, tileDict, appSettings));
                                        vm.DoWaitForEmail = false;
                                    }
                                }

                                // store a list of threatening tiles:
                                tileDict[newCoords.ToString()].ThreatenedByTileList = ThreatDetectionGameLogic.GetThreateningTilesList(tileDict, newCoords);

                                //Console.WriteLine("Old Coords after : " + "Is occupied? " + tileDict[oldCoordsString.ToString()].IsOccupied.ToString() + "\t| Coords: " + oldCoordsString.ToString() + "\t| Color = " + tileDict[oldCoordsString.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[oldCoordsString.ToString()].ChessPiece.ChessPieceType.ToString());
                                //Console.WriteLine("New Coords after : " + "Is occupied? " + tileDict[newCoordsString.ToString()].IsOccupied.ToString() + "\t| Coords: " + newCoordsString.ToString() + "\t| Color = " + tileDict[newCoordsString.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[newCoordsString.ToString()].ChessPiece.ChessPieceType.ToString());
                                //Console.WriteLine("MoveCount: " + tileDict[newCoordsString.ToString()].ChessPiece.MoveCount);
                                //Console.WriteLine();
                            }
                            else
                            {
                                Canvas.SetLeft(vm.CurrentlyDraggedChessPieceImage, vm.CurrentlyDraggedChessPieceOriginalCanvasLeft);
                                Canvas.SetTop(vm.CurrentlyDraggedChessPieceImage, vm.CurrentlyDraggedChessPieceOriginalCanvasTop);
                            }
                        }
                        else
                        {
                            Canvas.SetLeft(vm.CurrentlyDraggedChessPieceImage, vm.CurrentlyDraggedChessPieceOriginalCanvasLeft);
                            Canvas.SetTop(vm.CurrentlyDraggedChessPieceImage, vm.CurrentlyDraggedChessPieceOriginalCanvasTop);
                        }
                    }
                    vm.CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
                    vm.CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;
                    vm.CurrentlyDraggedChessPieceImage.SetValue(Panel.ZIndexProperty, 10);
                }
                vm.CurrentlyDraggedChessPieceImage = null;
                e.Handled = true;
            }
        }
        internal void ChessPieceMouseleftDownAction(object o)
        {
            if (!vm.DoWaitForEmail && vm.IsInputAllowed())
            {
                object param = ((CompositeCommandParameter)o).Parameter;
                MouseEventArgs e = ((CompositeCommandParameter)o).EventArgs as MouseEventArgs;
                vm.CurrentlyDraggedChessPieceImage = null;
                vm.CurrentlyDraggedChessPieceOriginalCanvasLeft = -1000;
                vm.CurrentlyDraggedChessPieceOriginalCanvasTop = -1000;
                vm.CurrentlyDraggedChessPieceImage = param as Image;
                if (!ChessPieceImages.IsEmpty(vm.CurrentlyDraggedChessPieceImage.Source))
                {
                    vm.ChessCanvas = VisualTreeHelper.GetParent(param as Image) as Canvas;

                    if (vm.CurrentlyDraggedChessPieceOriginalCanvasLeft < 0 && vm.CurrentlyDraggedChessPieceOriginalCanvasTop < 0)
                    {
                        vm.CurrentlyDraggedChessPieceOriginalCanvasLeft = int.Parse(vm.CurrentlyDraggedChessPieceImage.GetValue(Canvas.LeftProperty).ToString());
                        vm.CurrentlyDraggedChessPieceOriginalCanvasTop = int.Parse(vm.CurrentlyDraggedChessPieceImage.GetValue(Canvas.TopProperty).ToString());
                    }
                    vm.CurrentlyDraggedChessPieceImage.CaptureMouse();
                }
                vm.WasSideMenuOpen = false;
                e.Handled = true;
            }
        }
    }
}
