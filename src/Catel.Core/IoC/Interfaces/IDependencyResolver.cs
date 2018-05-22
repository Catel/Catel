// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDependencyResolver.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;

    /// <summary>
    /// Provides a simplified way to resolve dependencies and allows customization of the 
    /// way dependencies are resolved.
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        /// Determines whether the specified type with the specified tag can be resolved.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="tag">The tag.</param>
        /// <returns><c>true</c> if the specified type with the specified tag can be resolved; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        bool CanResolve(Type type, object tag = null);

        /// <summary>
        /// Determines whether all types specified can be resolved. Though <see cref="ResolveAll"/> will return <c>null</c>
        /// at the array index when a type cannot be resolved, this method will actually check whether all the specified types
        /// are registered.
        /// <para />
        /// It is still possible to call <see cref="ResolveAll"/>, even when this method returns <c>false</c>.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns><c>true</c> if all types specified can be resolved; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="types"/> is <c>null</c> or empty.</exception>
        [ObsoleteEx(ReplacementTypeOrMember = "CanResolveMultiple", TreatAsErrorFromVersion = "5.2", RemoveInVersion = "6.0")]
        bool CanResolveAll(Type[] types);

        /// <summary>
        /// Determines whether all types specified can be resolved. Though <see cref="ResolveMultiple"/> will return <c>null</c>
        /// at the array index when a type cannot be resolved, this method will actually check whether all the specified types
        /// are registered.
        /// <para />
        /// It is still possible to call <see cref="ResolveMultiple"/>, even when this method returns <c>false</c>.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <returns><c>true</c> if all types specified can be resolved; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="types"/> is <c>null</c> or empty.</exception>
        bool CanResolveMultiple(Type[] types);

        /// <summary>
        /// Resolves the specified type with the specified tag.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>The resolved object or <c>null</c> if the type could not be resolved.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        object Resolve(Type type, object tag = null);

        /// <summary>
        /// Resolves the specified types with the specified tag.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>A lost of resolved types. If one of the types cannot be resolved, that location in the array will be <c>null</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="types"/> is <c>null</c> or empty.</exception>
        [ObsoleteEx(ReplacementTypeOrMember = "ResolveMultiple", TreatAsErrorFromVersion = "5.2", RemoveInVersion = "6.0")]
        object[] ResolveAll(Type[] types, object tag = null);

        /// <summary>
        /// Resolves the specified types with the specified tag.
        /// </summary>
        /// <param name="types">The types.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>A lost of resolved types. If one of the types cannot be resolved, that location in the array will be <c>null</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="types"/> is <c>null</c> or empty.</exception>
        object[] ResolveMultiple(Type[] types, object tag = null);
    }
}