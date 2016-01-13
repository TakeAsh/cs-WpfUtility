using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using TakeAshUtility;

namespace WpfUtility {

    public static class OctaHedronExtensionMethods {

        private static readonly List<int> _octahedronIndices = new List<int>(){
            0, 1, 2,
            0, 2, 3,
            0, 3, 4,
            0, 4, 1,
            5, 2, 1,
            5, 1, 4,
            5, 4, 3,
            5, 3, 2,
        };

        public static GeometryModel3D ToOctaHedron(
            this Point3D point,
            Material material,
            double radius = 1,
            Transform3D transform = null
        ) {
            return new GeometryModel3D() {
                Geometry = new MeshGeometry3D() {
                    Positions = new Point3DCollection(new[] {
                        new Point3D(point.X + radius, point.Y, point.Z),
                        new Point3D(point.X, point.Y + radius, point.Z),
                        new Point3D(point.X, point.Y, point.Z + radius),
                        new Point3D(point.X, point.Y - radius, point.Z),
                        new Point3D(point.X, point.Y, point.Z - radius),
                        new Point3D(point.X - radius, point.Y, point.Z),
                    }),
                    TriangleIndices = new Int32Collection(_octahedronIndices),
                },
                Material = material,
                Transform = transform,
            };
        }

        public static Model3DGroup ToOctaHedrons(
            this List<Point3D> points,
            List<Material> materials,
            double radius = 1,
            Transform3D transform = null
        ) {
            if (points == null || materials == null) {
                return null;
            }
            return new Model3DGroup() {
                Children = new Model3DCollection(
                    points.Select((point, index) => point.ToOctaHedron(materials[index % materials.Count], radius))
                ),
                Transform = transform,
            };
        }

        public static GeometryModel3D ToOctaHedrons(
            this List<Point3D> points,
            Material material,
            double radius = 1,
            Transform3D transform = null
        ) {
            if (points == null) {
                return null;
            }
            var points2 = new Point3DCollection();
            var indicies2 = new Int32Collection();
            points.ForEach((point, index) => {
                new[] {
                    new Point3D(point.X + radius, point.Y, point.Z),
                    new Point3D(point.X, point.Y + radius, point.Z),
                    new Point3D(point.X, point.Y, point.Z + radius),
                    new Point3D(point.X, point.Y - radius, point.Z),
                    new Point3D(point.X, point.Y, point.Z - radius),
                    new Point3D(point.X - radius, point.Y, point.Z),
                }.ForEach(point2 => points2.Add(point2));
                _octahedronIndices.ForEach(pos => indicies2.Add(pos + index * 6));
            });
            return new GeometryModel3D() {
                Geometry = new MeshGeometry3D() {
                    Positions = points2,
                    TriangleIndices = indicies2,
                },
                Material = material,
                Transform = transform,
            };
        }
    }
}
