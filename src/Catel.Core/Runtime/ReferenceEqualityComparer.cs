// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceEqualityComparer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime
{
    using System.Collections.Generic;

    /// <summary>
    /// Equality comparer for by reference.
    /// </summary>
    /// <typeparam name="TObject">The type of the object.</typeparam>
    public class ReferenceEqualityComparer<TObject> : IEqualityComparer<TObject>
        where TObject : class
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(TObject x, TObject y)
        {
            return ReferenceEquals(x, y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int GetHashCode(TObject obj)
        {
            if (obj == null)
            {
                return 0;
            }

            // Note: we need to use the same hashcode, then the dictionary will use the Equals method instead
            return 0;
        }
    }
}