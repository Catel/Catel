// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingFlags.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Reflection
{
    using System.Reflection;

    /// <summary>
    /// Allows custom logic to be injected into <see cref="AssemblyHelper.GetEntryAssembly"/>.
    /// </summary>
    public interface IEntryAssemblyResolver
    {
        /// <summary>
        /// Resolves the entry assembly.
        /// </summary>
        /// <returns>Assembly.</returns>
        Assembly Resolve();
    }
}
