namespace Catel.MVVM
{
    /// <summary>
    /// Interface for task progress report.
    /// </summary>
    public interface ITaskProgressReport
    {
        /// <summary>
        /// Status of the task progress.
        /// </summary>
        /// <value>The status.</value>
        string Status { get; }
    }
}
