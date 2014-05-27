// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InteractivityManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !WIN80

namespace Catel.Windows.Interactivity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Interactivity manager to manage interactivity classes.
    /// </summary>
    public class InteractivityManager : IInteractivityManager
    {
        private readonly HashSet<IBehavior> _behaviors = new HashSet<IBehavior>();
        private readonly HashSet<ITrigger> _triggers = new HashSet<ITrigger>();

        #region Events
        /// <summary>
        /// Raised when a behavior is loaded.
        /// </summary>
        public event EventHandler<BehaviorEventArgs> BehaviorLoaded;

        /// <summary>
        /// Raised when a behavior is unloaded.
        /// </summary>
        public event EventHandler<BehaviorEventArgs> BehaviorUnloaded;

        /// <summary>
        /// Raised when a trigger is loaded.
        /// </summary>
        public event EventHandler<TriggerEventArgs> TriggerLoaded;

        /// <summary>
        /// Raised when a trigger is unloaded.
        /// </summary>
        public event EventHandler<TriggerEventArgs> TriggerUnloaded;
        #endregion

        #region Methods
        /// <summary>
        /// Registers the behavior.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="behavior"/> is <c>null</c>.</exception>
        public void RegisterBehavior(IBehavior behavior)
        {
            Argument.IsNotNull("behavior", behavior);

            _behaviors.Add(behavior);

            BehaviorLoaded.SafeInvoke(this, new BehaviorEventArgs(behavior));
        }

        /// <summary>
        /// Unregisters the behavior.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="behavior"/> is <c>null</c>.</exception>
        public void UnregisterBehavior(IBehavior behavior)
        {
            Argument.IsNotNull("behavior", behavior);

            _behaviors.Remove(behavior);

            BehaviorUnloaded.SafeInvoke(this, new BehaviorEventArgs(behavior));
        }

        /// <summary>
        /// Registers the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="trigger"/> is <c>null</c>.</exception>
        public void RegisterTrigger(ITrigger trigger)
        {
            Argument.IsNotNull("trigger", trigger);

            _triggers.Add(trigger);

            TriggerLoaded.SafeInvoke(this, new TriggerEventArgs(trigger));
        }

        /// <summary>
        /// Unregisters the trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="trigger"/> is <c>null</c>.</exception>
        public void UnregisterTrigger(ITrigger trigger)
        {
            Argument.IsNotNull("trigger", trigger);

            _triggers.Remove(trigger);

            TriggerUnloaded.SafeInvoke(this, new TriggerEventArgs(trigger));
        }

        /// <summary>
        /// Gets all the currently loaded behaviors.
        /// </summary>
        /// <returns>All the behaviors.</returns>
        public IEnumerable<IBehavior> GetBehaviors()
        {
            return _behaviors.ToArray();
        }

        /// <summary>
        /// Gets all the currently loaded triggers.
        /// </summary>
        /// <returns>All the triggers.</returns>
        public IEnumerable<ITrigger> GetTriggers()
        {
            return _triggers.ToArray();
        }
    #endregion
    }
}

#endif