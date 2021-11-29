namespace Catel.Tests
{
    using System;
    using System.IO;
    using Catel.Logging;

    public sealed class TemporaryFilesContext : IDisposable
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly Guid _randomGuid = Guid.NewGuid();
        private readonly string _rootDirectory;
        #endregion

        #region Constructors
        public TemporaryFilesContext(string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = _randomGuid.ToString();
            }

            _rootDirectory = Path.Combine(Path.GetTempPath(), "Catel.Tests", name);

            Directory.CreateDirectory(_rootDirectory);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Log.Info("Deleting temporary files from '{0}'", _rootDirectory);

            try
            {
                if (Directory.Exists(_rootDirectory))
                {
                    Directory.Delete(_rootDirectory, true);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to delete temporary files");
            }
        }
        #endregion

        public string GetDirectory(string relativeDirectoryName)
        {
            var fullPath = Path.Combine(_rootDirectory, relativeDirectoryName);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            return fullPath;
        }

        public string GetFile(string relativeFilePath, bool deleteIfExists = false)
        {
            var fullPath = Path.Combine(_rootDirectory, relativeFilePath);

            var directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (deleteIfExists)
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }

            return fullPath;
        }
    }
}
