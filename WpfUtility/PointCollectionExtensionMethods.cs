using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace WpfUtility {

    public static class PointCollectionExtensionMethods {

        public static PathFigure ToPathFigure(this PointCollection points, bool close = false) {
            if (points == null || points.Count < 2) {
                return null;
            }
            PointCollection workPoints;
            if (close) {
                workPoints = points.Clone();
                workPoints.Add(workPoints.First());
            } else {
                workPoints = points;
            }
            return new PathFigure() {
                StartPoint = workPoints.First(),
                Segments = { new PolyLineSegment(workPoints.Skip(1), true) },
            };
        }
    }
}
