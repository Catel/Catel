// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompositeCommand.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Composite command which allows several commands inside a single command being exposed to a view.
    /// </summary>
    public interface ICompositeCommand : ICatelCommand
    {
        /// <summary>
        /// Gets or sets whether this command should check the can execute of all commands to determine can execute for composite command.
        /// <para />
        /// The default value is <c>true</c> which means the composite command can only be executed if all commands can be executed. If
        /// there is a requirement to allow partial invocation, set this property to false.
        /// </summary>
        /// <value>The check can execute of all commands to determine can execute for composite command.</value>
        [ObsoleteEx(Replacement = "AllowPartialExecution (inverted!)", TreatAsErrorFromVersion = "4.0", RemoveInVersion = "5.0")]
        bool CheckCanExecuteOfAllCommandsToDetermineCanExecuteForCompositeCommand { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether partial execution of commands is allowed. If this value is <c>true</c>, this composite
        /// command will always be executable and only invoke the internal commands that are executable.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if partial execution is allowed; otherwise, <c>false</c>.</value>
        bool AllowPartialExecution { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether at least one command must be executable. This will prevent the command to be 
        /// executed without any commands.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value><c>true</c> if at least one command must be executed; otherwise, <c>false</c>.</value>
        bool AtLeastOneMustBeExecutable { get; set; }

        /// <summary>
        /// Registers the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="viewModel">The view model. If specified, the command will automatically be unregistered when the view model is closed.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Note that if the view model is not specified, the command must be unregistered manually in order to prevent memory leaks.
        /// </remarks>
        void RegisterCommand(ICommand command, IViewModel viewModel = null);

        /// <summary>
        /// Registers the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        void RegisterAction(Action action);

        /// <summary>
        /// Unregisters the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        void UnregisterCommand(ICommand command);

        /// <summary>
        /// Unregisters the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        void UnregisterAction(Action action);

        /// <summary>
        /// Registers the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        void RegisterAction(Action<object> action);

        /// <summary>
        /// Unregisters the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        void UnregisterAction(Action<object> action);
    }
}