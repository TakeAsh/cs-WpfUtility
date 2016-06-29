using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
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

        public static T SafeGetTag<T>(
            this FrameworkElement element,
            T defaultValue = default(T)
        ) {
            return element == null || element.Tag == null || !(element.Tag is T) ?
                defaultValue :
                (T)element.Tag;
        }

        /// <summary>
        /// Update the layout of the element according to its Width and Height.
        /// </summary>
        /// <param name="element">the FrameworkElement to be updated.</param>
        public static void UpdateLayoutEx(this FrameworkElement element) {
            element.Dispatcher.Invoke(
                DispatcherPriority.Render,
                new Action(() => {
                    if (element == null ||
                        double.IsNaN(element.Width) || double.IsNaN(element.Height) ||
                        double.IsInfinity(element.Width) || double.IsInfinity(element.Height)) {
                        return;
                    }
                    element.Measure(new Size(element.Width, element.Height));
                    element.Arrange(new Rect(0, 0, element.Width, element.Height));
                    element.UpdateLayout();
                })
            );
        }
    }
}
