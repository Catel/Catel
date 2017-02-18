// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveFileService.cs" company="Catel development team">
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
        public virtual async Task<StorageFile> DetermineFileAsync()
        {
            var fileDialog = new FileSavePicker();

            fileDialog.DefaultFileExtension = Filter;
            fileDialog.SuggestedFileName = FileName;

            var file = await fileDialog.PickSaveFileAsync();
            if (file != null)
            {
                FileName = file.Path;
            }

            return file;
        }
    }
}

#endif