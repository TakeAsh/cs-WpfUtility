using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WpfUtility {

    public static class Point3DHelper {

        public static Func<int, Point3D> MakeInterpolater(Point3D p0, Point3D p1, int div) {
            if (div < 2) {
                return null;
            }
            var div2 = div - 1;
            var delta = new Point3D(
                (p1.X - p0.X) / div2,
                (p1.Y - p0.Y) / div2,
                (p1.Z - p0.Z) / div2
            );
            return (t) => new Point3D(
                p0.X + delta.X * t,
                p0.Y + delta.Y * t,
                p0.Z + delta.Z * t
            );
        }

        public static Point3D Sum(this IEnumerable<Point3D> source) {
            if (source == null || source.Count() == 0) {
                return default(Point3D);
            }
            return source.Aggregate(
                new Point3D(),
                (current, point) => {
                    current.X += point.X;
                    current.Y += point.Y;
                    current.Z += point.Z;
                    return current;
                }
            );
        }

        public static Point3D Average(this IEnumerable<Point3D> source) {
            var count = 0;
            if (source == null || (count = source.Count()) == 0) {
                return default(Point3D);
            }
            var sum = source.Sum();
            return new Point3D(sum.X / count, sum.Y / count, sum.Z / count);
        }
    }
}
