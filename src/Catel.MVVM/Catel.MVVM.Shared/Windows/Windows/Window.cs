// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Window.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || SL5

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
#if NET
            SizeToContent = SizeToContent.Manual;
            ShowInTaskbar = true;
            ResizeMode = ResizeMode.CanResize;
            WindowStartupLocation = WindowStartupLocation.Manual;
#endif
        }
    }
}

#endif