using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Media3D;

namespace WpfUtility {

    public static class Model3DExtensionMethods {

        public static object GetTag(this Model3D model) {
            return model.GetValue(FrameworkElement.TagProperty);
        }

        public static void SetTag(this Model3D model, object value) {
            model.SetValue(FrameworkElement.TagProperty, value);
        }

        public static Model3DGroup ToModel3DGroup(
            this IEnumerable<Model3D> source,
            Transform3D transform = null
        ) {
            if (source == null || source.Count() == 0) {
                return null;
            }
            return new Model3DGroup() {
                Children = new Model3DCollection(source),
                Transform = transform,
            };
        }

        public static ModelVisual3D ToModelVisual3D(
            this Model3D model,
            Transform3D transform = null
        ) {
            if (model == null) {
                return null;
            }
            return new ModelVisual3D() {
                Content = model,
                Transform = transform,
            };
        }

        public static ModelVisual3D ToModelVisual3D(
            this IEnumerable<Model3D> source,
            Transform3D transform = null
        ) {
            if (source == null || source.Count() == 0) {
                return null;
            }
            return source.ToModel3DGroup()
                .ToModelVisual3D(transform);
        }

        /// <summary>
        /// Change BackMaterial automatically when Material is changed.
        /// </summary>
        /// <param name="model">Model to be DoubleSidenized</param>
        public static void DoubleSidenize(this GeometryModel3D model) {
            model.AddPropertyChanged(GeometryModel3D.MaterialProperty, OnMaterialChanged);
            model.BackMaterial = model.Material;
        }

        private static void OnMaterialChanged(object sender, EventArgs e) {
            var model = sender as GeometryModel3D;
            if (model == null) {
                return;
            }
            model.BackMaterial = model.Material;
        }

    }
}
