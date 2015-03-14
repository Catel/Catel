// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeValidator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections.Generic;

    using Threading;

    /// <summary>
    /// Allows the combination of several validators into a single validator. This class will combine all instances of the
    /// <see cref="IValidator"/> class found for a type using the <see cref="IValidatorProvider"/> into this single composite
    /// validator.
    /// </summary>
    public sealed class CompositeValidator : IValidator
    {
        #region Constants and Fields

        /// <summary>
        /// The validator list.
        /// </summary>
        private readonly HashSet<IValidator> _validators = new HashSet<IValidator>();

        /// <summary>
        /// The synchronization context.
        /// </summary>
        private readonly SynchronizationContext _synchronizationContext = new SynchronizationContext();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds the validator to this composite validator.
        /// </summary>
        /// <param name="validator">The validator to add.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="validator" /> is <c>null</c>.</exception>
        public void Add(IValidator validator)
        {
            Argument.IsNotNull("validator", validator);

            _synchronizationContext.Execute(
                () =>
                {
                    if (!_validators.Contains(validator))
                    {
                        _validators.Add(validator);
                    }
                });
        }

        /// <summary>
        /// Removes the validator from this composite validator.
        /// </summary>
        /// <param name="validator">The validator to remove.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="validator" /> is <c>null</c>.</exception>
        public void Remove(IValidator validator)
        {
            Argument.IsNotNull("validator", validator);

            _synchronizationContext.Execute(() => _validators.Remove(validator));
        }

        /// <summary>
        /// Determines whether this composite validator contains the specified validator.
        /// </summary>
        /// <param name="validator">The validator.</param>
        /// <returns><c>true</c> if this composite validator contains the specified validator; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="validator" /> is <c>null</c>.</exception>
        public bool Contains(IValidator validator)
        {
            Argument.IsNotNull("validator", validator);

            return _synchronizationContext.Execute(() => _validators.Contains(validator));
        }

        /// <summary>
        /// Validates the specified instance and allows the manipulation of the whole validation context.
        /// <para />
        /// This method can be used to manipulate the whole validation context and the implementation of this is enough.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <param name="validationContext">The validation context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationContext"/> is <c>null</c>.</exception>
        public void Validate(object instance, ValidationContext validationContext)
        {
            try
            {
                foreach (var validator in _validators)
                {
                    validator.Validate(instance, validationContext);
                }
            }
            catch (Exception)
            {
                _synchronizationContext.Release();
                throw;
            }
        }

        /// <summary>
        /// Called just before any validation is caused.
        /// </summary>
        /// <param name="instance">The instance that is about to be validated.</param>
        /// <param name="previousFieldValidationResults">The previous field validation results.</param>
        /// <param name="previousBusinessRuleValidationResults">The previous business rule validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="previousFieldValidationResults" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="previousBusinessRuleValidationResults" /> is <c>null</c>.</exception>
        public void BeforeValidation(object instance, List<IFieldValidationResult> previousFieldValidationResults, List<IBusinessRuleValidationResult> previousBusinessRuleValidationResults)
        {
            _synchronizationContext.Acquire();

            try
            {
                foreach (var validator in _validators)
                {
                    validator.BeforeValidation(instance, previousFieldValidationResults, previousBusinessRuleValidationResults);
                }
            }
            catch (Exception)
            {
                _synchronizationContext.Release();
                throw;
            }
        }

        /// <summary>
        /// Called just before the specified instance is about to be validate its fields.
        /// </summary>
        /// <param name="instance">The instance that is about to be validated.</param>
        /// <param name="previousValidationResults">The previous validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="previousValidationResults" /> is <c>null</c>.</exception>
        public void BeforeValidateFields(object instance, List<IFieldValidationResult> previousValidationResults)
        {
            try
            {
                foreach (var validator in _validators)
                {
                    validator.BeforeValidateFields(instance, previousValidationResults);
                }
            }
            catch (Exception)
            {
                _synchronizationContext.Release();
                throw;
            }
        }

        /// <summary>
        /// Called just before the specified instance is about to be validate its business rules.
        /// </summary>
        /// <param name="instance">The instance that is about to be validated.</param>
        /// <param name="previousValidationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="previousValidationResults" /> is <c>null</c>.</exception>
        public void BeforeValidateBusinessRules(object instance, List<IBusinessRuleValidationResult> previousValidationResults)
        {
            try
            {
                foreach (var validator in _validators)
                {
                    validator.BeforeValidateBusinessRules(instance, previousValidationResults);
                }
            }
            catch (Exception)
            {
                _synchronizationContext.Release();
                throw;
            }
        }

        /// <summary>
        /// Validates the fields of the specified instance. The results must be added to the list of validation
        /// results.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResults" /> is <c>null</c>.</exception>
        public void ValidateFields(object instance, List<IFieldValidationResult> validationResults)
        {
            try
            {
                foreach (var validator in _validators)
                {
                    validator.ValidateFields(instance, validationResults);
                }
            }
            catch (Exception)
            {
                _synchronizationContext.Release();
                throw;
            }
        }

        /// <summary>
        /// Validates the business rules of the specified instance. The results must be added to the list of validation
        /// results.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResults" /> is <c>null</c>.</exception>
        public void ValidateBusinessRules(object instance, List<IBusinessRuleValidationResult> validationResults)
        {
            try
            {
                foreach (var validator in _validators)
                {
                    validator.ValidateBusinessRules(instance, validationResults);
                }
            }
            catch (Exception)
            {
                _synchronizationContext.Release();
                throw;
            }
        }

        #endregion

        /// <summary>
        /// Called just after the specified instance has validated its business rules.
        /// </summary>
        /// <param name="instance">The instance that has just been validated.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResults" /> is <c>null</c>.</exception>
        public void AfterValidateBusinessRules(object instance, List<IBusinessRuleValidationResult> validationResults)
        {
            try
            {
                foreach (var validator in _validators)
                {
                    validator.AfterValidateBusinessRules(instance, validationResults);
                }
            }
            catch (Exception)
            {
                _synchronizationContext.Release();
                throw;
            }
        }

        /// <summary>
        /// Called just after the specified instance has validated its fields.
        /// </summary>
        /// <param name="instance">The instance that has just been validated.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResults" /> is <c>null</c>.</exception>
        public void AfterValidateFields(object instance, List<IFieldValidationResult> validationResults)
        {
            try
            {
                foreach (var validator in _validators)
                {
                    validator.AfterValidateFields(instance, validationResults);
                }
            }
            catch (Exception)
            {
                _synchronizationContext.Release();
                throw;
            }
        }

        /// <summary>
        /// Called just after all validation has been executed.
        /// </summary>
        /// <param name="instance">The instance that has just been validated.</param>
        /// <param name="fieldValidationResults">The current field validation results.</param>
        /// <param name="businessRuleValidationResults">The current business rule validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="fieldValidationResults" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="businessRuleValidationResults" /> is <c>null</c>.</exception>
        public void AfterValidation(object instance, List<IFieldValidationResult> fieldValidationResults, List<IBusinessRuleValidationResult> businessRuleValidationResults)
        {
            try
            {
                foreach (var validator in _validators)
                {
                    validator.AfterValidation(instance, fieldValidationResults, businessRuleValidationResults);
                }
            }
            finally
            {
                _synchronizationContext.Release();
            }
        }
    }
}