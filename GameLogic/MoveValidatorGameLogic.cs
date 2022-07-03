using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ChessDotNET.Helpers;
using static ChessDotNET.Helpers.Coords;

namespace ChessDotNET.GameLogic
{
    internal class MoveValidatorGameLogic
    {
        public MoveValidatorGameLogic()
        {
        }

        public static bool ValidateCurrentMove(List<List<string>> tileImageStringList, Image currentlyMovedChessPiece, string bottomColor, Coords oldCoords, Coords newCoords)
        {
            ChessPieces chessPieces = new ChessPieces();
            // only process white pawn's moves (if bottom color is white):
            if (currentlyMovedChessPiece.Source.ToString() == chessPieces.WhitePawn.ToString())
            {
                if (bottomColor == "white")
                {
                    return PawnBottom(tileImageStringList, oldCoords, newCoords);
                }
                else
                {
                    return false;
                }
            }
            else if (tileImageStringList[newCoords.Col][newCoords.Row] != "")
            {
                return false;
            }
            return true;
        }

        private static bool PawnBottom(List<List<string>> tileImageStringList, Coords oldCoords, Coords newCoords)
        {
            // don't allow to move backwards:
            if (newCoords.Row > oldCoords.Row)
            {
                return false;
            }
            // if it's the pawn's first move:
            if (oldCoords.Row == 6)
            {
                // if it's a straight move:
                if (oldCoords.Col == newCoords.Col)
                {
                    // don't allow to move up more than 2 tiles:
                    if (newCoords.Row + 2 < oldCoords.Row)
                    {
                        return false;
                    }
                    // check if something is in the way:
                    for (int row = oldCoords.Row - 1; row > oldCoords.Row - 2; row--)
                    {
                        if (tileImageStringList[newCoords.Col][row] != "")
                        {
                            return false;
                        }
                    }
                }
            }
            // if it's not the pawn's first move:
            if (oldCoords.Row != 6)
            {
                // if it's a straight move:
                if (oldCoords.Col == newCoords.Col)
                {
                    // don't allow to move up more than 1 tile:
                    if (newCoords.Row + 1 < oldCoords.Row)
                    {
                        return false;
                    }
                    // check if something is in the way:
                    if (tileImageStringList[newCoords.Col][newCoords.Row] != "")
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
