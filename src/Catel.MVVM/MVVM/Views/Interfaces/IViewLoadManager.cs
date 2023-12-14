namespace Catel.MVVM.Views
{
    using System;

    /// <summary>
    /// Manager that handles top =&gt; bottom loaded events for all views inside an application.
    /// <para>
    /// </para>
    /// The reason this class is built is that in non-WPF technologies, the visual tree is loaded from
    /// bottom =&gt; top. However, Catel heavily relies on the order to be top =&gt; bottom.
    /// </summary>
    public interface IViewLoadManager
    {
        /// <summary>
        /// Adds the view load state.
        /// </summary>
        /// <param name="viewLoadState">The view load state.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="viewLoadState" /> is <c>null</c>.</exception>
        void AddView(IViewLoadState viewLoadState);

        /// <summary>
        /// Cleans up the dead links.
        /// </summary>
        void CleanUp();

        /// <summary>
        /// Occurs when any of the subscribed framework elements are about to be loaded.
        /// </summary>
        event EventHandler<ViewLoadEventArgs>? ViewLoading;

        /// <summary>
        /// Occurs when any of the subscribed framework elements are loaded.
        /// </summary>
        event EventHandler<ViewLoadEventArgs>? ViewLoaded;

        /// <summary>
        /// Occurs when any of the subscribed framework elements are about to be unloaded.
        /// </summary>
        event EventHandler<ViewLoadEventArgs>? ViewUnloading;

        /// <summary>
        /// Occurs when any of the subscribed framework elements are unloaded.
        /// </summary>
        event EventHandler<ViewLoadEventArgs>? ViewUnloaded;
    }
}
