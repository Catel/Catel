namespace Catel.Windows
{
    /// <summary>
    /// Mode of the <see cref="DataWindow"/>.
    /// </summary>
    public enum DataWindowMode
    {
        /// <summary>
        /// Window contains OK and Cancel buttons.
        /// </summary>
        OkCancel,

        /// <summary>
        /// Window contains OK, Cancel and Apply buttons.
        /// </summary>
        OkCancelApply,

        /// <summary>
        /// Window contains Close button.
        /// </summary>
        Close,

        /// <summary>
        /// Window contains custom buttons.
        /// </summary>
        Custom
    }
}
