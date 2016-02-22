using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using TakeAshUtility;

namespace WpfUtility {

    public static class Surface3D {

        public static GeometryModel3D Create(
            IEnumerable<IEnumerable<Point3D>> points,
            Material material,
            bool doubleSide = false,
            Transform3D transform = null
        ) {
            var uCount = 0;
            var vCount = 0;
            if (points == null ||
                (vCount = points.Count()) < 2 ||
                points.First() == null ||
                (uCount = points.First().Count()) < 2 ||
                !points.Aggregate(
                    true,
                    (current, stride) => current && stride != null && stride.Count() == uCount
                )) {
                return null;
            }
            var numMainPoints = uCount * vCount;
            var numSubPoints = (uCount - 1) * (vCount - 1);
            Func<int, int, int> toPos = (u, v) => u + v * uCount;
            Func<int, int, int> toPosSub = (u, v) => u + v * (uCount - 1) + numMainPoints;
            var textureDeltaU = 1.0 / (uCount - 1);
            var textureDeltaV = 1.0 / (vCount - 1);
            var subTextureDelta = new Point(0.5 / (vCount - 1), 0.5 / (uCount - 1));
            var textureCoordinates = new PointCollection();
            var subTextureCoordinates = new PointCollection();
            var subPositions = new Point3DCollection(numSubPoints);
            var triangleIndices = new Int32Collection();
            var positions = points.Aggregate(
                new Point3DCollection(numMainPoints + numSubPoints),
                (current, stride) => {
                    current.AddRange(stride);
                    return current;
                }
            );
            Enumerable.Range(0, vCount - 1).ForEach(v => {
                var textureV = v * textureDeltaV;
                Enumerable.Range(0, uCount - 1).ForEach(u => {
                    var textureCoordinate = new Point(textureV, u * textureDeltaU);
                    textureCoordinates.Add(textureCoordinate);
                    subTextureCoordinates.Add(new[] { textureCoordinate, subTextureDelta }.Sum());
                    var pos00 = toPos(u, v);
                    var pos01 = toPos(u + 1, v);
                    var pos10 = toPos(u, v + 1);
                    var pos11 = toPos(u + 1, v + 1);
                    var posSub = toPosSub(u, v);
                    subPositions.Add(new[] {
                        positions[pos00],
                        positions[pos01],
                        positions[pos10],
                        positions[pos11],
                    }.Average());
                    triangleIndices.AddRange(new[] {
                        pos00, pos01, posSub,
                        pos00, posSub, pos10,
                        pos01, pos11, posSub,
                        pos10, posSub, pos11,
                    });
                });
                textureCoordinates.Add(new Point(textureV, 1));
            });
            Enumerable.Range(0, uCount).ForEach(u => textureCoordinates.Add(new Point(1, u * textureDeltaU)));
            positions.AddRange(subPositions);
            textureCoordinates.AddRange(subTextureCoordinates);
            var model = new GeometryModel3D() {
                Geometry = new MeshGeometry3D() {
                    Positions = positions,
                    TriangleIndices = triangleIndices,
                    TextureCoordinates = textureCoordinates,
                },
                Material = material,
                Transform = transform,
            };
            if (doubleSide) {
                model.DoubleSidenize();
            }
            return model;
        }
    }
}
