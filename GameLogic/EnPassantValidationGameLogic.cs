using ChessDotNET.CustomTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDotNET.GameLogic
{
    internal static class EnPassantValidationGameLogic
    {
        internal static bool CanCaptureEnPassant(TileDictionary tileDict, Coords oldCoords, Coords newCoords)
        {
            ChessPieceColor oldCoordsColor = tileDict[oldCoords.String].ChessPiece.ChessPieceColor;
            ChessPieceColor pawnMovedTwoTilesColor = tileDict[tileDict.CoordsPawnMovedTwoTiles.String].ChessPiece.ChessPieceColor;

            if (tileDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Pawn
                && oldCoordsColor != pawnMovedTwoTilesColor
                && tileDict[oldCoords.String].Coords.Y == tileDict.CoordsPawnMovedTwoTiles.Y
                && tileDict.CoordsPawnMovedTwoTiles.X == newCoords.X
                && (oldCoordsColor == ChessPieceColor.White && tileDict[newCoords.String].Coords.Y - tileDict.CoordsPawnMovedTwoTiles.Y == 1
                || oldCoordsColor == ChessPieceColor.Black && tileDict[newCoords.String].Coords.Y - tileDict.CoordsPawnMovedTwoTiles.Y == -1))
            {
                return true;
            }

            return false;
        }
    }
}
