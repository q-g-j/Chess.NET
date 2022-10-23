using System;
using System.Windows;


namespace ChessDotNET.CustomTypes

{
    internal class Coords
    {
        public Coords(int x, int y)
        {
            X = x;
            Y = y; 
            String = Enum.GetName(typeof(Columns), x).ToString() + y.ToString();
        }
        public Coords(Columns x, int y)
        {
            X = (int)x;
            Y = y;
            String = Enum.GetName(typeof(Columns), x).ToString() + y.ToString();
        }

        public int X { get; }
        public int Y { get; }
        public string String { get; }

        internal static Coords StringToCoords(string coordsString)
        {
            return new Coords(int.Parse(coordsString[0].ToString()), int.Parse(coordsString[1].ToString()));
        }

        internal static string ColToString(int row)
        {
            return Enum.GetName(typeof(Columns), row).ToString();
        }

        internal static string IntsToCoordsString(int col, int row)
        {
            return Enum.GetName(typeof(Columns), col).ToString() + row.ToString();
        }
        internal static (string, string) InvertCoords(string oldCoordsString, string newCoordsString)
        {
            if      (oldCoordsString[0] == 'A') oldCoordsString = 'H' + oldCoordsString[1].ToString();
            else if (oldCoordsString[0] == 'B') oldCoordsString = 'G' + oldCoordsString[1].ToString();
            else if (oldCoordsString[0] == 'C') oldCoordsString = 'F' + oldCoordsString[1].ToString();
            else if (oldCoordsString[0] == 'D') oldCoordsString = 'E' + oldCoordsString[1].ToString();
            else if (oldCoordsString[0] == 'E') oldCoordsString = 'D' + oldCoordsString[1].ToString();
            else if (oldCoordsString[0] == 'F') oldCoordsString = 'C' + oldCoordsString[1].ToString();
            else if (oldCoordsString[0] == 'G') oldCoordsString = 'B' + oldCoordsString[1].ToString();
            else if (oldCoordsString[0] == 'H') oldCoordsString = 'A' + oldCoordsString[1].ToString();

            if      (newCoordsString[0] == 'A') newCoordsString = 'H' + newCoordsString[1].ToString();
            else if (newCoordsString[0] == 'B') newCoordsString = 'G' + newCoordsString[1].ToString();
            else if (newCoordsString[0] == 'C') newCoordsString = 'F' + newCoordsString[1].ToString();
            else if (newCoordsString[0] == 'D') newCoordsString = 'E' + newCoordsString[1].ToString();
            else if (newCoordsString[0] == 'E') newCoordsString = 'D' + newCoordsString[1].ToString();
            else if (newCoordsString[0] == 'F') newCoordsString = 'C' + newCoordsString[1].ToString();
            else if (newCoordsString[0] == 'G') newCoordsString = 'B' + newCoordsString[1].ToString();
            else if (newCoordsString[0] == 'H') newCoordsString = 'A' + newCoordsString[1].ToString();

            if      (oldCoordsString[1] == '8') oldCoordsString = oldCoordsString[0].ToString() + '1';
            else if (oldCoordsString[1] == '7') oldCoordsString = oldCoordsString[0].ToString() + '2';
            else if (oldCoordsString[1] == '6') oldCoordsString = oldCoordsString[0].ToString() + '3';
            else if (oldCoordsString[1] == '5') oldCoordsString = oldCoordsString[0].ToString() + '4';
            else if (oldCoordsString[1] == '4') oldCoordsString = oldCoordsString[0].ToString() + '5';
            else if (oldCoordsString[1] == '3') oldCoordsString = oldCoordsString[0].ToString() + '6';
            else if (oldCoordsString[1] == '2') oldCoordsString = oldCoordsString[0].ToString() + '7';
            else if (oldCoordsString[1] == '1') oldCoordsString = oldCoordsString[0].ToString() + '8';

            if      (newCoordsString[1] == '8') newCoordsString = newCoordsString[0].ToString() + '1';
            else if (newCoordsString[1] == '7') newCoordsString = newCoordsString[0].ToString() + '2';
            else if (newCoordsString[1] == '6') newCoordsString = newCoordsString[0].ToString() + '3';
            else if (newCoordsString[1] == '5') newCoordsString = newCoordsString[0].ToString() + '4';
            else if (newCoordsString[1] == '4') newCoordsString = newCoordsString[0].ToString() + '5';
            else if (newCoordsString[1] == '3') newCoordsString = newCoordsString[0].ToString() + '6';
            else if (newCoordsString[1] == '2') newCoordsString = newCoordsString[0].ToString() + '7';
            else if (newCoordsString[1] == '1') newCoordsString = newCoordsString[0].ToString() + '8';

            return (oldCoordsString, newCoordsString);
        }
        internal static Coords CanvasPositionToCoords(Point point)
        {
            int col = (int)((point.X - point.X % 50) / 50) + 1;
            int row = (int)((point.Y - point.Y % 50) / 50) + 1;

            if (row == 1) row = 8;
            else if (row == 2) row = 7;
            else if (row == 3) row = 6;
            else if (row == 4) row = 5;
            else if (row == 5) row = 4;
            else if (row == 6) row = 3;
            else if (row == 7) row = 2;
            else if (row == 8) row = 1;

            return new Coords(col, row);
        }
    }

    internal enum Columns
    {
        A = 1, B = 2, C = 3, D = 4, E = 5, F = 6, G = 7, H = 8
    }
}
