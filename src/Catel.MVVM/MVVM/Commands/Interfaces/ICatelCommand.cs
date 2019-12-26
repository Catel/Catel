// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICatelCommand.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Advanced <see cref="ICommand"/> interface definition to provide advanced functionality.
    /// </summary>
    public interface ICatelCommand : ICommand
    {
        /// <summary>
        /// Gets the tag for this command. A tag is a way to link any object to a command so you can use your own
        /// methods to recognize the commands, for example by ID or string.
        /// <para />
        /// By default, the value is <c>null</c>.
        /// </summary>
        /// <value>The tag.</value>
        object Tag { get; }

        /// <summary>
        /// Occurs when the command has just been executed successfully.
        /// </summary>
        event EventHandler<CommandExecutedEventArgs> Executed;

        /// <summary>
        /// Raises the <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        void RaiseCanExecuteChanged();

        /// <summary>
        /// Invokes the <see cref="ICommand.CanExecute" /> with <c>null</c> as parameter.
        /// </summary>
        /// <returns><c>true</c> if this instance can execute; otherwise, <c>false</c>.</returns>
        bool CanExecute();

        /// <summary>
        /// Invokes the <see cref="ICommand.Execute" /> with <c>null</c> as parameter.
        /// </summary>
        /// <returns><c>true</c> if this instance can execute; otherwise, <c>false</c>.</returns>
        void Execute();
    }

    /// <summary>
    /// Advanced <see cref="ICommand" /> interface definition to provide advanced functionality.
    /// </summary>
    /// <typeparam name="TExecuteParameter">The type of the execute parameter.</typeparam>
    /// <typeparam name="TCanExecuteParameter">The type of the can execute parameter.</typeparam>
    public interface ICatelCommand<TExecuteParameter, TCanExecuteParameter> : ICatelCommand
    {
        /// <summary>
        /// Invokes the <see cref="ICommand.CanExecute" /> with <c>null</c> as parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns><c>true</c> if this instance can execute; otherwise, <c>false</c>.</returns>
        bool CanExecute(TCanExecuteParameter parameter);

        /// <summary>
        /// Invokes the <see cref="ICommand.Execute" /> with <c>null</c> as parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns><c>true</c> if this instance can execute; otherwise, <c>false</c>.</returns>
        void Execute(TExecuteParameter parameter);
    }
}
