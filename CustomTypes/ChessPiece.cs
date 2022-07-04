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

        internal void UpdateColorAndType()
        {
            if (ChessPieceImages.IsEmpty(ChessPieceImage))
            {
                ChessPieceColor = ChessPieceColor.Empty;
                ChessPieceType = ChessPieceType.Empty;
            }
            else
            {
                string imageFileName = System.IO.Path.GetFileName(ChessPieceImage.ToString());
                ChessPieceColor = imageFileName.Contains("white") ? ChessPieceColor = ChessPieceColor.White : ChessPieceColor = ChessPieceColor.Black;

                if      (imageFileName.Contains("pawn")) ChessPieceType = ChessPieceType.Pawn;
                else if (imageFileName.Contains("rook")) ChessPieceType = ChessPieceType.Rook;
                else if (imageFileName.Contains("knight")) ChessPieceType = ChessPieceType.Knight;
                else if (imageFileName.Contains("bishop")) ChessPieceType = ChessPieceType.Bishop;
                else if (imageFileName.Contains("queen")) ChessPieceType = ChessPieceType.Queen;
                else if (imageFileName.Contains("king")) ChessPieceType = ChessPieceType.King;
            }
        }
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
