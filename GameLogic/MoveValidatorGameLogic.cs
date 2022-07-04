using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ChessDotNET.CustomTypes;
using static ChessDotNET.CustomTypes.Coords;

namespace ChessDotNET.GameLogic
{
    internal class MoveValidatorGameLogic
    {
        public MoveValidatorGameLogic()
        {
        }

        public static bool ValidateCurrentMove(Dictionary<string, Tile> tileDict, Image currentlyMovedChessPiece, string bottomColor, Coords oldCoords, Coords newCoords)
        {
            string newCoordsString = CoordsToString(newCoords);

            // only process white pawn's moves (if bottom color is white):
            if (ChessPieceImages.Equals(currentlyMovedChessPiece.Source, ChessPieceImages.WhitePawn))
            {
                if (bottomColor == "white")
                {
                    return PawnBottom(tileDict, oldCoords, newCoords);
                }
                else
                {
                    return false;
                }
            }
            else if (!ChessPieceImages.IsEmpty(tileDict[newCoordsString].ChessPiece.ChessPieceImage))
            {
                return false;
            }
            return true;
        }

        private static bool PawnBottom(Dictionary<string, Tile> tileDict, Coords oldCoords, Coords newCoords)
        {
            string newCoordsString = Coords.CoordsToString(newCoords);
            // don't allow to move backwards:
            if (newCoords.Row < oldCoords.Row)
            {
                return false;
            }
            // if it's the pawn's first move:
            if (oldCoords.Row == 2)
            {
                // if it's a straight move:
                if (oldCoords.Col == newCoords.Col)
                {
                    // don't allow to move up more than 2 tiles:
                    if (newCoords.Row - 2 > oldCoords.Row)
                    {
                        return false;
                    }
                    // check if something is in the way:
                    for (int row = oldCoords.Row + 1; row < oldCoords.Row + 2; row++)
                    {
                        string tempCoordsString = Coords.CoordsToString(new Coords(newCoords.Col, row));
                        if (!ChessPieceImages.IsEmpty(tileDict[tempCoordsString].ChessPiece.ChessPieceImage))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            // if it's not the pawn's first move:
            else if (oldCoords.Row != 2)
            {
                // if it's a straight move:
                if (oldCoords.Col == newCoords.Col)
                {
                    // don't allow to move up more than 1 tile:
                    if (newCoords.Row - 1 > oldCoords.Row)
                    {
                        return false;
                    }
                    // check if something is in the way:
                    if (!ChessPieceImages.IsEmpty(tileDict[newCoordsString].ChessPiece.ChessPieceImage))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
