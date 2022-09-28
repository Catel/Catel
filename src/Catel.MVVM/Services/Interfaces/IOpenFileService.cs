namespace Catel.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for the Open File service.
    /// </summary>
    public interface IOpenFileService : IFileSupport
	{
        /// <summary>
        /// Determines the filename of the file what will be used.
        /// </summary>
        /// <returns>The determine open file result.</returns>
        Task<DetermineOpenFileResult> DetermineFileAsync(DetermineOpenFileContext context);
	}
}
