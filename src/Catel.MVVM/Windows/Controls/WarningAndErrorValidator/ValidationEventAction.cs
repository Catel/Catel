namespace Catel.Windows.Controls
{
    /// <summary>
    /// Validation event action.
    /// </summary>
    public enum ValidationEventAction
    {
        /// <summary>
        /// Added.
        /// </summary>
        Added,

        /// <summary>
        /// Removed.
        /// </summary>
        Removed,

        /// <summary>
        /// Refresh the validation, don't add or remove.
        /// </summary>
        Refresh,

        /// <summary>
        /// All validation info of the specified object should be cleared.
        /// </summary>
        ClearAll
    }
}
