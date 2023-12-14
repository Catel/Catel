namespace Catel.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for the Save File service.
    /// </summary>
    public interface ISaveFileService : IFileSupport
	{
        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns>The determine save file result.</returns>
        Task<DetermineSaveFileResult> DetermineFileAsync(DetermineSaveFileContext context);
    }
}
