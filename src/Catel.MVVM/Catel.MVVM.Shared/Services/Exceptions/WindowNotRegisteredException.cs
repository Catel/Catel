// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowNotRegisteredException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
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
            : base(string.Format(ResourceHelper.GetString("WindowNotRegistered"), name))
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