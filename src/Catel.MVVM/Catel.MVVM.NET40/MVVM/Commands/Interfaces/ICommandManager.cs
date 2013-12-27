// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM
{
    using System;
    using System.Windows.Input;

#if !WINDOWS_PHONE
    using InputGesture = Catel.Windows.Input.InputGesture;
#endif

    /// <summary>
    /// Manager that takes care of application-wide commands and can dynamically forward
    /// them to the right view models.
    /// </summary>
    public interface ICommandManager
    {
#if !WINDOWS_PHONE
        /// <summary>
        /// Creates the command inside the command manager.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="inputGesture">The input gesture.</param>
        /// <param name="compositeCommand">The composite command. If <c>null</c>, this will default to a new instance of <see cref="CompositeCommand"/>.</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="InvalidOperationException">The specified command is already created using the <see cref="CreateCommand"/> method.</exception>
        void CreateCommand(string commandName, InputGesture inputGesture = null, ICompositeCommand compositeCommand = null);
#else
        /// <summary>
        /// Creates the command inside the command manager.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="compositeCommand">The composite command. If <c>null</c>, this will default to a new instance of <see cref="CompositeCommand"/>.</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="InvalidOperationException">The specified command is already created using the <see cref="CommandManager.CreateCommand"/> method.</exception>
        void CreateCommand(string commandName, ICompositeCommand compositeCommand = null);
#endif

        /// <summary>
        /// Determines whether the specified command name is created.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <returns><c>true</c> if the specified command name is created; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        bool IsCommandCreated(string commandName);

        /// <summary>
        /// Registers a command with the specified
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="command">The command.</param>
        /// <param name="viewModel">The view model.</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The specified command is not created using the <see cref="CommandManager.CreateCommand"/> method.</exception>
        void RegisterCommand(string commandName, ICatelCommand command, IViewModel viewModel = null);

        /// <summary>
        /// Registers a command with the specified
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <param name="command">The command.</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The specified command is not created using the <see cref="CommandManager.CreateCommand"/> method.</exception>
        void UnregisterCommand(string commandName, ICatelCommand command);

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="InvalidOperationException">The specified command is not created using the <see cref="CommandManager.CreateCommand"/> method.</exception>
        void ExecuteCommand(string commandName);

        /// <summary>
        /// Gets the command created with the command name.
        /// </summary>
        /// <param name="commandName">Name of the command.</param>
        /// <returns>The <see cref="ICommand"/> or <c>null</c> if the command is not created.</returns>
        /// <exception cref="ArgumentException">The <paramref name="commandName"/> is <c>null</c> or whitespace.</exception>
        ICommand GetCommand(string commandName);

#if !WINDOWS_PHONE
        /// <summary>
        /// Subscribes to keyboard events.
        /// </summary>
        void SubscribeToKeyboardEvents();
#endif
    }
}