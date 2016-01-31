using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WpfUtility {

    public class CrossLines3D :
        ModelVisual3D {

        public const double DefaultThickness = 1;

        private readonly MeshGeometry3D _mesh;
        private readonly GeometryModel3D _model;

        public CrossLines3D() {
            _mesh = new MeshGeometry3D();
            _model = new GeometryModel3D() {
                Geometry = _mesh,
            };
            this.Content = _model;
            SetColor(this.Color);
            CompositionTarget.Rendering += OnRender;
        }

        public CrossLines3D(IEnumerable<Point3D> points, Color color, double thickness = DefaultThickness)
            : this() {

            this.Points = points;
            this.Color = color;
            this.Thickness = thickness;
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color",
            typeof(Color),
            typeof(CrossLines3D),
            new PropertyMetadata(Colors.Transparent, OnColorChanged));

        public Color Color {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        private static void OnColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            ((CrossLines3D)sender).SetColor((Color)args.NewValue);
        }

        private void SetColor(Color color) {
            var material = color.ToMaterial();
            _model.Material = material;
            _model.BackMaterial = material;
        }

        public static readonly DependencyProperty ThicknessProperty = DependencyProperty.Register(
            "Thickness",
            typeof(double),
            typeof(CrossLines3D),
            new PropertyMetadata(DefaultThickness, OnThicknessChanged));

        public double Thickness {
            get { return (double)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }

        private static void OnThicknessChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            ((CrossLines3D)sender).IsGeometryDirty = true;
        }

        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register(
            "Points",
            typeof(IEnumerable<Point3D>),
            typeof(CrossLines3D),
            new PropertyMetadata(null, OnPointsChanged));

        public IEnumerable<Point3D> Points {
            get { return (IEnumerable<Point3D>)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        private static void OnPointsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args) {
            ((CrossLines3D)sender).IsGeometryDirty = true;
        }

        public bool IsGeometryDirty { get; set; }

        private void OnRender(object sender, EventArgs e) {
            if (Points.Count() == 0 && _mesh.Positions.Count == 0 || !IsGeometryDirty) {
                return;
            }
            RebuildGeometry();
        }

        private void RebuildGeometry() {
            var halfThickness = Thickness / 2.0;

            var positions = new Point3DCollection(Points.Count() * 6);
            Points.ToList()
                .ForEach(point => {
                    new List<Point3D>() {
                        new Point3D(point.X + halfThickness, point.Y, point.Z),
                        new Point3D(point.X, point.Y + halfThickness, point.Z),
                        new Point3D(point.X, point.Y, point.Z + halfThickness),
                        new Point3D(point.X - halfThickness, point.Y, point.Z),
                        new Point3D(point.X, point.Y - halfThickness, point.Z),
                        new Point3D(point.X, point.Y, point.Z - halfThickness),
                    }.ForEach(p2 => positions.Add(p2));
                });
            positions.Freeze();
            _mesh.Positions = positions;

            var indices = new Int32Collection((Points.Count() - 1) * 3 * 6);
            Enumerable.Range(1, Points.Count() - 1)
                .ToList()
                .ForEach(i => {
                    var i1 = (i - 1) * 6;
                    var i2 = i * 6;
                    new List<int>() {
                        i1 + 0, i2 + 0, i1 + 3,
                        i2 + 0, i2 + 3, i1 + 3,
                        i1 + 1, i2 + 1, i1 + 4,
                        i2 + 1, i2 + 4, i1 + 4,
                        i1 + 2, i2 + 2, i1 + 5,
                        i2 + 2, i2 + 5, i1 + 5,
                    }.ForEach(index => indices.Add(index));
                });
            indices.Freeze();
            _mesh.TriangleIndices = indices;

            IsGeometryDirty = false;
        }
    }
}
