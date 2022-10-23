﻿using ChessDotNET.CustomTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDotNET.GameLogic
{
    internal static class CastlingValidationGameLogic
    {
        internal static bool CanCastle(TileDictionary tileDict, Coords oldCoords, Coords newCoords, ChessPieceColor ownColor)
        {
            // rules for castling:
            // 1. king must not have moved ### done
            // 2. rook must not have moved ### done
            // 3. there must not be any pieces between both ### done
            // 4. the king's old and new position and the crossed tile must not be threatened
            
            // rule 1: has king moved?
            if (tileDict[oldCoords.String].ChessPiece.HasMoved)
            {
                return false;
            }

            // check row 1:
            if (oldCoords.Y == 1)
            {
                // king wants to move left:
                if (newCoords.X == 3)
                {
                    // rule 2: has the associated rook moved?
                    if (tileDict[Coords.IntsToCoordsString(1, 1)].ChessPiece.HasMoved)
                    {
                        return false;
                    }
                    // rule 3: is there a chess piece in between?
                    if    (tileDict[Coords.IntsToCoordsString(2, 1)].ChessPiece.ChessPieceType != ChessPieceType.Empty
                        || tileDict[Coords.IntsToCoordsString(3, 1)].ChessPiece.ChessPieceType != ChessPieceType.Empty
                        || tileDict[Coords.IntsToCoordsString(4, 1)].ChessPiece.ChessPieceType != ChessPieceType.Empty)
                    {
                        return false;
                    }
                    // is the king (old or new coords) or the crossed tile threatened?
                    List<Coords> coordsListToCheck = new List<Coords>
                    {
                        oldCoords,
                        newCoords,
                        new Coords(4, 1)
                    };
                    if (AreTilesThreatened(tileDict, oldCoords, newCoords, coordsListToCheck))
                    {
                        return false;
                    }
                }
                // king wants to move right:
                else if (newCoords.X == 7)
                {
                    // rule 2: has the associated rook moved?
                    if (tileDict[Coords.IntsToCoordsString(8, 1)].ChessPiece.HasMoved)
                    {
                        return false;
                    }
                    // rule 3: is there a chess piece in between?
                    if (tileDict[Coords.IntsToCoordsString(6, 1)].ChessPiece.ChessPieceType != ChessPieceType.Empty
                        || tileDict[Coords.IntsToCoordsString(7, 1)].ChessPiece.ChessPieceType != ChessPieceType.Empty)
                    {
                        return false;
                    }
                    // is the king (old or new coords) or the crossed tile threatened?
                    List<Coords> coordsListToCheck = new List<Coords>
                    {
                        oldCoords,
                        newCoords,
                        new Coords(6, 1)
                    };
                    if (AreTilesThreatened(tileDict, oldCoords, newCoords, coordsListToCheck))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            // check row 8:
            else if (oldCoords.Y == 8)
            {
                // king wants to move left:
                if (newCoords.X == 3)
                {
                    // rule 2: has the associated rook moved?
                    if (tileDict[Coords.IntsToCoordsString(1, 8)].ChessPiece.HasMoved)
                    {
                        return false;
                    }
                    // rule 3: is there a chess piece in between?
                    if (tileDict[Coords.IntsToCoordsString(2, 8)].ChessPiece.ChessPieceType != ChessPieceType.Empty
                        || tileDict[Coords.IntsToCoordsString(3, 8)].ChessPiece.ChessPieceType != ChessPieceType.Empty
                        || tileDict[Coords.IntsToCoordsString(4, 8)].ChessPiece.ChessPieceType != ChessPieceType.Empty)
                    {
                        return false;
                    }
                    // is the king or the crossed tile threatened?
                    List<Coords> coordsListToCheck = new List<Coords>
                    {
                        oldCoords,
                        newCoords,
                        new Coords(4, 8)
                    };
                    if (AreTilesThreatened(tileDict, oldCoords, newCoords, coordsListToCheck))
                    {
                        return false;
                    }
                }
                // king wants to move right:
                else if (newCoords.X == 7)
                {
                    // has the associated rook moved?
                    if (tileDict[Coords.IntsToCoordsString(8, 8)].ChessPiece.HasMoved)
                    {
                        return false;
                    }
                    // rule 3: is there a chess piece in between?
                    if (tileDict[Coords.IntsToCoordsString(6, 8)].ChessPiece.ChessPieceType != ChessPieceType.Empty
                        || tileDict[Coords.IntsToCoordsString(7, 8)].ChessPiece.ChessPieceType != ChessPieceType.Empty)
                    {
                        return false;
                    }
                    // is the king (old or new coords) or the crossed tile threatened?
                    List<Coords> coordsListToCheck = new List<Coords>
                    {
                        oldCoords,
                        newCoords,
                        new Coords(6, 8)
                    };
                    if (AreTilesThreatened(tileDict, oldCoords, newCoords, coordsListToCheck))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            // if not in row 1 or 8, return false:
            else
            {
                return false;
            }

            return true;
        }

        private static bool AreTilesThreatened(TileDictionary tileDict, Coords oldCoords, Coords newCoords, List<Coords> coordsListToCheck)
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
                                && MoveValidationGameLogic.ValidatePawn(tileDict, coords, coordsToCheck, chessPiece.ChessPieceColor, ChessPieceColor.Empty))
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