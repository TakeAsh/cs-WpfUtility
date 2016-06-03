using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using TakeAshUtility;

namespace WpfUtility {

    public static class FrameworkElementExtensionMethods {

        public static T SafeTryFindResource<T>(
            this FrameworkElement element,
            object key,
            T defaultValue = default(T)
        ) {
            return element == null ?
                defaultValue :
                element.TryFindResource(key)
                    .SafeToObject(defaultValue);
        }
    }
}
