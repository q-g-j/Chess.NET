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
            images = new ResourceDictionary
            {
                Source = new Uri(@"/ChessDotNET;component/ResourceDictionaries/ChessPieceImages.xaml", UriKind.RelativeOrAbsolute)
            };

            Empty       = new BitmapImage();

            WhitePawn   = (ImageSource)images["WhitePawn"];
            WhiteRook   = (ImageSource)images["WhiteRook"];
            WhiteKnight = (ImageSource)images["WhiteKnight"];
            WhiteBishop = (ImageSource)images["WhiteBishop"];
            WhiteQueen  = (ImageSource)images["WhiteQueen"];
            WhiteKing   = (ImageSource)images["WhiteKing"];

            BlackPawn   = (ImageSource)images["BlackPawn"];
            BlackRook   = (ImageSource)images["BlackRook"];
            BlackKnight = (ImageSource)images["BlackKnight"];
            BlackBishop = (ImageSource)images["BlackBishop"];
            BlackQueen  = (ImageSource)images["BlackQueen"];
            BlackKing   = (ImageSource)images["BlackKing"];
        }

        private static readonly ResourceDictionary images;

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
