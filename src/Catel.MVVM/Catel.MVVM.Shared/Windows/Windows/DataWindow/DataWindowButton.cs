// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataWindowButton.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Windows
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Data;
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
        public static DataWindowButton FromSync(string text, Action execute, Func<bool> canExecute = null)
        {
            return new DataWindowButton(text, new Command(execute, canExecute));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataWindowButton"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="execute">The execute delegate.</param>
        /// <param name="canExecute">The can execute delegate.</param>
        /// <param name="contentBindingPath">The binding path expression of the content to bind to.</param>
        /// <param name="contentValueConverter">The value converter used with content binding.</param>
        /// <param name="visibilityBindingPath">The binding path expression of the visibility to bind to.</param>
        /// <param name="visibilityValueConverter">The value converter used with visibility binding.</param>
        /// <remarks>Text is ignored when contentBindingPath is set.</remarks>
        public static DataWindowButton FromSync(string text, Action execute, Func<bool> canExecute = null, string contentBindingPath = null, IValueConverter contentValueConverter = null, string visibilityBindingPath = null, IValueConverter visibilityValueConverter = null)
        {
            return new DataWindowButton(text, new Command(execute, canExecute), contentBindingPath, contentValueConverter, visibilityBindingPath, visibilityValueConverter);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataWindowButton"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="executeAsync">The async execute delegate.</param>
        /// <param name="canExecute">The can execute delegate.</param>
        public static DataWindowButton FromAsync(string text, Func<Task> executeAsync, Func<bool> canExecute = null)
        {
            return new DataWindowButton(text, new TaskCommand(executeAsync, canExecute));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataWindowButton"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="executeAsync">The async execute delegate.</param>
        /// <param name="canExecute">The can execute delegate.</param>
        /// <param name="contentBindingPath">The binding path expression of the content to bind to.</param>
        /// <param name="contentValueConverter">The value converter used with content binding.</param>
        /// <param name="visibilityBindingPath">The binding path expression of the visibility to bind to.</param>
        /// <param name="visibilityValueConverter">The value converter used with visibility binding.</param>
        /// <remarks>Text is ignored when contentBindingPath is set.</remarks>
        public static DataWindowButton FromAsync(string text, Func<Task> executeAsync, Func<bool> canExecute = null, string contentBindingPath = null, IValueConverter contentValueConverter = null, string visibilityBindingPath = null, IValueConverter visibilityValueConverter = null)
        {
            return new DataWindowButton(text, new TaskCommand(executeAsync, canExecute), contentBindingPath, contentValueConverter, visibilityBindingPath, visibilityValueConverter);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataWindowButton"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="command">The command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        public DataWindowButton(string text, ICommand command)
            : this(text, command, null, null, null, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataWindowButton"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="command">The command.</param>
        /// <param name="contentBindingPath">The binding path expression of the content to bind to.</param>
        /// <param name="contentValueConverter">The value converter used with content binding.</param>
        /// <param name="visibilityBindingPath">The binding path expression of the visibility to bind to.</param>
        /// <param name="visibilityValueConverter">The value converter used with visibility binding.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is <c>null</c>.</exception>
        /// <remarks>Text is ignored when contentBindingPath is set.</remarks>
        public DataWindowButton(string text, ICommand command, string contentBindingPath = null, IValueConverter contentValueConverter = null, string visibilityBindingPath = null, IValueConverter visibilityValueConverter = null)
        {
            Argument.IsNotNull("command", command);

            Text = text;
            Command = command;
            ContentBindingPath = contentBindingPath;
            ContentValueConverter = contentValueConverter;
            VisibilityBindingPath = visibilityBindingPath;
            VisibilityValueConverter = visibilityValueConverter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataWindowButton"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="commandBindingPath">The binding path expression of the command to bind to.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandBindingPath"/> is <c>null</c>.</exception>
        public DataWindowButton(string text, string commandBindingPath)
            : this(text, commandBindingPath, null, null, null, null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataWindowButton"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="commandBindingPath">The binding path expression of the command to bind to.</param>
        /// <param name="contentBindingPath">The binding path expression of the content to bind to.</param>
        /// <param name="contentValueConverter">The value converter used with content binding.</param>
        /// <param name="visibilityBindingPath">The binding path expression of the visibility to bind to.</param>
        /// <param name="visibilityValueConverter">The value converter used with visibility binding.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandBindingPath"/> is <c>null</c>.</exception>
        /// <remarks>Text is ignored when contentBindingPath is set.</remarks>
        public DataWindowButton(string text, string commandBindingPath, string contentBindingPath = null, IValueConverter contentValueConverter = null, string visibilityBindingPath = null, IValueConverter visibilityValueConverter = null)
        {
            Argument.IsNotNull("bindingPath", commandBindingPath);

            Text = text;
            CommandBindingPath = commandBindingPath;
            ContentBindingPath = contentBindingPath;
            ContentValueConverter = contentValueConverter;
            VisibilityBindingPath = visibilityBindingPath;
            VisibilityValueConverter = visibilityValueConverter;
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
        /// Gets the content binding path.
        /// </summary>
        /// <value>The content binding path.</value>
        public string ContentBindingPath { get; private set; }

        /// <summary>
        /// Gets the visibility binding path.
        /// </summary>
        /// <value>The visibility binding path.</value>
        public string VisibilityBindingPath { get; private set; }

        /// <summary>
        /// Gets the content value converter used with content binding.
        /// </summary>
        /// <value>The content value converter.</value>
        public IValueConverter ContentValueConverter { get; private set; }

        /// <summary>
        /// Gets the visibility value converter used with visibility binding.
        /// </summary>
        /// <value>The visibility value converter.</value>
        public IValueConverter VisibilityValueConverter { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this button is the default button.
        /// </summary>
        /// <value>
        /// <c>true</c> if this button is the default button; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this button is the cancel button.
        /// </summary>
        /// <value>
        /// <c>true</c> if this button is the cancel button; otherwise, <c>false</c>.
        /// </value>
        public bool IsCancel { get; set; }
        #endregion
    }
}

#endif