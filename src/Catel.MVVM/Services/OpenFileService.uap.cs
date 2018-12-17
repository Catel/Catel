// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenFileService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if UWP

namespace Catel.Services
{
    using Logging;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using global::Windows.Storage;

    /// <summary>
    /// Service to open files.
    /// </summary>
    public partial class OpenFileService
    {
        #region IFileSupport Members
        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns>The <see cref="StorageFile"/> of the file or <c>null</c> if no file was selected by the user.</returns>
        /// <remarks>
        /// If this method returns a valid <see cref="StorageFile"/> object, the <c>FileName</c> property will be filled 
        /// with the safe filename. This can be used for display purposes only.
        /// </remarks>
        public virtual async Task<StorageFile[]> DetermineFileAsync()
        {
            var fileDialog = new global::Windows.Storage.Pickers.FileOpenPicker();

            var filters = Filter?.Split(';');
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    var fileFilter = filter;

                    // Support full .NET filters (like "Text files|*.txt") as well
                    if (fileFilter.Contains("|"))
                    {
                        var splittedFilters = fileFilter.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        if (splittedFilters.Length == 2)
                        {
                            fileFilter = splittedFilters[1];
                        }
                        else
                        {
                            Log.Warning($"Failed to parse filter '{fileFilter}'");

                            fileFilter = null;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(fileFilter))
                    {
                        if (fileFilter.StartsWith("*"))
                        {
                            fileFilter = fileFilter.Replace("*", string.Empty);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(fileFilter))
                    {
                        fileDialog.FileTypeFilter.Add(fileFilter);
                    }
                }
            }

            var files = new List<StorageFile>();

            if (IsMultiSelect)
            {
                var foundFiles = await fileDialog.PickMultipleFilesAsync();
                if (foundFiles != null)
                {
                    files.AddRange(foundFiles);
                }
            }
            else
            {
                var foundFile = await fileDialog.PickSingleFileAsync();
                if (foundFile != null)
                {
                    files.Add(foundFile);
                }
            }

            FileName = (files.Count > 0) ? files[0].Path : null;
            FileNames = (files.Count > 0) ? files.Select(x => x.Path).ToArray() : null;

            return (files.Count > 0) ? files.ToArray() : null;
        }
        #endregion
    }
}

#endif
