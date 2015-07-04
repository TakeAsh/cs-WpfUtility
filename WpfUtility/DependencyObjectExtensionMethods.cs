using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfUtility {

    public static class DependencyObjectExtensionMethods {

        /// <summary>
        /// Attempts to find and return an object that has the specified name. The search starts from the specified object and continues into subnodes of the logical tree.
        /// </summary>
        /// <typeparam name="T">The type of the object to find.</typeparam>
        /// <param name="node">The object to start searching from. This object must be either a FrameworkElement or a FrameworkContentElement.</param>
        /// <param name="name">The name of the object to find.</param>
        /// <returns>The object with the matching name, if one is found; returns null if no matching name was found in the logical tree.</returns>
        /// <remarks>
        /// [LogicalTreeHelper.FindLogicalNode Method (System.Windows)](https://msdn.microsoft.com/en-us/library/system.windows.logicaltreehelper.findlogicalnode.aspx)
        /// </remarks>
        public static T FindChild<T>(this DependencyObject node, string name)
            where T : DependencyObject {

            return node != null ?
                LogicalTreeHelper.FindLogicalNode(node, name) as T :
                null;
        }
    }
}
