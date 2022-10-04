namespace Catel.Runtime
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class responsible for managing identifiers for circular dependencies.
    /// </summary>
    public class ReferenceManager
    {
        /// <summary>
        /// The default reference equality comparer.
        /// </summary>
        private static readonly IEqualityComparer<object> DefaultReferenceEqualityComparer = new ReferenceEqualityComparer<object>();

        /// <summary>
        /// The dictionary containing the actualy information by object reference.
        /// </summary>
        private readonly Dictionary<object, ReferenceInfo> _referenceInfoByInstance = new Dictionary<object, ReferenceInfo>(DefaultReferenceEqualityComparer);

        /// <summary>
        /// The dictionary containing the actualy information by id.
        /// </summary>
        private readonly Dictionary<int, ReferenceInfo> _referenceInfoById = new Dictionary<int, ReferenceInfo>();

        /// <summary>
        /// The thread-lock object.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// The counter which is increased every time an instance is added.
        /// </summary>
        private int _counter = 1;

        /// <summary>
        /// The hashset containing the used ids.
        /// </summary>
        private readonly HashSet<int> _usedIds = new HashSet<int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceManager"/> class.
        /// </summary>
        public ReferenceManager()
        {
        }

        /// <summary>
        /// Gets the number of items in the reference manager.
        /// </summary>
        /// <value>The number of items.</value>
        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _referenceInfoByInstance.Count;
                }
            }
        }

        /// <summary>
        /// Registers the specified instance manually.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="instance">The instance.</param>
        public void RegisterManually(int id, object? instance)
        {
            if (instance is null)
            {
                return;
            }

            lock (_lock)
            {
                var referenceInfo = new ReferenceInfo(instance, id, false);

                AddReferenceInfo(referenceInfo);
            }
        }

        /// <summary>
        /// Gets the info for the specified instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="autoAssignId">If set to <c>true</c>, a unique graph id will automatically be reserved for this object. Note that it's recommended to set this to <c>false</c> during deserialization.</param>
        /// <returns>The <see cref="ReferenceInfo" /> or <c>null</c> if <paramref name="instance" /> is <c>null</c>.</returns>
        public ReferenceInfo? GetInfo(object instance, bool autoAssignId = true)
        {
            if (instance is null)
            {
                return null;
            }

            lock (_lock)
            {
                if (_referenceInfoByInstance.TryGetValue(instance, out var referenceInfo))
                {
                    if (!referenceInfo.Id.HasValue)
                    {
                        if (autoAssignId)
                        {
                            referenceInfo.Id = GetNextId();

                            AddReferenceInfo(referenceInfo);
                        }
                    }
                    else
                    {
                        // Only treat as non-first usage if we already had a valid id
                        referenceInfo.IsFirstUsage = false;
                    }
                }
                else
                {
                    int? id = null;

                    if (autoAssignId)
                    {
                        id = GetNextId();
                    }

                    referenceInfo = new ReferenceInfo(instance, id, true);

                    AddReferenceInfo(referenceInfo);
                }

                return referenceInfo;
            }
        }

        /// <summary>
        /// Gets the information by the unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <returns>The <see cref="ReferenceInfo" /> or <c>null</c> if the id is not found.</returns>
        public ReferenceInfo? GetInfoById(int id)
        {
            lock (_lock)
            {
                if (_referenceInfoById.TryGetValue(id, out var referenceInfo))
                {
                    return referenceInfo;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the info at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="ReferenceInfo"/>.</returns>
        public ReferenceInfo GetInfoAt(int index)
        {
            lock (_lock)
            {
                var key = _referenceInfoByInstance.Keys.ElementAt(index);
                return _referenceInfoByInstance[key];
            }
        }

        private bool AddReferenceInfo(ReferenceInfo referenceInfo)
        {
            lock (_lock)
            {
                var id = referenceInfo.Id;
                var instance = referenceInfo.Instance;

                if (id.HasValue && _usedIds.Contains(id.Value))
                {
                    return false;
                }

                if (_referenceInfoByInstance.TryGetValue(instance, out var existingReferenceInfo))
                {
                    existingReferenceInfo.Id = id;
                }
                else
                {
                    _referenceInfoByInstance.Add(instance, referenceInfo);
                }

                // We might be adding a late id
                var added = false;
                if (id.HasValue)
                {
                    _referenceInfoById.Add(id.Value, existingReferenceInfo ?? referenceInfo);
                    _usedIds.Add(id.Value);

                    added = true;
                }

                return added;
            }
        }

        private int GetNextId()
        {
            lock (_lock)
            {
                var id = _counter++;
                while (_usedIds.Contains(id))
                {
                    id = _counter++;
                }

                return id;
            }
        }
    }
}
