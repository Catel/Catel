namespace Catel.Services
{
    using System;

    /// <summary>
    /// The result of a determine open file call.
    /// </summary>
    public class DetermineOpenFileResult : DetermineFileResult
    {
        public DetermineOpenFileResult()
        {
            FileNames = Array.Empty<string>();
        }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string[] FileNames { get; set; }
    }
}
