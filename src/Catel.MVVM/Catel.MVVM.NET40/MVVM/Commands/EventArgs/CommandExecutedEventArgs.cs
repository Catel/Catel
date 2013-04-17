// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandExecutedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// <see cref="EventArgs"/> implementation for the event when an <see cref="ICatelCommand"/> has been executed.
    /// </summary>
    public class CommandExecutedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutedEventArgs"/> class.
        /// </summary>
        /// <param name="command">The command that just has been executed.</param>
        /// <param name="commandParameter">The command parameter that was used for the execution.</param>
        /// <param name="commandPropertyName">The property name under which the command is registered.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        public CommandExecutedEventArgs(ICatelCommand command, object commandParameter = null, string commandPropertyName = null)
        {
            Argument.IsNotNull("command", command);

            Command = command;
            CommandParameter = commandParameter;
            CommandPropertyName = commandPropertyName;
        }

        /// <summary>
        /// Gets the command that just has been executed.
        /// </summary>
        /// <value>The command.</value>
        public ICatelCommand Command { get; private set; }

        /// <summary>
        /// Gets the command parameter used for the execution.
        /// </summary>
        /// <value>The command parameter.</value>
        public object CommandParameter { get; private set; }

        /// <summary>
        /// Gets the property name under which the command is registered
        /// </summary>
        /// <value>The name of the command property.</value>
        public string CommandPropertyName { get; private set; }
    }
}
