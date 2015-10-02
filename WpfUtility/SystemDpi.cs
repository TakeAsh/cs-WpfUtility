using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace WpfUtility {

    /// <summary>
    /// System DPI
    /// </summary>
    /// <remarks>
    /// [How can I get the DPI in WPF? - Stack Overflow](http://stackoverflow.com/questions/1918877/)
    /// </remarks>
    public static class SystemDpi {

        static SystemDpi() {
            Dpi = (int)typeof(SystemParameters)
                .GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null, null);
            DpiX = (int)typeof(SystemParameters)
                .GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static)
                .GetValue(null, null);
        }

        public static int Dpi { get; private set; }

        public static int DpiX { get; private set; }
    }
}
