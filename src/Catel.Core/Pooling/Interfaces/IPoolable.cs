// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPoolable.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Pooling
{
    using System;

    /// <summary>
    /// Interface defining pollable objects by the <see cref="IPoolManager" />.
    /// </summary>
    public interface IPoolable : IDisposable
    {
        /// <summary>
        /// The size of the object.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Resets the object to an initial state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Sets the pool manager of the polable object.
        /// </summary>
        /// <param name="poolManager">The pool manager.</param>
        void SetPoolManager(IPoolManager poolManager);
    }
}