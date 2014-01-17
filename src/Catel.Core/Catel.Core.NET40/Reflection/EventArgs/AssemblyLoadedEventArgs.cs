// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyLoadedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Event args for the <see cref="TypeCache.AssemblyLoaded"/> event.
    /// </summary>
    public class AssemblyLoadedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyLoadedEventArgs" /> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="loadedTypes">The loaded types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="assembly"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="loadedTypes"/> is <c>null</c>.</exception>
        public AssemblyLoadedEventArgs(Assembly assembly, IEnumerable<Type> loadedTypes)
        {
            Argument.IsNotNull("assembly", assembly);
            Argument.IsNotNull("loadedTypes", loadedTypes);

            Assembly = assembly;
            LoadedTypes = loadedTypes;
        }

        /// <summary>
        /// Gets the assembly.
        /// </summary>
        /// <value>The assembly.</value>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// Gets the loaded types.
        /// </summary>
        /// <value>The loaded types.</value>
        public IEnumerable<Type> LoadedTypes { get; private set; }
    }
}