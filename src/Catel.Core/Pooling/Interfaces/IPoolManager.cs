// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPoolManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Pooling
{
    /// <summary>
    /// Pool manager allowing objects to be pooled.
    /// </summary>
    public interface IPoolManager
    {
        /// <summary>
        /// Gets the total number of objects insider this pool.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Returns a used object back to the pool so it can be re-used.
        /// </summary>
        /// <param name="value">The value to return to the pool.</param>
        void ReturnObject(object value);
    }

    /// <summary>
    /// Pool manager allowing objects to be pooled.
    /// </summary>
    /// <typeparam name="TPoolable">Type of the object to be pooled.</typeparam>
    public interface IPoolManager<TPoolable> : IPoolManager
        where TPoolable : class, IPoolable, new()
    {
        /// <summary>
        /// Gets the poolable object from the pool.
        /// </summary>
        /// <returns>A free poolable object.</returns>
        TPoolable GetObject();

        /// <summary>
        /// Gets the current size.
        /// </summary>
        int CurrentSize { get; }
    }
}