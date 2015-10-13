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

    public static class TriangleTexture {

        public const int DefaultSize = 8;
        public const int DefaultThickness = 1;
        private const double DefaultDpi = 96;
        private const double C0Weight = 0.67;

        public static BitmapSource Gradient(Color c0, Color c1, Color c2, int size = DefaultSize) {
            if (size <= 0) {
                return null;
            }
            var format = PixelFormats.Bgra32;
            var bytesPerPixel = (format.BitsPerPixel + 7) / 8;
            var rawStride = size * bytesPerPixel;
            var rawImage = new byte[rawStride * size];
            var area = size * size / 2.0;
            for (var v = 0; v < size; ++v) {
                for (var u = 0; u <= size - v; ++u) {
                    var area1 = size * v / 2.0;
                    var area2 = size * u / 2.0;
                    var area0 = area - (area1 + area2);
                    var areaW = area0 * C0Weight + area1 + area2;
                    var r1 = area1 / areaW;
                    var r2 = area2 / areaW;
                    var c = c0.Blend(c1, r1, c2, r2);
                    SetColor(rawImage, u * bytesPerPixel + v * rawStride, c);
                }
            }
            return BitmapSource.Create(size, size, DefaultDpi, DefaultDpi, format, null, rawImage, rawStride);
        }

        public static ImageBrush GradientBrush(Color c0, Color c1, Color c2, int size = DefaultSize) {
            return new ImageBrush(Gradient(c0, c1, c2, size));
        }

        public static BitmapSource Frame(Color stroke, Color fill, int thickness = DefaultThickness, int size = DefaultSize) {
            if (thickness <= 0 || size <= 0) {
                return null;
            }
            var format = PixelFormats.Bgra32;
            var bytesPerPixel = (format.BitsPerPixel + 7) / 8;
            var rawStride = size * bytesPerPixel;
            var rawImage = new byte[rawStride * size];
            for (var v = 0; v < size; ++v) {
                for (var u = 0; u < size - v; ++u) {
                    SetColor(rawImage, u * bytesPerPixel + v * rawStride, fill);
                }
            }
            for (var t = 0; t < thickness; ++t) {
                for (var u = 0; u <= size - t; ++u) {
                    SetColor(rawImage, u * bytesPerPixel + t * rawStride, stroke);
                }
                for (var v = 0; v < size - t; ++v) {
                    SetColor(rawImage, t * bytesPerPixel + v * rawStride, stroke);
                }
                for (var w = 0; w < size - t; ++w) {
                    SetColor(rawImage, (size - t - w) * bytesPerPixel + w * rawStride, stroke);
                }
            }
            var blendedColor = stroke.Blend(fill, 0.5);
            for (var w = thickness; w < size - thickness - 1; ++w) {
                SetColor(rawImage, (size - thickness - w) * bytesPerPixel + w * rawStride, blendedColor);
            }
            return BitmapSource.Create(size, size, DefaultDpi, DefaultDpi, format, null, rawImage, rawStride);
        }

        public static ImageBrush FrameBrush(Color stroke, Color fill, int thickness = DefaultThickness, int size = DefaultSize) {
            return new ImageBrush(Frame(stroke, fill, thickness, size));
        }

        private static void SetColor(byte[] image, int index, Color color) {
            image[index + 0] = color.B;
            image[index + 1] = color.G;
            image[index + 2] = color.R;
            image[index + 3] = color.A;
        }
    }

    [MarkupExtensionReturnType(typeof(BitmapSource))]
    public class TriangleGradientTextureExtension :
        MarkupExtension {

        public const string TriangleGradientTextureSizeKey = "TriangleGradientTexture_Size";

        private Color _c0;
        private Color _c1;
        private Color _c2;
        private int _size = TriangleTexture.DefaultSize;

        public int Size {
            get { return _size; }
            set { _size = value; }
        }

        public TriangleGradientTextureExtension(string c0, string c1, string c2) {
            _c0 = (Color)ColorConverter.ConvertFromString(c0);
            _c1 = (Color)ColorConverter.ConvertFromString(c1);
            _c2 = (Color)ColorConverter.ConvertFromString(c2);
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            if (Size == TriangleTexture.DefaultSize &&
                serviceProvider.GetService<IXamlSchemaContextProvider>() != null) {
                try {
                    var sizeStatic = new StaticResourceExtension(TriangleGradientTextureSizeKey);
                    Size = (int)sizeStatic.ProvideValue(serviceProvider);
                }
                catch {
                    // When StaticResource 'TriangleTexture_Size' is not defined, an exception will be thrown.
                    // Cannot find resource named 'TriangleTexture_Size'. Resource names are case sensitive.
                    // This exception is ignored, and Size is not changed.
                }
            }
            var sizeDynamic = Application.Current.TryFindResource(TriangleGradientTextureSizeKey);
            if (Size == TriangleTexture.DefaultSize &&
                sizeDynamic != null) {
                Size = (int)sizeDynamic;
            }
            return TriangleTexture.Gradient(_c0, _c1, _c2, Size);
        }
    }

    [MarkupExtensionReturnType(typeof(BitmapSource))]
    public class TriangleFrameTextureExtension :
        MarkupExtension {

        public const string TriangleFrameTextureSizeKey = "TriangleFrameTexture_Size";
        public const string TriangleFrameTextureThicknessKey = "TriangleFrameTexture_Thickness";

        private Color _stroke;
        private Color _fill;
        private int _size = TriangleTexture.DefaultSize;
        private int _thickness = TriangleTexture.DefaultThickness;

        public int Size {
            get { return _size; }
            set { _size = value; }
        }

        public int Thickness {
            get { return _thickness; }
            set { _thickness = value; }
        }

        public TriangleFrameTextureExtension(string stroke, string fill) {
            _stroke = (Color)ColorConverter.ConvertFromString(stroke);
            _fill = (Color)ColorConverter.ConvertFromString(fill);
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            if (Size == TriangleTexture.DefaultSize &&
                serviceProvider.GetService<IXamlSchemaContextProvider>() != null) {
                try {
                    var sizeStatic = new StaticResourceExtension(TriangleFrameTextureSizeKey);
                    Size = (int)sizeStatic.ProvideValue(serviceProvider);
                    var thicknessStatic = new StaticResourceExtension(TriangleFrameTextureThicknessKey);
                    Thickness = (int)thicknessStatic.ProvideValue(serviceProvider);
                }
                catch {
                    // When StaticResource 'TriangleTexture_Size' is not defined, an exception will be thrown.
                    // Cannot find resource named 'TriangleTexture_Size'. Resource names are case sensitive.
                    // This exception is ignored, and Size is not changed.
                }
            }
            var sizeDynamic = Application.Current.TryFindResource(TriangleFrameTextureSizeKey);
            if (Size == TriangleTexture.DefaultSize &&
                sizeDynamic != null) {
                Size = (int)sizeDynamic;
            }
            var thicknessDynamic = Application.Current.TryFindResource(TriangleFrameTextureThicknessKey);
            if (Thickness == TriangleTexture.DefaultThickness &&
                thicknessDynamic != null) {
                    Thickness = (int)thicknessDynamic;
            }
            return TriangleTexture.Frame(_stroke, _fill, Thickness, Size);
        }
    }
}
