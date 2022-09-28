namespace Catel.Windows
{
    using System.Windows;

    /// <summary>
    /// Easy implementation of the <see cref="DataWindow"/> that adds some features to make
    /// the data window behave as a normal window.
    /// </summary>
    public class Window : DataWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        public Window()
            : base(DataWindowMode.Custom)
        {
            SizeToContent = SizeToContent.Manual;
            ShowInTaskbar = true;
            ResizeMode = ResizeMode.CanResize;
            WindowStartupLocation = WindowStartupLocation.Manual;
        }
    }
}
