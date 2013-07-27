﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileLogListener.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Logging
{
    using System;
    using System.IO;

    /// <summary>
    /// Log listener which writes all data to a file.
    /// </summary>
    public class FileLogListener : BatchLogListenerBase
    {
        private readonly string _filePath;
        private readonly int _maxSizeInKiloBytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogListener" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="maxSizeInKiloBytes">The max size in kilo bytes.</param>
        /// <exception cref="ArgumentException">The <paramref name="filePath" /> is <c>null</c> or whitespace.</exception>
        public FileLogListener(string filePath, int maxSizeInKiloBytes)
        {
            Argument.IsNotNullOrWhitespace(() => filePath);

            _filePath = filePath;
            _maxSizeInKiloBytes = maxSizeInKiloBytes;
        }

        /// <summary>
        /// Writes the batch of entries.
        /// </summary>
        /// <param name="batchEntries">The batch entries.</param>
        protected override void WriteBatch(System.Collections.Generic.List<LogBatchEntry> batchEntries)
        {
            try
            {
                var fileInfo = new FileInfo(_filePath);
                if (fileInfo.Exists && (fileInfo.Length / 1024 >= _maxSizeInKiloBytes))
                {
                    CreateCopyOfCurrentLogFile(_filePath);
                }

                using (var fileStream = new FileStream(_filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    using (var writer = new StreamWriter(fileStream))
                    {
                        foreach (var batchEntry in batchEntries)
                        {
                            var message = FormatLogEvent(batchEntry.Log, batchEntry.Message, batchEntry.LogEvent, batchEntry.ExtraData);

                            writer.WriteLine(message);
                        }
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
    }
}