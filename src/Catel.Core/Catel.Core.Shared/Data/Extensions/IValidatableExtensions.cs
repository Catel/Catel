// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValidatableExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System;
    using System.Text;

    /// <summary>
    /// Extensions for IValidatable.
    /// </summary>
    public static class IValidatableExtensions
    {
        /// <summary>
        /// Gets the validation context of the specified model.
        /// </summary>
        /// <param name="validatable">The model.</param>
        /// <returns>The validation context.</returns>
        public static IValidationContext GetValidationContext(this IValidatable validatable)
        {
            Argument.IsNotNull("model", validatable);

            return validatable.ValidationContext;
        }

        /// <summary>
        /// Adds the business rule validation result.
        /// </summary>
        /// <param name="validatable">The validatable model.</param>
        /// <param name="businessRuleValidationResult">The business rule validation result.</param>
        /// <param name="validate">if set to <c>true</c> [validate].</param>
        public static void AddBusinessRuleValidationResult(this IValidatable validatable, IBusinessRuleValidationResult businessRuleValidationResult, bool validate = false)
        {
            Argument.IsNotNull("modelValidation", validatable);
            Argument.IsNotNull("businessRuleValidationResult", businessRuleValidationResult);

            EventHandler<ValidationEventArgs> validating = null;
            validating = (sender, e) =>
            {
                validatable.Validating -= validating;
                e.ValidationContext.AddBusinessRuleValidationResult(businessRuleValidationResult);
            };

            validatable.Validating += validating;

            if (validate)
            {
                validatable.Validate(true);
            }
        }

        /// <summary>
        /// Adds the field validation result.
        /// </summary>
        /// <param name="validatable">The validatable model.</param>
        /// <param name="fieldValidationResult">The field validation result.</param>
        /// <param name="validate">if set to <c>true</c> [validate].</param>
        public static void AddFieldValidationResult(this IValidatable validatable, IFieldValidationResult fieldValidationResult, bool validate = false)
        {
            Argument.IsNotNull("modelValidation", validatable);
            Argument.IsNotNull("fieldValidationResult", fieldValidationResult);

            EventHandler<ValidationEventArgs> validating = null;
            validating = (sender, e) =>
            {
                validatable.Validating -= validating;
                e.ValidationContext.AddFieldValidationResult(fieldValidationResult);
            };

            validatable.Validating += validating;

            if (validate)
            {
                validatable.Validate(true);
            }
        }

        /// <summary>
        /// Returns a message that contains all the current warnings.
        /// </summary>
        /// <param name="validatable">The model base.</param>
        /// <param name="userFriendlyObjectName">Name of the user friendly object.</param>
        /// <returns>
        /// Warning string or empty in case of no warnings.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="validatable"/> is <c>null</c>.</exception>
        public static string GetWarningMessage(this IValidatable validatable, string userFriendlyObjectName = null)
        {
            Argument.IsNotNull("model", validatable);

            var validationContext = validatable.ValidationContext;

            if (!validationContext.HasWarnings)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(userFriendlyObjectName))
            {
                // Use the real entity name (stupid developer that passes a useless value)
                userFriendlyObjectName = validatable.GetType().Name;
            }

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine($"Found the following warnings in '{userFriendlyObjectName}'");
            messageBuilder.Append(validationContext.GetValidationsAsStringList(ValidationResultType.Warning));

            return messageBuilder.ToString();
        }

        /// <summary>
        /// Returns a message that contains all the current errors.
        /// </summary>
        /// <param name="validatable">The model base.</param>
        /// <param name="userFriendlyObjectName">Name of the user friendly object.</param>
        /// <returns>
        /// Error string or empty in case of no errors.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="validatable"/> is <c>null</c>.</exception>
        public static string GetErrorMessage(this IValidatable validatable, string userFriendlyObjectName = null)
        {
            Argument.IsNotNull("model", validatable);

            var validationContext = validatable.ValidationContext;

            if (!validationContext.HasErrors)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(userFriendlyObjectName))
            {
                // Use the real entity name (stupid developer that passes a useless value)
                userFriendlyObjectName = validatable.GetType().Name;
            }

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine($"Found the following errors in '{userFriendlyObjectName}'");
            messageBuilder.Append(validationContext.GetValidationsAsStringList(ValidationResultType.Error));

            return messageBuilder.ToString();
        }
    }
}