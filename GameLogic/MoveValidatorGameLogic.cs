using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ChessDotNET.CustomTypes;
using static ChessDotNET.CustomTypes.Coords;

namespace ChessDotNET.GameLogic
{
    internal static class MoveValidatorGameLogic
    {
        public static bool ValidateCurrentMove(Dictionary<string, Tile> tileDict, string bottomColor, Coords oldCoords, Coords newCoords)
        {
            // validate pawn's move:
            if (tileDict[oldCoords.ToString()].ChessPiece.ChessPieceType == ChessPieceType.Pawn)
            {
                bool isBottom = false;
                if (bottomColor == "white") isBottom = tileDict[oldCoords.ToString()].ChessPiece.ChessPieceColor == ChessPieceColor.White;
                if (bottomColor == "black") isBottom = tileDict[oldCoords.ToString()].ChessPiece.ChessPieceColor == ChessPieceColor.Black;

                return ValidatePawn(tileDict, oldCoords, newCoords, isBottom);
            }
            // validate bishop's move:
            else if (tileDict[oldCoords.ToString()].ChessPiece.ChessPieceType == ChessPieceType.Bishop)
            {
                if (oldCoords.Col == newCoords.Col || oldCoords.Row == newCoords.Row) return false;
                bool isValidDiagonal = ValidateBishopAndQueenDiagonal(tileDict, oldCoords, newCoords);
                return isValidDiagonal;
            }
            // validate queen's move:
            else if (tileDict[oldCoords.ToString()].ChessPiece.ChessPieceType == ChessPieceType.Queen)
            {
                bool isValidStraight = ValidateRookAndKingStraight(tileDict, oldCoords, newCoords);
                bool isValidDiagonal = ValidateBishopAndQueenDiagonal(tileDict, oldCoords, newCoords);
                return isValidStraight && isValidDiagonal;
            }
            // validate rook's move:
            else if (tileDict[oldCoords.ToString()].ChessPiece.ChessPieceType == ChessPieceType.Rook)
            {
                if (oldCoords.Col != newCoords.Col && oldCoords.Row != newCoords.Row) return false;
                bool isValidStraight = ValidateRookAndKingStraight(tileDict, oldCoords, newCoords);
                return isValidStraight;
            }
            // validate knight's move:
            else if (tileDict[oldCoords.ToString()].ChessPiece.ChessPieceType == ChessPieceType.Knight)
            {
                bool isValidStraight = ValdiateKnight(tileDict, oldCoords, newCoords);
                return isValidStraight;
            }
            else
            {
                return true;
            }
        }
        private static bool ValidateRookAndKingStraight(Dictionary<string, Tile> tileDict, Coords oldCoords, Coords newCoords)
        {
            ChessPieceColor oldCoordsColor = tileDict[oldCoords.ToString()].ChessPiece.ChessPieceColor;
            ChessPieceColor newCoordsColor = tileDict[newCoords.ToString()].ChessPiece.ChessPieceColor;

            // don't allow to capture same color:
            if (tileDict[newCoords.ToString()].IsOccupied && oldCoordsColor == newCoordsColor) return false;
            // check if the path towards top is free:
            if (newCoords.Col == oldCoords.Col && newCoords.Row > oldCoords.Row)
            {
                for (int i = oldCoords.Row + 1; i < newCoords.Row; i++)
                {
                    if (tileDict[IntsToCoordsString(oldCoords.Col, i)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }
            // check if the path towards bottom is free:
            if (newCoords.Col == oldCoords.Col && newCoords.Row < oldCoords.Row)
            {
                for (int i = oldCoords.Row - 1; i > newCoords.Row; i--)
                {
                    if (tileDict[IntsToCoordsString(oldCoords.Col, i)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }
            // check if the path towards right is free:
            if (newCoords.Col > oldCoords.Col && newCoords.Row == oldCoords.Row)
            {
                for (int i = oldCoords.Col + 1; i < newCoords.Col; i++)
                {
                    if (tileDict[IntsToCoordsString(i, oldCoords.Row)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }
            // check if the path towards left is free:
            if (newCoords.Col < oldCoords.Col && newCoords.Row == oldCoords.Row)
            {
                for (int i = oldCoords.Col - 1; i > newCoords.Col; i--)
                {
                    if (tileDict[IntsToCoordsString(i, oldCoords.Row)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }

            return true;
        }
        private static bool ValidateBishopAndQueenDiagonal(Dictionary<string, Tile> tileDict, Coords oldCoords, Coords newCoords)
        {
            ChessPieceColor oldCoordsColor = tileDict[oldCoords.ToString()].ChessPiece.ChessPieceColor;
            ChessPieceColor newCoordsColor = tileDict[newCoords.ToString()].ChessPiece.ChessPieceColor;

            // don't allow to capture same color:
            if (tileDict[newCoords.ToString()].IsOccupied && oldCoordsColor == newCoordsColor) return false;

            // check if the path towards top right is free:
            if (newCoords.Col > oldCoords.Col && newCoords.Row > oldCoords.Row)
            {
                // first check if it's a diagnoal move:
                if (newCoords.Col - oldCoords.Col != newCoords.Row - oldCoords.Row) return false;
                for (int i = oldCoords.Col + 1, j = oldCoords.Row + 1; i < newCoords.Col && j < newCoords.Row; i++, j++)
                {
                    if (tileDict[IntsToCoordsString(i, j)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }
            // check if the path towards top left is free:
            else if (newCoords.Col < oldCoords.Col && newCoords.Row > oldCoords.Row)
            {
                // first check if it's a diagnoal move:
                if (oldCoords.Col - newCoords.Col != newCoords.Row - oldCoords.Row) return false;
                for (int i = oldCoords.Col - 1, j = oldCoords.Row + 1; i > newCoords.Col && j < newCoords.Row; i--, j++)
                {
                    if (tileDict[IntsToCoordsString(i, j)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }
            // check if the path towards bottom right is free:
            else if (newCoords.Col > oldCoords.Col && newCoords.Row < oldCoords.Row)
            {
                // first check if it's a diagnoal move:
                if (newCoords.Col - oldCoords.Col != oldCoords.Row - newCoords.Row) return false;
                for (int i = oldCoords.Col + 1, j = oldCoords.Row - 1; i < newCoords.Col && j > newCoords.Row; i++, j--)
                {
                    if (tileDict[IntsToCoordsString(i, j)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }
            // check if the path towards bottom left is free:
            else if (newCoords.Col < oldCoords.Col && newCoords.Row < oldCoords.Row)
            {
                // first check if it's a diagnoal move:
                if (oldCoords.Col - newCoords.Col != oldCoords.Row - newCoords.Row) return false;
                for (int i = oldCoords.Col - 1, j = oldCoords.Row - 1; i > newCoords.Col && j > newCoords.Row; i--, j--)
                {
                    if (tileDict[IntsToCoordsString(i, j)].ChessPiece.ChessPieceColor != ChessPieceColor.Empty) return false;
                }
            }
            return true;
        }
        private static bool ValidatePawn(Dictionary<string, Tile> tileDict, Coords oldCoords, Coords newCoords, bool isBottom)
        {
            ChessPieceColor oldCoordsColor = tileDict[oldCoords.ToString()].ChessPiece.ChessPieceColor;
            ChessPieceColor newCoordsColor = tileDict[newCoords.ToString()].ChessPiece.ChessPieceColor;

            if (isBottom)
            {
                // don't allow to move backwards:
                if (oldCoords.Row > newCoords.Row) return false;
                // don't allow to move along the same row:
                if (oldCoords.Row == newCoords.Row) return false;
                // if it's the pawn's first move:
                if (oldCoords.Row == 2)
                {
                    // don't allow to move forward more than 2 tiles, 
                    if (newCoords.Col == oldCoords.Col && newCoords.Row - 2 > oldCoords.Row) return false;
                    // don't allow to jump over another piece:
                    if (newCoords.Row == oldCoords.Row + 2 && tileDict[IntsToCoordsString(oldCoords.Col, oldCoords.Row + 1)].IsOccupied) return false;
                }
                // don't allow to move forward more than 1 tile in any following move:
                if (oldCoords.Row != 2 && newCoords.Col == oldCoords.Col && newCoords.Row - 1 > oldCoords.Row) return false;
                // dont't allow to capture a piece of the same color:
                if (oldCoordsColor == newCoordsColor) return false;
                // only allow to capture an ememie's piece, if it's 1 diagonal tile away:
                else if (newCoordsColor != ChessPieceColor.Empty)
                {
                    if (newCoords.Row > oldCoords.Row + 1) return false;
                    if (oldCoords.Col == newCoords.Col) return false;
                    else if (newCoords.Col < oldCoords.Col - 1 || newCoords.Col > oldCoords.Col + 1) return false;
                }
                // don't allow to move other than vertical:
                else if (oldCoords.Col != newCoords.Col) return false;
            }
            else
            {
                // don't allow to move backwards:
                if (oldCoords.Row < newCoords.Row) return false;
                // don't allow to move along the same row:
                if (oldCoords.Row == newCoords.Row) return false;
                // if it's the pawn's first move:
                if (oldCoords.Row == 7)
                {
                    // don't allow to move forward more than 2 tiles, 
                    if (newCoords.Col == oldCoords.Col && newCoords.Row + 2 < oldCoords.Row) return false;
                    // don't allow to jump over another piece:
                    if (newCoords.Row == oldCoords.Row - 2 && tileDict[IntsToCoordsString(oldCoords.Col, oldCoords.Row - 1)].IsOccupied) return false;
                }
                // don't allow to move forward more than 1 tile in any following move:
                if (oldCoords.Row != 7 && newCoords.Col == oldCoords.Col && newCoords.Row + 1 < oldCoords.Row) return false;
                // dont't allow to capture a piece of the same color:
                if (oldCoordsColor == newCoordsColor) return false;
                // only allow to capture an ememie's piece, if it's 1 diagonal tile away:
                else if (newCoordsColor != ChessPieceColor.Empty)
                {
                    if (newCoords.Row < oldCoords.Row - 1) return false;
                    if (oldCoords.Col == newCoords.Col) return false;
                    else if (newCoords.Col < oldCoords.Col - 1 || newCoords.Col > oldCoords.Col + 1) return false;
                }
                // don't allow to move other than vertical:
                else if (oldCoords.Col != newCoords.Col) return false;
            }

            return true;
        }
        private static bool ValdiateKnight(Dictionary<string, Tile> tileDict, Coords oldCoords, Coords newCoords)
        {
            ChessPieceColor oldCoordsColor = tileDict[oldCoords.ToString()].ChessPiece.ChessPieceColor;
            ChessPieceColor newCoordsColor = tileDict[newCoords.ToString()].ChessPiece.ChessPieceColor;

            // don't allow to capture same color:
            if (tileDict[newCoords.ToString()].IsOccupied && oldCoordsColor == newCoordsColor) return false;

            if (oldCoords.Row - newCoords.Row == -2 && oldCoords.Col - newCoords.Col == +1) return true; // -2 + 1
            else if (oldCoords.Row - newCoords.Row == -2 && oldCoords.Col - newCoords.Col == -1) return true; // -2 - 1
            else if (oldCoords.Row - newCoords.Row == +2 && oldCoords.Col - newCoords.Col == +1) return true; // +2 + 1
            else if (oldCoords.Row - newCoords.Row == +2 && oldCoords.Col - newCoords.Col == -1) return true; // +2 -1
            else if (oldCoords.Row - newCoords.Row == -1 && newCoords.Col - oldCoords.Col == +2) return true; // -1 +2
            else if (oldCoords.Row - newCoords.Row == -1 && newCoords.Col - oldCoords.Col == -2) return true; // -1 -2
            else if (oldCoords.Row - newCoords.Row == +1 && newCoords.Col - oldCoords.Col == +2) return true; // +1 +2
            else if (oldCoords.Row - newCoords.Row == +1 && newCoords.Col - oldCoords.Col == -2) return true; // +1 -2
            else return false;
        }
    }
}
