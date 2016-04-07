using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TakeAshUtility;

namespace WpfUtility {

    public static class ItemCollectionExtensionMethods {

        public static void SafeAdd<T>(this ItemCollection items, T item)
            where T : UIElement {

            if (items == null || item == null) {
                return;
            }
            items.Add(item);
        }

        public static void AddRange<T>(this ItemCollection items, IEnumerable<UIElement> list)
            where T : UIElement {

            if (items == null || list == null) {
                return;
            }
            list.ForEach(item => items.Add(item));
        }

        public static void SafeAddRange<T>(this ItemCollection items, IEnumerable<UIElement> list)
            where T : UIElement {

            if (items == null || list == null) {
                return;
            }
            list.Where(item => item != null)
                .ForEach(item => items.Add(item));
        }
    }
}
