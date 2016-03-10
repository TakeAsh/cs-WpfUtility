using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using TakeAshUtility;

namespace WpfUtility {

    public interface IResizeEvent {
        event EventHandler Resizing;
        event EventHandler Resized;
    }

    public static class IResizeEventExtensionMethods {

        const string ResizingEventHandlerName = "Resizing";
        const string ResizedEventHandlerName = "Resized";
        const int WM_ENTERSIZEMOVE = 0x0231;
        const int WM_EXITSIZEMOVE = 0x0232;

        /// <summary>
        /// Add Resize hook that fire Resizing/Resized events
        /// </summary>
        /// <typeparam name="TWindow">Window type with IResizeEvent</typeparam>
        /// <param name="window">Window</param>
        public static void AddResizeHook<TWindow>(
            this TWindow window,
            EventHandler resizingEventHandler = null,
            EventHandler resizedEventHandler = null
        ) where TWindow : Window, IResizeEvent {

            if (window == null) {
                return;
            }
            window.Loaded += (sender, e) => {
                var source = HwndSource.FromHwnd(new WindowInteropHelper(window).Handle);
                source.AddHook((IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) => {
                    switch (msg) {
                        case WM_ENTERSIZEMOVE:
                            var resizingHandler = window.GetDelegate(ResizingEventHandlerName)
                                .GetHandler<EventHandler>();
                            if (resizingHandler != null) {
                                resizingHandler(window, EventArgs.Empty);
                            }
                            break;
                        case WM_EXITSIZEMOVE:
                            var resizedHandler = window.GetDelegate(ResizedEventHandlerName)
                                .GetHandler<EventHandler>();
                            if (resizedHandler != null) {
                                resizedHandler(window, EventArgs.Empty);
                            }
                            break;
                    }
                    return IntPtr.Zero;
                });
            };
            if (resizingEventHandler != null) {
                window.Resizing += resizingEventHandler;
            }
            if (resizedEventHandler != null) {
                window.Resized += resizedEventHandler;
            }
        }
    }
}
