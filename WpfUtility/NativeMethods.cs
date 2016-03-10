using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WpfUtility.Native {

    static class NativeMethods {

        [DllImport("user32.dll")]
        public static extern bool SetWindowPlacement(
            IntPtr hWnd,
            [In] ref WINDOWPLACEMENT lpwndpl
        );

        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(
            IntPtr hWnd,
            out WINDOWPLACEMENT lpwndpl
        );

        /// <summary>
        /// Convert a value to int.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Int value</returns>
        /// <remarks>
        /// [winapi - Win api in C#. Get Hi and low word from IntPtr - Stack Overflow](http://stackoverflow.com/questions/7913325/)
        /// </remarks>
        public static int ToIntUnchecked(this IntPtr value) {
            return IntPtr.Size == 8 ?
                unchecked((int)value.ToInt64()) :
                value.ToInt32();
        }

        /// <summary>
        /// Get low word of a value.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>low word of a value</returns>
        public static int GetLowWord(this IntPtr value) {
            return unchecked((short)value.ToIntUnchecked());
        }

        /// <summary>
        /// Get high word of a value.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>high word of a value</returns>
        public static int GetHighWord(this IntPtr value) {
            return unchecked((short)(((uint)value.ToIntUnchecked()) >> 16));
        }
    }
}
