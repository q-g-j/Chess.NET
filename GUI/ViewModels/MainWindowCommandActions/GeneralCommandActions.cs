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
        public GeneralCommandActions(MainWindowViewModel _mainWindowViewModel)
        {
            vm = _mainWindowViewModel;
        }

        private readonly MainWindowViewModel vm;
        internal void OpenSideMenuAction()
        {
            if (!vm.WasSideMenuOpen)
            {
                if (vm.SideMenuVisibility != "Visible" && vm.SettingsVisibility == "Hidden" && vm.NewEmailGameVisibility == "Hidden")
                {
                    vm.SideMenuNewGameModeVisibility = "Hidden";
                    vm.SideMenuButtonsNewGameLocalColorVisibility = "Hidden";
                    vm.SideMenuMainMenuVisibility = "Visible";
                    vm.SideMenuVisibility = "Visible";
                }
                else vm.SideMenuVisibility = "Hidden";
            }
            else
            {
                vm.WasSideMenuOpen = false;
            }
        }
        internal void WindowMouseMoveAction(object o)
        {
            MouseEventArgs e = o as MouseEventArgs;

            if (vm.CurrentlyDraggedChessPieceImage != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed && !vm.DoWaitForEmail && vm.IsInputAllowed())
                {
                    if (vm.IsEmailGame && vm.EmailGameOwnColor != ChessPieceImages.GetImageColor(vm.CurrentlyDraggedChessPieceImage.Source)) return;
                    if (vm.SideMenuVisibility == "Visible")
                    {
                        vm.WasSideMenuOpen = true;
                        vm.SideMenuVisibility = "Hidden";
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
                    if (vm.SideMenuVisibility == "Visible")
                    {
                        vm.WasSideMenuOpen = true;
                        vm.SideMenuVisibility = "Hidden";
                    }
                    else
                    {
                        vm.WasSideMenuOpen = false;
                    }
                }
            }
        }
        internal async void WindowMouseLeftUpAction(object o, TileDictionary tileDict, AppSettings appSettings)
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

                            bool isValidMove = MoveValidatorGameLogic.ValidateCurrentMove(vm.TileDictReadOnly, vm.BottomColor, oldCoords, newCoords);

                            if (isValidMove)
                            {
                                ChessPieceColor oldColor = tileDict[oldCoords.ToString()].ChessPiece.ChessPieceColor;

                                Canvas.SetLeft(vm.CurrentlyDraggedChessPieceImage, vm.CurrentlyDraggedChessPieceOriginalCanvasLeft);
                                Canvas.SetTop(vm.CurrentlyDraggedChessPieceImage, vm.CurrentlyDraggedChessPieceOriginalCanvasTop);
                                //Console.WriteLine("Old Coords before: " + "Is occupied? " + tileDict[oldCoordsString.ToString()].IsOccupied.ToString() + "\t| Coords: " + oldCoordsString.ToString() + "\t| Color = " + tileDict[oldCoordsString.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[oldCoordsString.ToString()].ChessPiece.ChessPieceType.ToString());
                                //Console.WriteLine("New Coords before: " + "Is occupied? " + tileDict[newCoordsString.ToString()].IsOccupied.ToString() + "\t| Coords: " + newCoordsString.ToString() + "\t| Color = " + tileDict[newCoordsString.ToString()].ChessPiece.ChessPieceColor.ToString() + "\t| Type = " + tileDict[newCoordsString.ToString()].ChessPiece.ChessPieceType.ToString());

                                tileDict[newCoords.ToString()].ChessPiece = tileDict[oldCoords.ToString()].ChessPiece;
                                tileDict[oldCoords.ToString()].ChessPiece = new ChessPiece(ChessPieceImages.Empty, ChessPieceColor.Empty, ChessPieceType.Empty);
                                tileDict[oldCoords.ToString()].IsOccupied = false;
                                tileDict[newCoords.ToString()].IsOccupied = true;
                                tileDict[newCoords.ToString()].ChessPiece.MoveCount++;
                                vm.TileDict = tileDict;

                                if (vm.IsEmailGame)
                                {
                                    if (oldColor == ChessPieceColor.White)
                                    {
                                        if (vm.DeleteOldEmailsTask != null) await vm.DeleteOldEmailsTask;
                                        await Task.Run(() => EmailChess.Send.SendEmailWhiteMoveTask(oldCoords, newCoords, appSettings, vm.NewEmailGameTextBoxOpponentEmail));
                                        vm.DoWaitForEmail = true;
                                        await Task.Run(() => EmailChess.Receive.WaitForEmailNextBlackMoveTask(vm, tileDict, appSettings));
                                        vm.DoWaitForEmail = false;
                                    }
                                    else
                                    {
                                        if (vm.DeleteOldEmailsTask != null) await vm.DeleteOldEmailsTask;
                                        await Task.Run(() => EmailChess.Send.SendEmailBlackMoveTask(oldCoords, newCoords, appSettings, vm.NewEmailGameTextBoxOpponentEmail));
                                        vm.DoWaitForEmail = true;
                                        await Task.Run(() => EmailChess.Receive.WaitForEmailNextWhiteMoveTask(vm, tileDict, appSettings));
                                        vm.DoWaitForEmail = false;
                                    }
                                }

                                // store a list of threatening tiles:
                                tileDict[newCoords.ToString()].ThreatenedByTileList = ThreatDetectionGameLogic.GetThreateningTilesList(tileDict, newCoords, vm.BottomColor);

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
