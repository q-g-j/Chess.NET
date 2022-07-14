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
                    this[coords.ToString()] = new Tile(col, row, false, new ChessPiece(ChessPieceImages.Empty, ChessPieceColor.Empty, ChessPieceType.Empty));
                }
            }
        }
        internal void PlaceChessPiece(Coords coords, ChessPieceColor color, ChessPieceType type, bool doRotate)
        {
            ImageSource image = ChessPieceImages.Empty;
            if (color == ChessPieceColor.White)
            {
                if (type == ChessPieceType.Pawn) image = doRotate ? ChessPieceImages.WhitePawnRotated : ChessPieceImages.WhitePawn;
                else if (type == ChessPieceType.Rook) image = doRotate ? ChessPieceImages.WhiteRookRotated : ChessPieceImages.WhiteRook;
                else if (type == ChessPieceType.Knight) image = doRotate ? ChessPieceImages.WhiteKnightRotated : ChessPieceImages.WhiteKnight;
                else if (type == ChessPieceType.Bishop) image = doRotate ? ChessPieceImages.WhiteBishopRotated : ChessPieceImages.WhiteBishop;
                else if (type == ChessPieceType.Queen) image = doRotate ? ChessPieceImages.WhiteQueenRotated : ChessPieceImages.WhiteQueen;
                else if (type == ChessPieceType.King) image = doRotate ? ChessPieceImages.WhiteKingRotated : ChessPieceImages.WhiteKing;
            }
            else
            {
                if (type == ChessPieceType.Pawn) image = doRotate ? ChessPieceImages.BlackPawnRotated : ChessPieceImages.BlackPawn;
                else if (type == ChessPieceType.Rook) image = doRotate ? ChessPieceImages.BlackRookRotated : ChessPieceImages.BlackRook;
                else if (type == ChessPieceType.Knight) image = doRotate ? ChessPieceImages.BlackKnightRotated : ChessPieceImages.BlackKnight;
                else if (type == ChessPieceType.Bishop) image = doRotate ? ChessPieceImages.BlackBishopRotated : ChessPieceImages.BlackBishop;
                else if (type == ChessPieceType.Queen) image = doRotate ? ChessPieceImages.BlackQueenRotated : ChessPieceImages.BlackQueen;
                else if (type == ChessPieceType.King) image = doRotate ? ChessPieceImages.BlackKingRotated : ChessPieceImages.BlackKing;
            }

            this[coords.ToString()].SetChessPiece(image);
        }
    }
}
