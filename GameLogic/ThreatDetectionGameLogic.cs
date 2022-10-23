using System.Collections.Generic;
using ChessDotNET.CustomTypes;


namespace ChessDotNET.GameLogic
{
    internal static class ThreatDetectionGameLogic
    {
        internal static List<Tile> GetThreateningTilesList(TileDictionary tileDict, Coords currentCoords)
        {
            List<Tile> returnList = new List<Tile>();

            foreach (var tile in tileDict.Values)
            {
                if (!(tile.Col == currentCoords.X && tile.Row == currentCoords.Y)
                    && tile.ChessPiece.ChessPieceColor != tileDict[currentCoords.String].ChessPiece.ChessPieceColor
                    && tile.ChessPiece.ChessPieceColor != ChessPieceColor.Empty)
                {
                    if (tile.ChessPiece.ChessPieceType == ChessPieceType.Pawn)
                    {
                        //if (tileDict[currentCoords.String].ChessPiece.ChessPieceColor == bottomColor)
                        {
                            if ((currentCoords.X == tile.Col + 1 || currentCoords.X == tile.Col - 1)
                                && tile.Row - 1 == currentCoords.Y)
                            {
                                returnList.Add(tile);
                            }
                        }
                        //else
                        //{
                        //    if ((currentCoords.X == tile.X + 1 || currentCoords.X == tile.X - 1)
                        //        && tile.Y + 1 == currentCoords.Y)
                        //    {
                        //        returnList.Add(tile);
                        //    }
                        //}
                    }
                }
            }

            return returnList;
        }
    }
}
