// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValidationResult.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    /// <summary>
    /// Types of validation results.
    /// </summary>
    public enum ValidationResultType
    {
        /// <summary>
        /// Validation result represents a warning.
        /// </summary>
        Warning,

        /// <summary>
        /// Validation result represents an error.
        /// </summary>
        Error
    }

    /// <summary>
    /// Validation result with information about validations.
    /// </summary>
    public interface IValidationResult
    {
        /// <summary>
        /// Gets the type of the validation result.
        /// </summary>
        /// <value>The type of the validation result.</value>
        ValidationResultType ValidationResultType { get; }

        /// <summary>
        /// Gets the validation result message.
        /// </summary>
        /// <value>The message.</value>
        /// <remarks>
        /// This value has a public setter so it is possible to customize the message
        /// in derived classes.
        /// <para />
        /// One should be careful and know what they are doing when overwriting an error message.
        /// </remarks>
        string Message { get; set; }

        /// <summary>
        /// Gets or sets the tag that allows grouping of validations.
        /// </summary>
        /// <value>The tag.</value>
        object Tag { get; set; }
    }

    /// <summary>
    /// Field validation result with information about field validations.
    /// </summary>
    public interface IFieldValidationResult : IValidationResult
    {
        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        string PropertyName { get; }
    }

    /// <summary>
    /// Business rule validation result with information about business rule validations.
    /// </summary>
    public interface IBusinessRuleValidationResult : IValidationResult
    {
    }
}
