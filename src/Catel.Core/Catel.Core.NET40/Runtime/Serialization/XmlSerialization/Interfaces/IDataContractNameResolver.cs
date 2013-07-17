// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataContractNameResolver.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;

    /// <summary>
    /// Interface defining a data contract name resolver.
    /// </summary>
    public interface IDataContractNameResolver
    {
        /// <summary>
        /// Gets the data contract name of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A string representing the data contract name.</returns>
        string GetDataContractName(Type type);
    }
}