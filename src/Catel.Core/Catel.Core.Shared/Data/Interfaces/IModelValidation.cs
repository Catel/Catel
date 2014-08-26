// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelValidation.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System.ComponentModel;

    /// <summary>
    /// Defines all validation members for the models.
    /// </summary>
    public interface IModelValidation : INotifyDataErrorInfo, INotifyDataWarningInfo, IDataErrorInfo, IDataWarningInfo
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
        /// Gets the number of field warnings.
        /// </summary>
        /// <value>The field warning count.</value>
        int FieldWarningCount { get; }

        /// <summary>
        /// Gets the number of business rule warnings.
        /// </summary>
        /// <value>The business rule warning count.</value>
        int BusinessRuleWarningCount { get; }

        /// <summary>
        /// Gets the number of field errors.
        /// </summary>
        /// <value>The field error count.</value>
        int FieldErrorCount { get; }

        /// <summary>
        /// Gets the number of business rule errors.
        /// </summary>
        /// <value>The business rule error count.</value>
        int BusinessRuleErrorCount { get; }

        /// <summary>
        /// Gets a value indicating whether the object is currently hiding its validation results. If the object
        /// hides its validation results, it is still possible to retrieve the validation results using the
        /// <see cref="ValidationContext"/>.
        /// </summary>
        bool IsHidingValidationResults { get; }

        /// <summary>
        /// Validates the current object for field and business rule errors.
        /// </summary>
        /// <param name="force">if set to <c>true</c>, a validation is forced. When the validation is not forced, it means 
        /// that when the object is already validated, and no properties have been changed, no validation actually occurs 
        /// since there is no reason for any values to have changed.
        /// </param>
        /// <remarks>
        /// To check wether this object contains any errors, use the <see cref="INotifyDataErrorInfo.HasErrors"/> property.
        /// </remarks>
        void Validate(bool force = false);
    }
}