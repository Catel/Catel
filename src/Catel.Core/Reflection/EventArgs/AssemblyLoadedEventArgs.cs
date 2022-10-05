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
        private readonly Lazy<IEnumerable<Type>>? _lazyLoadedTypes;
        private readonly List<Type> _loadedTypes = new List<Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyLoadedEventArgs" /> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="loadedTypesLazy">The lazy loaded types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="assembly"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="loadedTypesLazy"/> is <c>null</c>.</exception>
        public AssemblyLoadedEventArgs(Assembly assembly, Lazy<IEnumerable<Type>> loadedTypesLazy)
        {
            Assembly = assembly;
            _lazyLoadedTypes = loadedTypesLazy;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyLoadedEventArgs" /> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="loadedTypes">The loaded types.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="assembly"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="loadedTypes"/> is <c>null</c>.</exception>
        public AssemblyLoadedEventArgs(Assembly assembly, IEnumerable<Type> loadedTypes)
        {
            Assembly = assembly;
            _loadedTypes.AddRange(loadedTypes);
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
        public IEnumerable<Type> LoadedTypes
        {
            get
            {
                if (_loadedTypes.Count == 0)
                {
                    if (_lazyLoadedTypes is not null)
                    {
                        _loadedTypes.AddRange(_lazyLoadedTypes.Value);
                    }
                }

                return _loadedTypes;
            }
        }
    }
}
