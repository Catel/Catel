// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValidator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Validator that can handle the validation of an object.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Called just before any validation is caused.
        /// </summary>
        /// <param name="instance">The instance that is about to be validated.</param>
        /// <param name="previousFieldValidationResults">The previous field validation results.</param>
        /// <param name="previousBusinessRuleValidationResults">The previous business rule validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="previousFieldValidationResults"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="previousBusinessRuleValidationResults"/> is <c>null</c>.</exception>
        void BeforeValidation(object instance, List<IFieldValidationResult> previousFieldValidationResults, 
            List<IBusinessRuleValidationResult> previousBusinessRuleValidationResults);

        /// <summary>
        /// Called just before the specified instance is about to be validate its fields.
        /// </summary>
        /// <param name="instance">The instance that is about to be validated.</param>
        /// <param name="previousValidationResults">The previous validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="previousValidationResults"/> is <c>null</c>.</exception>
        void BeforeValidateFields(object instance, List<IFieldValidationResult> previousValidationResults);

        /// <summary>
        /// Validates the fields of the specified instance. The results must be added to the list of validation
        /// results.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResults"/> is <c>null</c>.</exception>
        void ValidateFields(object instance, List<IFieldValidationResult> validationResults);

        /// <summary>
        /// Called just after the specified instance has validated its fields.
        /// </summary>
        /// <param name="instance">The instance that has just been validated.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResults"/> is <c>null</c>.</exception>
        void AfterValidateFields(object instance, List<IFieldValidationResult> validationResults);

        /// <summary>
        /// Called just before the specified instance is about to be validate its business rules.
        /// </summary>
        /// <param name="instance">The instance that is about to be validated.</param>
        /// <param name="previousValidationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="previousValidationResults"/> is <c>null</c>.</exception>
        void BeforeValidateBusinessRules(object instance, List<IBusinessRuleValidationResult> previousValidationResults);

        /// <summary>
        /// Validates the business rules of the specified instance. The results must be added to the list of validation
        /// results.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResults"/> is <c>null</c>.</exception>
        void ValidateBusinessRules(object instance, List<IBusinessRuleValidationResult> validationResults);

        /// <summary>
        /// Called just after the specified instance has validated its business rules.
        /// </summary>
        /// <param name="instance">The instance that has just been validated.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResults"/> is <c>null</c>.</exception>
        void AfterValidateBusinessRules(object instance, List<IBusinessRuleValidationResult> validationResults);

        /// <summary>
        /// Called just after all validation has been executed.
        /// </summary>
        /// <param name="instance">The instance that has just been validated.</param>
        /// <param name="fieldValidationResults">The current field validation results.</param>
        /// <param name="businessRuleValidationResults">The current business rule validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="fieldValidationResults"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="businessRuleValidationResults"/> is <c>null</c>.</exception>
        void AfterValidation(object instance, List<IFieldValidationResult> fieldValidationResults,
            List<IBusinessRuleValidationResult> businessRuleValidationResults);
    }
}
