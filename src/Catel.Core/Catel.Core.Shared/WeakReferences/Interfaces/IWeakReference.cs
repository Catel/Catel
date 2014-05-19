// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWeakReference.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    /// <summary>
    /// Weak reference interface.
    /// </summary>
    public interface IWeakReference
    {
        /// <summary>
        /// Gets a value indicating whether the target has not yet been garbage collected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the target has not yet been garbage collected; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// In case of static event handlers, this property always returns <c>false</c>.
        /// </remarks>
        bool IsTargetAlive { get; }

        /// <summary>
        /// Gets the target of the weak reference. Will be <c>null</c> when the target is no longer alive.
        /// </summary>
        /// <remarks>
        /// In case of static event handlers, this property always returns <c>null</c>.
        /// </remarks>
        object Target { get; }
    }
}
