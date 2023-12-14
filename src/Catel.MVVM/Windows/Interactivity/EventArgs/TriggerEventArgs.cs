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
            ArgumentNullException.ThrowIfNull(trigger);

            Trigger = trigger;
        }

        /// <summary>
        /// Gets the trigger.
        /// </summary>
        public ITrigger Trigger { get; private set; }
    }
}
