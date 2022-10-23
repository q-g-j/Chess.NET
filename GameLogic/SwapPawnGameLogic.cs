using ChessDotNET.CustomTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDotNET.GameLogic
{
    internal static class SwapPawnGameLogic
    {
        internal static bool CanSwap(TileDictionary tileDict, Coords oldCoords, Coords newCoords)
        {
            if ((tileDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Pawn && newCoords.Y == 8)
                || (tileDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Pawn && newCoords.Y == 1))
            {
                return true;
            }

            return false;
        }
    }
}
