namespace Catel.Win32
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        /// <summary>
        /// Left.
        /// </summary>
        public int Left;

        /// <summary>
        /// Top.
        /// </summary>
        public int Top;

        /// <summary>
        /// Right.
        /// </summary>
        public int Right;

        /// <summary>
        /// Bottom.
        /// </summary>
        public int Bottom;
    }
}
