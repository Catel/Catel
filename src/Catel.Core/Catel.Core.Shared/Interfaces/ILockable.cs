// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILockable.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    /// <summary>
    /// Interface defining locking functionality.
    /// </summary>
    public interface ILockable
    {
        /// <summary>
        /// Gets the lock object.
        /// </summary>
        /// <value>
        /// The lock object.
        /// </value>
        object LockObject { get; }
    }
}