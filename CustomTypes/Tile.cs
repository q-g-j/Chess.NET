using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ChessDotNET.CustomTypes
{
    internal class Tile
    {
        public Tile(int col, int row, bool occupied, ChessPiece chessPiece)
        {
            Col = col;
            Row = row;
            IsOccupied = occupied;
            ChessPiece = chessPiece;
        }

        public int Col { get; set; }
        public int Row { get; set; }
        public bool IsOccupied { get; set; }
        public ChessPiece ChessPiece { get; set; }
    }
}
