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
        /// <inheritdoc/>
        public virtual async Task<DetermineSaveFileResult> DetermineFileAsync(DetermineSaveFileContext context)
        {
            Argument.IsNotNull("context", context);

            var fileDialog = new SaveFileDialog();
            ConfigureFileDialog(fileDialog, context);

            var result = new DetermineSaveFileResult
            {
                Result = fileDialog.ShowDialog() ?? false,
                FileName = fileDialog.FileName
            };

            return result;
        }
    }
}

#endif
