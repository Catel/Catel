// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FluentValidatorToCatelValidatorAdapter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Data;
    using Reflection;

    using IValidator = FluentValidation.IValidator;
    using ValidationResult = FluentValidation.Results.ValidationResult;

    /// <summary>
    /// The fluent to catel validator adapter.
    /// </summary>
    internal class FluentValidatorToCatelValidatorAdapter : ValidatorBase<ModelBase>
    {
        #region Constants and Fields

        /// <summary>
        /// The validator.
        /// </summary>
        private readonly IValidator _validator;

        /// <summary>
        /// The validator description attribute.
        /// </summary>
        private readonly ValidatorDescriptionAttribute _validatorDescriptionAttribute;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FluentValidatorToCatelValidatorAdapter"/> class.
        /// </summary>
        /// <param name="validatorType">
        /// The validator type.
        /// </param>
        private FluentValidatorToCatelValidatorAdapter(Type validatorType)
        {
            ConstructorInfo constructorInfo = validatorType.GetConstructor(new Type[] { });
            if (constructorInfo != null)
            {
                _validator = (IValidator)constructorInfo.Invoke(new object[] { });
            }

            if (!AttributeHelper.TryGetAttribute(validatorType, out _validatorDescriptionAttribute))
            {
                _validatorDescriptionAttribute = new ValidatorDescriptionAttribute(validatorType.Name);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The validate business rules.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="validationResults">The validation results.</param>
        public override void ValidateBusinessRules(ModelBase instance, List<IBusinessRuleValidationResult> validationResults)
        {
            if (_validatorDescriptionAttribute.ValidationType == ValidationType.BusinessRule)
            {
                ValidationResult validationResult = _validator.Validate(instance);

                if (!validationResult.IsValid)
                {
                    validationResults.AddRange(validationResult.Errors.Select(validationFailure => new BusinessRuleValidationResult(
                    _validatorDescriptionAttribute.ValidationResultType, validationFailure.ErrorMessage)
                    {
                        Tag = _validatorDescriptionAttribute.Tag 
                    }).Cast<IBusinessRuleValidationResult>());
                }
            }
        }

        /// <summary>
        /// The validate fields.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="validationResults">The validation results.</param>
        public override void ValidateFields(ModelBase instance, List<IFieldValidationResult> validationResults)
        {
            if (_validatorDescriptionAttribute.ValidationType == ValidationType.Field)
            {
                ValidationResult validationResult = _validator.Validate(instance);
                if (!validationResult.IsValid)
                {
                    validationResults.AddRange(validationResult.Errors.Select(fieldValidationResult =>new FieldValidationResult(
                                fieldValidationResult.PropertyName, _validatorDescriptionAttribute.ValidationResultType,
                                fieldValidationResult.ErrorMessage)
                                {
                                    Tag = _validatorDescriptionAttribute.Tag 
                                }).Cast<IFieldValidationResult>());
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates an instance of <see cref="Data.IValidator" /> from an <see cref="FluentValidation.AbstractValidator{T}" /> type implementation.
        /// </summary>
        /// <param name="validatorType"><see cref="FluentValidation.IValidator" /> type implementation.</param>
        /// <returns>An instance of <see cref="FluentValidatorToCatelValidatorAdapter" />.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="validatorType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="validatorType" /> is not of type <see cref="IValidator" />.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="validatorType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="validatorType" /> is not of type <see cref="IValidator" />.</exception>
        internal static Data.IValidator From(Type validatorType)
        {
            Argument.IsNotNull("validatorType", validatorType);
            Argument.IsOfType("validatorType", validatorType, typeof(IValidator));

            return new FluentValidatorToCatelValidatorAdapter(validatorType);
        }

        /// <summary>
        /// Creates an instance of <see cref="Data.IValidator" /> from a collection of <see cref="FluentValidation.AbstractValidator{T}" /> types.
        /// </summary>
        /// <param name="validatorTypes">Collection of <see cref="FluentValidation.IValidator" /> types.</param>
        /// <returns>An instance of a class the implements <see cref="Catel.Data.IValidator" />. If the collection contains one element an instance of <see cref="FluentValidatorToCatelValidatorAdapter" /> is returned otherwise
        /// a <see cref="CompositeValidator" /> is created in order to aggregate the validators in a single one.</returns>
        /// <exception cref="System.ArgumentException">Argument 'validatorTypes' must contains at least one element.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="validatorTypes" /> is <c>null</c>.</exception>
        internal static Data.IValidator From(IList<Type> validatorTypes)
        {
            Argument.IsNotNull("validatorTypes", validatorTypes);
            
            if (validatorTypes.Count == 0)
            {
                throw new ArgumentException("Argument 'validatorTypes' must contains at least one element.");
            }

            Data.IValidator validator;
            if (validatorTypes.Count > 1)
            {
                var compositeValidator = new CompositeValidator();
                foreach (Type validatorType in validatorTypes)
                {
                    compositeValidator.Add(From(validatorType));
                }

                validator = compositeValidator;
            }
            else
            {
                validator = From(validatorTypes.FirstOrDefault());
            }

            return validator;
        }

        /// <summary>
        /// Creates an instance of <see cref="Data.IValidator" /> from a generic type <see cref="FluentValidation.IValidator" /> parameter.
        /// </summary>
        /// <typeparam name="TValidator">Type of <see cref="FluentValidation.IValidator" />.</typeparam>
        /// <returns>An instance of <see cref="FluentValidatorToCatelValidatorAdapter" />.</returns>
        internal static Data.IValidator From<TValidator>() where TValidator : IValidator, new()
        {
            return new FluentValidatorToCatelValidatorAdapter(typeof(TValidator));
        }

        #endregion
    }
}