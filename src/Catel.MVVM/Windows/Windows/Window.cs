namespace Catel.Windows
{
    using System;
    using System.Windows;
    using Catel.Services;

    /// <summary>
    /// Easy implementation of the <see cref="DataWindow"/> that adds some features to make
    /// the data window behave as a normal window.
    /// </summary>
    public class Window : DataWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        public Window(IServiceProvider serviceProvider, IWrapControlService wrapControlService, ILanguageService languageService)
            : base(serviceProvider, wrapControlService, languageService, DataWindowMode.Custom)
        {
            SizeToContent = SizeToContent.Manual;
            ShowInTaskbar = true;
            ResizeMode = ResizeMode.CanResize;
            WindowStartupLocation = WindowStartupLocation.Manual;
        }
    }
}
