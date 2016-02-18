using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace WpfUtility {

    public static class BrushExtensionMethods {

        public static Material ToMaterial(this Brush brush) {
            return brush == null ?
                null :
                new DiffuseMaterial(brush);
        }
    }
}
