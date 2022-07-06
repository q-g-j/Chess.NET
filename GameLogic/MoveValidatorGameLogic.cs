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
    internal class MoveValidatorGameLogic
    {
        public MoveValidatorGameLogic()
        {
        }

        public static bool ValidateCurrentMove(Dictionary<string, Tile> tileDict, Image currentlyMovedChessPiece, string bottomColor, Coords oldCoords, Coords newCoords)
        {
            // validate pawn's move:
            if (ChessPieceImages.Equals(currentlyMovedChessPiece.Source, ChessPieceImages.WhitePawn)
                || ChessPieceImages.Equals(currentlyMovedChessPiece.Source, ChessPieceImages.BlackPawn))
            {
                bool isBottom = false;
                if (bottomColor == "white") isBottom = tileDict[CoordsToString(oldCoords)].ChessPiece.ChessPieceColor == ChessPieceColor.White;
                if (bottomColor == "black") isBottom = tileDict[CoordsToString(oldCoords)].ChessPiece.ChessPieceColor == ChessPieceColor.Black;

                return ValidatePawn(tileDict, oldCoords, newCoords, isBottom);
            }
            else if (ChessPieceImages.Equals(currentlyMovedChessPiece.Source, ChessPieceImages.WhiteBishop)
                || ChessPieceImages.Equals(currentlyMovedChessPiece.Source, ChessPieceImages.BlackBishop)
                || ChessPieceImages.Equals(currentlyMovedChessPiece.Source, ChessPieceImages.WhiteQueen)
                || ChessPieceImages.Equals(currentlyMovedChessPiece.Source, ChessPieceImages.BlackQueen))
            {
                return ValidateBishopAndQueenDiagonal(tileDict, oldCoords, newCoords);
            }
            else
            {
                return true;
            }
        }

        private static bool ValidateBishopAndQueenDiagonal(Dictionary<string, Tile> tileDict, Coords oldCoords, Coords newCoords)
        {
            ChessPieceColor oldCoordsColor = tileDict[CoordsToString(oldCoords)].ChessPiece.ChessPieceColor;
            ChessPieceColor newCoordsColor = tileDict[CoordsToString(newCoords)].ChessPiece.ChessPieceColor;
            string newCoordsString = CoordsToString(newCoords);

            // don't allow to capture same color:
            if (tileDict[newCoordsString].IsOccupied && oldCoordsColor == newCoordsColor) return false;

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
            ChessPieceColor oldCoordsColor = tileDict[CoordsToString(oldCoords)].ChessPiece.ChessPieceColor;
            ChessPieceColor newCoordsColor = tileDict[CoordsToString(newCoords)].ChessPiece.ChessPieceColor;

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
    }
}
