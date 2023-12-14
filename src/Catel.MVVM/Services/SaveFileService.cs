namespace Catel.Services
{
    using Logging;

    /// <summary>
    /// Service to save files.
    /// </summary>
    public partial class SaveFileService : FileServiceBase, ISaveFileService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
    }
}
