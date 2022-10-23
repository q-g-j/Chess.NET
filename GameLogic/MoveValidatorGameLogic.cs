using System;
using System.Collections.Generic;
using ChessDotNET.CustomTypes;
using static ChessDotNET.CustomTypes.Coords;


namespace ChessDotNET.GameLogic
{
    internal static class MoveValidationGameLogic
    {
        public static bool ValidateCurrentMove(TileDictionary tileDict, Coords oldCoords, Coords newCoords)
        {
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
                if (oldCoords.X == newCoords.X || oldCoords.Y == newCoords.Y) return false;
                bool isValidDiagonal = ValidateBishopAndQueenDiagonal(
                    tileDict, oldCoords, newCoords, oldCoordsColor, newCoordsColor);

                return isValidDiagonal;
            }
            // validate queen's move:
            else if (tileDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Queen)
            {
                bool isValidStraight = ValidateRookAndQueenHorizontal(
                    tileDict, oldCoords, newCoords, oldCoordsColor, newCoordsColor);
                bool isValidDiagonal = ValidateBishopAndQueenDiagonal(
                    tileDict, oldCoords, newCoords, oldCoordsColor, newCoordsColor);

                return isValidStraight || isValidDiagonal;
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
                bool isValidStraight = ValidateRookAndQueenHorizontal(
                    tileDict, oldCoords, newCoords, oldCoordsColor, newCoordsColor);

                return isValidStraight;
            }
            // validate knight's move:
            else if (tileDict[oldCoords.String].ChessPiece.ChessPieceType == ChessPieceType.Knight)
            {
                return ValidateKnight(
                    tileDict, oldCoords, newCoords, oldCoordsColor, newCoordsColor);
            }
            else
            {
                return true;
            }
        }
        private static bool ValidateRookAndQueenHorizontal(
            Dictionary<string, Tile> tileDict,
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
        private static bool ValidateBishopAndQueenDiagonal(
            Dictionary<string, Tile> tileDict,
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
        private static bool ValidatePawn(
            Dictionary<string, Tile> tileDict,
            Coords oldCoords,
            Coords newCoords,
            ChessPieceColor oldCoordsColor,
            ChessPieceColor newCoordsColor)
        {
            if (oldCoordsColor == ChessPieceColor.White)
            {
                // don't allow to move backwards:
                if (oldCoords.Y > newCoords.Y) return false;
                // don't allow to move along the same row:
                if (oldCoords.Y == newCoords.Y) return false;
                // if it's the pawn's first move:
                if (oldCoords.Y == 2)
                {
                    // don't allow to move forward more than 2 tiles, 
                    if (newCoords.X == oldCoords.X && newCoords.Y - 2 > oldCoords.Y) return false;
                    // don't allow to jump over another piece:
                    if (newCoords.Y == oldCoords.Y + 2 && tileDict[IntsToCoordsString(oldCoords.X, oldCoords.Y + 1)].IsOccupied) return false;
                }
                // don't allow to move forward more than 1 tile in any following move:
                if (oldCoords.Y != 2 && newCoords.X == oldCoords.X && newCoords.Y - 1 > oldCoords.Y) return false;
                // dont't allow to capture a piece of the same color:
                if (oldCoordsColor == newCoordsColor) return false;
                // only allow to capture an ememie's piece, if it's 1 diagonal tile away:
                else if (newCoordsColor != ChessPieceColor.Empty)
                {
                    if (newCoords.Y > oldCoords.Y + 1) return false;
                    if (oldCoords.X == newCoords.X) return false;
                    else if (newCoords.X < oldCoords.X - 1 || newCoords.X > oldCoords.X + 1) return false;
                }
                // don't allow to move other than vertical:
                else if (oldCoords.X != newCoords.X) return false;
            }
            else
            {
                // don't allow to move backwards:
                if (oldCoords.Y < newCoords.Y) return false;
                // don't allow to move along the same row:
                if (oldCoords.Y == newCoords.Y) return false;
                // if it's the pawn's first move:
                if (oldCoords.Y == 7)
                {
                    // don't allow to move forward more than 2 tiles, 
                    if (newCoords.X == oldCoords.X && newCoords.Y + 2 < oldCoords.Y) return false;
                    // don't allow to jump over another piece:
                    if (newCoords.Y == oldCoords.Y - 2 && tileDict[IntsToCoordsString(oldCoords.X, oldCoords.Y - 1)].IsOccupied) return false;
                }
                // don't allow to move forward more than 1 tile in any following move:
                if (oldCoords.Y != 7 && newCoords.X == oldCoords.X && newCoords.Y + 1 < oldCoords.Y) return false;
                // dont't allow to capture a piece of the same color:
                if (oldCoordsColor == newCoordsColor) return false;
                // only allow to capture an ememie's piece, if it's 1 diagonal tile away:
                else if (newCoordsColor != ChessPieceColor.Empty)
                {
                    if (newCoords.Y < oldCoords.Y - 1) return false;
                    if (oldCoords.X == newCoords.X) return false;
                    else if (newCoords.X < oldCoords.X - 1 || newCoords.X > oldCoords.X + 1) return false;
                }
                // don't allow to move other than vertical:
                else if (oldCoords.X != newCoords.X) return false;
            }

            return true;
        }
        private static bool ValidateKing(
            Dictionary<string, Tile> tileDict,
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
        private static bool ValidateKnight(
            Dictionary<string, Tile> tileDict,
            Coords oldCoords,
            Coords newCoords,
            ChessPieceColor oldCoordsColor,
            ChessPieceColor newCoordsColor)
        {
            // don't allow to capture same color:
            if (tileDict[newCoords.String].IsOccupied && oldCoordsColor == newCoordsColor) return false;

            // check each possible move:
            if (
                   ! (oldCoords.Y - newCoords.Y == -2 && oldCoords.X - newCoords.X == +1) // -2 +1
                && ! (oldCoords.Y - newCoords.Y == -2 && oldCoords.X - newCoords.X == -1) // -2 -1
                && ! (oldCoords.Y - newCoords.Y == +2 && oldCoords.X - newCoords.X == +1) // +2 +1
                && ! (oldCoords.Y - newCoords.Y == +2 && oldCoords.X - newCoords.X == -1) // +2 -1
                && ! (oldCoords.Y - newCoords.Y == -1 && newCoords.X - oldCoords.X == +2) // -1 +2
                && ! (oldCoords.Y - newCoords.Y == -1 && newCoords.X - oldCoords.X == -2) // -1 -2
                && ! (oldCoords.Y - newCoords.Y == +1 && newCoords.X - oldCoords.X == +2) // +1 +2
                && ! (oldCoords.Y - newCoords.Y == +1 && newCoords.X - oldCoords.X == -2) // +1 -2
                )
            {
                return false;
            }

            return true;
        }
    }
}
