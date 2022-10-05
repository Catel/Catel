namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Exception in case there is an invalid view model.
    /// </summary>
    public class InvalidViewModelException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidViewModelException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidViewModelException(string message)
            : base(message)
        {
        }
    }
}
