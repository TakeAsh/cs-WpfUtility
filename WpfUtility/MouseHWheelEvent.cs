using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using TakeAshUtility;
using WpfUtility.Native;

namespace WpfUtility {

    public interface IMouseHWheelEvent {
        event MouseWheelEventHandler MouseHWheel;
    }

    public static class IMouseHWheelEventExtensionMethods {

        const string MouseHWheelEventHandlerName = "MouseHWheel";
        const int WM_MOUSEHWHEEL = 0x020E;

        public static void AddMouseHWheelHook<TWindow>(
            this TWindow window,
            MouseWheelEventHandler mouseWheelEventHandler = null
        ) where TWindow : Window, IMouseHWheelEvent {

            if (window == null) {
                return;
            }
            window.Loaded += (sender, e) => {
                var source = HwndSource.FromHwnd(new WindowInteropHelper(window).Handle);
                source.AddHook((IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) => {
                    switch (msg) {
                        case WM_MOUSEHWHEEL:
                            var handlers = window.GetDelegate(MouseHWheelEventHandlerName)
                                .GetHandlers<MouseWheelEventHandler>();
                            if (handlers != null) {
                                var delta = wParam.GetHighWord();
                                var args = new MouseWheelEventArgs(Mouse.PrimaryDevice, Environment.TickCount, delta);
                                handlers.ForEach(handler => handler(window, args));
                            }
                            break;
                    }
                    return IntPtr.Zero;
                });
            };
            if (mouseWheelEventHandler != null) {
                window.MouseHWheel += mouseWheelEventHandler;
            }
        }
    }
}
