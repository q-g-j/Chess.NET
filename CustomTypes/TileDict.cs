using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using ChessDotNET.CustomTypes;

namespace ChessDotNET.CustomTypes
{
    internal class TileDict : Dictionary<string, Tile>
    {
        public TileDict()
        {
            for (int col = 1; col < 9; col++)
            {
                for (int row = 8; row > 0; row--)
                {
                    Coords coords = new Coords(col, row);
                    this[Coords.CoordsToString(coords)] = new Tile(col, row, false, new ChessPiece(ChessPieceImages.Empty, ChessPieceColor.Empty, ChessPieceType.Empty));
                }
            }
        }
    }
}
