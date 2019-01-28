// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OpenFileService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Services
{
    using System.Threading.Tasks;
    using Microsoft.Win32;

    /// <summary>
    /// Service to open files.
    /// </summary>
    public partial class OpenFileService
    {
        #region IFileSupport Members
        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns><c>true</c> if a file is selected; otherwise <c>false</c>.</returns>
        /// <remarks>
        /// If this method returns <c>true</c>, the <c>FileName</c> property will be filled with the filename. Otherwise,
        /// no changes will occur to the data of this object.
        /// </remarks>
        public virtual async Task<bool> DetermineFileAsync()
        {
            var fileDialog = new OpenFileDialog();
            ConfigureFileDialog(fileDialog);

            fileDialog.Multiselect = IsMultiSelect;

            bool result = fileDialog.ShowDialog() ?? false;
            if (result)
            {
                FileName = fileDialog.FileName;
                FileNames = fileDialog.FileNames;
            }
            else
            {
                FileName = null;
                FileNames = null;
            }

            return result;
        }
        #endregion
    }
}

#endif
