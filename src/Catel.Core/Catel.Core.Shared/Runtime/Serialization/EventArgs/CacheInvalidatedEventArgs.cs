// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheInvalidatedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;

    /// <summary>
    /// Event args for when a type cache has been invalidated.
    /// </summary>
    public class CacheInvalidatedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheInvalidatedEventArgs"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public CacheInvalidatedEventArgs(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// Gets the type that was invalidated.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; private set; }
    }
}