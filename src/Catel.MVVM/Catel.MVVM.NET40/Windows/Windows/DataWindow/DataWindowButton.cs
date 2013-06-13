// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataWindowButton.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows
{
    using System;
    using System.Windows.Input;
    using MVVM;

    /// <summary>
    /// Information for a button that should be generated.
    /// </summary>
    public class DataWindowButton
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DataWindowButton"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="execute">The execute delegate.</param>
        /// <param name="canExecute">The can execute delegate.</param>
        public DataWindowButton(string text, Action execute, Func<bool> canExecute = null)
            : this(text, new Command(execute, canExecute)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataWindowButton"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="command">The command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        public DataWindowButton(string text, ICommand command)
        {
            Argument.IsNotNull("command", command);

            Text = text;
            Command = command;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataWindowButton"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="bindingPath">The binding path expression of the command to bind to.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="bindingPath"/> is <c>null</c>.</exception>
        public DataWindowButton(string text, string bindingPath)
        {
            Argument.IsNotNull("bindingPath", bindingPath);

            Text = text;
            CommandBindingPath = bindingPath;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the text as it is displayed on the button.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the command associated with this button.
        /// </summary>
        /// <value>The command.</value>
        public ICommand Command { get; private set; }

        /// <summary>
        /// Gets the command binding path.
        /// </summary>
        /// <value>The command binding path.</value>
        public string CommandBindingPath { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this button is the default button.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this button is the default button; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this button is the cancel button.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this button is the cancel button; otherwise, <c>false</c>.
        /// </value>
        public bool IsCancel { get; set; }
        #endregion
    }
}