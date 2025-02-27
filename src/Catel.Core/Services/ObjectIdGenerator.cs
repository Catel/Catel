﻿namespace Catel.Services
{
    using System;
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
        private static Queue<TUniqueIdentifier>? ReleasedUniqueIdentifiers;

        private static readonly object SyncObj = new object();

        private static readonly ConditionalWeakTable<TObjectType, InstanceWrapper> AllocatedUniqueIdentifierPerInstances = new ConditionalWeakTable<TObjectType, InstanceWrapper>();

        protected readonly object _lock = new object();

        /// <inheritdoc />
        public TUniqueIdentifier GetUniqueIdentifier(Type objectType, bool reuse = false)
        {
            if (reuse)
            {
                lock (SyncObj)
                {
                    if (ReleasedUniqueIdentifiers is not null && ReleasedUniqueIdentifiers.Count > 0)
                    {
                        return ReleasedUniqueIdentifiers.Dequeue();
                    }
                }
            }

            return GenerateUniqueIdentifier();
        }

        /// <inheritdoc />
        public void ReleaseIdentifier(Type objectType, TUniqueIdentifier identifier)
        {
            lock (SyncObj)
            {
                if (ReleasedUniqueIdentifiers is null)
                {
                    ReleasedUniqueIdentifiers = new Queue<TUniqueIdentifier>();
                }

                ReleasedUniqueIdentifiers.Enqueue(identifier);
            }
        }

        /// <inheritdoc />
        public TUniqueIdentifier GetUniqueIdentifierForInstance(TObjectType instance, bool reuse = false)
        {
            lock (SyncObj)
            {
                var objectType = instance.GetType();

                if (AllocatedUniqueIdentifierPerInstances.TryGetValue(instance, out var wrapper))
                {
                    return wrapper.UniqueIdentifier;
                }

                wrapper = new InstanceWrapper(this, objectType, GetUniqueIdentifier(objectType, reuse));

                AllocatedUniqueIdentifierPerInstances.Add(instance, wrapper);

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
            public InstanceWrapper(IObjectIdGenerator<TObjectType, TUniqueIdentifier> objectIdGenerator, Type objectType, TUniqueIdentifier uniqueIdentifier)
            {
                ObjectIdGenerator = objectIdGenerator;
                ObjectType = objectType;
                UniqueIdentifier = uniqueIdentifier;
            }

            public IObjectIdGenerator<TObjectType, TUniqueIdentifier> ObjectIdGenerator { get; }

            public Type ObjectType { get; }

            public TUniqueIdentifier UniqueIdentifier { get; }

            ~InstanceWrapper()
            {
#pragma warning disable IDISP023 // Don't use reference types in finalizer context.
                ObjectIdGenerator?.ReleaseIdentifier(ObjectType, UniqueIdentifier);
#pragma warning restore IDISP023 // Don't use reference types in finalizer context.
            }
        }
    }
}
