namespace Catel.Services
{
    /// <summary>
    /// The result of a determine directory call.
    /// </summary>
    public class DetermineDirectoryResult
    {
        /// <summary>
        /// Gets the result of the selection.
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// Gets the directory name.
        /// </summary>
        /// <value>The name of the directory.</value>
        public string? DirectoryName { get; set; }
    }
}
