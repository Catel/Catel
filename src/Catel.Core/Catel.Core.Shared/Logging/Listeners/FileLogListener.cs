// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Logging
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// Log listener which writes all data to a file.
    /// </summary>
    public class FileLogListener : BatchLogListenerBase
    {
        private const string AppData = "{AppData}";
        private const string AppDir = "{AppDir}";
        private const string Date = "{Date}";
        private const string Time = "{Time}";
        private const string AssemblyName = "{AssemblyName}";
        private const string ProcessId = "{ProcessId}";
        private const string AutoLogFileName = "{AutoLogFileName}";
        private readonly string AutoLogFileNameReplacement = string.Format("{0}_{1}_{2}_{3}", AssemblyName, Date, Time, ProcessId);

        private readonly Assembly _assembly;
        private string _filePath;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogListener" /> class.
        /// </summary>
        /// <param name="assembly">The assembly to load the product info from. If <c>null</c>, the entry assembly will be used.</param>
        public FileLogListener(Assembly assembly = null)
        {
            MaxSizeInKiloBytes = 1000 * 10; // 10 MB

            _assembly = assembly ?? AssemblyHelper.GetEntryAssembly() ?? Assembly.GetCallingAssembly();

            if (string.IsNullOrWhiteSpace(_filePath))
            {
                _filePath = DetermineFilePath(AutoLogFileName);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogListener" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="maxSizeInKiloBytes">The max size in kilo bytes.</param>
        /// <param name="assembly">The assembly to load the product info from. If <c>null</c>, the entry assembly will be used.</param>
        /// <exception cref="ArgumentException">The <paramref name="filePath" /> is <c>null</c> or whitespace.</exception>
        public FileLogListener(string filePath, int maxSizeInKiloBytes, Assembly assembly = null)
            : this(assembly)
        {
            Argument.IsNotNullOrWhitespace(() => filePath);

            FilePath = filePath;
            MaxSizeInKiloBytes = maxSizeInKiloBytes;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>The file path.</value>
        public string FilePath
        {
            get { return _filePath; }
            set { _filePath = DetermineFilePath(value); }
        }

        /// <summary>
        /// Gets or sets the maximum size information kilo bytes.
        /// </summary>
        /// <value>The maximum size information kilo bytes.</value>
        public int MaxSizeInKiloBytes { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Writes the batch of entries.
        /// </summary>
        /// <param name="batchEntries">The batch entries.</param>
        protected override async Task WriteBatch(System.Collections.Generic.List<LogBatchEntry> batchEntries)
        {
            try
            {
                var filePath = FilePath;

                var directoryName = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrWhiteSpace(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists && (fileInfo.Length / 1024 >= MaxSizeInKiloBytes))
                {
                    CreateCopyOfCurrentLogFile(FilePath);
                }

                using (var fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    using (var writer = new StreamWriter(fileStream))
                    {
                        foreach (var batchEntry in batchEntries)
                        {
                            var message = FormatLogEvent(batchEntry.Log, batchEntry.Message, batchEntry.LogEvent, batchEntry.ExtraData, batchEntry.Time);

                            writer.WriteLine(message);
                        }

                        writer.Flush();
                    }
                }
            }
            catch (Exception)
            {
                // Swallow
            }
        }

        private void CreateCopyOfCurrentLogFile(string filePath)
        {
            for (int i = 1; i < 999; i++)
            {
                var possibleFilePath = string.Format("{0}.{1:000}", filePath, i);
                if (!File.Exists(possibleFilePath))
                {
                    File.Move(filePath, possibleFilePath);
                }
            }
        }

        private string DetermineFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                filePath = string.Empty;
            }

            if (filePath.Contains(AutoLogFileName))
            {
                filePath = filePath.Replace(AutoLogFileName, AutoLogFileNameReplacement);
            }

            string dataDirectory;

            if (_assembly != null)
            {
                dataDirectory = IO.Path.GetApplicationDataDirectory(_assembly.Company(), _assembly.Product());
            }
            else
            {
                dataDirectory = IO.Path.GetApplicationDataDirectory();
            }

            if (filePath.Contains(AssemblyName))
            {
                filePath = filePath.Replace(AssemblyName, _assembly.GetName().Name);
            }

            if (filePath.Contains(ProcessId))
            {
                filePath = filePath.Replace(ProcessId, Process.GetCurrentProcess().Id.ToString());
            }

            if (filePath.Contains(Date))
            {
                filePath = filePath.Replace(Date, DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }

            if (filePath.Contains(Time))
            {
                filePath = filePath.Replace(Time, DateTime.Now.ToString("HHmmss", CultureInfo.InvariantCulture));
            }

            if (filePath.Contains(AppData))
            {
                filePath = filePath.Replace(AppData, dataDirectory);
            }

            if (filePath.Contains(AppDir))
            {
                filePath = filePath.Replace(AppDir, AppDomain.CurrentDomain.BaseDirectory);
            }

            filePath = IO.Path.GetFullPath(filePath, dataDirectory);

            if (!filePath.EndsWith(".log"))
            {
                filePath += ".log";
            }

            return filePath;
        }
        #endregion
    }
}

#endif