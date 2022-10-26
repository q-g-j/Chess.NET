using ChessDotNET.CustomTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDotNET.GameLogic
{
    internal static class CheckValidationGameLogic
    {
        internal static bool IsCheck(TileDictionary tileDict, Coords oldCoords, Coords newCoords)
        {
            ChessPieceColor kingColor = tileDict[oldCoords.String].ChessPiece.ChessPieceColor;
            Coords kingCoords;
            bool isCheck = false;

            ChessPiece chessPieceOnNewCoordsBackup = new ChessPiece(tileDict[newCoords.String].ChessPiece);

            tileDict.MoveChessPiece(oldCoords, newCoords, false);

            if (kingColor == ChessPieceColor.White)
            {
                kingCoords = tileDict.KingsCoords["WhiteKing"];
            }
            else
            {
                kingCoords = tileDict.KingsCoords["BlackKing"];
            }

            if (ThreateningValidationGameLogic.AreTilesThreatened(tileDict, newCoords, new List<Coords>() { kingCoords }))
            {
                isCheck = true;
            }

            tileDict.MoveChessPiece(newCoords, oldCoords, false);
            tileDict[newCoords.String].ChessPiece = chessPieceOnNewCoordsBackup;


            return isCheck;
        }
    }
}
