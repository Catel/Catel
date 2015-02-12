// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInteractivityManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !WIN80

namespace Catel.Windows.Interactivity
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interactivity manager to manage interactivity classes.
    /// </summary>
    [ObsoleteEx(Message = "Unused feature, will be removed", TreatAsErrorFromVersion = "4.1", RemoveInVersion = "5.0")]
    public interface IInteractivityManager
    {
        /// <summary>
        /// Raised when a behavior is loaded.
        /// </summary>
        event EventHandler<BehaviorEventArgs> BehaviorLoaded;

        /// <summary>
        /// Raised when a behavior is unloaded.
        /// </summary>
        event EventHandler<BehaviorEventArgs> BehaviorUnloaded;

        /// <summary>
        /// Raised when a trigger is loaded.
        /// </summary>
        event EventHandler<TriggerEventArgs> TriggerLoaded;

        /// <summary>
        /// Raised when a trigger is unloaded.
        /// </summary>
        event EventHandler<TriggerEventArgs> TriggerUnloaded;

        /// <summary>
        /// Registers the behavior.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="behavior"/> is <c>null</c>.</exception>
        void RegisterBehavior(IBehavior behavior);

        /// <summary>
        /// Unregisters the behavior.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="behavior"/> is <c>null</c>.</exception>
        void UnregisterBehavior(IBehavior behavior);

        /// <summary>
        /// Registers the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="trigger"/> is <c>null</c>.</exception>
        void RegisterTrigger(ITrigger trigger);

        /// <summary>
        /// Unregisters the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="trigger"/> is <c>null</c>.</exception>
        void UnregisterTrigger(ITrigger trigger);

        /// <summary>
        /// Gets all the currently loaded behaviors.
        /// </summary>
        /// <returns>All the behaviors.</returns>
        IEnumerable<IBehavior> GetBehaviors();

        /// <summary>
        /// Gets all the currently loaded triggers.
        /// </summary>
        /// <returns>All the triggers.</returns>
        IEnumerable<ITrigger> GetTriggers();
    }
}

#endif