// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeCommand.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using Catel.Logging;

    /// <summary>
    /// Composite command which allows several commands inside a single command being exposed to a view.
    /// </summary>
    public class CompositeCommand : Command, ICompositeCommand
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly object _lock = new object();
        private readonly List<CommandInfo> _commandInfo = new List<CommandInfo>();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Command{TCanExecuteParameter,TExecuteParameter}" /> class.
        /// </summary>
        public CompositeCommand()
            : base(null)
        {
            InitializeActions(null, ExecuteCompositeCommand, null, CanExecuteCompositeCommand);
        }

        #endregion

        #region Methods
        private void ExecuteCompositeCommand()
        {
            lock (_lock)
            {
                foreach (var commandInfo in _commandInfo)
                {
                    try
                    {
                        var command = commandInfo.Command;
                        if (command.CanExecute())
                        {
                            command.Execute();
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to execute one of the commands in the composite commands, execution will continue");
                    }
                }
            }
        }

        private bool CanExecuteCompositeCommand()
        {
			bool res = false;
			
            lock (_lock)
            {
				res = _commandInfo.Count > 0;
                if (res)
                {
                    foreach (var commandInfo in _commandInfo)
                    {
                        res &= commandInfo.Command.CanExecute();
                        if(res)
                            break;
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Registers the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="viewModel">The view model. If specified, the command will automatically be unregistered when the view model is closed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Note that if the view model is not specified, the command must be unregistered manually in order to prevent memory leaks.
        /// </remarks>
        public void RegisterCommand(ICatelCommand command, IViewModel viewModel = null)
        {
            Argument.IsNotNull("command", command);

            lock (_lock)
            {
                _commandInfo.Add(new CommandInfo(this, command, viewModel));

                Log.Debug("Registered command in CompositeCommand");
            }
        }

        /// <summary>
        /// Unregisters the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        public void UnregisterCommand(ICatelCommand command)
        {
            Argument.IsNotNull("command", command);

            lock (_lock)
            {
                for (int i = _commandInfo.Count - 1; i >= 0; i--)
                {
                    var commandInfo = _commandInfo[i];

                    if (ReferenceEquals(commandInfo.Command, command))
                    {
                        _commandInfo.RemoveAt(i);

                        Log.Debug("Unregistered command from CompositeCommand");
                    }
                }
            }
        }

        #endregion

        #region Nested type: CommandInfo
        private class CommandInfo
        {
            private readonly CompositeCommand _compositeCommand;

            #region Constructors
            public CommandInfo(CompositeCommand compositeCommand, ICatelCommand command, IViewModel viewModel)
            {
                _compositeCommand = compositeCommand;

                Command = command;
                ViewModel = viewModel;

                if (viewModel != null)
                {
                    viewModel.Closed += OnViewModelClosed;
                }
            }
            #endregion

            #region Properties
            public ICatelCommand Command { get; private set; }
            public IViewModel ViewModel { get; private set; }
            #endregion

            private void OnViewModelClosed(object sender, ViewModelClosedEventArgs e)
            {
                Log.Debug("ViewModel '{0}' is closed, automatically unregistering command from CompositeCommand", ViewModel);

                _compositeCommand.UnregisterCommand(Command);

                ViewModel.Closed -= OnViewModelClosed;
                ViewModel = null;
            }
        }
        #endregion
    }
}