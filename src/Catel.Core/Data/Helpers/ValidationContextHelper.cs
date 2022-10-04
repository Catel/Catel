namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The type of change that occurred to a validation context item.
    /// </summary>
    public enum ValidationContextChangeType
    {
        /// <summary>
        /// The item was added.
        /// </summary>
        Added,

        /// <summary>
        /// The item was removed.
        /// </summary>
        Removed
    }

    /// <summary>
    /// Class containing change information about an item in the validation context.
    /// </summary>
    public class ValidationContextChange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationContextChange"/> class.
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        /// <param name="changeType">Type of the change.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResult"/> is <c>null</c>.</exception>
        public ValidationContextChange(IValidationResult validationResult, ValidationContextChangeType changeType)
        {
            ValidationResult = validationResult;
            ChangeType = changeType;
        }

        /// <summary>
        /// Gets the validation result.
        /// </summary>
        /// <value>The validation result.</value>
        public IValidationResult ValidationResult { get; private set; }

        /// <summary>
        /// Gets the type of the change.
        /// </summary>
        /// <value>The type of the change.</value>
        public ValidationContextChangeType ChangeType { get; private set; }
    }

    /// <summary>
    /// Helper class for <see cref="IValidationContext"/> classes.
    /// </summary>
    public static class ValidationContextHelper
    {
        /// <summary>
        /// Gets the changes between two different validation contexts.
        /// </summary>
        /// <param name="firstContext">The first context.</param>
        /// <param name="secondContext">The second context.</param>
        /// <returns>The list of changes.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="firstContext"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="secondContext"/> is <c>null</c>.</exception>
        public static List<ValidationContextChange> GetChanges(IValidationContext firstContext, IValidationContext secondContext)
        {
            var changes = new List<ValidationContextChange>();

            // Loop all fields, check removed items
            foreach (var fieldValidationResult in firstContext.GetFieldValidations())
            {
                var secondContextFieldValidationResults = secondContext.GetFieldValidations(fieldValidationResult.PropertyName);
                bool stillContainsValidationResult = (from result in secondContextFieldValidationResults
                                                      where result.ValidationResultType == fieldValidationResult.ValidationResultType &&
                                                            string.Compare(result.Message, fieldValidationResult.Message) == 0
                                                      select result).Any();

                if (!stillContainsValidationResult)
                {
                    changes.Add(new ValidationContextChange(fieldValidationResult, ValidationContextChangeType.Removed));
                }
            }

            // Loop all fields, check added items
            foreach (var fieldValidationResult in secondContext.GetFieldValidations())
            {
                var firstContextFieldValidationResults = firstContext.GetFieldValidations(fieldValidationResult.PropertyName);
                bool existedInPreviousVersion = (from result in firstContextFieldValidationResults
                                                 where result.ValidationResultType == fieldValidationResult.ValidationResultType &&
                                                       string.Compare(result.Message, fieldValidationResult.Message) == 0
                                                 select result).Any();

                if (!existedInPreviousVersion)
                {
                    changes.Add(new ValidationContextChange(fieldValidationResult, ValidationContextChangeType.Added));
                }
            }

            // Loop all business rules, check removed items
            foreach (var businessRuleValidation in firstContext.GetBusinessRuleValidations())
            {
                var secondContextBusinessRuleValidationResults = secondContext.GetBusinessRuleValidations();
                bool stillContainsValidationResult = (from result in secondContextBusinessRuleValidationResults
                                                      where result.ValidationResultType == businessRuleValidation.ValidationResultType &&
                                                            string.Compare(result.Message, businessRuleValidation.Message) == 0
                                                      select result).Any();

                if (!stillContainsValidationResult)
                {
                    changes.Add(new ValidationContextChange(businessRuleValidation, ValidationContextChangeType.Removed));
                }
            }

            // Loop all business rules, check added items
            foreach (var businessRuleValidation in secondContext.GetBusinessRuleValidations())
            {
                var firstContextBusinessRuleValidationResults = firstContext.GetBusinessRuleValidations();
                bool existedInPreviousVersion = (from result in firstContextBusinessRuleValidationResults
                                                 where result.ValidationResultType == businessRuleValidation.ValidationResultType &&
                                                       string.Compare(result.Message, businessRuleValidation.Message) == 0
                                                 select result).Any();

                if (!existedInPreviousVersion)
                {
                    changes.Add(new ValidationContextChange(businessRuleValidation, ValidationContextChangeType.Added));
                }
            }

            return changes;
        }
    }
}
