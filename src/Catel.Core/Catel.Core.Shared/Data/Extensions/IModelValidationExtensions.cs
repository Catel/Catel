// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelValidationExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;

    /// <summary>
    /// Extensions for model validation.
    /// </summary>
    public static class IModelValidationExtensions
    {
        #region Methods
        /// <summary>
        /// Gets the validation context of the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>The validation context.</returns>
        public static IValidationContext GetValidationContext(this IModelValidation model)
        {
            Argument.IsNotNull("model", model);

            return model.ValidationContext;
        }

        #region Public Methods and Operators
        /// <summary>
        /// Adds the business rule validation result.
        /// <para />
        /// This method is great to add asynchronous validation.
        /// </summary>
        /// <param name="modelValidation">The model validation.</param>
        /// <param name="businessRuleValidationResult">The business rule validation result.</param>
        /// <param name="validate">if set to <c>true</c>, this method call will immediately force a call to validate the model.</param>
        public static void AddBusinessRuleValidationResult(this IModelValidation modelValidation, IBusinessRuleValidationResult businessRuleValidationResult, bool validate = false)
        {
            Argument.IsNotNull(() => modelValidation);
            Argument.IsNotNull(() => businessRuleValidationResult);

            EventHandler<ValidationEventArgs> validating = null;
            validating = (sender, e) =>
            {
                modelValidation.Validating -= validating;
                e.ValidationContext.AddBusinessRuleValidationResult(businessRuleValidationResult);
            };

            modelValidation.Validating += validating;

            if (validate)
            {
                modelValidation.Validate(true);
            }
        }

        /// <summary>
        /// Adds the field validation result.
        /// <para />
        /// This method is great to add asynchronous validation.
        /// </summary>
        /// <param name="modelValidation">The model validation.</param>
        /// <param name="fieldValidationResult">The field validation result.</param>
        /// <param name="validate">if set to <c>true</c>, this method call will immediately force a call to validate the model.</param>
        public static void AddFieldValidationResult(this IModelValidation modelValidation, IFieldValidationResult fieldValidationResult, bool validate = false)
        {
            Argument.IsNotNull(() => modelValidation);
            Argument.IsNotNull(() => fieldValidationResult);

            EventHandler<ValidationEventArgs> validating = null;
            validating = (sender, e) =>
            {
                modelValidation.Validating -= validating;
                e.ValidationContext.AddFieldValidationResult(fieldValidationResult);
            };

            modelValidation.Validating += validating;

            if (validate)
            {
                modelValidation.Validate(true);
            }
        }
        #endregion

        #endregion
    }
}