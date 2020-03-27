#if UWP

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
        /// <inheritdoc />
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineDirectoryAsync(DetermineDirectoryContext)", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        public virtual async Task<StorageFolder> DetermineDirectoryAsync()
        {
            var folderPicker = new FolderPicker();

            folderPicker.FileTypeFilter.Add(Filter);

            var folder = await folderPicker.PickSingleFolderAsync();

            DirectoryName = folder?.Path;

            return folder;
        }

        /// <inheritdoc /> 
        public virtual async Task<DetermineDirectoryResult> DetermineDirectoryAsync(DetermineDirectoryContext context)
        {
            Argument.IsNotNull(() => context);

            var folderPicker = new FolderPicker();

            folderPicker.FileTypeFilter.Add(context.Filter);

            var folder = await folderPicker.PickSingleFolderAsync();

            var result = new DetermineDirectoryResult
            {
                Result = folder != null,
                Directory = folder,
                DirectoryName = folder?.Path
            };

            return result;
        }
    }
}

#endif
