// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// CommandEventArgs, simply holds the command parameter.
    /// </summary>
    public class CommandEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandEventArgs"/> class.
        /// </summary>
        /// <param name="commandParameter">The command parameter.</param>
        public CommandEventArgs(object commandParameter = null)
        {
            CommandParameter = commandParameter;
        }

        /// <summary>
        /// Gets the command parameter used for the execution.
        /// </summary>
        /// <value>The command parameter.</value>
        public object CommandParameter { get;  set; }
    }
}