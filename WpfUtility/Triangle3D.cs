using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using TakeAshUtility;

namespace WpfUtility {

    public static class Triangle3D {

        public static GeometryModel3D Create(
            Point3D angleA, Point3D angleB, Point3D angleC,
            Material material
        ) {
            return new GeometryModel3D() {
                Geometry = new MeshGeometry3D() {
                    Positions = { angleA, angleB, angleC },
                    TriangleIndices = { 0, 1, 2 },
                    TextureCoordinates = { new Point(0, 0), new Point(0, 1), new Point(1, 0) },
                },
                Material = material,
            };
        }
        
        public static GeometryModel3D Create(
            Point3D angleA,
            IEnumerable<Point3D> sideAB,
            IEnumerable<Point3D> sideAC,
            Material material
        ) {
            var positions = new Point3DCollection() { angleA };
            var stepsAB = new List<double>();
            var stepsAC = new List<double>();
            var textureCoordinates = new PointCollection() { new Point(0, 0) };
            sideAB.ForEach((item, index) => {
                positions.Add(item);
                var step = (index + 1.0) / sideAB.Count();
                stepsAB.Add(step);
                textureCoordinates.Add(new Point(0, step));
            });
            sideAC.ForEach((item, index) => {
                positions.Add(item);
                var step = (index + 1.0) / sideAC.Count();
                stepsAC.Add(step);
                textureCoordinates.Add(new Point(step, 0));
            });
            var indicies = new Int32Collection();
            var shiftB = sideAB.Count() + 1;
            var indexAB = 0;
            var indexAC = 0;
            indicies.AddRange(new[] { 0, indexAB + 1, indexAC + shiftB, });
            while (indexAB < sideAB.Count() && indexAC < sideAC.Count()) {
                var points = new List<int>();
                var isIndexAEnd = indexAB + 1 >= sideAB.Count();
                var isIndexBEnd = indexAC + 1 >= sideAC.Count();
                if (isIndexAEnd && isIndexBEnd) {
                    break;
                } else if (isIndexAEnd) {
                    points = new List<int> { indexAB + 1, indexAC + 1 + shiftB, indexAC + shiftB };
                    ++indexAC;
                } else if (isIndexBEnd) {
                    points = new List<int> { indexAB + 1, indexAB + 2, indexAC + shiftB };
                    ++indexAB;
                } else if (stepsAB[indexAB + 1] <= stepsAC[indexAC + 1]) {
                    points = new List<int> { indexAB + 1, indexAB + 2, indexAC + shiftB };
                    ++indexAB;
                } else {
                    points = new List<int> { indexAB + 1, indexAC + 1 + shiftB, indexAC + shiftB };
                    ++indexAC;
                }
                indicies.AddRange(points);
            }
            return new GeometryModel3D() {
                Geometry = new MeshGeometry3D() {
                    Positions = positions,
                    TriangleIndices = indicies,
                    TextureCoordinates = textureCoordinates,
                },
                Material = material,
            };
        }
    }
}
