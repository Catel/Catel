// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValidationContext.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface describing the validation 
    /// </summary>
    public interface IValidationContext
    {
        /// <summary>
        /// Gets a value indicating whether this instance contains warnings.
        /// </summary>
        bool HasWarnings { get; }

        /// <summary>
        /// Gets a value indicating whether this instance contains errors.
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// Gets the last modified date/time.
        /// <para />
        /// Note that this is just an informational value and should not be used for comparisons. The <see cref="DateTime"/> 
        /// is not accurate enough. Use the <c>LastModifiedTicks</c> instead. 
        /// </summary>
        /// <value>The last modified date/time.</value>
        DateTime LastModified { get; }

        /// <summary>
        /// Gets the last modified ticks which is much more precise that the <see cref="LastModified"/>. Use this value
        /// to compare last modification ticks on other validation contexts.
        /// <para />
        /// Because only full .NET provides a stopwatch, this property is only available in full .NET. All other target frameworks
        /// will return the <see cref="DateTime.Ticks"/> which is <c>not</c> reliable.
        /// </summary>
        /// <value>The last modified ticks.</value>
        long LastModifiedTicks { get; }

        /// <summary>
        /// Gets the total validation count of all fields and business rules.
        /// </summary>
        /// <returns>
        /// The number of validations available.
        /// </returns>
        int GetValidationCount();

        /// <summary>
        /// Gets the total validation count of all fields and business rules with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>The number of validations available.</returns>
        int GetValidationCount(object tag);

        /// <summary>
        /// Gets all the field and business rule validations.
        /// </summary>
        /// <returns>
        /// List of <see cref="IValidationResult"/> items.
        /// </returns>
        List<IValidationResult> GetValidations();

        /// <summary>
        /// Gets all the field and business rule validations with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// List of <see cref="IValidationResult"/> items.
        /// </returns>
        List<IValidationResult> GetValidations(object tag);

        /// <summary>
        /// Gets the number of field and business rule warnings inside this context.
        /// </summary>
        /// <returns>
        /// The number of warnings available.
        /// </returns>
        int GetWarningCount();

        /// <summary>
        /// Gets the number of field and business rule warnings with the specified tag inside this context.
        /// </summary>
        /// <returns>
        /// The number of warnings available.
        /// </returns>
        int GetWarningCount(object tag);

        /// <summary>
        /// Gets all field and business rule warnings.
        /// </summary>
        /// <returns>
        /// List of <see cref="IValidationResult"/> items.
        /// </returns>
        List<IValidationResult> GetWarnings();

        /// <summary>
        /// Gets all field and business rule warnings with the specified tag.
        /// </summary>
        /// <returns>
        /// List of <see cref="IValidationResult"/> items.
        /// </returns>
        List<IValidationResult> GetWarnings(object tag);

        /// <summary>
        /// Gets the number of field and business rule errors inside this context.
        /// </summary>
        /// <returns>
        /// The number of errors available.
        /// </returns>
        int GetErrorCount();

        /// <summary>
        /// Gets the number of field and business rule errors with the specified tag inside this context.
        /// </summary>
        /// <returns>
        /// The number of errors available.
        /// </returns>
        int GetErrorCount(object tag);

        /// <summary>
        /// Gets all field and business rule errors.
        /// </summary>
        /// <returns>
        /// List of <see cref="IValidationResult"/> items.
        /// </returns>
        List<IValidationResult> GetErrors();

        /// <summary>
        /// Gets all field and business rule errors with the specified tag.
        /// </summary>
        /// <returns>
        /// List of <see cref="IValidationResult"/> items.
        /// </returns>
        List<IValidationResult> GetErrors(object tag); 

        /// <summary>
        /// Gets the field validation count of all fields.
        /// </summary>
        /// <returns>
        /// The number of field validations available.
        /// </returns>
        int GetFieldValidationCount();

        /// <summary>
        /// Gets the field validation count of all fields with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// The number of field validations available.
        /// </returns>
        int GetFieldValidationCount(object tag);

        /// <summary>
        /// Gets all the field validations.
        /// </summary>
        /// <returns>
        /// List of <see cref="IFieldValidationResult"/> items.
        /// </returns>
        List<IFieldValidationResult> GetFieldValidations();

        /// <summary>
        /// Gets all the field validations with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// List of <see cref="IFieldValidationResult"/> items.
        /// </returns>
        List<IFieldValidationResult> GetFieldValidations(object tag);

        /// <summary>
        /// Gets all the field validations for the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// List of <see cref="IFieldValidationResult"/> items.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        List<IFieldValidationResult> GetFieldValidations(string propertyName);

        /// <summary>
        /// Gets all the field validations for the specified property name with the specified tag.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// List of <see cref="IFieldValidationResult"/> items.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        List<IFieldValidationResult> GetFieldValidations(string propertyName, object tag);

        /// <summary>
        /// Gets the field warning count of all fields.
        /// </summary>
        /// <returns>The number of field warnings available.</returns>
        int GetFieldWarningCount();

        /// <summary>
        /// Gets the field warning count of all fields with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>The number of field warnings available.</returns>
        int GetFieldWarningCount(object tag);

        /// <summary>
        /// Gets all the field warnings.
        /// </summary>
        /// <returns>
        /// List of <see cref="IFieldValidationResult"/> items.
        /// </returns>
        List<IFieldValidationResult> GetFieldWarnings();

        /// <summary>
        /// Gets all the field warnings with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// List of <see cref="IFieldValidationResult"/> items.
        /// </returns>
        List<IFieldValidationResult> GetFieldWarnings(object tag);

        /// <summary>
        /// Gets all the field warnings for the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// List of <see cref="IFieldValidationResult"/> items.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        List<IFieldValidationResult> GetFieldWarnings(string propertyName);

        /// <summary>
        /// Gets all the field warnings for the specified property name with the specified tag.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// List of <see cref="IFieldValidationResult"/> items.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        List<IFieldValidationResult> GetFieldWarnings(string propertyName, object tag);

        /// <summary>
        /// Gets the field error count of all fields.
        /// </summary>
        /// <returns>The number of field errors available.</returns>
        int GetFieldErrorCount();

        /// <summary>
        /// Gets the field error count of all fields with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>The number of field errors available.</returns>
        int GetFieldErrorCount(object tag);

        /// <summary>
        /// Gets all the field errors.
        /// </summary>
        /// <returns>
        /// List of <see cref="IFieldValidationResult"/> items.
        /// </returns>
        List<IFieldValidationResult> GetFieldErrors();

        /// <summary>
        /// Gets all the field errors with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// List of <see cref="IFieldValidationResult"/> items.
        /// </returns>
        List<IFieldValidationResult> GetFieldErrors(object tag);

        /// <summary>
        /// Gets all the field errors for the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// List of <see cref="IFieldValidationResult"/> items.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        List<IFieldValidationResult> GetFieldErrors(string propertyName);

        /// <summary>
        /// Gets all the field errors for the specified property name with the specified tag.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// List of <see cref="IFieldValidationResult"/> items.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        List<IFieldValidationResult> GetFieldErrors(string propertyName, object tag);

        /// <summary>
        /// Gets the business rule validation count.
        /// </summary>
        /// <returns>The number of business rule validations available.</returns>
        int GetBusinessRuleValidationCount();

        /// <summary>
        /// Gets the business rule validation count with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// The number of business rule validations available.
        /// </returns>
        int GetBusinessRuleValidationCount(object tag);

        /// <summary>
        /// Gets all the business rule validations.
        /// </summary>
        /// <returns>
        /// List of <see cref="IBusinessRuleValidationResult"/> items.
        /// </returns>
        List<IBusinessRuleValidationResult> GetBusinessRuleValidations();

        /// <summary>
        /// Gets all the business rule validations with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// List of <see cref="IBusinessRuleValidationResult"/> items.
        /// </returns>
        List<IBusinessRuleValidationResult> GetBusinessRuleValidations(object tag);

        /// <summary>
        /// Gets the business rule warning count.
        /// </summary>
        /// <returns>The number of business rule warnings available.</returns>
        int GetBusinessRuleWarningCount();

        /// <summary>
        /// Gets the business rule warning count with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// The number of business rule warnings available.
        /// </returns>
        int GetBusinessRuleWarningCount(object tag);

        /// <summary>
        /// Gets all the business rule warnings.
        /// </summary>
        /// <returns>
        /// List of <see cref="IBusinessRuleValidationResult"/> items.
        /// </returns>
        List<IBusinessRuleValidationResult> GetBusinessRuleWarnings();

        /// <summary>
        /// Gets all the business rule warnings with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// List of <see cref="IBusinessRuleValidationResult"/> items.
        /// </returns>
        List<IBusinessRuleValidationResult> GetBusinessRuleWarnings(object tag);

        /// <summary>
        /// Gets the business rule error count.
        /// </summary>
        /// <returns>The number of business rule errors available.</returns>
        int GetBusinessRuleErrorCount();

        /// <summary>
        /// Gets the business rule error count with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// The number of business rule errors available.
        /// </returns>
        int GetBusinessRuleErrorCount(object tag);

        /// <summary>
        /// Gets all the business rule errors.
        /// </summary>
        /// <returns>
        /// List of <see cref="IBusinessRuleValidationResult"/> items.
        /// </returns>
        List<IBusinessRuleValidationResult> GetBusinessRuleErrors();

        /// <summary>
        /// Gets all the business rule errors with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// List of <see cref="IBusinessRuleValidationResult"/> items.
        /// </returns>
        List<IBusinessRuleValidationResult> GetBusinessRuleErrors(object tag);

        /// <summary>
        /// Adds the field validation result.
        /// </summary>
        /// <param name="fieldValidationResult">The field validation result.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="fieldValidationResult"/> is <c>null</c>.</exception>
        void AddFieldValidationResult(IFieldValidationResult fieldValidationResult);

        /// <summary>
        /// Removes the field validation result.
        /// </summary>
        /// <param name="fieldValidationResult">The field validation result.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="fieldValidationResult"/> is <c>null</c>.</exception>
        void RemoveFieldValidationResult(IFieldValidationResult fieldValidationResult);

        /// <summary>
        /// Adds the business rule validation result.
        /// </summary>
        /// <param name="businessRuleValidationResult">The business rule validation result.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="businessRuleValidationResult"/> is <c>null</c>.</exception>
        void AddBusinessRuleValidationResult(IBusinessRuleValidationResult businessRuleValidationResult);

        /// <summary>
        /// Removes the business rule validation result.
        /// </summary>
        /// <param name="businessRuleValidationResult">The business rule validation result.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="businessRuleValidationResult"/> is <c>null</c>.</exception>
        void RemoveBusinessRuleValidationResult(IBusinessRuleValidationResult businessRuleValidationResult);
    }
}
