using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ChessDotNET.CustomTypes
{
    public class ChessPiece
    {
        internal ChessPiece(ImageSource chessPieceImage, ChessPieceColor chessPieceColor, ChessPieceType chessPieceType)
        {
            ChessPieceImage = chessPieceImage;
            ChessPieceColor = chessPieceColor;
            ChessPieceType = chessPieceType;
        }

        public ImageSource ChessPieceImage { get; set; }
        internal ChessPieceColor ChessPieceColor { get; set; }
        internal ChessPieceType ChessPieceType { get; set; }
    }
    internal enum ChessPieceColor
    {
        White,
        Black,
        Empty
    }
    internal enum ChessPieceType
    {
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King,
        Empty
    }
}
