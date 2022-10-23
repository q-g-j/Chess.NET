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
    }
}
