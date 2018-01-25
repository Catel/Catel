// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IObjectIdGenerator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2018 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    /// <summary>
    /// The object id generator service.
    /// </summary>
    /// <typeparam name="TUniqueIdentifier">The unique identifier type</typeparam>
    public interface IObjectIdGenerator<TUniqueIdentifier>
    {
        /// <summary>
        /// Gets the unique identifier for the specified type.
        /// </summary>
        /// <returns>A new unique identifier but if <paramref name="reuse"/> is <c>true</c> will return a released identifier.</returns>
        TUniqueIdentifier GetUniqueIdentifier(bool reuse);

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
    public interface IObjectIdGenerator<TObjectType, TUniqueIdentifier> : IObjectIdGenerator<TUniqueIdentifier>
    {
    }
}
