// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectIdGenerator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2018 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// The ObjectIdGenerator class.
    /// </summary>
    /// <typeparam name="TObjectType">The object type</typeparam>
    /// <typeparam name="TUniqueIdentifier">The unique identifier type</typeparam>
    public abstract class ObjectIdGenerator<TObjectType, TUniqueIdentifier> : IObjectIdGenerator<TObjectType, TUniqueIdentifier>
        where TObjectType : class
    {
        private static Queue<TUniqueIdentifier> _releasedUniqueIdentifiers;

        private static readonly object _syncObj = new object();

        private static readonly ConditionalWeakTable<TObjectType, InstanceWrapper> _allocatedUniqueIdentifierPerInstances = new ConditionalWeakTable<TObjectType, InstanceWrapper>();

        /// <inheritdoc />
        public TUniqueIdentifier GetUniqueIdentifier(bool reuse = false)
        {
            if (reuse)
            {
                lock (_syncObj)
                {
                    if (_releasedUniqueIdentifiers != null && _releasedUniqueIdentifiers.Count > 0)
                    {
                        return _releasedUniqueIdentifiers.Dequeue();
                    }
                }
            }

            return GenerateUniqueIdentifier();
        }

        /// <inheritdoc />
        public void ReleaseIdentifier(TUniqueIdentifier identifier)
        {
            lock (_syncObj)
            {
                if (_releasedUniqueIdentifiers == null)
                {
                    _releasedUniqueIdentifiers = new Queue<TUniqueIdentifier>();
                }

                _releasedUniqueIdentifiers.Enqueue(identifier);
            }
        }

        /// <inheritdoc />
        public TUniqueIdentifier GetUniqueIdentifierForInstance(TObjectType instance, bool reuse = false)
        {
            Argument.IsNotNull("instance", instance);

            lock (_syncObj)
            {
                if (_allocatedUniqueIdentifierPerInstances.TryGetValue(instance, out var wrapper))
                {
                    return wrapper.UniqueIdentifier;
                }

                wrapper = new InstanceWrapper(this, GetUniqueIdentifier(reuse));

                _allocatedUniqueIdentifierPerInstances.Add(instance, wrapper);

                return wrapper.UniqueIdentifier;
            }
        }

        /// <summary>
        /// Generates the unique identifier.
        /// </summary>
        /// <returns>
        /// The unique identifier.
        /// </returns>
        protected abstract TUniqueIdentifier GenerateUniqueIdentifier();

        private class InstanceWrapper
        {
            public InstanceWrapper(IObjectIdGenerator<TObjectType, TUniqueIdentifier> objectIdGenerator, TUniqueIdentifier uniqueIdentifier)
            {
                Argument.IsNotNull("objectIdGenerator", objectIdGenerator);

                ObjectIdGenerator = objectIdGenerator;
                UniqueIdentifier = uniqueIdentifier;
            }

            public IObjectIdGenerator<TObjectType, TUniqueIdentifier> ObjectIdGenerator { get; }

            public TUniqueIdentifier UniqueIdentifier { get; }

            ~InstanceWrapper()
            {
                ObjectIdGenerator?.ReleaseIdentifier(UniqueIdentifier);
            }
        }
    }
}