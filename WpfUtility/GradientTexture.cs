using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xaml;

namespace WpfUtility {

    public static class GradientTexture {

        public const int DefaultSize = 8;
        private const double DefaultDpi = 96;

        public static BitmapSource Square(Color c0, Color c1, Color c2, Color c3, int size = DefaultSize) {
            if (size <= 0) {
                return null;
            }
            var format = PixelFormats.Bgra32;
            var bytesPerPixel = format.ToBytesPerPixel();
            var rawStride = size * bytesPerPixel;
            var rawImage = new byte[rawStride * size];
            for (var v = 0; v < size; ++v) {
                var ca = c0.Blend(c1, (double)v / (size - 1));
                var cb = c2.Blend(c3, (double)v / (size - 1));
                for (var u = 0; u < size; ++u) {
                    var c = ca.Blend(cb, (double)u / (size - 1));
                    SetColor(rawImage, u * bytesPerPixel + v * rawStride, c);
                }
            }
            return BitmapSource.Create(size, size, DefaultDpi, DefaultDpi, format, null, rawImage, rawStride);
        }

        public static ImageBrush SquareBrush(Color c0, Color c1, Color c2, Color c3, int size = DefaultSize) {
            return new ImageBrush(Square(c0, c1, c2, c3, size));
        }

        private static void SetColor(byte[] image, int index, Color color) {
            image[index + 0] = color.B;
            image[index + 1] = color.G;
            image[index + 2] = color.R;
            image[index + 3] = color.A;
        }
    }
}
