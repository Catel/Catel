// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PoolableBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Pooling
{
    using System;

    /// <summary>
    /// Base implementation for any poolable object.
    /// </summary>
    public abstract class PoolableBase : IPoolable, IUniqueIdentifyable
    {
        /// <summary>
        /// The pool manager.
        /// </summary>
        protected IPoolManager _poolManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolableBase"/> class.
        /// </summary>
        protected PoolableBase()
        {
            var type = GetType();
            UniqueIdentifier = UniqueIdentifierHelper.GetUniqueIdentifier(type);
        }

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public int UniqueIdentifier { get; private set; }

        /// <summary>
        /// The size of the object.
        /// </summary>
        public abstract int Size { get; }

        /// <summary>
        /// Resets the object to an initial state.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Sets the pool manager of the polable object.
        /// </summary>
        /// <param name="poolManager">The pool manager.</param>
        void IPoolable.SetPoolManager(IPoolManager poolManager)
        {
            _poolManager = poolManager;
        }

        /// <summary>
        /// Disposes the object and returns the object to the pool manager.
        /// </summary>
        public void Dispose()
        {
            _poolManager.ReturnObject(this);
        }
    }
}