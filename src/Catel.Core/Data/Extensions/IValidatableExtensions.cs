namespace Catel.Data
{
    using System;
    using System.Linq;
    using System.Text;
    using Text;

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
            return validatable.ValidationContext;
        }

        /// <summary>
        /// Adds the business rule validation result.
        /// </summary>
        /// <param name="validatable">The validatable model.</param>
        /// <param name="businessRuleValidationResult">The business rule validation result.</param>
        /// <param name="validate">if set to <c>true</c> [validate].</param>
        public static void Add(this IValidatable validatable, IBusinessRuleValidationResult businessRuleValidationResult, bool validate = false)
        {
            EventHandler<ValidationEventArgs>? validating = null;
            validating = (sender, e) =>
            {
                validatable.Validating -= validating;
                e.ValidationContext.Add(businessRuleValidationResult);
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
        public static void Add(this IValidatable validatable, IFieldValidationResult fieldValidationResult, bool validate = false)
        {
            EventHandler<ValidationEventArgs>? validating = null;
            validating = (sender, e) =>
            {
                validatable.Validating -= validating;
                e.ValidationContext.Add(fieldValidationResult);
            };

            validatable.Validating += validating;

            if (validate)
            {
                validatable.Validate(true);
            }
        }

        /// <summary>
        /// Gets the current business warnings.
        /// </summary>
        /// <returns>The warnings or <see cref="string.Empty"/> if no warning is available.</returns>
        public static string GetBusinessRuleWarnings(this IValidatable validatable)
        {
            var warning = (from businessRuleWarning in validatable.ValidationContext.GetBusinessRuleWarnings()
                           select businessRuleWarning.Message).FirstOrDefault();

            return warning ?? string.Empty;
        }

        /// <summary>
        /// Gets the warnings for a specific column.
        /// </summary>
        /// <param name="validatable">The model.</param>
        /// <param name="columnName">Column name.</param>
        /// <returns>
        /// The warnings or <see cref="string.Empty" /> if no warning is available.
        /// </returns>
        public static string GetFieldWarnings(this IValidatable validatable, string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
            {
                return string.Empty;
            }

            var warning = (from fieldWarning in validatable.ValidationContext.GetFieldWarnings(columnName)
                           select fieldWarning.Message).FirstOrDefault();

            return warning ?? string.Empty;
        }

        /// <summary>
        /// Gets the current errors errors.
        /// </summary>
        /// <returns>The errors or <see cref="string.Empty"/> if no error is available.</returns>
        public static string GetBusinessRuleErrors(this IValidatable validatable)
        {
            var error = (from businessRuleError in validatable.ValidationContext.GetBusinessRuleErrors()
                         select businessRuleError.Message).FirstOrDefault();

            return error ?? string.Empty;
        }

        /// <summary>
        /// Gets the errors for a specific column.
        /// </summary>
        /// <param name="validatable">The model.</param>
        /// <param name="columnName">Column name.</param>
        /// <returns>
        /// The errors or <see cref="string.Empty" /> if no error is available.
        /// </returns>
        public static string GetFieldErrors(this IValidatable validatable, string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
            {
                return string.Empty;
            }

            var error = (from fieldError in validatable.ValidationContext.GetFieldErrors(columnName)
                         select fieldError.Message).FirstOrDefault();

            return error ?? string.Empty;
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
