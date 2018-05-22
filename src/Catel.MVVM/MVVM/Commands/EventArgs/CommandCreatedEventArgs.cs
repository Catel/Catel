// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandCreatedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Event args when a command is created.
    /// </summary>
    public class CommandCreatedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandCreatedEventArgs" /> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="name">The name.</param>
        public CommandCreatedEventArgs(ICommand command, string name)
        {
            Command = command;
            Name = name;
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <value>The command.</value>
        public ICommand Command { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }
    }
}