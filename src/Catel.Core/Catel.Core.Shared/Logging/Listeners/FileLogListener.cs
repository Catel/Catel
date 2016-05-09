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

    using Catel.IO;

    using Path = System.IO.Path;

    /// <summary>
    /// Log listener which writes all data to a file.
    /// </summary>
    public class FileLogListener : BatchLogListenerBase
    {
        private const string AppData = "{AppData}";
        private const string AppDataLocal = "{AppDataLocal}";
        private const string AppDataRoaming = "{AppDataRoaming}";
        private const string AppDataMachine = "{AppDataMachine}";
        private const string AppDir = "{AppDir}";
        private const string Date = "{Date}";
        private const string Time = "{Time}";
        private const string AssemblyName = "{AssemblyName}";
        private const string ProcessId = "{ProcessId}";
        private const string AutoLogFileName = "{AutoLogFileName}";
        private readonly string AutoLogFileNameReplacement = string.Format("{0}_{1}_{2}_{3}", AssemblyName, Date, Time, ProcessId);

        private Assembly _assembly;
        private string _filePath;

#region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogListener" /> class.
        /// </summary>
        /// <param name="assembly">The assembly to load the product info from. If <c>null</c>, the entry assembly will be used.</param>
        public FileLogListener(Assembly assembly = null)
        {
            Initialize(true, assembly);

            MaxSizeInKiloBytes = 1000 * 10; // 10 MB
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogListener" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="maxSizeInKiloBytes">The max size in kilo bytes.</param>
        /// <param name="assembly">The assembly to load the product info from. If <c>null</c>, the entry assembly will be used.</param>
        /// <exception cref="ArgumentException">The <paramref name="filePath" /> is <c>null</c> or whitespace.</exception>
        public FileLogListener(string filePath, int maxSizeInKiloBytes, Assembly assembly = null)
        {
            Argument.IsNotNullOrWhitespace(() => filePath);

            Initialize(false, assembly);

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
        /// Determines the real file path.
        /// </summary>
        /// <param name="filePath">The file path to examine.</param>
        /// <returns>The real file path.</returns>
        protected virtual string DetermineFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                filePath = string.Empty;
            }

            if (filePath.Contains(AutoLogFileName))
            {
                filePath = filePath.Replace(AutoLogFileName, AutoLogFileNameReplacement);
            }

            var isWebApp = HttpContextHelper.HasHttpContext();

            string dataDirectory;

            if (_assembly != null)
            {
                dataDirectory = isWebApp ? IO.Path.GetApplicationDataDirectoryForAllUsers(_assembly.Company(), _assembly.Product()) 
                                         : IO.Path.GetApplicationDataDirectory(_assembly.Company(), _assembly.Product());
            }
            else
            {
                dataDirectory = isWebApp ? IO.Path.GetApplicationDataDirectoryForAllUsers() 
                                         : IO.Path.GetApplicationDataDirectory();
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

            if (filePath.Contains(AppDataLocal))
            {
                var dataDirectoryLocal = _assembly != null ? IO.Path.GetApplicationDataDirectory(ApplicationDataTarget.UserLocal, 
                                                                                                 _assembly.Company(), 
                                                                                                 _assembly.Product()) 
                                                            : IO.Path.GetApplicationDataDirectory(ApplicationDataTarget.UserLocal);


                filePath = filePath.Replace(AppDataLocal, dataDirectoryLocal);
            }

            if (filePath.Contains(AppDataRoaming))
            {
                var dataDirectoryRoaming = _assembly != null ? IO.Path.GetApplicationDataDirectory(ApplicationDataTarget.UserRoaming, 
                                                                                                   _assembly.Company(), 
                                                                                                   _assembly.Product()) 
                                                             : IO.Path.GetApplicationDataDirectory(ApplicationDataTarget.UserRoaming);


                filePath = filePath.Replace(AppDataRoaming, dataDirectoryRoaming);
            }

            if (filePath.Contains(AppDataMachine))
            {
                var dataDirectoryMachine = _assembly != null ? IO.Path.GetApplicationDataDirectory(ApplicationDataTarget.Machine, 
                                                                                                   _assembly.Company(), 
                                                                                                   _assembly.Product()) 
                                                             : IO.Path.GetApplicationDataDirectory(ApplicationDataTarget.Machine);

                filePath = filePath.Replace(AppDataMachine, dataDirectoryMachine);
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

        private void Initialize(bool initFilePath, Assembly assembly = null)
        {
           _assembly = assembly ?? AssemblyHelper.GetEntryAssembly() ?? Assembly.GetCallingAssembly();

            if (initFilePath && string.IsNullOrWhiteSpace(_filePath))
            {
                _filePath = DetermineFilePath(AutoLogFileName);
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
#endregion
    }
}

#endif