namespace Catel.MVVM
{
    using System;
    using System.Threading.Tasks;
    using Auditing;

    /// <summary>
    /// Container for application-wide commands.
    /// </summary>
    public abstract class CommandContainerBase : CommandContainerBase<object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandContainerBase"/> class.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="commandManager">The command manager.</param>
        protected CommandContainerBase(string commandName, ICommandManager commandManager)
            : base(commandName, commandManager)
        {
        }
    }

    /// <summary>
    /// Container for application-wide commands.
    /// </summary>
    /// <typeparam name="TParameter">The type of the command parameter.</typeparam>
    public abstract class CommandContainerBase<TParameter> : CommandContainerBase<TParameter, TParameter, ITaskProgressReport>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandContainerBase{TParameter}"/> class.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="commandManager">The command manager.</param>
        protected CommandContainerBase(string commandName, ICommandManager commandManager)
            : base(commandName, commandManager)
        {
        }
    }

    /// <summary>
    /// Container for application-wide commands.
    /// </summary>
    /// <typeparam name="TExecuteParameter">The type of the command execute parameter.</typeparam>
    /// <typeparam name="TCanExecuteParameter">The type of the command can execute parameter.</typeparam>
    public abstract class CommandContainerBase<TExecuteParameter, TCanExecuteParameter> : CommandContainerBase<TExecuteParameter, TCanExecuteParameter, ITaskProgressReport>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandContainerBase{TExecuteParameter, TCanExecuteParameter}"/> class.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="commandManager">The command manager.</param>
        protected CommandContainerBase(string commandName, ICommandManager commandManager)
            : base(commandName, commandManager)
        {
        }
    }

    /// <summary>
    /// Container for application-wide commands.
    /// </summary>
    /// <typeparam name="TExecuteParameter">The type of the command execute parameter.</typeparam>
    /// <typeparam name="TCanExecuteParameter">The type of the command can execute parameter.</typeparam>
    /// <typeparam name="TPogress">The type of the pogress.</typeparam>
    public abstract class CommandContainerBase<TExecuteParameter, TCanExecuteParameter, TPogress> 
        where TPogress : ITaskProgressReport
    {
        private readonly ICatelCommand _command;
        private readonly ICommandManager _commandManager;
        private readonly ICompositeCommand _compositeCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandContainerBase{TExecuteParameter, TCanExecuteParameter, TPogress}"/> class.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="commandManager">The command manager.</param>
        protected CommandContainerBase(string commandName, ICommandManager commandManager)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);
            ArgumentNullException.ThrowIfNull(commandManager);

            CommandName = commandName;
            _commandManager = commandManager;

            _compositeCommand = (ICompositeCommand) _commandManager.GetCommand(commandName);
            _command = new TaskCommand<TExecuteParameter, TCanExecuteParameter, TPogress>(ExecuteInternalAsync, CanExecute);

            _commandManager.RegisterCommand(commandName, _command);
        }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        /// <value>The name of the command.</value>
        public string CommandName { get; private set; }

        /// <summary>
        /// Invalidates the command.
        /// </summary>
        protected void InvalidateCommand()
        {
            _compositeCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Determines whether the command can be executed.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns><c>true</c> if this instance can execute the specified parameter; otherwise, <c>false</c>.</returns>
        protected virtual bool CanExecute(TCanExecuteParameter? parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Task.</returns>
        private async Task ExecuteInternalAsync(TExecuteParameter? parameter)
        {
            await ExecuteAsync(parameter);

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
            AuditingManager.OnCommandExecuted(null, CommandName, _command, parameter);
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
        }

        /// <summary>
        /// Execute the command as an asynchronous operation.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Task.</returns>
        protected virtual async Task ExecuteAsync(TExecuteParameter? parameter)
        {
            Execute(parameter);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected virtual void Execute(TExecuteParameter? parameter)
        {
        }
    }
}
