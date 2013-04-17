// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidatorBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using Logging;
    using Catel.Reflection;

    /// <summary>
    /// Base class that implements the <see cref="IValidator"/> interface, but already implements dummy
    /// methods for the <c>Before</c> and <c>After</c> methods, which are rarely used (but still very
    /// useful in some cases).
    /// </summary>
    /// <typeparam name="TTargetType">The target type.</typeparam>
    public abstract class ValidatorBase<TTargetType> : IValidator
        where TTargetType : class
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Called just before any validation is caused.
        /// </summary>
        /// <param name="instance">The instance that is about to be validated.</param>
        /// <param name="previousFieldValidationResults">The previous field validation results.</param>
        /// <param name="previousBusinessRuleValidationResults">The previous business rule validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="previousFieldValidationResults"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="previousBusinessRuleValidationResults"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="instance"/> cannot be casted to <typeparamref name="TTargetType"/>.</exception>
        public void BeforeValidation(object instance, List<IFieldValidationResult> previousFieldValidationResults,
            List<IBusinessRuleValidationResult> previousBusinessRuleValidationResults)
        {
            var typedInstance = TypeHelper.GetTypedInstance<TTargetType>(instance);
            BeforeValidation(typedInstance, previousFieldValidationResults, previousBusinessRuleValidationResults);
        }

        /// <summary>
        /// Called just before any validation is caused.
        /// </summary>
        /// <param name="instance">The instance that is about to be validated.</param>
        /// <param name="previousFieldValidationResults">The previous field validation results.</param>
        /// <param name="previousBusinessRuleValidationResults">The previous business rule validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="previousFieldValidationResults"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="previousBusinessRuleValidationResults"/> is <c>null</c>.</exception>
        public virtual void BeforeValidation(TTargetType instance, List<IFieldValidationResult> previousFieldValidationResults,
            List<IBusinessRuleValidationResult> previousBusinessRuleValidationResults)
        {
            Argument.IsNotNull("instance", instance);
            Argument.IsNotNull("previousFieldValidationResults", previousFieldValidationResults);
            Argument.IsNotNull("previousBusinessRuleValidationResults", previousBusinessRuleValidationResults);

            // No implementation by default
        }

        /// <summary>
        /// Called just before the specified instance is about to be validate its fields.
        /// </summary>
        /// <param name="instance">The instance that is about to be validated.</param>
        /// <param name="previousValidationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="previousValidationResults"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="instance"/> cannot be casted to <typeparamref name="TTargetType"/>.</exception>
        public void BeforeValidateFields(object instance, List<IFieldValidationResult> previousValidationResults)
        {
            var typedInstance = TypeHelper.GetTypedInstance<TTargetType>(instance);
            BeforeValidateFields(typedInstance, previousValidationResults);
        }

        /// <summary>
        /// Called just before the specified instance is about to be validate its fields.
        /// </summary>
        /// <param name="instance">The instance that is about to be validated.</param>
        /// <param name="previousValidationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="previousValidationResults"/> is <c>null</c>.</exception>
        public virtual void BeforeValidateFields(TTargetType instance, List<IFieldValidationResult> previousValidationResults)
        {
            Argument.IsNotNull("instance", instance);
            Argument.IsNotNull("previousValidationResults", previousValidationResults);

            // No implementation by default
        }

        /// <summary>
        /// Validates the fields of the specified instance. The results must be added to the list of validation
        /// results.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResults"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="instance"/> cannot be casted to <typeparamref name="TTargetType"/>.</exception>
        public void ValidateFields(object instance, List<IFieldValidationResult> validationResults)
        {
            var typedInstance = TypeHelper.GetTypedInstance<TTargetType>(instance);
            ValidateFields(typedInstance, validationResults);
        }

        /// <summary>
        /// Validates the fields of the specified instance. The results must be added to the list of validation
        /// results.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResults"/> is <c>null</c>.</exception>
        /// <remarks>
        /// There is no need to check for the arguments, they are already ensured to be correct in the <see cref="ValidatorBase{TTargetType}"/>.
        /// </remarks>
        public abstract void ValidateFields(TTargetType instance, List<IFieldValidationResult> validationResults);

        /// <summary>
        /// Called just after the specified instance has validated its fields.
        /// </summary>
        /// <param name="instance">The instance that has just been validated.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResults"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="instance"/> cannot be casted to <typeparamref name="TTargetType"/>.</exception>
        public void AfterValidateFields(object instance, List<IFieldValidationResult> validationResults)
        {
            var typedInstance = TypeHelper.GetTypedInstance<TTargetType>(instance);
            AfterValidateFields(typedInstance, validationResults);
        }

        /// <summary>
        /// Called just after the specified instance has validated its fields.
        /// </summary>
        /// <param name="instance">The instance that has just been validated.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResults"/> is <c>null</c>.</exception>
        public virtual void AfterValidateFields(TTargetType instance, List<IFieldValidationResult> validationResults)
        {
            Argument.IsNotNull("instance", instance);
            Argument.IsNotNull("validationResults", validationResults);

            // No implementation by default
        }

        /// <summary>
        /// Called just before the specified instance is about to be validate its business rules.
        /// </summary>
        /// <param name="instance">The instance that is about to be validated.</param>
        /// <param name="previousValidationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="previousValidationResults"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="instance"/> cannot be casted to <typeparamref name="TTargetType"/>.</exception>
        public void BeforeValidateBusinessRules(object instance, List<IBusinessRuleValidationResult> previousValidationResults)
        {
            var typedInstance = TypeHelper.GetTypedInstance<TTargetType>(instance);
            BeforeValidateBusinessRules(typedInstance, previousValidationResults);
        }

        /// <summary>
        /// Called just before the specified instance is about to be validate its business rules.
        /// </summary>
        /// <param name="instance">The instance that is about to be validated.</param>
        /// <param name="previousValidationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="previousValidationResults"/> is <c>null</c>.</exception>
        public virtual void BeforeValidateBusinessRules(TTargetType instance, List<IBusinessRuleValidationResult> previousValidationResults)
        {
            Argument.IsNotNull("instance", instance);
            Argument.IsNotNull("previousValidationResults", previousValidationResults);

            // No implementation by default
        }

        /// <summary>
        /// Validates the business rules of the specified instance. The results must be added to the list of validation
        /// results.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResults"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="instance"/> cannot be casted to <typeparamref name="TTargetType"/>.</exception>
        public void ValidateBusinessRules(object instance, List<IBusinessRuleValidationResult> validationResults)
        {
            var typedInstance = TypeHelper.GetTypedInstance<TTargetType>(instance);
            ValidateBusinessRules(typedInstance, validationResults);
        }

        /// <summary>
        /// Validates the business rules of the specified instance. The results must be added to the list of validation
        /// results.
        /// </summary>
        /// <param name="instance">The instance to validate.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResults"/> is <c>null</c>.</exception>
        /// <remarks>
        /// There is no need to check for the arguments, they are already ensured to be correct in the <see cref="ValidatorBase{TTargetType}"/>.
        /// </remarks>
        public abstract void ValidateBusinessRules(TTargetType instance, List<IBusinessRuleValidationResult> validationResults);

        /// <summary>
        /// Called just after the specified instance has validated its business rules.
        /// </summary>
        /// <param name="instance">The instance that has just been validated.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResults"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="instance"/> cannot be casted to <typeparamref name="TTargetType"/>.</exception>
        public void AfterValidateBusinessRules(object instance, List<IBusinessRuleValidationResult> validationResults)
        {
            var typedInstance = TypeHelper.GetTypedInstance<TTargetType>(instance);
            AfterValidateBusinessRules(typedInstance, validationResults);
        }

        /// <summary>
        /// Called just after the specified instance has validated its business rules.
        /// </summary>
        /// <param name="instance">The instance that has just been validated.</param>
        /// <param name="validationResults">The validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResults"/> is <c>null</c>.</exception>
        public virtual void AfterValidateBusinessRules(TTargetType instance, List<IBusinessRuleValidationResult> validationResults)
        {
            Argument.IsNotNull("instance", instance);
            Argument.IsNotNull("validationResults", validationResults);

            // No implementation by default
        }

        /// <summary>
        /// Called just after all validation has been executed.
        /// </summary>
        /// <param name="instance">The instance that has just been validated.</param>
        /// <param name="fieldValidationResults">The current field validation results.</param>
        /// <param name="businessRuleValidationResults">The current business rule validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="fieldValidationResults"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="businessRuleValidationResults"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="instance"/> cannot be casted to <typeparamref name="TTargetType"/>.</exception>
        public void AfterValidation(object instance, List<IFieldValidationResult> fieldValidationResults,
            List<IBusinessRuleValidationResult> businessRuleValidationResults)
        {
            var typedInstance = TypeHelper.GetTypedInstance<TTargetType>(instance);
            AfterValidation(typedInstance, fieldValidationResults, businessRuleValidationResults);
        }

        /// <summary>
        /// Called just after all validation has been executed.
        /// </summary>
        /// <param name="instance">The instance that has just been validated.</param>
        /// <param name="fieldValidationResults">The current field validation results.</param>
        /// <param name="businessRuleValidationResults">The current business rule validation results.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="fieldValidationResults"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="businessRuleValidationResults"/> is <c>null</c>.</exception>
        public virtual void AfterValidation(TTargetType instance, List<IFieldValidationResult> fieldValidationResults,
            List<IBusinessRuleValidationResult> businessRuleValidationResults)
        {
            Argument.IsNotNull("instance", instance);
            Argument.IsNotNull("fieldValidationResults", fieldValidationResults);
            Argument.IsNotNull("businessRuleValidationResults", businessRuleValidationResults);

            // No implementation by default
        }
    }
}
