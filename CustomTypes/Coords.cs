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
        }
        public Coords(Columns col, int y)
        {
            X = (int)col;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
        public string String
        {
            get
            {
                return Enum.GetName(typeof(Columns), X).ToString() + Y.ToString();
            }
        }

        internal static string IntsToCoordsString(int col, int row)
        {
            return Enum.GetName(typeof(Columns), col).ToString() + row.ToString();
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

        public override string ToString()
        {
            return String;
        }
    }

    internal enum Columns
    {
        A = 1, B = 2, C = 3, D = 4, E = 5, F = 6, G = 7, H = 8
    }
}
