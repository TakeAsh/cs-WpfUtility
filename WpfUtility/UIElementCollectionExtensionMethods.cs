using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfUtility {

    public static class UIElementCollectionExtensionMethods {

        public static void SafeAdd(this UIElementCollection items, IEnumerable<UIElement> list) {
            if (items == null || list == null) {
                return;
            }
            list.Where(item => item != null)
                .ToList()
                .ForEach(item => items.Add(item));
        }

        public static void SafeAdd(this UIElementCollection items, UIElement element) {
            if (items == null || element == null) {
                return;
            }
            items.Add(element);
        }
    }
}
