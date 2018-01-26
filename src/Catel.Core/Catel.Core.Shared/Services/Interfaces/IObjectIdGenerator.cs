// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IObjectIdGenerator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2018 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;

    /// <summary>
    /// The object id generator service.
    /// </summary>
    /// <typeparam name="TUniqueIdentifier">The unique identifier type</typeparam>
    public interface IObjectIdGenerator<TUniqueIdentifier>
    {
        /// <summary>
        /// Gets the unique identifier for the specified type.
        /// </summary>
        /// <param name="reuse">Indicates whether the id will be returned from released id pool</param>
        /// <returns>A new unique identifier but if <paramref name="reuse"/> is <c>true</c> will return a released identifier.</returns>
        TUniqueIdentifier GetUniqueIdentifier(bool reuse = false);

        /// <summary>
        /// Release the unique identifier for the specified type.
        /// </summary>
        void ReleaseIdentifier(TUniqueIdentifier identifier);
    }

    /// <summary>
    /// The object id generator service.
    /// </summary>
    /// <typeparam name="TObjectType">The object type</typeparam>
    /// <typeparam name="TUniqueIdentifier">The unique identifier type</typeparam>
    public interface IObjectIdGenerator<in TObjectType, TUniqueIdentifier> : IObjectIdGenerator<TUniqueIdentifier>
        where TObjectType : class
    {
        /// <summary>
        /// Gets the unique identifier for the specified instance.
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <param name="reuse">Indicates whether the id will be returned from released id pool</param>
        /// <returns></returns>
        TUniqueIdentifier GetUniqueIdentifierForInstance(TObjectType instance, bool reuse = false);

        /// <summary>
        /// Gets and sets the instance check interval.
        /// </summary>
        TimeSpan? InstanceCheckInterval { get; set; }
    }
}
