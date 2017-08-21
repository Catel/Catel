// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeRequestPath.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface defining the public members of <see cref="TypeRequestPath"/> to be used inside exceptions.
    /// </summary>
    public interface ITypeRequestPath
    {
        /// <summary>
        /// Gets all types in the right order.
        /// </summary>
        /// <value>All types.</value>
        IEnumerable<TypeRequestInfo> AllTypes { get; }

        /// <summary>
        /// Gets the first type in the type path.
        /// </summary>
        /// <value>The first type.</value>
        TypeRequestInfo FirstType { get; }

        /// <summary>
        /// Gets the last type in the type path.
        /// </summary>
        /// <value>The last type.</value>
        TypeRequestInfo LastType { get; }
    }
}