// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMemberDefinition.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the type member definition interface.
    /// </summary>
    public interface IMemberDefinition
    {
        #region Properties
        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        IList<Type> Parameters { get; }

        /// <summary>
        /// Gets the name of the member.
        /// </summary>
        /// <value>
        /// The name of the member.
        /// </value>
        string MemberName { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        bool Equals(object obj);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        int GetHashCode();
        #endregion
    }
}