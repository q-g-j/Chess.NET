using System;
using System.Collections.Generic;
using ChessDotNET.CustomTypes;
using static ChessDotNET.CustomTypes.Coords;


namespace ChessDotNET.GameLogic
{
    internal struct MoveValidationStruct
    {
        public bool IsValid { get; set; }
        public bool CanCastle { get; set; }
        public bool CanCaptureEnPassant { get; set; }
        public bool MovedTwoTiles { get; set; }
        public List<Coords> Coords { get; set; }
    }
    internal static class MoveValidationGameLogic
    {
        internal static MoveValidationStruct ValidateCurrentMove(TileDictionary tileDict, Coords oldCoords, Coords newCoords)
        {
            MoveValidationStruct returnStruct = new MoveValidationStruct();

            ChessPieceColor oldCoordsColor = tileDict[oldCoords.String].ChessPiece.ChessPieceColor;
            ChessPieceColor newCoordsColor = tileDict[newCoords.String].ChessPiece.ChessPieceColor;

            // validate pawn's move:
            if (tileDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Pawn)
            {
                return ValidatePawn(
                    tileDict, oldCoords, newCoords, oldCoordsColor, newCoordsColor);
            }
            // validate bishop's move:
            else if (tileDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Bishop)
            {
                if (oldCoords.X == newCoords.X || oldCoords.Y == newCoords.Y)
                {
                    returnStruct.IsValid = true;
                    return returnStruct;
                }
                if (ValidateBishopAndQueenDiagonal(tileDict, oldCoords, newCoords, oldCoordsColor, newCoordsColor))
                {
                    returnStruct.IsValid = true;;
                }
                else
                {
                    returnStruct.IsValid = false;
                }
            }
            // validate queen's move:
            else if (tileDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Queen)
            {
                bool isValidStraight = ValidateRookAndQueenHorizontal(tileDict, oldCoords, newCoords, oldCoordsColor, newCoordsColor);
                bool isValidDiagonal = ValidateBishopAndQueenDiagonal(tileDict, oldCoords, newCoords, oldCoordsColor, newCoordsColor);

                if (isValidStraight || isValidDiagonal)
                {
                    returnStruct.IsValid = true;
                }
                else
                {
                    returnStruct.IsValid = false;
                }
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
                if (ValidateRookAndQueenHorizontal(tileDict, oldCoords, newCoords, oldCoordsColor, newCoordsColor))
                {
                    returnStruct.IsValid = true;
                }
                else
                {
                    returnStruct.IsValid = false;
                }
            }
            // validate knight's move:
            else if (tileDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Knight)
            {
                if (ValidateKnight(tileDict, oldCoords, newCoords, oldCoordsColor, newCoordsColor))
                {
                    returnStruct.IsValid = true;
                }
                else
                {
                    returnStruct.IsValid = false;
                }
            }
            else
            {
                returnStruct.IsValid = true;
            }
            return returnStruct;
        }
        internal static MoveValidationStruct ValidatePawn(
            TileDictionary tileDict,
            Coords oldCoords,
            Coords newCoords,
            ChessPieceColor oldCoordsColor,
            ChessPieceColor newCoordsColor)
        {
            MoveValidationStruct returnStruct = new MoveValidationStruct
            {
                Coords = new List<Coords>(),
                IsValid = false
            };

            // has a pawn move two tiles in the last turn:
            if (tileDict.CoordsPawnMovedTwoTiles != null)
            {
                // validate if it can be captured:
                if (EnPassantValidationGameLogic.CanCaptureEnPassant(tileDict, oldCoords, newCoords))
                {
                    returnStruct.IsValid = true;
                    returnStruct.CanCaptureEnPassant = true;
                    return returnStruct;
                }
            }

            // don't allow to move along the same row:
            if (oldCoords.Y == newCoords.Y) return returnStruct;
            // dont't allow to capture a piece of the same color:
            if (oldCoordsColor == newCoordsColor) return returnStruct;

            if (oldCoordsColor == ChessPieceColor.White)
            {
                // don't allow to move backwards:
                if (oldCoords.Y > newCoords.Y) return returnStruct;
                // if it's the pawn's first move:
                if (oldCoords.Y == 2)
                {
                    if (newCoords.Y - 2 == oldCoords.Y)
                    {
                        returnStruct.MovedTwoTiles = true;
                        returnStruct.Coords.Add(newCoords);
                        tileDict.CoordsPawnMovedTwoTiles = null;
                    }

                    // don't allow to move forward more than 2 tiles, 
                    if (newCoords.X == oldCoords.X && newCoords.Y - 2 > oldCoords.Y) return returnStruct;
                    // don't allow to jump over another piece:
                    if (
                        newCoords.Y == oldCoords.Y + 2
                        && tileDict[IntsToCoordsString(oldCoords.X, oldCoords.Y + 1)].IsOccupied
                        ) return returnStruct;
                }
                // don't allow to move forward more than 1 tile in any following move:
                if (oldCoords.Y != 2 && newCoords.X == oldCoords.X && newCoords.Y - 1 > oldCoords.Y) return returnStruct;
                // only allow to capture an ememie's piece, if it's 1 diagonal tile away:
                else if (newCoordsColor != ChessPieceColor.Empty)
                {
                    if (newCoords.Y > oldCoords.Y + 1) return returnStruct;
                    if (oldCoords.X == newCoords.X) return returnStruct;
                    else if (newCoords.X < oldCoords.X - 1 || newCoords.X > oldCoords.X + 1) return returnStruct;
                }
                // don't allow to move other than vertically:
                else if (oldCoords.X != newCoords.X) return returnStruct;
            }
            else
            {
                // don't allow to move backwards:
                if (oldCoords.Y < newCoords.Y) return returnStruct;
                // if it's the pawn's first move:
                if (oldCoords.Y == 7)
                {
                    if (newCoords.Y + 2 == oldCoords.Y)
                    {
                        returnStruct.MovedTwoTiles = true;
                        returnStruct.Coords.Add(newCoords);
                        tileDict.CoordsPawnMovedTwoTiles = null;
                    }

                    // don't allow to move forward more than 2 tiles, 
                    if (newCoords.X == oldCoords.X && newCoords.Y + 2 < oldCoords.Y) return returnStruct;
                    // don't allow to jump over another piece:
                    if (
                        newCoords.Y == oldCoords.Y - 2
                        && tileDict[IntsToCoordsString(oldCoords.X, oldCoords.Y - 1)].IsOccupied
                        ) return returnStruct;
                }
                // don't allow to move forward more than 1 tile in any following move:
                if (oldCoords.Y != 7 && newCoords.X == oldCoords.X && newCoords.Y + 1 < oldCoords.Y) return returnStruct;
                // only allow to capture an ememie's piece, if it's 1 diagonal tile away:
                else if (newCoordsColor != ChessPieceColor.Empty)
                {
                    if (newCoords.Y < oldCoords.Y - 1) return returnStruct;
                    if (oldCoords.X == newCoords.X) return returnStruct;
                    else if (newCoords.X < oldCoords.X - 1 || newCoords.X > oldCoords.X + 1) return returnStruct;
                }
                // don't allow to move other than vertically:
                else if (oldCoords.X != newCoords.X) return returnStruct;
            }

            returnStruct.IsValid = true;
            return returnStruct;
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
        internal static MoveValidationStruct ValidateKing(
            TileDictionary tileDict,
            Coords oldCoords,
            Coords newCoords,
            ChessPieceColor oldCoordsColor,
            ChessPieceColor newCoordsColor)
        {
            MoveValidationStruct returnStruct = new MoveValidationStruct
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
