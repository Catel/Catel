// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandCanceledEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    /// <summary>
    /// CommandCanceledEventArgs, just like above but allows the event to 
    /// be cancelled.
    /// </summary>
    public class CommandCanceledEventArgs : CommandEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandCanceledEventArgs"/> class.
        /// </summary>
        /// <param name="commandParameter">The command parameter.</param>
        public CommandCanceledEventArgs(object commandParameter = null)
        {
            CommandParameter = commandParameter;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CommandCanceledEventArgs"/> command should be cancelled.
        /// </summary>
        /// <value><c>true</c> if cancel; otherwise, <c>false</c>.</value>
        public bool Cancel { get; set; }
    }
}