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
        /// <inheritdoc/>
        public virtual async Task<DetermineOpenFileResult> DetermineFileAsync(DetermineOpenFileContext context)
        {
            Argument.IsNotNull("context", context);

            var fileDialog = new OpenFileDialog();
            ConfigureFileDialog(fileDialog, context);

            fileDialog.Multiselect = context.IsMultiSelect;

            var result = new DetermineOpenFileResult
            {
                Result = fileDialog.ShowDialog() ?? false,
                FileName = fileDialog.FileName,
                FileNames = fileDialog.FileNames,
            };

            return result;
        }
    }
}

#endif
