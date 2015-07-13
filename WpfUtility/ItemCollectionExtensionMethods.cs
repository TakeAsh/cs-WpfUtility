using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfUtility {

    public static class ItemCollectionExtensionMethods {

        public static void Add<T>(this ItemCollection items, IEnumerable<UIElement> list)
            where T : UIElement {

            if (items == null || list == null) {
                return;
            }
            list.ToList()
                .ForEach(item => items.Add(item));
        }
    }
}
