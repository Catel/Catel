namespace Catel.Win32
{
    using System;
    using System.Runtime.InteropServices;

    internal delegate IntPtr LowLevelHookProc(int nCode, IntPtr wParam, IntPtr lParam);

    internal static partial class User32
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        internal static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hwnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool BringWindowToTop(IntPtr hWnd);
    }
}
