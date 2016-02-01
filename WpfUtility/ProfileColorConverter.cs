using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TakeAshUtility;

namespace WpfUtility {

    public static class ProfileColorConverter {

        public static readonly string ProfilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.System),
            @"spool/drivers/color"
        );

        public const string DefaultCmykProfileName = "RSWOP.icm";
        public const string DefaultRgbProfileName = "sRGB Color Space Profile.icm";

        public static int ToBytesPerPixel(this PixelFormat pixelFormat) {
            return (pixelFormat.BitsPerPixel + 7) / 8;
        }

        public static ColorContext ToColorContext(this string profileName) {
            return new ColorContext(new Uri(Path.Combine(ProfilePath, profileName), UriKind.Absolute));
        }

        public static IEnumerable<byte> CmykToBytes(this IEnumerable<double> cmyk) {
            return cmyk == null ?
                null :
                cmyk.Select(value => (byte)(value * 255 / 100));
        }

        public static IEnumerable<Color> CmykToColors(
            this IEnumerable<IEnumerable<byte>> enumerableBytes,
            string fromProfileName = null,
            string toProfileName = null
        ) {
            if (enumerableBytes == null) {
                return null;
            }
            fromProfileName = String.IsNullOrEmpty(fromProfileName) ?
                DefaultCmykProfileName :
                fromProfileName;
            toProfileName = String.IsNullOrEmpty(toProfileName) ?
                DefaultRgbProfileName :
                toProfileName;
            return enumerableBytes.ToBuffer(PixelFormats.Cmyk32)
                .Convert(
                    PixelFormats.Cmyk32,
                    fromProfileName,
                    PixelFormats.Rgb24,
                    toProfileName
                ).ToEnumerableBytes(PixelFormats.Rgb24)
                .ToColors(PixelFormats.Rgb24);
        }

        public static byte[] ToBuffer(this IEnumerable<IEnumerable<byte>> enumerableBytes, PixelFormat format) {
            if (enumerableBytes == null) {
                return null;
            }
            var bytesPerPixel = format.ToBytesPerPixel();
            var index = 0;
            return enumerableBytes.Aggregate(
                new byte[enumerableBytes.Count() * bytesPerPixel],
                (current, bytes) => {
                    bytes.Take(bytesPerPixel)
                        .ForEach((value, i) => current[index + i] = value);
                    index += bytesPerPixel;
                    return current;
                }
            );
        }

        public static byte[] Convert(
            this byte[] buffer,
            PixelFormat fromFormat,
            string fromProfileName,
            PixelFormat toFormat,
            string toProfileName
        ) {
            if (buffer == null ||
                String.IsNullOrEmpty(fromProfileName) ||
                !File.Exists(Path.Combine(ProfilePath, fromProfileName)) ||
                String.IsNullOrEmpty(toProfileName) ||
                !File.Exists(Path.Combine(ProfilePath, toProfileName))) {
                return null;
            }
            var fromBytesPerPixel = fromFormat.ToBytesPerPixel();
            var count = buffer.Length / fromBytesPerPixel;
            var fromBitmap = BitmapSource.Create(count, 1, 96, 96, fromFormat, null, buffer, buffer.Length);
            var fromProfile = fromProfileName.ToColorContext();
            var toProfile = toProfileName.ToColorContext();
            var toBitmap = new ColorConvertedBitmap(fromBitmap, fromProfile, toProfile, toFormat);
            var toBytesPerPixel = toFormat.ToBytesPerPixel();
            var toBuffer = new byte[toBitmap.PixelWidth * toBitmap.PixelHeight * toBytesPerPixel];
            toBitmap.CopyPixels(toBuffer, toBitmap.PixelWidth * toBytesPerPixel, 0);
            return toBuffer;
        }

        public static IEnumerable<IEnumerable<byte>> ToEnumerableBytes(this byte[] buffer, PixelFormat format) {
            if (buffer == null) {
                return null;
            }
            var bytesPerPixel = format.ToBytesPerPixel();
            var count = buffer.Length / bytesPerPixel;
            var index = -bytesPerPixel;
            return Enumerable.Range(0, count)
                .Select(i => {
                    index += bytesPerPixel;
                    return Enumerable.Range(0, bytesPerPixel)
                        .Select(j => buffer[index + j]);
                });
        }

        public static IEnumerable<Color> ToColors(this IEnumerable<IEnumerable<byte>> enumerableBytes, PixelFormat format) {
            if (enumerableBytes == null) {
                return null;
            }
            var toColor =
                format == PixelFormats.Rgb24 ? (rgb) => Color.FromRgb(rgb[0], rgb[1], rgb[2]) :
                format == PixelFormats.Bgr24 ? (bgr) => Color.FromRgb(bgr[2], bgr[1], bgr[0]) :
                format == PixelFormats.Bgra32 ? (bgra) => Color.FromArgb(bgra[3], bgra[2], bgra[1], bgra[0]) :
                format == PixelFormats.Pbgra32 ? (bgra) => Color.FromArgb(bgra[3], bgra[2], bgra[1], bgra[0]) :
                    (Func<byte[], Color>)((bytes) => default(Color));
            return enumerableBytes.Select(bytes => bytes.ToArray())
                .Select(bytes => toColor(bytes));
        }
    }
}
