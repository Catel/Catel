// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BehaviorEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !WIN80

namespace Catel.Windows.Interactivity
{
    using System;

    /// <summary>
    /// Behavior event args.
    /// </summary>
    public class BehaviorEventArgs : EventArgs
    {
        /// <summary>
        /// Creates the event args.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="behavior"/> is <c>null</c>.</exception>
        public BehaviorEventArgs(IBehavior behavior)
        {
            Argument.IsNotNull("behavior", behavior);

            Behavior = behavior;
        }

        /// <summary>
        /// Gets the behavior.
        /// </summary>
        public IBehavior Behavior { get; private set; }
    }
}

#endif