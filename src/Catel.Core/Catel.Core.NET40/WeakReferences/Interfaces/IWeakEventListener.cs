// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWeakEventListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;

    /// <summary>
    /// Interface defining a weak event listener.
    /// </summary>
    public interface IWeakEventListener
    {
        /// <summary>
        /// Gets the type of the target.
        /// </summary>
        /// <value>The type of the target.</value>
        Type TargetType { get; }

        /// <summary>
        /// Gets the actual target object. This property returns <c>null</c> if the handler is static or
        /// the target is no longer alive.
        /// </summary>
        object Target { get; }

        /// <summary>
        /// Gets the type of the source.
        /// </summary>
        /// <value>The type of the source.</value>
        Type SourceType { get; }

        /// <summary>
        /// Gets the actual source object. This property returns <c>null</c> if the event is static or
        /// the source is no longer alive.
        /// </summary>
        object Source { get; }

        /// <summary>
        /// Gets the type of the event args.
        /// </summary>
        /// <value>The type of the event args.</value>
        Type EventArgsType { get; }

        /// <summary>
        /// Gets a value indicating whether the event source has not yet been garbage collected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the event source has not yet been garbage collected; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// In case of static events, this property always returns <c>false</c>.
        /// </remarks>
        bool IsSourceAlive { get; }

        /// <summary>
        /// Gets a value indicating whether the event target has not yet been garbage collected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the event target has not yet been garbage collected; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// In case of static event handlers, this property always returns <c>false</c>.
        /// </remarks>
        bool IsTargetAlive { get; }

        /// <summary>
        /// Gets a value indicating whether this instance represents a static event.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance represents a static event; otherwise, <c>false</c>.
        /// </value>
        bool IsStaticEvent { get; }

        /// <summary>
        /// Gets a value indicating whether this instance represents a static event handler.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance represents a static event handler; otherwise, <c>false</c>.
        /// </value>
        bool IsStaticEventHandler { get; }

        /// <summary>
        /// Detaches from the subscribed event.
        /// </summary>
        void Detach();
    }
}