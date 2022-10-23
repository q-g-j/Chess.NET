using System.Collections.Generic;
using ChessDotNET.CustomTypes;


namespace ChessDotNET.GameLogic
{
    internal static class ThreatDetectionGameLogic
    {
        internal static List<Tile> GetThreateningTilesList(TileDictionary tileDict, Coords coordsToCheck, ChessPieceColor ownColor)
        {
            List<Tile> returnList = new List<Tile>();

            foreach (var tile in tileDict.Values)
            {
                if (!(tile.Col == coordsToCheck.X && tile.Row == coordsToCheck.Y)
                    && tile.ChessPiece.ChessPieceColor != tileDict[coordsToCheck.String].ChessPiece.ChessPieceColor
                    && tile.ChessPiece.ChessPieceColor != ChessPieceColor.Empty)
                {
                    if (tile.ChessPiece.ChessPieceType == ChessPieceType.Pawn)
                    {
                        //if (tileDict[coordsToCheck.String].ChessPiece.ChessPieceColor == bottomColor)
                        {
                            if ((coordsToCheck.X == tile.Col + 1 || coordsToCheck.X == tile.Col - 1)
                                && tile.Row - 1 == coordsToCheck.Y)
                            {
                                returnList.Add(tile);
                            }
                        }
                        //else
                        //{
                        //    if ((coordsToCheck.X == tile.X + 1 || coordsToCheck.X == tile.X - 1)
                        //        && tile.Y + 1 == coordsToCheck.Y)
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
