using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TakeAshUtility;

namespace WpfUtility {

    public static class DataGridBoundColumnExtensionMethods {

        public static Style GetCurrentStyle(
            this DataGridBoundColumn column,
            Type type
        ) {
            if (column == null) {
                return null;
            }
            return column.ElementStyle.Setters.Aggregate(
                new Style(type),
                (current, setter) => {
                    current.Setters.Add(setter);
                    return current;
                }
            );
        }
    }
}
