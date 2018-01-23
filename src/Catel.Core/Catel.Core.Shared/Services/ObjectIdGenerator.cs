// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectIdGenerator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2018 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System.Collections.Generic;

    /// <summary>
    /// The ObjectIdGenerator class.
    /// </summary>
    /// <typeparam name="TObjectType">The object type</typeparam>
    /// <typeparam name="TUniqueIdentifier">The unique identifier type</typeparam>
    public abstract class ObjectIdGenerator<TObjectType, TUniqueIdentifier> : IObjectIdGenerator<TObjectType, TUniqueIdentifier>
    {
        private static readonly Queue<TUniqueIdentifier> _releasedUniqueIdentifiers = new Queue<TUniqueIdentifier>();

        private static readonly object _syncObj = new object();

        /// <inheritdoc />
        public TUniqueIdentifier GetUniqueIdentifier()
        {
            lock (_syncObj)
            {
                return GenerateUniqueIdentifier();
            }
        }

        /// <inheritdoc />
        public TUniqueIdentifier GetUniqueIdentifier(bool reuse)
        {
            lock (_syncObj)
            {
                if (reuse && _releasedUniqueIdentifiers.Count > 0)
                {
                    return _releasedUniqueIdentifiers.Dequeue();
                }

                return GenerateUniqueIdentifier();
            }
        }

        /// <inheritdoc />
        public void ReleaseIdentifier(TUniqueIdentifier identifier)
        {
            lock (_syncObj)
            {
                _releasedUniqueIdentifiers.Enqueue(identifier);
            }
        }

        /// <summary>
        /// Generates the unique identifier.
        /// </summary>
        /// <returns>
        /// The unique identifier.
        /// </returns>
        protected abstract TUniqueIdentifier GenerateUniqueIdentifier();
    }
}