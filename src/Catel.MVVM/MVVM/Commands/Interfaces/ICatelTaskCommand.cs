namespace Catel.MVVM
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Advanced <see cref="ICommand" /> interface definition to provide advanced functionality.
    /// Supports async/await/Task methods for commands with progress reporting and cancellation token.
    /// </summary>
    /// <typeparam name="TProgress">The type of the progress.</typeparam>
    public interface ICatelTaskCommand<TProgress> : ICatelCommand
        where TProgress : ITaskProgressReport
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is executing.
        /// </summary>
        /// <value><c>true</c> if this instance is executing; otherwise, <c>false</c>.</value>
        bool IsExecuting { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is cancellation requested.
        /// </summary>
        /// <value><c>true</c> if this instance is cancellation requested; otherwise, <c>false</c>.</value>
        bool IsCancellationRequested { get; }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        /// <value>The cancel command.</value>
        Command? CancelCommand { get; }

        /// <summary>
        /// Requests cancellation of the command.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Occurs when the command is about to execute.
        /// </summary>
        event EventHandler<CommandCanceledEventArgs>? Executing;

        /// <summary>
        /// Occurs when the command is canceled.
        /// </summary>
        event EventHandler<CommandEventArgs>? Canceled;

        /// <summary>
        /// Raised for each reported progress value.
        /// </summary>
        event EventHandler<CommandProgressChangedEventArgs<TProgress>>? ProgressChanged;
    }
}
