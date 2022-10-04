namespace Catel.Data
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Interface defining a validatable object.
    /// </summary>
    public interface IValidatable : INotifyDataErrorInfo, INotifyDataWarningInfo, IDataErrorInfo, IDataWarningInfo
    {
        /// <summary>
        /// Gets or sets the validator to use.
        /// <para />
        /// By default, this value retrieves the default validator from them <see cref="IValidatorProvider"/> if it is
        /// registered in the <see cref="Catel.IoC.ServiceLocator"/>.
        /// </summary>
        IValidator Validator { get; set; }

        /// <summary>
        /// Gets the validation context which contains all information about the validation.
        /// </summary>
        /// <value>The validation context.</value>
        IValidationContext ValidationContext { get; }

        /// <summary>
        /// Gets a value indicating whether the object is currently hiding its validation results. If the object
        /// hides its validation results, it is still possible to retrieve the validation results using the
        /// <see cref="ValidationContext"/>.
        /// </summary>
        bool IsHidingValidationResults { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this object is validated or not.
        /// </summary>
        bool IsValidated { get; }

        /// <summary>
        /// Occurs when the object is validating.
        /// </summary>
        event EventHandler<ValidationEventArgs>? Validating;

        /// <summary>
        /// Occurs when the object is validated.
        /// </summary>
        event EventHandler<ValidationEventArgs>? Validated;

        /// <summary>
        /// Validates the current object for field and business rule errors.
        /// </summary>
        /// <param name="force">if set to <c>true</c>, a validation is forced. When the validation is not forced, it means 
        /// that when the object is already validated, and no properties have been changed, no validation actually occurs 
        /// since there is no reason for any values to have changed.
        /// </param>
        /// <remarks>
        /// To check wether this object contains any errors, use the ValidationContext property.
        /// </remarks>
        void Validate(bool force = false);
    }
}
