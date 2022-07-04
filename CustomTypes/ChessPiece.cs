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
        public ChessPiece(ImageSource chessPieceImage, ChessPieceColor chessPieceColor, ChessPieceType chessPieceType)
        {
            ChessPieceImage = chessPieceImage;
            ChessPieceColor = chessPieceColor;
            ChessPieceType = chessPieceType;
        }

        public ImageSource ChessPieceImage { get; set; }
        public ChessPieceColor ChessPieceColor { get; set; }
        public ChessPieceType ChessPieceType { get; set; }
    }
    public enum ChessPieceColor
    {
        White,
        Black,
        Empty
    }
    public enum ChessPieceType
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
