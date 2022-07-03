using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDotNET.Helpers
{
    internal class Coords
    {
        public Coords(int x, int y)
        {
            Col = x;
            Row = y;
        }

        public int Col { get; }
        public int Row { get; }

        public override string ToString() => $"({Col}, {Row})";

        internal static Coords StringToCoords(string coordsString)
        {
            return new Coords(int.Parse(coordsString[0].ToString()), int.Parse(coordsString[1].ToString()));
        }
    }

    internal struct Rows
    {
        public static readonly int A = 1, B = 2, C = 3, D = 4, E = 5, F = 6, G = 7, H = 8;
    }
}
