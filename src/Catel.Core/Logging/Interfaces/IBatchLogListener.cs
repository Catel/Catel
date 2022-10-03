namespace Catel.Logging
{
    using System.Threading.Tasks;

    /// <summary>
    /// Log listener base which allows to write log files in batches.
    /// </summary>
    public interface IBatchLogListener
    {
        /// <summary>
        /// Flushes the current queue asynchronous.
        /// </summary>
        /// <returns>Task so it can be awaited.</returns>
        Task FlushAsync();
    }
}
