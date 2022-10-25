﻿using System.Collections.Generic;
using System.Windows.Media;


namespace ChessDotNET.CustomTypes
{
    internal class Tile
    {
        public Tile(int col, int row, bool occupied, ChessPiece chessPiece)
        {
            X = col;
            Y = row;
            IsOccupied = occupied;
            ChessPiece = chessPiece;
        }
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsOccupied { get; set; }
        public ChessPiece ChessPiece { get; set; }
    }
}
