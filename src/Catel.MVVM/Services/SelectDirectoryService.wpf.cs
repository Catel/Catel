#if NET || NETCORE

namespace Catel.Services
{
    using System.Threading.Tasks;
    using System.Windows.Forms;

    /// <summary>
    /// Service to open files.
    /// </summary>
    public partial class SelectDirectoryService
    {
        /// <inheritdoc />
        [ObsoleteEx(ReplacementTypeOrMember = "DetermineDirectoryAsync(DetermineDirectoryContext)", TreatAsErrorFromVersion = "6.0", RemoveInVersion = "6.0")]
        public virtual async Task<bool> DetermineDirectoryAsync()
        {
            var browserDialog = new FolderBrowserDialog();
            browserDialog.Description = Title;
            browserDialog.ShowNewFolderButton = ShowNewFolderButton;

            var initialDirectory = InitialDirectory;

            if (!string.IsNullOrEmpty(initialDirectory))
            {
                browserDialog.SelectedPath = IO.Path.AppendTrailingSlash(initialDirectory);
            }
            else
            {
                browserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            }

            bool result = browserDialog.ShowDialog() == DialogResult.OK;
            if (result)
            {
                DirectoryName = browserDialog.SelectedPath;
            }
            else
            {
                DirectoryName = null;
            }

            return result;
        }

        /// <inheritdoc />
        public virtual async Task<DetermineDirectoryResult> DetermineDirectoryAsync(DetermineDirectoryContext context)
        {
            Argument.IsNotNull(nameof(context), context);

            var browserDialog = new FolderBrowserDialog();
            browserDialog.Description = context.Title;
            browserDialog.ShowNewFolderButton = context.ShowNewFolderButton;

            var initialDirectory = context.InitialDirectory;

            if (!string.IsNullOrEmpty(initialDirectory))
            {
                browserDialog.SelectedPath = IO.Path.AppendTrailingSlash(initialDirectory);
            }
            else
            {
                browserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            }

            var result = new DetermineDirectoryResult
            {
                Result = browserDialog.ShowDialog() == DialogResult.OK,
                DirectoryName = browserDialog.SelectedPath
            };

            return result;
        }
    }
}

#endif
