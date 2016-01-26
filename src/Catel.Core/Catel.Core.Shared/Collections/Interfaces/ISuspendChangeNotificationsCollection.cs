// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISuspendCollectionChangedNotifications.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Collections
{
    using System;
    using System.Collections;

    /// <summary>
    /// Interface to specify that collection supports suspending change notifications.
    /// </summary>
    public interface ISuspendChangeNotificationsCollection : ICollection
    {
        /// <summary>
        /// Gets or sets a value indicating whether change to the collection is made when
        /// its notifications are suspended.
        /// </summary>
        /// <value><c>true</c> if this instance is has been changed while notifications are
        /// suspended; otherwise, <c>false</c>.</value>
        bool IsDirty { get; }

        /// <summary>
        /// Gets a value indicating whether change notifications are suspended.
        /// </summary>
        /// <value>
        /// <c>True</c> if notifications are suspended, otherwise, <c>false</c>.
        /// </value>
        bool NotificationsSuspended { get; }

        /// <summary>
        /// Raises change notifications of type 'Reset'.
        /// </summary>
        void Reset();

        /// <summary>
        /// Suspends the change notifications until the returned <see cref="IDisposable"/> is disposed.
        /// </summary>
        /// <returns>IDisposable.</returns>
        IDisposable SuspendChangeNotifications();
    }
}
