using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDotNET.CustomTypes
{
    internal class Move
    {
        public Move(Coords startCoords, Coords endCoords, ChessPieceColor chessPieceColor, ChessPieceType chessPieceType)
        {
            StartCoords = startCoords;
            EndCoords = endCoords;
            ChessPieceColor = chessPieceColor;
            ChessPieceType = chessPieceType;
        }

        #region Properties
        internal Coords StartCoords { get; set; }
        internal Coords EndCoords { get; set; }
        internal ChessPieceColor ChessPieceColor { get; set; }
        internal ChessPieceType ChessPieceType { get; set; }
        #endregion Properties
    }
}
