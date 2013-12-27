namespace Catel.MVVM
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Command manager for view models.
    /// </summary>
    public interface IViewModelCommandManager
    {
        /// <summary>
        /// Adds a new handler when a command is executed on the specified view model.
        /// </summary>
        /// <param name="handler">The handler to execute when a command is executed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        void AddHandler(Action<IViewModel, string, ICommand, object> handler);

        /// <summary>
        /// Invalidates all the commands that implement the <see cref="ICatelCommand"/>.
        /// </summary>
        /// <param name="force">If <c>true</c>, the commands are re-initialized. The default value is <c>false</c>.</param>
        void InvalidateCommands(bool force = false);

        /// <summary>
        /// Add command binding for routed command
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="routedCommand"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="commandToExecute"/> is <c>null</c>.</exception>
        void AddCommandBinding(ICommand routedCommand, ICommand commandToExecute);

        /// <summary>
        /// Remove command binding for routed command
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="routedCommand"/> is <c>null</c>.</exception>
        void RemoveCommandBinding(ICommand routedCommand);

        /// <summary>
        /// Remove command binding for routed command
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="routedCommand"/> is <c>null</c>.</exception>
        ICommand GetCommandForRoutedCommand(ICommand routedCommand);
    }
}