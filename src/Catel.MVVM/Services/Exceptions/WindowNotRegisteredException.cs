﻿namespace Catel.Services
{
    using System;

    /// <summary>
    /// Exception in case a window not registered, but still being used.
    /// </summary>
    public class WindowNotRegisteredException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowNotRegisteredException"/> class.
        /// </summary>
        /// <param name="name">The name of the window.</param>
        public WindowNotRegisteredException(string name)
            : base($"There is no window registered as '{name}'")
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the window.
        /// </summary>
        /// <value>The name of the window.</value>
        public string Name { get; private set; }
    }
}
