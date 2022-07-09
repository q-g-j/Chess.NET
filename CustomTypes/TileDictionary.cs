using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using ChessDotNET.CustomTypes;

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
                    this[coords.ToString()] = new Tile(col, row, false, new ChessPiece(ChessPieceImages.Empty, ChessPieceColor.Empty, ChessPieceType.Empty));
                }
            }
        }
    }
}
