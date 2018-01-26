// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectIdGenerator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2018 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Catel.Threading;

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

        private static SortedDictionary<TUniqueIdentifier, WeakReference<TObjectType>> _allocatedUniqueIdentifierPerInstances = new SortedDictionary<TUniqueIdentifier, WeakReference<TObjectType>>();

        private static readonly TimeSpan DefaultInterval = TimeSpan.FromMinutes(1);

        private static TimeSpan? _interval;

        private static Timer _timer;

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

            var uniqueIdentifier = GetUniqueIdentifier(reuse);

            lock (_syncObj)
            {
                if (_allocatedUniqueIdentifierPerInstances == null)
                {
                    _allocatedUniqueIdentifierPerInstances = new SortedDictionary<TUniqueIdentifier, WeakReference<TObjectType>>();
                }

                _allocatedUniqueIdentifierPerInstances.Add(uniqueIdentifier, new WeakReference<TObjectType>(instance));

                var interval = _interval ?? DefaultInterval;
                if (_timer == null)
                {
                    _timer = new Timer(OnTimerElapsed, null, interval, interval);
                }
                else
                {
                    _timer.Change(interval, interval);
                }
            }

            return uniqueIdentifier;
        }

        /// <inheritdoc />
        public TimeSpan? InstanceCheckInterval
        {
            get
            {
                lock (_syncObj)
                {
                    return _interval;
                }
            }
            set
            {
                lock (_syncObj)
                {
                    _interval = value;
                }
            }
        }

        private void OnTimerElapsed(object state)
        {
            lock (_syncObj)
            {
                var uniqueIdentifiers = _allocatedUniqueIdentifierPerInstances.Where(pair => !pair.Value.TryGetTarget(out var _)).Select(pair => pair.Key).ToList();
                if (uniqueIdentifiers.Any() && _releasedUniqueIdentifiers == null)
                {
                    _releasedUniqueIdentifiers = new Queue<TUniqueIdentifier>();
                }

                foreach (var uniqueIdentifier in uniqueIdentifiers)
                {
                    _allocatedUniqueIdentifierPerInstances.Remove(uniqueIdentifier);
                    _releasedUniqueIdentifiers.Enqueue(uniqueIdentifier);
                }

                if (_timer != null && _allocatedUniqueIdentifierPerInstances.Count == 0)
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                }
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