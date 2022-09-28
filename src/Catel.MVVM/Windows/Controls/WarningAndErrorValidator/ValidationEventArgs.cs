namespace Catel.Windows.Controls
{
    using UIEventArgs = System.EventArgs;

    /// <summary>
    /// Event arguments for event <see cref="WarningAndErrorValidator"/> Validation.
    /// </summary>
    public class ValidationEventArgs : UIEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationEventArgs"/> class.
        /// </summary>
        /// <param name="value">The value that contains the warning or error.</param>
        /// <param name="message">The actual warning or error message.</param>
        /// <param name="action">The action of the validation event.</param>
        /// <param name="type">The type of validation.</param>
        internal ValidationEventArgs(object value, string message, ValidationEventAction action, ValidationType type)
        {
            Value = value;
            Message = message;
            Action = action;
            Type = type;
        }

        /// <summary>
        /// Gets the value that contains the warning or error.
        /// </summary>
        /// <value>The value that contains the warning or error.</value>
        public object Value { get; private set; }

        /// <summary>
        /// Gets the actual warning or error message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; private set; }

        /// <summary>
        /// A action for handling event.
        /// </summary>
        public ValidationEventAction Action { get; private set; }

        /// <summary>
        /// Gets the type of the validation.
        /// </summary>
        /// <value>The type of the validation.</value>
        public ValidationType Type { get; private set; }
    }
}
