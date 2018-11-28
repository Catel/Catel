// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveFileService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Services
{
    using System.Threading.Tasks;
    using Microsoft.Win32;

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
        public virtual async Task<bool> DetermineFileAsync()
        {
            var fileDialog = new SaveFileDialog();
            ConfigureFileDialog(fileDialog);

            bool result = fileDialog.ShowDialog() ?? false;
            if (result)
            {
                FileName = fileDialog.FileName;
            }

            return result;
        }
    }
}

#endif
