namespace Catel.Services
{
    using System;
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
            ArgumentNullException.ThrowIfNull(context);

            var fileDialog = new OpenFileDialog();

            await ConfigureFileDialogAsync(fileDialog, context);

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
