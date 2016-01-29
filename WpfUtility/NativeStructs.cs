using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WpfUtility.Native {

    /// <summary>
    /// WINDOWPLACEMENT stores the position, size, and state of a window
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>[WINDOWPLACEMENT Structure](https://msdn.microsoft.com/en-us/library/kb89946z.aspx)</item>
    /// <item>Header: winuser.h</item>
    /// </list>
    /// </remarks>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    struct WINDOWPLACEMENT {
        public uint length;
        public uint flags;
        public SW showCmd;
        public POINT minPosition;
        public POINT maxPosition;
        public RECT normalPosition;

        public override string ToString() {
            return String.Join(", ", new[] {
                "flags:" + flags,
                "showCmd:" + showCmd,
                "minPosition:{" + minPosition + "}",
                "maxPosition:{" + maxPosition + "}",
                "normalPosition:{" + normalPosition + "}",
            });
        }
    }

    /// <summary>
    /// ShowWindow() Commands
    /// </summary>
    enum SW : uint {
        HIDE = 0,
        SHOWNORMAL = 1,
        NORMAL = 1,
        SHOWMINIMIZED = 2,
        SHOWMAXIMIZED = 3,
        MAXIMIZE = 3,
        SHOWNOACTIVATE = 4,
        SHOW = 5,
        MINIMIZE = 6,
        SHOWMINNOACTIVE = 7,
        SHOWNA = 8,
        RESTORE = 9,
        SHOWDEFAULT = 10,
        FORCEMINIMIZE = 11,
        MAX = 11,
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    struct POINT {
        public int X;
        public int Y;

        public POINT(int x, int y) {
            X = x;
            Y = y;
        }

        public override string ToString() {
            return String.Join(", ", new[] {
                "X:" + X,
                "Y:" + Y,
            });
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    struct RECT {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left, int top, int right, int bottom) {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public override string ToString() {
            return String.Join(", ", new[] {
                "Left:" + Left,
                "Top:" + Top,
                "Right:" + Right,
                "Bottom:" + Bottom,
            });
        }
    }
}
