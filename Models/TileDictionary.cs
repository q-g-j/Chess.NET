using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessDotNET.Models
{
    internal class TileDictionary : Dictionary<string, Tile>
    {
        #region Constructors
        public TileDictionary()
        {
            for (int col = 1; col < 9; col++)
            {
                for (int row = 8; row > 0; row--)
                {
                    Coords coords = new Coords(col, row);
                    this[coords.String] = new Tile(col, row, false, new ChessPiece(ChessPieceColor.Empty, ChessPieceType.Empty, false));
                }
            }

            WhiteKingCoords = new Coords(Columns.E, 1);
            BlackKingCoords = new Coords(Columns.E, 8);
        }
        #endregion Constructors

        #region Properties
        internal Coords CoordsPawnMovedTwoTiles { get; set; }
        internal Coords WhiteKingCoords { get; set; }
        internal Coords BlackKingCoords { get; set; }
        #endregion Properties

        #region Methods
        internal void MoveChessPiece(Coords oldCoords, Coords newCoords, bool doChangeCounter)
        {
            this[newCoords.String].ChessPiece = this[oldCoords.String].ChessPiece;
            this[oldCoords.String].ChessPiece = new ChessPiece();


            if (doChangeCounter)
            {
                this[oldCoords.String].IsOccupied = false;
                this[newCoords.String].IsOccupied = true;

                this[newCoords.String].ChessPiece.MoveCount++;
                this[newCoords.String].ChessPiece.HasMoved = true;
            }

            if (this[newCoords.String].ChessPiece.ChessPieceType == ChessPieceType.King)
            {
                if (this[newCoords.String].ChessPiece.ChessPieceColor == ChessPieceColor.White)
                {
                    WhiteKingCoords = newCoords;
                }
                else
                {
                    BlackKingCoords = newCoords;
                }
            }
        }
        #endregion Methods
    }
}
