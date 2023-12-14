namespace Catel.Windows
{
    using Controls;

    /// <summary>
    /// Defines the way the <see cref="InfoBarMessageControl"/> is included in the <see cref="DataWindow"/>.
    /// </summary>
    public enum InfoBarMessageControlGenerationMode
    {
        /// <summary>
        /// No <see cref="InfoBarMessageControl"/> is generated.
        /// </summary>
        None,

        /// <summary>
        /// Generate the <see cref="InfoBarMessageControl"/> as inline.
        /// </summary>
        Inline,

        /// <summary>
        /// Generate the <see cref="InfoBarMessageControl"/> as overlay.
        /// </summary>
        Overlay
    }
}
