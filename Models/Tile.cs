using System.Collections.Generic;
using System.Windows.Media;


namespace ChessDotNET.Models
{
    internal class Tile
    {
        public Tile(int col, int row, bool occupied, ChessPiece chessPiece)
        {
            Coords = new Coords(col, row);
            IsOccupied = occupied;
            ChessPiece = chessPiece;
        }
        public Coords Coords { get; set; }
        public bool IsOccupied { get; set; }
        public ChessPiece ChessPiece { get; set; }
    }
}
