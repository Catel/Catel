namespace Catel.MVVM
{
    /// <summary>
    /// View model lifetime management options.
    /// </summary>
    public enum ViewModelLifetimeManagement
    {
        /// <summary>
        /// Automatic view model lifetime management.
        /// </summary>
        Automatic,

        /// <summary>
        /// Partly manual, view model will be created and initialized, but not be closed.
        /// </summary>
        PartlyManual,

        /// <summary>
        /// Fully manual, view model will not be created.
        /// </summary>
        FullyManual,
    }
}
