using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using TakeAshUtility;
using WpfUtility.Native;

namespace WpfUtility {

    /// <summary>
    /// WINDOWPLACEMENT Structure wrapper class
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>[WPF でウィンドウ位置とサイズを保存・復元しよう | grabacr.nét](http://grabacr.net/archives/1585)</item>
    /// <item>[Save Window Placement State Sample](https://msdn.microsoft.com/en-us/library/aa972163%28v=vs.90%29.aspx)</item>
    /// <item>[WINDOWPLACEMENT Structure](https://msdn.microsoft.com/en-us/library/kb89946z.aspx)</item>
    /// </list>
    /// </remarks>
    public class WindowPlacement {

        private IntPtr _hWnd;
        private bool _allowMinimized;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="window">Window to bind</param>
        /// <param name="allowMinimized">flag to allow minimized when Set()</param>
        public WindowPlacement(Window window, bool allowMinimized = false) {
            _hWnd = new WindowInteropHelper(window).Handle;
            _allowMinimized = allowMinimized;
        }

        /// <summary>
        /// Base64 converted window placement information.
        /// </summary>
        [ToStringMember]
        public string Placement {
            get {
                WINDOWPLACEMENT placement;
                NativeMethods.GetWindowPlacement(_hWnd, out placement);
                return placement.ToBase64String();
            }
            set {
                if (String.IsNullOrEmpty(value)) {
                    return;
                }
                var placement = Base64Converter.FromBase64String<WINDOWPLACEMENT>(value);
                if (!_allowMinimized && placement.showCmd == SW.SHOWMINIMIZED) {
                    placement.showCmd = SW.SHOWNORMAL;
                }
                NativeMethods.SetWindowPlacement(_hWnd, ref placement);
            }
        }

        public override string ToString() {
            return this.ToStringMembers();
        }
    }
}
