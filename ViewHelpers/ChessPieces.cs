using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessDotNET.Helpers
{
    internal class ChessPieces
    {
        public ChessPieces()
        {
            mainWindowImages = new ResourceDictionary
            {
                Source = new Uri(@"/ChessDotNET;component/ResourceDictionaries/MainWindowImages.xaml", UriKind.RelativeOrAbsolute)
            };

            Empty       = new BitmapImage();

            WhitePawn   = (ImageSource)mainWindowImages["WhitePawn"];
            WhiteRook   = (ImageSource)mainWindowImages["WhiteRook"];
            WhiteKnight = (ImageSource)mainWindowImages["WhiteKnight"];
            WhiteBishop = (ImageSource)mainWindowImages["WhiteBishop"];
            WhiteQueen  = (ImageSource)mainWindowImages["WhiteQueen"];
            WhiteKing   = (ImageSource)mainWindowImages["WhiteKing"];

            BlackPawn   = (ImageSource)mainWindowImages["BlackPawn"];
            BlackRook   = (ImageSource)mainWindowImages["BlackRook"];
            BlackKnight = (ImageSource)mainWindowImages["BlackKnight"];
            BlackBishop = (ImageSource)mainWindowImages["BlackBishop"];
            BlackQueen  = (ImageSource)mainWindowImages["BlackQueen"];
            BlackKing   = (ImageSource)mainWindowImages["BlackKing"];
        }

        private readonly ResourceDictionary mainWindowImages;

        public ImageSource Empty;

        public ImageSource WhitePawn;
        public ImageSource WhiteRook;
        public ImageSource WhiteKnight;
        public ImageSource WhiteBishop;
        public ImageSource WhiteQueen;
        public ImageSource WhiteKing;

        public ImageSource BlackPawn;
        public ImageSource BlackRook;
        public ImageSource BlackKnight;
        public ImageSource BlackBishop;
        public ImageSource BlackQueen;
        public ImageSource BlackKing;

    }
}
