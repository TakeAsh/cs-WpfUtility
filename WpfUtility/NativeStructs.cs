using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using TakeAshUtility;

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

        [PrintMember("x4")]
        public uint flags;

        [PrintMember]
        public SW showCmd;

        [PrintMember]
        public POINT minPosition;

        [PrintMember]
        public POINT maxPosition;

        [PrintMember]
        public RECT normalPosition;

        public override string ToString() {
            return this.MembersToString();
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

        [PrintMember]
        public int X;

        [PrintMember]
        public int Y;

        public POINT(int x, int y) {
            X = x;
            Y = y;
        }

        public override string ToString() {
            return this.MembersToString();
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    struct RECT {

        [PrintMember]
        public int Left;

        [PrintMember]
        public int Top;

        [PrintMember]
        public int Right;

        [PrintMember]
        public int Bottom;

        public RECT(int left, int top, int right, int bottom) {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public override string ToString() {
            return this.MembersToString();
        }
    }
}
