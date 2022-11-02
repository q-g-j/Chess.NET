using ChessDotNET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDotNET.GameLogic
{
    internal static class PromotePawnGameLogic
    {
        internal static bool CanPromote(TileDictionary tileDict, Coords oldCoords, Coords newCoords)
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
