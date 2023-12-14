namespace Catel.Data
{
    using System;

    /// <summary>
    /// The validation event args.
    /// </summary>
    public class ValidationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationEventArgs"/> class.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        public ValidationEventArgs(IValidationContext validationContext)
        {
            ValidationContext = validationContext;
        }

        /// <summary>
        /// Gets the validation context.
        /// </summary>
        /// <value>The validation context.</value>
        public IValidationContext ValidationContext { get; private set; }
    }
}
