// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PoolManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Pooling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Logging;

    /// <summary>
    /// Pool manager allowing objects to be pooled.
    /// <para/>
    /// The implementation removes objects from the internal stack and releases them. If no instance is
    /// available, a new one will be created that should be returned to the pool once disposed.
    /// </summary>
    /// <typeparam name="TPoolable">Type of the object to be pooled.</typeparam>
    public class PoolManager<TPoolable> : IPoolManager<TPoolable> 
        where TPoolable : class, IPoolable, new()
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private const int DefaultMaxSize = 1024 * 1024 * 5; // 5 MB 

        private readonly Stack<IPoolable> _stack = new Stack<IPoolable>();

        /// <summary>
        /// Creates a new instance of the pool manager.
        /// </summary>
        public PoolManager()
        {
            MaxSize = DefaultMaxSize;
        }

        /// <summary>
        /// Gets the total number of objects insider this pool.
        /// </summary>
        public int Count
        {
            get
            {
                lock (_stack)
                {
                    return _stack.Count;
                }
            }
        }

        /// <summary>
        /// Gets the current size.
        /// </summary>
        public int CurrentSize { get; private set; }

        /// <summary>
        /// Gets the maximum size of the pool.
        /// </summary>
        public int MaxSize { get; set; }

        /// <summary>
        /// Gets the poolable object from the pool.
        /// </summary>
        /// <returns>A free poolable object.</returns>
        public TPoolable GetObject()
        {
            TPoolable valueToReturn = null;

            lock (_stack)
            {
                if (_stack.Count > 0)
                {
                    Log.Debug($"Returning object from pool");

                    valueToReturn = _stack.Pop() as TPoolable;

                    if (valueToReturn != null)
                    {
                        CurrentSize -= valueToReturn.Size;
                    }
                }
            }
            
            if (valueToReturn == null)
            {
                Log.Debug($"No objects available in the pool, creating new object");

                valueToReturn = new TPoolable();
                valueToReturn.SetPoolManager(this);
            }

            return valueToReturn;
        }

        /// <summary>
        /// Returns a used object back to the pool so it can be re-used.
        /// </summary>
        /// <param name="value">The value to return to the pool.</param>
        void IPoolManager.ReturnObject(object value)
        {
            ReturnObject((TPoolable)value);
        }

        /// <summary>
        /// Returns a used object back to the pool so it can be re-used.
        /// </summary>
        /// <param name="value">The value to return to the pool.</param>
        public void ReturnObject(TPoolable value)
        {
            lock (_stack)
            {
                var currentSize = CurrentSize;
                var valueSize = value.Size;
                var newSize = currentSize + valueSize;
                var maxSize = MaxSize;

                if (newSize > maxSize)
                {
                    Log.Debug($"Pool will become larger than '{maxSize}', object will not be returned to the pool");
                }
                else
                {
                    CurrentSize += valueSize;
                    value.Reset();
                    _stack.Push(value);

                    Log.Debug($"Returned object to pool, new size is '{newSize}' with '{_stack.Count}' items");
                }
            }
        }
    }
}