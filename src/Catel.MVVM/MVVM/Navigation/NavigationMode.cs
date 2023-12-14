namespace Catel.MVVM.Navigation
{
    /// <summary>
    /// Navigation modes.
    /// </summary>
    public enum NavigationMode
    {
        /// <summary>
        /// Navigating back.
        /// </summary>
        Back,

        /// <summary>
        /// Navigating forward.
        /// </summary>
        Forward,

        /// <summary>
        /// Navigating to a new page.
        /// </summary>
        New,

        /// <summary>
        /// Refreshing current view.
        /// </summary>
        Refresh,

        /// <summary>
        /// The navigation mode is unknown at this stage.
        /// </summary>
        Unknown
    }
}
