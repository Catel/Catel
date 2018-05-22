// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TriggerEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.Windows.Interactivity
{
    using System;

    /// <summary>
    /// Trigger event args.
    /// </summary>
    public class TriggerEventArgs : EventArgs
    {
        /// <summary>
        /// Creates the event args.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="trigger"/> is <c>null</c>.</exception>
        public TriggerEventArgs(ITrigger trigger)
        {
            Argument.IsNotNull("trigger", trigger);

            Trigger = trigger;
        }

        /// <summary>
        /// Gets the trigger.
        /// </summary>
        public ITrigger Trigger { get; private set; }
    }
}

#endif