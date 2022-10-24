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

            if (tileDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Pawn && oldCoordsColor != pawnMovedTwoTilesColor)
            {
                if (tileDict[oldCoords.String].Y == tileDict.CoordsPawnMovedTwoTiles.Y)
                {
                    if (tileDict.CoordsPawnMovedTwoTiles.X == newCoords.X)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
