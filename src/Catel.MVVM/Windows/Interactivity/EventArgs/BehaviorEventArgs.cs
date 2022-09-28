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
