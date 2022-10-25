﻿using System;
using System.Collections.Generic;
using ChessDotNET.CustomTypes;
using static ChessDotNET.CustomTypes.Coords;


namespace ChessDotNET.GameLogic
{
    internal class MoveValidationData
    {
        internal bool IsValid { get; set; }
        internal bool CanCastle { get; set; }
        internal bool CanCaptureEnPassant { get; set; }
        internal bool MovedTwoTiles { get; set; }
        internal List<Coords> Coords { get; set; }

        internal MoveValidationData()
        {
            Coords = new List<Coords>();
        }
    }
    internal static class MoveValidationGameLogic
    {
        internal static MoveValidationData ValidateCurrentMove(TileDictionary tileDict, Coords oldCoords, Coords newCoords)
        {
            MoveValidationData moveValidationData = new MoveValidationData();

            ChessPieceColor oldCoordsColor = tileDict[oldCoords.String].ChessPiece.ChessPieceColor;
            ChessPieceColor newCoordsColor = tileDict[newCoords.String].ChessPiece.ChessPieceColor;

            // validate pawn's move:
            if (tileDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Pawn)
            {
                moveValidationData = ValidatePawn(
                    tileDict, oldCoords, newCoords, oldCoordsColor, newCoordsColor);
            }
            // validate bishop's move:
            else if (tileDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Bishop)
            {
                moveValidationData.IsValid = ValidateBishopAndQueenDiagonal(
                    tileDict, oldCoords, newCoords, oldCoordsColor, newCoordsColor);
            }
            // validate queen's move:
            else if (tileDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Queen)
            {
                bool isValidStraight = ValidateRookAndQueenHorizontal(tileDict, oldCoords, newCoords, oldCoordsColor, newCoordsColor);
                bool isValidDiagonal = ValidateBishopAndQueenDiagonal(tileDict, oldCoords, newCoords, oldCoordsColor, newCoordsColor);

                moveValidationData.IsValid = isValidStraight || isValidDiagonal;
            }
            // validate king's move:
            else if (tileDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.King)
            {
                return ValidateKing(
                    tileDict, oldCoords, newCoords, oldCoordsColor, newCoordsColor);
            }
            // validate rook's move:
            else if (tileDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Rook)
            {
                moveValidationData.IsValid = ValidateRookAndQueenHorizontal(
                    tileDict, oldCoords, newCoords, oldCoordsColor, newCoordsColor);
            }
            // validate knight's move:
            else if (tileDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Knight)
            {
                moveValidationData.IsValid = ValidateKnight(tileDict, oldCoords, newCoords, oldCoordsColor, newCoordsColor);
            }
            else
            {
                moveValidationData.IsValid = true;
            }

            return moveValidationData;
        }
        internal static MoveValidationData ValidatePawn(
            TileDictionary tileDict,
            Coords oldCoords,
            Coords newCoords,
            ChessPieceColor oldCoordsColor,
            ChessPieceColor newCoordsColor)
        {
            // initialize return type:
            MoveValidationData moveValidationData = new MoveValidationData
            {
                IsValid = false
            };

            // has a pawn move two tiles in the last turn:
            if (tileDict.CoordsPawnMovedTwoTiles != null)
            {
                // validate if it can be captured:
                if (EnPassantValidationGameLogic.CanCaptureEnPassant(tileDict, oldCoords, newCoords))
                {
                    moveValidationData.IsValid = true;
                    moveValidationData.CanCaptureEnPassant = true;
                    return moveValidationData;
                }
            }

            // don't allow to move along the same row:
            if (oldCoords.Y == newCoords.Y) return moveValidationData;
            // dont't allow to capture a piece of the same color:
            if (oldCoordsColor == newCoordsColor) return moveValidationData;

            // validate white pawns:
            if (oldCoordsColor == ChessPieceColor.White)
            {
                // don't allow to move backwards:
                if (oldCoords.Y > newCoords.Y) return moveValidationData;
                // if it's the pawn's first move:
                if (oldCoords.Y == 2)
                {
                    if (newCoords.Y - 2 == oldCoords.Y)
                    {
                        moveValidationData.MovedTwoTiles = true;
                        moveValidationData.Coords.Add(newCoords);
                        tileDict.CoordsPawnMovedTwoTiles = null;
                    }

                    // don't allow to move forward more than 2 tiles, 
                    if (newCoords.X == oldCoords.X && newCoords.Y - 2 > oldCoords.Y) return moveValidationData;
                    // don't allow to jump over another piece:
                    if (newCoords.Y == oldCoords.Y + 2
                        && tileDict[IntsToCoordsString(oldCoords.X, oldCoords.Y + 1)].IsOccupied) return moveValidationData;
                }
                // don't allow to move forward more than 1 tile in any following move:
                if (oldCoords.Y != 2 && newCoords.X == oldCoords.X && newCoords.Y - 1 > oldCoords.Y) return moveValidationData;
                // only allow to capture an ememie's piece, if it's 1 diagonal tile away:
                else if (newCoordsColor != ChessPieceColor.Empty)
                {
                    if (newCoords.Y > oldCoords.Y + 1) return moveValidationData;
                    if (oldCoords.X == newCoords.X) return moveValidationData;
                    else if (newCoords.X < oldCoords.X - 1 || newCoords.X > oldCoords.X + 1) return moveValidationData;
                }
                // don't allow to move other than vertically:
                else if (oldCoords.X != newCoords.X) return moveValidationData;
            }
            // validate black pawns:
            else
            {
                // don't allow to move backwards:
                if (oldCoords.Y < newCoords.Y) return moveValidationData;
                // if it's the pawn's first move:
                if (oldCoords.Y == 7)
                {
                    if (newCoords.Y + 2 == oldCoords.Y)
                    {
                        moveValidationData.MovedTwoTiles = true;
                        moveValidationData.Coords.Add(newCoords);
                        tileDict.CoordsPawnMovedTwoTiles = null;
                    }

                    // don't allow to move forward more than 2 tiles, 
                    if (newCoords.X == oldCoords.X && newCoords.Y + 2 < oldCoords.Y) return moveValidationData;
                    // don't allow to jump over another piece:
                    if (newCoords.Y == oldCoords.Y - 2
                        && tileDict[IntsToCoordsString(oldCoords.X, oldCoords.Y - 1)].IsOccupied) return moveValidationData;
                }
                // don't allow to move forward more than 1 tile in any following move:
                if (oldCoords.Y != 7 && newCoords.X == oldCoords.X && newCoords.Y + 1 < oldCoords.Y) return moveValidationData;
                // only allow to capture an ememie's piece, if it's 1 diagonal tile away:
                else if (newCoordsColor != ChessPieceColor.Empty)
                {
                    if (newCoords.Y < oldCoords.Y - 1) return moveValidationData;
                    if (oldCoords.X == newCoords.X) return moveValidationData;
                    else if (newCoords.X < oldCoords.X - 1 || newCoords.X > oldCoords.X + 1) return moveValidationData;
                }
                // don't allow to move other than vertically:
                else if (oldCoords.X != newCoords.X) return moveValidationData;
            }

            moveValidationData.IsValid = true;
            return moveValidationData;
        }
        internal static bool ValidateRookAndQueenHorizontal(
            TileDictionary tileDict,
            Coords oldCoords,
            Coords newCoords,
            ChessPieceColor oldCoordsColor,
            ChessPieceColor newCoordsColor)
        {
            // don't allow to capture same color:
            if (tileDict[newCoords.String].IsOccupied && oldCoordsColor == newCoordsColor) return false;

            // don't allow to move diagonally:
            if (oldCoords.X != newCoords.X && oldCoords.Y != newCoords.Y) return false;

            // check if the path towards top is free:
            if (newCoords.X == oldCoords.X && newCoords.Y > oldCoords.Y)
            {
                for (int i = oldCoords.Y + 1; i < newCoords.Y; i++)
                {
                    if (tileDict[IntsToCoordsString(oldCoords.X, i)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }

            // check if the path towards bottom is free:
            if (newCoords.X == oldCoords.X && newCoords.Y < oldCoords.Y)
            {
                for (int i = oldCoords.Y - 1; i > newCoords.Y; i--)
                {
                    if (tileDict[IntsToCoordsString(oldCoords.X, i)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }

            // check if the path towards right is free:
            if (newCoords.X > oldCoords.X && newCoords.Y == oldCoords.Y)
            {
                for (int i = oldCoords.X + 1; i < newCoords.X; i++)
                {
                    if (tileDict[IntsToCoordsString(i, oldCoords.Y)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }

            // check if the path towards left is free:
            if (newCoords.X < oldCoords.X && newCoords.Y == oldCoords.Y)
            {
                for (int i = oldCoords.X - 1; i > newCoords.X; i--)
                {
                    if (tileDict[IntsToCoordsString(i, oldCoords.Y)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }

            return true;
        }
        internal static bool ValidateBishopAndQueenDiagonal(
            TileDictionary tileDict,
            Coords oldCoords,
            Coords newCoords,
            ChessPieceColor oldCoordsColor,
            ChessPieceColor newCoordsColor)
        {
            if (oldCoords.X == newCoords.X || oldCoords.Y == newCoords.Y)
            {                
                return false;
            }
            // don't allow to capture same color:
            if (tileDict[newCoords.String].IsOccupied && oldCoordsColor == newCoordsColor) return false;

            // don't allow to move horizontally:
            if (newCoords.X == oldCoords.X || newCoords.Y == oldCoords.Y) return false;

            // only allow to move in a straight line:
            if (Math.Abs(newCoords.X - oldCoords.X) != Math.Abs(newCoords.Y - oldCoords.Y)) return false;

            // check if the path towards top right is free:
            if (newCoords.X > oldCoords.X && newCoords.Y > oldCoords.Y)
            {
                for (int i = oldCoords.X + 1, j = oldCoords.Y + 1; i < newCoords.X && j < newCoords.Y; i++, j++)
                {
                    if (tileDict[IntsToCoordsString(i, j)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }
            // check if the path towards top left is free:
            else if (newCoords.X < oldCoords.X && newCoords.Y > oldCoords.Y)
            {
                for (int i = oldCoords.X - 1, j = oldCoords.Y + 1; i > newCoords.X && j < newCoords.Y; i--, j++)
                {
                    if (tileDict[IntsToCoordsString(i, j)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }
            // check if the path towards bottom right is free:
            else if (newCoords.X > oldCoords.X && newCoords.Y < oldCoords.Y)
            {
                for (int i = oldCoords.X + 1, j = oldCoords.Y - 1; i < newCoords.X && j > newCoords.Y; i++, j--)
                {
                    if (tileDict[IntsToCoordsString(i, j)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }
            // check if the path towards bottom left is free:
            else if (newCoords.X < oldCoords.X && newCoords.Y < oldCoords.Y)
            {
                for (int i = oldCoords.X - 1, j = oldCoords.Y - 1; i > newCoords.X && j > newCoords.Y; i--, j--)
                {
                    if (tileDict[IntsToCoordsString(i, j)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }
            return true;
        }
        internal static bool ValidateKnight(
            TileDictionary tileDict,
            Coords oldCoords,
            Coords newCoords,
            ChessPieceColor oldCoordsColor,
            ChessPieceColor newCoordsColor)
        {
            // don't allow to capture same color:
            if (tileDict[newCoords.String].IsOccupied && oldCoordsColor == newCoordsColor) return false;

            // check each possible move:
            if (
                   !(oldCoords.Y - newCoords.Y == -2 && oldCoords.X - newCoords.X == +1) // -2 +1
                && !(oldCoords.Y - newCoords.Y == -2 && oldCoords.X - newCoords.X == -1) // -2 -1
                && !(oldCoords.Y - newCoords.Y == +2 && oldCoords.X - newCoords.X == +1) // +2 +1
                && !(oldCoords.Y - newCoords.Y == +2 && oldCoords.X - newCoords.X == -1) // +2 -1
                && !(oldCoords.Y - newCoords.Y == -1 && newCoords.X - oldCoords.X == +2) // -1 +2
                && !(oldCoords.Y - newCoords.Y == -1 && newCoords.X - oldCoords.X == -2) // -1 -2
                && !(oldCoords.Y - newCoords.Y == +1 && newCoords.X - oldCoords.X == +2) // +1 +2
                && !(oldCoords.Y - newCoords.Y == +1 && newCoords.X - oldCoords.X == -2) // +1 -2
                )
            {
                return false;
            }

            return true;
        }
        internal static MoveValidationData ValidateKing(
            TileDictionary tileDict,
            Coords oldCoords,
            Coords newCoords,
            ChessPieceColor oldCoordsColor,
            ChessPieceColor newCoordsColor)
        {
            MoveValidationData returnStruct = new MoveValidationData
            {
                IsValid = false
            };

            // don't allow to capture same color:
            if (tileDict[newCoords.String].IsOccupied && oldCoordsColor == newCoordsColor) return returnStruct;

            returnStruct = CastlingValidationGameLogic.CanCastle(tileDict, oldCoords, newCoords);
            if (! returnStruct.CanCastle)
            {
                // don't allow to move farther than 1 tile:
                if (newCoords.X > oldCoords.X + 1
                    || newCoords.X < oldCoords.X - 1
                    || newCoords.Y > oldCoords.Y + 1
                    || newCoords.Y < oldCoords.Y - 1) return returnStruct;
            }

            returnStruct.IsValid = true;
            return returnStruct;
        }
        internal static bool ValidateKingCastling(
            TileDictionary tileDict,
            Coords oldCoords,
            Coords newCoords,
            ChessPieceColor oldCoordsColor,
            ChessPieceColor newCoordsColor)
        {
            // don't allow to capture same color:
            if (tileDict[newCoords.String].IsOccupied && oldCoordsColor == newCoordsColor) return false;
            // don't allow to move farther than 1 tile:
            if (newCoords.X > oldCoords.X + 1
                || newCoords.X < oldCoords.X - 1
                || newCoords.Y > oldCoords.Y + 1
                || newCoords.Y < oldCoords.Y - 1) return false;

            return true;
        }
    }
}
