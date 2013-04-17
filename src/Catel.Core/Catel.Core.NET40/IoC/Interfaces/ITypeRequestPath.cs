// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeRequestPath.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.IoC
{
    /// <summary>
    /// Interface defining the public members of <see cref="TypeRequestPath"/> to be used inside exceptions.
    /// </summary>
    public interface ITypeRequestPath
    {
        /// <summary>
        /// Gets a value indicating whether value types should be ignored in the path.
        /// </summary>
        /// <value><c>true</c> if value types should be ignored; otherwise, <c>false</c>.</value>
        bool IgnoreValueTypes { get; }

        /// <summary>
        /// Gets all types in the right order.
        /// </summary>
        /// <value>All types.</value>
        TypeRequestInfo[] AllTypes { get; }

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

        /// <summary>
        /// Gets a value indicating whether this path is valid, which means that the same type does not occur multiple times.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        bool IsValid { get; }

        /// <summary>
        /// Gets the number of types in the type path.
        /// </summary>
        /// <value>The type count.</value>
        int TypeCount { get; }
    }
}