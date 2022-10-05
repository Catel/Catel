namespace Catel.Data
{
    using System;
    using System.Text;
    using Text;

    /// <summary>
    /// Extension methods for the validation context.
    /// </summary>
    public static class IValidationContextExtensions
    {
        /// <summary>
        /// Checks whether the validation context contains warnings or errors.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>
        /// String representing the output of all items in the fields an business object.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="validationContext"/> is <c>null</c>.</exception>
        public static bool HasWarningsOrErrors(this IValidationContext validationContext)
        {
            return validationContext.HasWarnings || validationContext.HasErrors;
        }

        /// <summary>
        /// Gets the list messages.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <param name="validationResult">The validation result.</param>
        /// <returns>
        /// String representing the output of all items in the fields an business object.
        /// </returns>
        /// <remarks>
        /// This method is used to create a message string for field warnings or errors and business warnings
        /// or errors. Just pass the right dictionary and list to this method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="validationContext"/> is <c>null</c>.</exception>
        public static string GetValidationsAsStringList(this IValidationContext validationContext, ValidationResultType validationResult)
        {
            var messageBuilder = new StringBuilder();

            switch (validationResult)
            {
                case ValidationResultType.Warning:
                    foreach (var field in validationContext.GetFieldWarnings())
                    {
                        messageBuilder.AppendLine("* {0}", field.Message);
                    }

                    foreach (var businessItem in validationContext.GetBusinessRuleWarnings())
                    {
                        messageBuilder.AppendLine("* {0}", businessItem.Message);
                    }
                    break;

                case ValidationResultType.Error:
                    foreach (var field in validationContext.GetFieldErrors())
                    {
                        messageBuilder.AppendLine("* {0}", field.Message);
                    }

                    foreach (var businessItem in validationContext.GetBusinessRuleErrors())
                    {
                        messageBuilder.AppendLine("* {0}", businessItem.Message);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(validationResult));
            }

            return messageBuilder.ToString();
        }
    }
}
