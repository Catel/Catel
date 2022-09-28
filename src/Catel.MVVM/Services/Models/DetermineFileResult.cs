namespace Catel.Services
{
    /// <summary>
    /// The result of a determine file call.
    /// </summary>
    public abstract class DetermineFileResult
    {
        /// <summary>
        /// Gets the result of the selection.
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }
    }
}
