using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using TakeAshUtility;

namespace WpfUtility {

    public static class GeometryExtensionMethods {

        public static GeometryCollection ToGeometryCollection(this IEnumerable<Geometry> source) {
            if (source.SafeCount() == 0) {
                return null;
            }
            return new GeometryCollection(source);
        }

        public static GeometryGroup ToGeometryGroup(this GeometryCollection source) {
            if (source.SafeCount() == 0) {
                return null;
            }
            return new GeometryGroup() {
                Children = source,
            };
        }

        public static GeometryGroup ToGeometryGroup(this IEnumerable<Geometry> source) {
            if (source.SafeCount() == 0) {
                return null;
            }
            return new GeometryGroup() {
                Children = new GeometryCollection(source),
            };
        }
    }
}
