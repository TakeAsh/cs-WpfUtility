using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfUtility {

    public static class BitmapSourceExtensionMethods {

        public static int GetStride(this BitmapSource source) {
            if (source == null) {
                return -1;
            }
            return source.PixelWidth * source.Format.ToBytesPerPixel();
        }

        public static byte[] GetPixels(this BitmapSource source) {
            if (source == null) {
                return null;
            }
            var stride = source.GetStride();
            var pixels = new byte[stride * source.PixelHeight];
            source.CopyPixels(pixels, stride, 0);
            return pixels;
        }

        /// <summary>
        /// Remove surrounding color that equals to top left from an image
        /// </summary>
        /// <param name="source">BitmapSource to be cropped.</param>
        /// <returns>Cropped BitmapSource. If there is no surrounding, returns the source.</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item>[c# - Remove surrounding whitespace from an image - Stack Overflow](http://stackoverflow.com/questions/248141/)</item>
        /// <item>[c# - Automatically trim a bitmap to minimum size? - Stack Overflow](http://stackoverflow.com/questions/4820212/)</item>
        /// </list>
        /// </remarks>
        public static BitmapSource Crop(this BitmapSource source) {
            if (source == null) {
                return null;
            }
            var width = source.PixelWidth;
            var height = source.PixelHeight;
            var bytesPerPixel = source.Format.ToBytesPerPixel();
            var stride = width * bytesPerPixel;
            var pixels = new byte[stride * height];
            source.CopyPixels(pixels, stride, 0);
            Func<int, bool> equalsToTopLeft = (position) => {
                for (var i = 0; i < bytesPerPixel; ++i) {
                    if (pixels[position + i] != pixels[i]) {
                        return false;
                    }
                }
                return true;
            };
            Func<int, bool> equalsHorizontal = (y) => {
                var position = y * stride;
                for (var x = 0; x < width; ++x) {
                    if (!equalsToTopLeft(position)) {
                        return false;
                    }
                    position += bytesPerPixel;
                }
                return true;
            };
            var top = 0;
            while (top < height && equalsHorizontal(top)) {
                ++top;
            }
            var bottom = height - 1;
            while (bottom > top && equalsHorizontal(bottom)) {
                --bottom;
            }
            Func<int, bool> equalsVertical = (x) => {
                var position = x * bytesPerPixel + top * stride;
                for (var y = top; y <= bottom; ++y) {
                    if (!equalsToTopLeft(position)) {
                        return false;
                    }
                    position += stride;
                }
                return true;
            };
            var left = 0;
            while (left < width && equalsVertical(left)) {
                ++left;
            }
            var right = width - 1;
            while (right > left && equalsVertical(right)) {
                --right;
            }
            if (top == 0 &&
                left == 0 &&
                bottom == height - 1 &&
                right == width - 1) {
                return source;
            }
            var croppedWidth = right - left + 1;
            var croppedHeight = bottom - top + 1;
            var croppedStride = croppedWidth * bytesPerPixel;
            var croppedPexels = new byte[croppedStride * croppedHeight];
            source.CopyPixels(
                new Int32Rect(left, top, croppedWidth, croppedHeight),
                croppedPexels,
                croppedStride,
                0
            );
            return BitmapSource.Create(
                croppedWidth,
                croppedHeight,
                source.DpiX,
                source.DpiY,
                source.Format,
                null,
                croppedPexels,
                croppedStride
            );
        }
    }
}
