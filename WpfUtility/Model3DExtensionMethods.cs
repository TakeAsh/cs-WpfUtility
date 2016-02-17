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
    }
}
