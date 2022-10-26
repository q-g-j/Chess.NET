using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessDotNET.CustomTypes
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

            kingsCoords = new Dictionary<string, Coords>();
        }
        #endregion Constructors

        #region Fields
        private Dictionary<string, Coords> kingsCoords;
        private Coords coordsPawnMovedTwoTiles = null;
        #endregion Fields

        #region Properties
        internal Coords CoordsPawnMovedTwoTiles { get => coordsPawnMovedTwoTiles; set { coordsPawnMovedTwoTiles = value; } }
        internal Dictionary<string, Coords> KingsCoords { get => kingsCoords; set { kingsCoords = value; } }
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
                System.Diagnostics.Debug.WriteLine(oldCoords.String + " -> " + newCoords.String);
            }

            if (this[newCoords.String].ChessPiece.ChessPieceType == ChessPieceType.King)
            {
                if (this[newCoords.String].ChessPiece.ChessPieceColor == ChessPieceColor.White)
                {

                    KingsCoords["WhiteKing"] = newCoords;
                }
                else
                {

                    KingsCoords["BlackKing"] = newCoords;
                }
            }
        }
        #endregion Methods
    }
}
