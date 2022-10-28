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
        internal static bool IsTileThreatened(TileDictionary tileDict, ChessPieceColor ownColor, Coords coordsToCheck, bool isCheckMateCheck)
        {
            for (int i = 1; i < 9; i++)
            {
                for (int j = 1; j < 9; j++)
                {
                    Coords coords = new Coords(i, j);
                    Tile tile = tileDict[coords.String];
                    ChessPiece chessPiece = tile.ChessPiece;
                    if (chessPiece.ChessPieceColor != ChessPieceColor.Empty
                        && chessPiece.ChessPieceColor != ownColor)
                    {
                        if (chessPiece.ChessPieceType == ChessPieceType.Pawn
                            && MoveValidationGameLogic.ValidatePawnThreatening(coords, coordsToCheck, chessPiece.ChessPieceColor))
                        {
                            return true;
                        }
                        if ((chessPiece.ChessPieceType == ChessPieceType.Rook || chessPiece.ChessPieceType == ChessPieceType.Queen)
                            && MoveValidationGameLogic.ValidateRookAndQueenHorizontal(tileDict, coords, coordsToCheck))
                        {
                            return true;
                        }
                        if (chessPiece.ChessPieceType == ChessPieceType.Knight
                            && MoveValidationGameLogic.ValidateKnight(coords, coordsToCheck))
                        {
                            return true;
                        }
                        if ((chessPiece.ChessPieceType == ChessPieceType.Bishop || chessPiece.ChessPieceType == ChessPieceType.Queen)
                            && MoveValidationGameLogic.ValidateBishopAndQueenDiagonal(tileDict, coords, coordsToCheck))
                        {
                            return true;
                        }
                        if (chessPiece.ChessPieceType == ChessPieceType.King
                            && MoveValidationGameLogic.ValidateKing(coords, coordsToCheck)
                            && ! isCheckMateCheck)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        internal static List<Coords> IsTileThreatenedList(TileDictionary tileDict, ChessPieceColor ownColor, Coords coordsToCheck)
        {
            List<Coords> result = new List<Coords>();

            for (int i = 1; i < 9; i++)
            {
                for (int j = 1; j < 9; j++)
                {
                    Coords coords = new Coords(i, j);
                    Tile tile = tileDict[coords.String];
                    ChessPiece chessPiece = tile.ChessPiece;
                    if (chessPiece.ChessPieceColor != ChessPieceColor.Empty
                        && chessPiece.ChessPieceColor != ownColor)
                    {
                        if (chessPiece.ChessPieceType == ChessPieceType.Pawn
                            && MoveValidationGameLogic.ValidatePawnThreatening(coords, coordsToCheck, chessPiece.ChessPieceColor))
                        {
                            result.Add(coords);
                        }
                        if ((chessPiece.ChessPieceType == ChessPieceType.Rook || chessPiece.ChessPieceType == ChessPieceType.Queen)
                            && MoveValidationGameLogic.ValidateRookAndQueenHorizontal(tileDict, coords, coordsToCheck))
                        {
                            result.Add(coords);
                        }
                        if (chessPiece.ChessPieceType == ChessPieceType.Knight
                            && MoveValidationGameLogic.ValidateKnight(coords, coordsToCheck))
                        {
                            result.Add(coords);
                        }
                        if ((chessPiece.ChessPieceType == ChessPieceType.Bishop || chessPiece.ChessPieceType == ChessPieceType.Queen)
                            && MoveValidationGameLogic.ValidateBishopAndQueenDiagonal(tileDict, coords, coordsToCheck))
                        {
                            result.Add(coords);
                        }
                        if (chessPiece.ChessPieceType == ChessPieceType.King
                            && MoveValidationGameLogic.ValidateKing(coords, coordsToCheck))
                        {
                            result.Add(coords);
                        }
                    }
                }
            }
            return result;
        }
        internal static bool AreTilesThreatened(TileDictionary tileDict, ChessPieceColor ownColor, List<Coords> coordsListToCheck)
        {
            for (int i = 1; i < 9; i++)
            {
                for (int j = 1; j < 9; j++)
                {
                    Coords coords = new Coords(i, j);
                    Tile tile = tileDict[coords.String];
                    ChessPiece chessPiece = tile.ChessPiece;
                    if (chessPiece.ChessPieceColor != ChessPieceColor.Empty
                        && chessPiece.ChessPieceColor != ownColor)
                    {
                        foreach (Coords coordsToCheck in coordsListToCheck)
                        {
                            if (chessPiece.ChessPieceType == ChessPieceType.Pawn
                                && MoveValidationGameLogic.ValidatePawnThreatening(coords, coordsToCheck, chessPiece.ChessPieceColor))
                            {
                                return true;
                            }
                            if ((chessPiece.ChessPieceType == ChessPieceType.Rook || chessPiece.ChessPieceType == ChessPieceType.Queen)
                                && MoveValidationGameLogic.ValidateRookAndQueenHorizontal(tileDict, coords, coordsToCheck))
                            {
                                return true;
                            }
                            if (chessPiece.ChessPieceType == ChessPieceType.Knight
                                && MoveValidationGameLogic.ValidateKnight(coords, coordsToCheck))
                            {
                                return true;
                            }
                            if ((chessPiece.ChessPieceType == ChessPieceType.Bishop || chessPiece.ChessPieceType == ChessPieceType.Queen)
                                && MoveValidationGameLogic.ValidateBishopAndQueenDiagonal(tileDict, coords, coordsToCheck))
                            {
                                return true;
                            }
                            if (chessPiece.ChessPieceType == ChessPieceType.King
                                && MoveValidationGameLogic.ValidateKing(coords, coordsToCheck))
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
