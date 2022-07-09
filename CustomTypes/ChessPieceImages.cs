using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChessDotNET.CustomTypes

{
    internal class ChessPieceImages
    {
        static ChessPieceImages()
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

        private static readonly ResourceDictionary mainWindowImages;

        public static ImageSource Empty;
        public static ImageSource WhitePawn;
        public static ImageSource WhiteRook;
        public static ImageSource WhiteKnight;
        public static ImageSource WhiteBishop;
        public static ImageSource WhiteQueen;
        public static ImageSource WhiteKing;
        public static ImageSource BlackPawn;
        public static ImageSource BlackRook;
        public static ImageSource BlackKnight;
        public static ImageSource BlackBishop;
        public static ImageSource BlackQueen;
        public static ImageSource BlackKing;

        internal static bool Equals(ImageSource image1, ImageSource image2)
        {
            return image1.ToString() == image2.ToString();
        }
        internal static bool IsEmpty(ImageSource image)
        {
            return image.ToString() == ChessPieceImages.Empty.ToString();
        }
        internal static ChessPieceColor GetImageColor(ImageSource image)
        {
            if (image.ToString().Contains("white")) return ChessPieceColor.White;
            else if (image.ToString().Contains("black")) return ChessPieceColor.Black;
            else return ChessPieceColor.Empty;
        }
    }
}
