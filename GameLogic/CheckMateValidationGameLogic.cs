using ChessDotNET.CustomTypes;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Documents;

namespace ChessDotNET.GameLogic
{
    internal static class CheckMateValidationGameLogic
    {
        internal static bool IsCheckMate(TileDictionary tileDict, Coords kingCoords)
        {
            ChessPieceColor kingColor = tileDict[kingCoords.String].ChessPiece.ChessPieceColor;
            ChessPieceColor ownColor = kingColor == ChessPieceColor.White ? ChessPieceColor.Black : ChessPieceColor.White;
            List<Coords> threateningTiles = ThreateningValidationGameLogic.IsTileThreatenedList(tileDict, kingColor, kingCoords);
            int threateningTilesNumber = threateningTiles.Count;

            if (threateningTilesNumber > 0)
            {
                // can the king escape the check mate without being in check again?
                List<Coords> coordsKingNeighbors = new List<Coords>()
                {                    
                    new Coords(kingCoords.X - 1, kingCoords.Y + 1), // top left
                    new Coords(kingCoords.X,     kingCoords.Y + 1), // top center
                    new Coords(kingCoords.X + 1, kingCoords.Y + 1), // top right
                    new Coords(kingCoords.X - 1, kingCoords.Y),     // left
                    new Coords(kingCoords.X + 1, kingCoords.Y),     // right
                    new Coords(kingCoords.X - 1, kingCoords.Y - 1), // bottom left
                    new Coords(kingCoords.X,     kingCoords.Y - 1), // bottom center
                    new Coords(kingCoords.X + 1, kingCoords.Y - 1)  // bottom right
                };

                for (int i = 0; i < coordsKingNeighbors.Count; i++)
                {
                    if (coordsKingNeighbors[i].X >= 1 && coordsKingNeighbors[i].X <= 8
                        && coordsKingNeighbors[i].Y >= 1 && coordsKingNeighbors[i].Y <= 8)
                    {
                        if (tileDict[coordsKingNeighbors[i].String].ChessPiece.ChessPieceColor != kingColor
                            && !CheckValidationGameLogic.IsCheck(tileDict, kingCoords, coordsKingNeighbors[i]))
                        {
                            return false;
                        }
                    }
                }

                // if there is no double check, test if the tiles between the king and the threatening chess piece
                // can be reached by a chess piece of the same color:
                if (threateningTilesNumber == 1)
                {
                    Tile threateningTile = tileDict[threateningTiles[0].String];
                    ChessPieceType threateningType = threateningTile.ChessPiece.ChessPieceType;
                    //System.Diagnostics.Debug.WriteLine(threateningType.ToString());

                    // check if an opponent's queen is threatening own king:
                    if (threateningType == ChessPieceType.Queen)
                    {
                        if (CanBlockQueenAndRookHorizontally(tileDict, threateningTile, kingCoords, ownColor))
                        {
                            return false;
                        }
                        else if (CanBlockQueenAndRookVertically(tileDict, threateningTile, kingCoords, ownColor))
                        {
                            return false;
                        }
                        else if (CanBlockQueenAndBishopDiagonally(tileDict, threateningTile, kingCoords, ownColor))
                        {
                            return false;
                        }
                    }

                    // check if an opponent's rook is threatening own king:
                    else if (threateningType == ChessPieceType.Rook)
                    {
                        if (CanBlockQueenAndRookHorizontally(tileDict, threateningTile, kingCoords, ownColor))
                        {
                            return false;
                        }
                    }

                    // check if an opponent's bishop is threatening own king:
                    else if (threateningTile.ChessPiece.ChessPieceType == ChessPieceType.Bishop)
                    {
                        if (CanBlockQueenAndBishopDiagonally(tileDict, threateningTile, kingCoords, ownColor))
                        {
                            return false;
                        }
                    }

                    // check if threatening chess piece can be captured:
                    else if (ThreateningValidationGameLogic.IsTileThreatened(tileDict, ownColor, threateningTiles[0], true))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
        private static bool CanBlockQueenAndRookHorizontally(
            TileDictionary tileDict, Tile threateningTile, Coords kingCoords, ChessPieceColor ownColor)
        {
            if (threateningTile.Coords.Y == kingCoords.Y)
            {
                if (threateningTile.Coords.X < kingCoords.X)
                {
                    for (int column = threateningTile.Coords.X + 1; column <= kingCoords.X - 1; column++)
                    {
                        Coords coordsInBetween = new Coords(column, threateningTile.Coords.Y);
                        if (MoveValidationGameLogic.CanReachTile(tileDict, ownColor, coordsInBetween))
                        {
                            return true;
                        }
                    }
                }
                else if (threateningTile.Coords.X > kingCoords.X)
                {
                    for (int column = threateningTile.Coords.X - 1; column >= kingCoords.X + 1; column--)
                    {
                        Coords coordsInBetween = new Coords(column, threateningTile.Coords.Y);
                        if (MoveValidationGameLogic.CanReachTile(tileDict, ownColor, coordsInBetween))
                        {
                            return true;
                        }
                        //System.Diagnostics.Debug.WriteLine(coordsInBetween.ToString());
                    }
                }
            }

            return false;
        }
        private static bool CanBlockQueenAndRookVertically(
            TileDictionary tileDict, Tile threateningTile, Coords kingCoords, ChessPieceColor ownColor)
        {
            if (threateningTile.Coords.X == kingCoords.X)
            {
                if (threateningTile.Coords.Y < kingCoords.Y)
                {
                    for (int row = threateningTile.Coords.Y + 1; row <= kingCoords.Y - 1; row++)
                    {
                        Coords coordsInBetween = new Coords(row, threateningTile.Coords.Y);
                        if (MoveValidationGameLogic.CanReachTile(tileDict, ownColor, coordsInBetween))
                        {
                            System.Diagnostics.Debug.WriteLine("no check mate");
                            return false;
                        }
                    }
                }
                else if (threateningTile.Coords.Y > kingCoords.Y)
                {
                    for (int row = threateningTile.Coords.Y - 1; row >= kingCoords.Y + 1; row--)
                    {
                        Coords coordsInBetween = new Coords(row, threateningTile.Coords.Y);
                        if (MoveValidationGameLogic.CanReachTile(tileDict, ownColor, coordsInBetween))
                        {
                            System.Diagnostics.Debug.WriteLine("no check mate");
                            return false;
                        }
                        //System.Diagnostics.Debug.WriteLine(coordsInBetween.ToString());
                    }
                }
            }

            return false;
        }
        private static bool CanBlockQueenAndBishopDiagonally(
            TileDictionary tileDict, Tile threateningTile, Coords kingCoords, ChessPieceColor ownColor)
        {
            if (threateningTile.Coords.X < kingCoords.X)
            {
                if (threateningTile.Coords.Y < kingCoords.Y)
                {
                    for (int column = threateningTile.Coords.X + 1; column <= kingCoords.X - 1; column++)
                    {
                        Coords coordsInBetween = new Coords(column, threateningTile.Coords.Y + (column - threateningTile.Coords.X));
                        System.Diagnostics.Debug.WriteLine(coordsInBetween.String);
                        if (MoveValidationGameLogic.CanReachTile(tileDict, ownColor, coordsInBetween))
                        {
                            return true;
                        }
                    }
                }
                else if (threateningTile.Coords.Y > kingCoords.Y)
                {
                    for (int column = threateningTile.Coords.X + 1; column <= kingCoords.X - 1; column++)
                    {
                        Coords coordsInBetween = new Coords(column, threateningTile.Coords.Y - (column - threateningTile.Coords.X));
                        System.Diagnostics.Debug.WriteLine(coordsInBetween.String);
                        if (MoveValidationGameLogic.CanReachTile(tileDict, ownColor, coordsInBetween))
                        {
                            return true;
                        }
                    }
                }
            }

            else if (threateningTile.Coords.X > kingCoords.X)
            {
                if (threateningTile.Coords.Y < kingCoords.Y)
                {
                    for (int column = threateningTile.Coords.X - 1; column >= kingCoords.X + 1; column--)
                    {
                        Coords coordsInBetween = new Coords(column, threateningTile.Coords.Y - (column - threateningTile.Coords.X));
                        System.Diagnostics.Debug.WriteLine(coordsInBetween.String);
                        if (MoveValidationGameLogic.CanReachTile(tileDict, ownColor, coordsInBetween))
                        {
                            return true;
                        }
                    }
                }
                else if (threateningTile.Coords.Y > kingCoords.Y)
                {
                    for (int column = threateningTile.Coords.X - 1; column >= kingCoords.X + 1; column--)
                    {
                        Coords coordsInBetween = new Coords(column, threateningTile.Coords.Y + (column - threateningTile.Coords.X));
                        System.Diagnostics.Debug.WriteLine("here" + coordsInBetween.String);
                        if (MoveValidationGameLogic.CanReachTile(tileDict, ownColor, coordsInBetween))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
