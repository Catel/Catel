// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UniqueIdentifierHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Helper class that allows to generic unique identifiers for objects.
    /// <para />
    /// This class internally keeps a counter per type and will increase the counter every time
    /// a new unique identifier is requested.
    /// </summary>
    public static class UniqueIdentifierHelper
    {
        /// <summary>
        /// The dictionary containing the unique identifiers per type.
        /// </summary>
        private static readonly Dictionary<Type, int> _uniqueIdentifiers = new Dictionary<Type, int>();

        private static readonly object _lockObject = new object();

        /// <summary>
        /// Gets the unique identifier for the specified type.
        /// </summary>
        /// <typeparam name="T">The type to retrieve the unique identifier for.</typeparam>
        /// <returns>A new unique identifier for the type.</returns>
        public static int GetUniqueIdentifier<T>()
        {
            return GetUniqueIdentifier(typeof (T));
        }

        /// <summary>
        /// Gets a unique identifier for the specified type.
        /// </summary>
        /// <param name="type">The type to retrieve the unique identifier for.</param>
        /// <returns>A new unique identifier for the type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public static int GetUniqueIdentifier(Type type)
        {
            Argument.IsNotNull("type", type);

            lock (_lockObject)
            {
                if (!_uniqueIdentifiers.TryGetValue(type, out var id))
                {
                    id = 0;
                }

                id++;
                _uniqueIdentifiers[type] = id;

                return id;
            }
        }
    }
}
