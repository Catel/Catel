// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class responsible for managing identifiers for circular dependencies.
    /// </summary>
    public class ReferenceManager
    {
        #region Constants
        /// <summary>
        /// The default reference equality comparer.
        /// </summary>
        private static readonly IEqualityComparer<object> DefaultReferenceEqualityComparer = new ReferenceEqualityComparer<object>();
        #endregion

        #region Fields
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
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceManager"/> class.
        /// </summary>
        public ReferenceManager()
        {
        }
        #endregion

        #region Properties
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
        #endregion

        #region Methods
        /// <summary>
        /// Registers the specified instance manually.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="instance">The instance.</param>
        public void RegisterManually(int id, object instance)
        {
            if (instance == null)
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
        /// <returns>The <see cref="ReferenceInfo" /> or <c>null</c> if <paramref name="instance" /> is <c>null</c>.</returns>
        public ReferenceInfo GetInfo(object instance)
        {
            if (instance == null)
            {
                return null;
            }

            lock (_lock)
            {
                ReferenceInfo referenceInfo = null;

                if (!_referenceInfoByInstance.ContainsKey(instance))
                {
                    var id = GetNextId();
                    referenceInfo = new ReferenceInfo(instance, id, true);

                    AddReferenceInfo(referenceInfo);
                }
                else
                {
                    referenceInfo = _referenceInfoByInstance[instance];
                    referenceInfo.IsFirstUsage = false;
                }

                return referenceInfo;
            }
        }

        /// <summary>
        /// Gets the information by the unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <returns>The <see cref="ReferenceInfo" /> or <c>null</c> if the id is not found.</returns>
        public ReferenceInfo GetInfoById(int id)
        {
            lock (_lock)
            {
                if (_referenceInfoById.ContainsKey(id))
                {
                    return _referenceInfoById[id];
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
                if (_referenceInfoByInstance.ContainsKey(referenceInfo) || _usedIds.Contains(referenceInfo.Id))
                {
                    return false;
                }

                _referenceInfoByInstance.Add(referenceInfo.Instance, referenceInfo);
                _referenceInfoById.Add(referenceInfo.Id, referenceInfo);

                _usedIds.Add(referenceInfo.Id);

                return true;
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
        #endregion
    }
}