using System;
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
                Source = new Uri(@"/ChessDotNET;component/GUI/ResourceDictionaries/ChessPieceImages.xaml", UriKind.RelativeOrAbsolute)
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

            WhitePawnRotated    = RotateImage(WhitePawn);
            WhiteRookRotated    = RotateImage(WhiteRook);
            WhiteKnightRotated  = RotateImage(WhiteKnight);
            WhiteBishopRotated  = RotateImage(WhiteBishop);
            WhiteQueenRotated   = RotateImage(WhiteQueen);
            WhiteKingRotated    = RotateImage(WhiteKing);

            BlackPawnRotated    = RotateImage(BlackPawn);
            BlackRookRotated    = RotateImage(BlackRook);
            BlackKnightRotated  = RotateImage(BlackKnight);
            BlackBishopRotated  = RotateImage(BlackBishop);
            BlackQueenRotated   = RotateImage(BlackQueen);
            BlackKingRotated    = RotateImage(BlackKing);
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

        public static ImageSource WhitePawnRotated;
        public static ImageSource WhiteRookRotated;
        public static ImageSource WhiteKnightRotated;
        public static ImageSource WhiteBishopRotated;
        public static ImageSource WhiteQueenRotated;
        public static ImageSource WhiteKingRotated;

        public static ImageSource BlackPawnRotated;
        public static ImageSource BlackRookRotated;
        public static ImageSource BlackKnightRotated;
        public static ImageSource BlackBishopRotated;
        public static ImageSource BlackQueenRotated;
        public static ImageSource BlackKingRotated;

        internal static bool Equals(ImageSource image1, ImageSource image2)
        {
            return image1.ToString() == image2.ToString();
        }
        internal static bool IsEmpty(ImageSource image)
        {
            return image.ToString() == Empty.ToString();
        }
        internal static ChessPieceColor GetImageColor(ImageSource image)
        {
            if (image.ToString().Contains("white")) return ChessPieceColor.White;
            else if (image.ToString().Contains("black")) return ChessPieceColor.Black;
            else return ChessPieceColor.Empty;
        }
        private static ImageSource RotateImage(ImageSource image)
        {
            var originalImage = image as BitmapImage;
            var rotatedImage = new BitmapImage();
            rotatedImage.BeginInit();
            rotatedImage.UriSource = originalImage.UriSource;
            rotatedImage.Rotation = Rotation.Rotate180;
            rotatedImage.EndInit();
            return rotatedImage;
        }
    }
}
