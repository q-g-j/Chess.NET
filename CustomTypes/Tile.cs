using System.Collections.Generic;
using System.Windows.Media;


namespace ChessDotNET.CustomTypes
{
    internal class Tile
    {
        public Tile(int col, int row, bool occupied, ChessPiece chessPiece)
        {
            Col = col;
            Row = row;
            IsOccupied = occupied;
            ChessPiece = chessPiece;
            ThreatenedByTileList = new List<Tile>();
        }
        public int Col { get; set; }
        public int Row { get; set; }
        public bool IsOccupied { get; set; }
        public List<Tile> ThreatenedByTileList { get; set; }
        public ChessPiece ChessPiece { get; set; }

        internal void SetChessPiece(ImageSource image)
        {
            ChessPiece.ChessPieceImage = image;

            if (ChessPieceImages.IsEmpty(image))
            {
                ChessPiece.ChessPieceColor = ChessPieceColor.Empty;
                ChessPiece.ChessPieceType = ChessPieceType.Empty;
                IsOccupied = false;
            }
            else
            {
                string imageFileName = System.IO.Path.GetFileName(ChessPiece.ChessPieceImage.ToString());
                ChessPiece.ChessPieceColor = imageFileName.Contains("white") 
                    ? ChessPiece.ChessPieceColor = ChessPieceColor.White 
                    : ChessPiece.ChessPieceColor = ChessPieceColor.Black;

                if      (imageFileName.Contains("pawn"))    ChessPiece.ChessPieceType = ChessPieceType.Pawn;
                else if (imageFileName.Contains("rook"))    ChessPiece.ChessPieceType = ChessPieceType.Rook;
                else if (imageFileName.Contains("knight"))  ChessPiece.ChessPieceType = ChessPieceType.Knight;
                else if (imageFileName.Contains("bishop"))  ChessPiece.ChessPieceType = ChessPieceType.Bishop;
                else if (imageFileName.Contains("queen"))   ChessPiece.ChessPieceType = ChessPieceType.Queen;
                else if (imageFileName.Contains("king"))    ChessPiece.ChessPieceType = ChessPieceType.King;

                IsOccupied = true;
            }
        }
    }
}
