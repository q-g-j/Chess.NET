using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessDotNET.CustomTypes;

namespace ChessDotNET.GameLogic
{
    internal static class ThreatDetectionGameLogic
    {
        internal static List<Tile> GetThreateningTilesList(Dictionary<string, Tile> tileDict, Coords currentCoords, ChessPieceColor bottomColor)
        {
            List<Tile> returnList = new List<Tile>();

            foreach (var tile in tileDict.Values)
            {
                if (!(tile.Col == currentCoords.Col && tile.Row == currentCoords.Row)
                    && tile.ChessPiece.ChessPieceColor != tileDict[currentCoords.ToString()].ChessPiece.ChessPieceColor
                    && tile.ChessPiece.ChessPieceColor != ChessPieceColor.Empty)
                {
                    if (tile.ChessPiece.ChessPieceType == ChessPieceType.Pawn)
                    {
                        if (tileDict[currentCoords.ToString()].ChessPiece.ChessPieceColor == bottomColor)
                        {
                            if ((currentCoords.Col == tile.Col + 1 || currentCoords.Col == tile.Col - 1)
                                && tile.Row - 1 == currentCoords.Row)
                            {
                                returnList.Add(tile);
                            }
                        }
                        else
                        {
                            if ((currentCoords.Col == tile.Col + 1 || currentCoords.Col == tile.Col - 1)
                                && tile.Row + 1 == currentCoords.Row)
                            {
                                returnList.Add(tile);
                            }
                        }
                    }
                }
            }

            return returnList;
        }
    }
}
