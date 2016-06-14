using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using Microsoft.Windows.Controls.Ribbon;

namespace WpfUtility {

    public static class ICommandSourceExtensionMethods {

        /// <summary>
        /// Reset CommandSource manually
        /// </summary>
        /// <param name="commandSource">The CommandSource to be reset</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item>.Net framework 4.6.1 has a bug that a RibbonGallery is not reset and grayed out.</item>
        /// <item>[wpf - RibbonGallery disabled in .net 4.6 - Stack Overflow](http://stackoverflow.com/questions/34306045/)</item>
        /// </list>
        /// </remarks>
        public static void Reset_461_Bug(this ICommandSource commandSource) {
            PropertyInfo pi;
            if (commandSource == null ||
                (pi = commandSource.GetType().GetProperty("Command")) == null ||
                pi.GetSetMethod() == null) {
                return;
            }
            pi.SetValue(commandSource, ApplicationCommands.Print, null);
            pi.SetValue(commandSource, null, null);
        }
    }
}
