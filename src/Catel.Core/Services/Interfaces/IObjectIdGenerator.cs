﻿namespace Catel.Services
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
        /// <param name="objectType">Type of the object.</param>
        /// <param name="reuse">Indicates whether the id will be returned from released id pool</param>
        /// <returns>A new unique identifier but if <paramref name="reuse"/> is <c>true</c> will return a released identifier.</returns>
        TUniqueIdentifier GetUniqueIdentifier(Type objectType, bool reuse = false);

        /// <summary>
        /// Release the unique identifier for the specified type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="identifier">The identifier.</param>
        void ReleaseIdentifier(Type objectType, TUniqueIdentifier identifier);
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
        /// <param name="instance">The instance.</param>
        /// <param name="reuse">Indicates whether the id will be returned from released id pool.</param>
        /// <returns></returns>
        TUniqueIdentifier GetUniqueIdentifierForInstance(TObjectType instance, bool reuse = false);
    }
}
