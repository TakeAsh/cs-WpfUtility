using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TakeAshUtility;

namespace ImageViewer {

    public struct ZoomItem :
        IEquatable<ZoomItem> {

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

        #region IEquatable

        public bool Equals(ZoomItem other) {
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return this.Value == other.Value;
        }

        public override bool Equals(object other) {
            if (other == null || !(other is ZoomItem)) {
                return false;
            }
            return this.Equals((ZoomItem)other);
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }

        public static bool operator ==(ZoomItem x, ZoomItem y) {
            return x.Equals(y);
        }

        public static bool operator !=(ZoomItem x, ZoomItem y) {
            return !x.Equals(y);
        }

        #endregion
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
