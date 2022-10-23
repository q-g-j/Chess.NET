using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessDotNET.CustomTypes
{
    internal class TileDictionary : Dictionary<string, Tile>
    {
        public TileDictionary()
        {
            for (int col = 1; col < 9; col++)
            {
                for (int row = 8; row > 0; row--)
                {
                    Coords coords = new Coords(col, row);
                    this[coords.String] = new Tile(col, row, false, new ChessPiece(ChessPieceColor.Empty, ChessPieceType.Empty, false));
                }
            }
        }
        internal void MoveChessPiece(Coords oldCoords, Coords newCoords)
        {
            this[newCoords.String].ChessPiece = this[oldCoords.String].ChessPiece;
            this[oldCoords.String].ChessPiece = new ChessPiece();
            this[oldCoords.String].IsOccupied = false;
            this[newCoords.String].IsOccupied = true;
            this[newCoords.String].ChessPiece.MoveCount++;
            this[newCoords.String].ChessPiece.HasMoved = true;
        }
    }
}
