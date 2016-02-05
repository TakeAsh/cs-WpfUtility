using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Windows.Controls.Ribbon;

namespace WpfUtility {

    public static class RibbonGalleryExtensionMethods {

        /// <summary>
        /// Reset RibbonGallery manually
        /// </summary>
        /// <param name="ribbonGallery">The RibbonGallery to be reset</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item>.Net framework 4.6.1 has a bug that a RibbonGallery is not reset and grayed out.</item>
        /// <item>[wpf - RibbonGallery disabled in .net 4.6 - Stack Overflow](http://stackoverflow.com/questions/34306045/)</item>
        /// </list>
        /// </remarks>
        public static void Reset_461_Bug(this RibbonGallery ribbonGallery) {
            ribbonGallery.Command = ApplicationCommands.Print;
            ribbonGallery.Command = null;
        }
    }
}
