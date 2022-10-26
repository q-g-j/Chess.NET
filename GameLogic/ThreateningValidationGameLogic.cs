using ChessDotNET.CustomTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDotNET.GameLogic
{
    internal static class ThreateningValidationGameLogic
    {
        internal static bool AreTilesThreatened(TileDictionary tileDict, Coords oldCoords, List<Coords> coordsListToCheck)
        {
            for (int i = 1; i < 9; i++)
            {
                for (int j = 1; j < 9; j++)
                {
                    Coords coords = new Coords(i, j);
                    Tile tile = tileDict[coords.String];
                    ChessPiece chessPiece = tile.ChessPiece;
                    if (chessPiece.ChessPieceColor != ChessPieceColor.Empty
                        && chessPiece.ChessPieceColor != tileDict[oldCoords.String].ChessPiece.ChessPieceColor)
                    {
                        foreach (Coords coordsToCheck in coordsListToCheck)
                        {
                            if (chessPiece.ChessPieceType == ChessPieceType.Pawn
                                && MoveValidationGameLogic.ValidatePawnThreatening(coords, coordsToCheck, chessPiece.ChessPieceColor))
                            {
                                return true;
                            }
                            if ((chessPiece.ChessPieceType == ChessPieceType.Rook || chessPiece.ChessPieceType == ChessPieceType.Queen)
                                && MoveValidationGameLogic.ValidateRookAndQueenHorizontal(tileDict, coords, coordsToCheck, chessPiece.ChessPieceColor, ChessPieceColor.Empty))
                            {
                                return true;
                            }
                            if (chessPiece.ChessPieceType == ChessPieceType.Knight
                                && MoveValidationGameLogic.ValidateKnight(tileDict, coords, coordsToCheck, chessPiece.ChessPieceColor, ChessPieceColor.Empty))
                            {
                                return true;
                            }
                            if ((chessPiece.ChessPieceType == ChessPieceType.Bishop || chessPiece.ChessPieceType == ChessPieceType.Queen)
                                && MoveValidationGameLogic.ValidateBishopAndQueenDiagonal(tileDict, coords, coordsToCheck, chessPiece.ChessPieceColor, ChessPieceColor.Empty))
                            {
                                return true;
                            }
                            if (chessPiece.ChessPieceType == ChessPieceType.King
                                && MoveValidationGameLogic.ValidateKingCastling(tileDict, coords, coordsToCheck, chessPiece.ChessPieceColor, ChessPieceColor.Empty))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
