// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SelectDirectoryService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE

namespace Catel.Services
{
    using System;
    using System.Threading.Tasks;
    using global::Windows.Storage;
    using global::Windows.Storage.Pickers;

    /// <summary>
    /// Service to open files.
    /// </summary>
    public partial class SelectDirectoryService
    {
        /// <summary>
        /// Determines the DirectoryName of the Directory what will be used.
        /// </summary>
        /// <returns>
        /// <c>true</c> if a directory is selected; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// If this method returns <c>true</c>, the <see cref="DirectoryName"/> property will be filled with the directory name. Otherwise,
        /// no changes will occur to the data of this object.
        /// </remarks>
        public virtual async Task<StorageFolder> DetermineDirectoryAsync()
        {
            var folderPicker = new FolderPicker();

            folderPicker.FileTypeFilter.Add(Filter);

            var folder = await folderPicker.PickSingleFolderAsync();

            DirectoryName = folder?.Path;

            return folder;
        }
    }
}

#endif