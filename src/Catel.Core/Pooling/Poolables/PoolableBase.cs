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
        protected IPoolManager _poolManager;
        private bool _disposedValue;

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

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _poolManager.ReturnObject(this);
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
