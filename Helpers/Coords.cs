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
}
