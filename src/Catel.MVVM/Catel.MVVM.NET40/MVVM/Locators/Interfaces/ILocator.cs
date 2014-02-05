// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILocator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface defining shared information about locators. This way, any locator can be casted
    /// to this interface and be used to locate something by naming convention.
    /// </summary>
    public interface ILocator
    {
        /// <summary>
        /// Gets or sets the naming conventions to use to locate types.
        /// <para />
        /// By adding or removing conventions to this property, the service can use custom resolving of types.
        /// <para />
        /// Each implementation should add its own default naming convention.
        /// </summary>
        /// <value>The naming conventions.</value>
        /// <remarks>
        /// Keep in mind that all results are cached. The cache itself is not automatically cleared when the
        /// <see cref="NamingConventions"/> are changed. If the <see cref="NamingConventions"/> are changed,
        /// the cache must be cleared manually.
        /// </remarks>
        List<string> NamingConventions { get; }

        /// <summary>
        /// Clears the cache of the resolved naming conventions.
        /// </summary>
        /// <remarks>
        /// Note that clearing the cache will also clear all manually registered types
        /// registered via the <c>Register</c> method.
        /// </remarks>
        void ClearCache();
    }
}