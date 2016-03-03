using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TakeAshUtility;

namespace ImageViewer {

    public struct ZoomItem {

        public ZoomItem(double value)
            : this() {
            Value = value;
            Title = value > 0 ?
                value + "%" :
                "Show All";
        }

        public double Value { get; private set; }
        public string Title { get; private set; }

        public override string ToString() {
            return Title;
        }
    }

    public static class ZoomItemExtensionMethods {

        public static List<ZoomItem> ToZoomItems(this string source) {
            if (String.IsNullOrEmpty(source)) {
                return null;
            }
            return source.SplitTrim()
                .Select(value => new ZoomItem(value.TryParse<double>()))
                .ToList();
        }
    }
}
