using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfUtility {

    public static class DependencyObjectExtensionMethods {

        public static T FindName<T>(this DependencyObject obj, string name)
            where T : class {

            return LogicalTreeHelper.FindLogicalNode(obj, name) as T;
        }
    }
}
