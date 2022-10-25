using System.Windows.Media;


namespace ChessDotNET.CustomTypes
{
    public class ChessPiece
    {
        internal ChessPiece(ChessPieceColor chessPieceColor, ChessPieceType chessPieceType, bool isRotated)
        {
            if (chessPieceColor == ChessPieceColor.White)
            {
                if (chessPieceType == ChessPieceType.Pawn)
                    ChessPieceImage = isRotated ? ChessPieceImages.WhitePawnRotated : ChessPieceImages.WhitePawn;
                else if (chessPieceType == ChessPieceType.Rook)
                    ChessPieceImage = isRotated ? ChessPieceImages.WhiteRookRotated : ChessPieceImages.WhiteRook;
                else if (chessPieceType == ChessPieceType.Knight)
                    ChessPieceImage = isRotated ? ChessPieceImages.WhiteKnightRotated : ChessPieceImages.WhiteKnight;
                else if (chessPieceType == ChessPieceType.Bishop)
                    ChessPieceImage = isRotated ? ChessPieceImages.WhiteBishopRotated : ChessPieceImages.WhiteBishop;
                else if (chessPieceType == ChessPieceType.Queen)
                    ChessPieceImage = isRotated ? ChessPieceImages.WhiteQueenRotated : ChessPieceImages.WhiteQueen;
                else if (chessPieceType == ChessPieceType.King)
                    ChessPieceImage = isRotated ? ChessPieceImages.WhiteKingRotated : ChessPieceImages.WhiteKing;
            }
            if (chessPieceColor == ChessPieceColor.Black)
            {
                if (chessPieceType == ChessPieceType.Pawn)
                    ChessPieceImage = isRotated ? ChessPieceImages.BlackPawnRotated : ChessPieceImages.BlackPawn;
                else if (chessPieceType == ChessPieceType.Rook)
                    ChessPieceImage = isRotated ? ChessPieceImages.BlackRookRotated : ChessPieceImages.BlackRook;
                else if (chessPieceType == ChessPieceType.Knight)
                    ChessPieceImage = isRotated ? ChessPieceImages.BlackKnightRotated : ChessPieceImages.BlackKnight;
                else if (chessPieceType == ChessPieceType.Bishop)
                    ChessPieceImage = isRotated ? ChessPieceImages.BlackBishopRotated : ChessPieceImages.BlackBishop;
                else if (chessPieceType == ChessPieceType.Queen)
                    ChessPieceImage = isRotated ? ChessPieceImages.BlackQueenRotated : ChessPieceImages.BlackQueen;
                else if (chessPieceType == ChessPieceType.King)
                    ChessPieceImage = isRotated ? ChessPieceImages.BlackKingRotated : ChessPieceImages.BlackKing;
            }
            ChessPieceColor = chessPieceColor;
            ChessPieceType = chessPieceType;
            HasMoved = false;
            CanBeCapturedEnPassant = false;
            MoveCount = 0;
        }

        internal ChessPiece()
        {
            ChessPieceImage = ChessPieceImages.Empty;
            ChessPieceColor = ChessPieceColor.Empty;
            ChessPieceType = ChessPieceType.Empty;
            HasMoved = false;
            CanBeCapturedEnPassant = false;
            MoveCount = 0;
        }

        public ImageSource ChessPieceImage { get; set; }
        internal ChessPieceColor ChessPieceColor { get; set; }
        internal ChessPieceType ChessPieceType { get; set; }
        internal bool HasMoved { get; set; }
        internal bool CanBeCapturedEnPassant { get; set; }
        internal int MoveCount { get; set; }
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
