using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfUtility {

    public static class PointHelper {

        public static Point Sum(this IEnumerable<Point> source) {
            if (source == null || source.Count() == 0) {
                return default(Point);
            }
            return source.Aggregate(
                new Point(),
                (current, point) => {
                    current.X += point.X;
                    current.Y += point.Y;
                    return current;
                }
            );
        }

        public static Point Average(this IEnumerable<Point> source) {
            var count = 0;
            if (source == null || (count = source.Count()) == 0) {
                return default(Point);
            }
            var sum = source.Sum();
            return new Point(sum.X / count, sum.Y / count);
        }
    }
}
