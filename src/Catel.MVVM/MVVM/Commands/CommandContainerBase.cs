namespace Catel.MVVM
{
    using System;
    using System.Threading.Tasks;
    using Auditing;
    using Catel.Logging;
    using Catel.Services;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Container for application-wide commands.
    /// </summary>
    public abstract class CommandContainerBase : CommandContainerBase<object?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandContainerBase"/> class.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="commandManager">The command manager.</param>
        /// <param name="serviceProvider">The service provider.</param>
        protected CommandContainerBase(string commandName, ICommandManager commandManager, IServiceProvider serviceProvider)
            : base(commandName, commandManager, serviceProvider)
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
        /// <param name="serviceProvider">The service provider.</param>
        protected CommandContainerBase(string commandName, ICommandManager commandManager, IServiceProvider serviceProvider)
            : base(commandName, commandManager, serviceProvider)
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
        /// <param name="serviceProvider">The service provider.</param>
        protected CommandContainerBase(string commandName, ICommandManager commandManager, IServiceProvider serviceProvider)
            : base(commandName, commandManager, serviceProvider)
        {
        }
    }

    /// <summary>
    /// Container for application-wide commands.
    /// </summary>
    /// <typeparam name="TExecuteParameter">The type of the command execute parameter.</typeparam>
    /// <typeparam name="TCanExecuteParameter">The type of the command can execute parameter.</typeparam>
    /// <typeparam name="TProgress">The type of the progress.</typeparam>
    public abstract class CommandContainerBase<TExecuteParameter, TCanExecuteParameter, TProgress> 
        where TProgress : ITaskProgressReport
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ICatelCommand _command;
        private readonly ICommandManager _commandManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAuditingManager _auditingManager;

        private readonly ICompositeCommand _compositeCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandContainerBase{TExecuteParameter, TCanExecuteParameter, TPogress}"/> class.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="commandManager">The command manager.</param>
        /// <param name="serviceProvider">The service provider.</param>
        protected CommandContainerBase(string commandName, ICommandManager commandManager, IServiceProvider serviceProvider)
        {
            Argument.IsNotNullOrWhitespace("commandName", commandName);
            ArgumentNullException.ThrowIfNull(commandManager);

            CommandName = commandName;
            _commandManager = commandManager;
            _serviceProvider = serviceProvider;
            _auditingManager = serviceProvider.GetRequiredService<IAuditingManager>();

            var compositeCommand = _commandManager.GetCommand(commandName) as ICompositeCommand;
            if (compositeCommand is null)
            {
                throw Log.ErrorAndCreateException<CatelException>($"Cannot find composite command command '{commandName}'");
            }

            var authenticationProvider = serviceProvider.GetRequiredService<IAuthenticationProvider>();
            var dispatcherService = serviceProvider.GetRequiredService<IDispatcherService>();

            _compositeCommand = compositeCommand;
            _command = new TaskCommand<TExecuteParameter, TCanExecuteParameter, TProgress>(authenticationProvider, dispatcherService, ExecuteInternalAsync, CanExecute);

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
        public virtual void InvalidateCommand()
        {
            _compositeCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Determines whether the command can be executed.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns><c>true</c> if this instance can execute the specified parameter; otherwise, <c>false</c>.</returns>
        public virtual bool CanExecute(TCanExecuteParameter? parameter)
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

            _auditingManager.OnCommandExecuted(null, CommandName, _command, parameter);
        }

        /// <summary>
        /// Execute the command as an asynchronous operation.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>Task.</returns>
        public virtual async Task ExecuteAsync(TExecuteParameter? parameter)
        {
            Execute(parameter);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public virtual void Execute(TExecuteParameter? parameter)
        {
        }
    }
}
