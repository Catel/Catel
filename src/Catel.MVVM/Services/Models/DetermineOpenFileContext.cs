namespace Catel.Services
{
    /// <summary>
    /// The context of a determine open file call.
    /// </summary>
    public class DetermineOpenFileContext : DetermineFileContext
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is multi select.
        /// </summary>
        /// <value><c>true</c> if this instance is multi select; otherwise, <c>false</c>.</value>
        public bool IsMultiSelect { get; set; }
    }
}
