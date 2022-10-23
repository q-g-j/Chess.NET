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
                {
                    if (isRotated)
                    {
                        ChessPieceImage = ChessPieceImages.WhitePawnRotated;
                    }
                    else
                    {
                        ChessPieceImage = ChessPieceImages.WhitePawn;
                    }
                }
                else if (chessPieceType == ChessPieceType.Rook)
                {
                    if (isRotated)
                    {
                        ChessPieceImage = ChessPieceImages.WhiteRookRotated;
                    }
                    else
                    {
                        ChessPieceImage = ChessPieceImages.WhiteRook;
                    }
                }
                else if (chessPieceType == ChessPieceType.Knight)
                {
                    if (isRotated)
                    {
                        ChessPieceImage = ChessPieceImages.WhiteKnightRotated;
                    }
                    else
                    {
                        ChessPieceImage = ChessPieceImages.WhiteKnight;
                    }
                }
                else if (chessPieceType == ChessPieceType.Bishop)
                {
                    if (isRotated)
                    {
                        ChessPieceImage = ChessPieceImages.WhiteBishopRotated;
                    }
                    else
                    {
                        ChessPieceImage = ChessPieceImages.WhiteBishop;
                    }
                }
                else if (chessPieceType == ChessPieceType.Queen)
                {
                    if (isRotated)
                    {
                        ChessPieceImage = ChessPieceImages.WhiteQueenRotated;
                    }
                    else
                    {
                        ChessPieceImage = ChessPieceImages.WhiteQueen;
                    }
                }
                else if (chessPieceType == ChessPieceType.King)
                {
                    if (isRotated)
                    {
                        ChessPieceImage = ChessPieceImages.WhiteKingRotated;
                    }
                    else
                    {
                        ChessPieceImage = ChessPieceImages.WhiteKing;
                    }
                }
            }
            if (chessPieceColor == ChessPieceColor.Black)
            {
                if (chessPieceType == ChessPieceType.Pawn)
                {
                    if (isRotated)
                    {
                        ChessPieceImage = ChessPieceImages.BlackPawnRotated;
                    }
                    else
                    {
                        ChessPieceImage = ChessPieceImages.BlackPawn;
                    }
                }
                else if (chessPieceType == ChessPieceType.Rook)
                {
                    if (isRotated)
                    {
                        ChessPieceImage = ChessPieceImages.BlackRookRotated;
                    }
                    else
                    {
                        ChessPieceImage = ChessPieceImages.BlackRook;
                    }
                }
                else if (chessPieceType == ChessPieceType.Knight)
                {
                    if (isRotated)
                    {
                        ChessPieceImage = ChessPieceImages.BlackKnightRotated;
                    }
                    else
                    {
                        ChessPieceImage = ChessPieceImages.BlackKnight;
                    }
                }
                else if (chessPieceType == ChessPieceType.Bishop)
                {
                    if (isRotated)
                    {
                        ChessPieceImage = ChessPieceImages.BlackBishopRotated;
                    }
                    else
                    {
                        ChessPieceImage = ChessPieceImages.BlackBishop;
                    }
                }
                else if (chessPieceType == ChessPieceType.Queen)
                {
                    if (isRotated)
                    {
                        ChessPieceImage = ChessPieceImages.BlackQueenRotated;
                    }
                    else
                    {
                        ChessPieceImage = ChessPieceImages.BlackQueen;
                    }
                }
                else if (chessPieceType == ChessPieceType.King)
                {
                    if (isRotated)
                    {
                        ChessPieceImage = ChessPieceImages.BlackKingRotated;
                    }
                    else
                    {
                        ChessPieceImage = ChessPieceImages.BlackKing;
                    }
                }
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
