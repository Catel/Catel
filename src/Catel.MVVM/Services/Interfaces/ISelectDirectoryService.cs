namespace Catel.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for the Select Directory service.
    /// </summary>
    public interface ISelectDirectoryService
    {
        /// <summary>
        /// Determines the name of the directory what will be used.
        /// </summary>
        /// <returns>
        /// <c>true</c> if a directory is selected; otherwise <c>false</c>.
        /// </returns>
        Task<DetermineDirectoryResult> DetermineDirectoryAsync(DetermineDirectoryContext context);
    }
}
