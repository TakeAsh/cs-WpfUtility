using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfUtility {

    public static class GridHelper {

        public static readonly GridLength AutoGridLength = new GridLength(1, GridUnitType.Auto);

        public static ColumnDefinition AutoWidthColumnDefinition {
            get { return new ColumnDefinition() { Width = AutoGridLength, }; }
        }

        public static RowDefinition AutoHeightRowDefinition {
            get { return new RowDefinition() { Height = AutoGridLength, }; }
        }

        public static GridLength ToGridLength(this double pixels) {
            return new GridLength(pixels);
        }

        public static GridLength ToGridLength(this double value, GridUnitType type) {
            return new GridLength(value, type);
        }
    }
}
