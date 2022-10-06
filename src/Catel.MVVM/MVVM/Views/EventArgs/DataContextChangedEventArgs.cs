namespace Catel.MVVM.Views
{
    using System;

    /// <summary>
    /// Contains information about DataContext changed events.
    /// </summary>
    public class DataContextChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextChangedEventArgs"/> class.
        /// </summary>
        /// <param name="oldContext">The old context.</param>
        /// <param name="newContext">The new context.</param>
        public DataContextChangedEventArgs(object? oldContext, object? newContext)
        {
            OldContext = oldContext;
            NewContext = newContext;
        }

        /// <summary>
        /// Gets the old context.
        /// </summary>
        /// <value>The old context.</value>
        public object? OldContext { get; private set; }

        /// <summary>
        /// Gets the new context.
        /// </summary>
        /// <value>The new context.</value>
        public object? NewContext { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the old and new context are equal.
        /// </summary>
        /// <value><c>true</c> if the old and new context are equal; otherwise, <c>false</c>.</value>
        public bool AreEqual
        {
            get { return ObjectHelper.AreEqual(OldContext, NewContext); }
        }
    }
}
