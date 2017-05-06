using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TakeAshUtility;

namespace WpfUtility {

    public enum DataGridBoundColumnStyles {
        ElementStyle,
        CellStyle,
        HeaderStyle,
        DragIndicatorStyle,
        EditingElementStyle,
    }

    public static class DataGridBoundColumnExtensionMethods {

        public static Style GetCurrentStyle<T>(
            this DataGridBoundColumn column,
            DataGridBoundColumnStyles style = default(DataGridBoundColumnStyles)
        ) {
            if (column == null) {
                return null;
            }
            var property = DataGridBoundColumn.ElementStyleProperty;
            switch (style) {
                case DataGridBoundColumnStyles.ElementStyle:
                    property = DataGridBoundColumn.ElementStyleProperty;
                    break;
                case DataGridBoundColumnStyles.CellStyle:
                    property = DataGridBoundColumn.CellStyleProperty;
                    break;
                case DataGridBoundColumnStyles.HeaderStyle:
                    property = DataGridBoundColumn.HeaderStyleProperty;
                    break;
                case DataGridBoundColumnStyles.DragIndicatorStyle:
                    property = DataGridBoundColumn.DragIndicatorStyleProperty;
                    break;
                case DataGridBoundColumnStyles.EditingElementStyle:
                    property = DataGridBoundColumn.EditingElementStyleProperty;
                    break;
            }
            var currentStyle = column.GetValue(property) as Style;
            if (currentStyle == null) {
                return new Style(typeof(T));
            }
            return currentStyle.Setters.Aggregate(
                new Style(typeof(T)),
                (current, setter) => {
                    current.Setters.Add(setter);
                    return current;
                }
            );
        }
    }
}
