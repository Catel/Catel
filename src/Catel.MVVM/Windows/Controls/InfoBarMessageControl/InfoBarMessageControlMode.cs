namespace Catel.Windows.Controls
{
    /// <summary>
    /// The display mode for the <see cref="InfoBarMessageControl"/>.
    /// </summary>
    public enum InfoBarMessageControlMode
    {
        /// <summary>
        /// Displays the control inline, which means all controls below are moved down a bit when the
        /// control becomes visible.
        /// </summary>
        Inline,

        /// <summary>
        /// Displays the control as an overlay, which might lead to overlapping of existing controls.
        /// </summary>
        Overlay
    }
}
