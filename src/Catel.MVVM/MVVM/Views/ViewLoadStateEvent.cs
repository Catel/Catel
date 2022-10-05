namespace Catel.MVVM.Views
{
    /// <summary>
    /// Available view load state events.
    /// </summary>
    public enum ViewLoadStateEvent
    {
        /// <summary>
        /// The view is about to be loaded.
        /// </summary>
        Loading,

        /// <summary>
        /// The view has just been loaded.
        /// </summary>
        Loaded,

        /// <summary>
        /// The view is about to be unloaded.
        /// </summary>
        Unloading,

        /// <summary>
        /// The view has just been unloaded.
        /// </summary>
        Unloaded
    }
}
