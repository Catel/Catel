// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveFileService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if UWP

namespace Catel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel.Logging;
    using global::Windows.Storage;
    using global::Windows.Storage.Pickers;

    /// <summary>
    /// Service to save files.
    /// </summary>
    public partial class SaveFileService
    {
        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns><c>true</c> if a file is selected; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// If this method returns <c>true</c>, the <see cref="FileServiceBase.FileName"/> property will be filled with the filename. Otherwise,
        /// no changes will occur to the data of this object.
        /// </remarks>
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineFileContext / DetermineFileResult", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        public virtual async Task<StorageFile> DetermineFileAsync()
        {
            var fileDialog = new FileSavePicker();

            var filters = Filter?.Split(';');
            if (filters is not null)
            {
                foreach (var filter in filters)
                {
                    var filterName = string.Empty;
                    var fileFilters = new List<string>();
                    var fileFilter = filter;

                    // Support full .NET filters (like "Text files|*.txt") as well
                    if (fileFilter.Contains("|"))
                    {
                        var splittedFilters = fileFilter.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        if (splittedFilters.Length == 2)
                        {
                            filterName = splittedFilters[0];
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
                        fileFilters.AddRange(fileFilter.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                    }

                    for (int i = 0; i < fileFilters.Count; i++)
                    {
                        fileFilters[i] = fileFilters[i].Trim().Replace("*", string.Empty);
                    }

                    if (fileFilters.Count > 0)
                    {
                        fileDialog.FileTypeChoices.Add(filterName, fileFilters);
                    }
                }
            }

            fileDialog.SuggestedFileName = FileName;

            var file = await fileDialog.PickSaveFileAsync();
            if (file is not null)
            {
                FileName = file.Path;
            }

            return file;
        }
    }
}

#endif
