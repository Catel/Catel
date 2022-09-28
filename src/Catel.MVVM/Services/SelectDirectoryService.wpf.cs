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
        public virtual async Task<DetermineDirectoryResult> DetermineDirectoryAsync(DetermineDirectoryContext context)
        {
            Argument.IsNotNull(nameof(context), context);

            using (var browserDialog = new FolderBrowserDialog())
            {
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
}
