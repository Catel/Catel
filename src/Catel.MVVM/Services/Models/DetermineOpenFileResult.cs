namespace Catel.Services
{
    /// <summary>
    /// The result of a determine open file call.
    /// </summary>
    public class DetermineOpenFileResult : DetermineFileResult
    {
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string[] FileNames { get; set; }

#if UWP
        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>The file.</value>
		public StorageFile[] Files { get; set; }
#endif
    }
}
