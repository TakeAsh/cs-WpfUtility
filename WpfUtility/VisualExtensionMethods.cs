using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfUtility {

    public static class VisualExtensionMethods {

        public const double DefaultDpi = 96;

        /// <summary>
        /// Get an Image control from a Visual.
        /// </summary>
        /// <param name="target">Visual.</param>
        /// <param name="dpi">dpi for returning image.</param>
        /// <param name="dpiY">dpiY for returning image. If omitted, dpiY is same to dpi.</param>
        /// <returns>Image control created from the visual.</returns>
        public static Image GetImage(
            this Visual target,
            double dpi = DefaultDpi,
            double dpiY = 0
        ) {
            if (target == null) {
                return null;
            }
            return new Image() {
                Source = target.GetBitmap(dpi, dpiY),
            };
        }

        /// <summary>
        /// Get a BitmapSource from a Visual.
        /// </summary>
        /// <param name="target">Visual.</param>
        /// <param name="dpi">dpi for returning image.</param>
        /// <param name="dpiY">dpiY for returning image. If omitted, dpiY is same to dpi.</param>
        /// <returns>Bitmap image created from the visual.</returns>
        /// <remarks>
        /// [wpf - Get a bitmap image from a Control view - Stack Overflow](http://stackoverflow.com/questions/2522380/)
        /// </remarks>
        public static BitmapSource GetBitmap(
            this Visual target,
            double dpi = DefaultDpi,
            double dpiY = 0
        ) {
            if (target == null) {
                return null;
            }
            if (dpiY == 0) {
                dpiY = dpi;
            }
            var bounds = VisualTreeHelper.GetDescendantBounds(target);
            var rtb = new RenderTargetBitmap(
                (int)(bounds.Width * dpi / DefaultDpi),
                (int)(bounds.Height * dpiY / DefaultDpi),
                dpi,
                dpiY,
                PixelFormats.Pbgra32
            );
            var dv = new DrawingVisual();
            using (var ctx = dv.RenderOpen()) {
                ctx.DrawRectangle(
                    new VisualBrush(target),
                    null,
                    new Rect(new Point(), bounds.Size)
                );
            }
            rtb.Render(dv);
            return rtb;
        }
    }
}
