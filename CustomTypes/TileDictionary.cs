using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ChessDotNET.CustomTypes;
using ChessDotNET.ViewModels;

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
        internal void PlaceChessPiece(Coords coords, ChessPieceColor color, ChessPieceType type)
        {
            ImageSource image = ChessPieceImages.Empty;
            if (color == ChessPieceColor.White)
            {
                if (type == ChessPieceType.Pawn) image = ChessPieceImages.WhitePawn;
                else if (type == ChessPieceType.Rook) image = ChessPieceImages.WhiteRook;
                else if (type == ChessPieceType.Knight) image = ChessPieceImages.WhiteKnight;
                else if (type == ChessPieceType.Bishop) image = ChessPieceImages.WhiteBishop;
                else if (type == ChessPieceType.Queen) image = ChessPieceImages.WhiteQueen;
                else if (type == ChessPieceType.King) image = ChessPieceImages.WhiteKing;
            }
            else
            {
                if (type == ChessPieceType.Pawn) image = ChessPieceImages.BlackPawn;
                else if (type == ChessPieceType.Rook) image = ChessPieceImages.BlackRook;
                else if (type == ChessPieceType.Knight) image = ChessPieceImages.BlackKnight;
                else if (type == ChessPieceType.Bishop) image = ChessPieceImages.BlackBishop;
                else if (type == ChessPieceType.Queen) image = ChessPieceImages.BlackQueen;
                else if (type == ChessPieceType.King) image = ChessPieceImages.BlackKing;
            }
            this[coords.ToString()].ChessPiece = new ChessPiece(image, color, type);
            this[coords.ToString()].IsOccupied = true;
        }
    }
}
