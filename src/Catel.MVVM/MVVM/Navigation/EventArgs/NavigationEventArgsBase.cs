namespace Catel.MVVM.Navigation
{
    using System;

    /// <summary>
    /// Base class for navigation event args.
    /// </summary>
    public abstract class NavigationEventArgsBase : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationEventArgsBase"/> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="navigationMode">The navigation mode.</param>
        protected NavigationEventArgsBase(string uri, NavigationMode navigationMode)
        {
            Uri = uri;
            NavigationMode = navigationMode;
        }

        /// <summary>
        /// Gets the URI.
        /// </summary>
        /// <value>The URI.</value>
        public string Uri { get; private set; }

        /// <summary>
        /// Gets the navigation mode.
        /// </summary>
        /// <value>The navigation mode.</value>
        public NavigationMode NavigationMode { get; private set; }
    }
}
