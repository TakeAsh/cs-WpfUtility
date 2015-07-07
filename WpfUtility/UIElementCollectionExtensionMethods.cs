using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfUtility {

    public static class UIElementCollectionExtensionMethods {

        public static void Add(this UIElementCollection items, IEnumerable<UIElement> list) {
            if (items == null || list == null) {
                return;
            }
            list.ToList()
                .ForEach(item => items.Add(item));
        }
    }
}
