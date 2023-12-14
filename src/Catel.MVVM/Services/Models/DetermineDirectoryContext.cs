namespace Catel.Services
{
    /// <summary>
    /// The context to use for a determine directory call.
    /// </summary>
    public class DetermineDirectoryContext
    {
        /// <summary>
        /// Gets the directory name.
        /// </summary>
        /// <value>The name of the directory.</value>
        public string? DirectoryName { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the new folder button to be able to create new folders while browsing.
        /// </summary>
        /// <value><c>true</c> if the new folder button should be shown; otherwise, <c>false</c>.</value>
        public bool ShowNewFolderButton { get; set; }

        /// <summary>
        /// Gets or sets the initial directory.
        /// </summary>
        /// <value>
        /// The initial directory.
        /// </value>
        public string? InitialDirectory { get; set; }

        /// <summary>
        /// Gets or sets the title which will be used for display.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the filter to use when opening or saving the file.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public string? Filter { get; set; }
    }
}
