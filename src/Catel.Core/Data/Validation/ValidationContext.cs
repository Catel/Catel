namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Text;
    using System.Diagnostics;

    /// <summary>
    /// Context containing all validation and provides several methods to gather this information.
    /// </summary>
    public class ValidationContext : IValidationContext
    {
        /// <summary>
        /// The stop watch which will give accurate modification stamps.
        /// </summary>
        private static readonly Stopwatch _stopWatch = new Stopwatch();

        /// <summary>
        /// List of field validations.
        /// </summary>
        private readonly List<IFieldValidationResult> _fieldValidations = new List<IFieldValidationResult>();

        /// <summary>
        /// List of business rule validations.
        /// </summary>
        private readonly List<IBusinessRuleValidationResult> _businessRuleValidations = new List<IBusinessRuleValidationResult>();

        /// <summary>
        /// Initializes static members of the <see cref="ValidationContext"/> class.
        /// </summary>
        static ValidationContext()
        {
            _stopWatch.Start();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationContext"/> class.
        /// </summary>
        public ValidationContext()
            : this(null, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationContext"/> class.
        /// </summary>
        /// <param name="fieldValidationResults">The field validation results. Can be <c>null</c> to add no field validation results.</param>
        /// <param name="businessRuleValidationResults">The business rule validation results. Can be <c>null</c> to add no business rule validations.</param>
        public ValidationContext(IEnumerable<IFieldValidationResult>? fieldValidationResults, IEnumerable<IBusinessRuleValidationResult>? businessRuleValidationResults)
            : this(fieldValidationResults, businessRuleValidationResults, FastDateTime.Now)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationContext" /> class.
        /// </summary>
        /// <param name="fieldValidationResults">The field validation results. Can be <c>null</c> to add no field validation results.</param>
        /// <param name="businessRuleValidationResults">The business rule validation results. Can be <c>null</c> to add no business rule validations.</param>
        /// <param name="lastModified">The last modified date/time.</param>
        public ValidationContext(IEnumerable<IFieldValidationResult>? fieldValidationResults, IEnumerable<IBusinessRuleValidationResult>? businessRuleValidationResults, DateTime lastModified)
        {
            bool fieldValidationsIsNull = true;
            if (fieldValidationResults is not null)
            {
                fieldValidationsIsNull = false;
                _fieldValidations.AddRange(fieldValidationResults);
            }

            bool businessRuleValidationsIsNull = true;
            if (businessRuleValidationResults is not null)
            {
                businessRuleValidationsIsNull = false;
                _businessRuleValidations.AddRange(businessRuleValidationResults);
            }

            UpdateLastModificationStamp(lastModified, fieldValidationsIsNull && businessRuleValidationsIsNull);
        }

        /// <summary>
        /// Gets the last modified date/time.
        /// <para />
        /// Note that this is just an informational value and should not be used for comparisons. The <see cref="DateTime"/> 
        /// is not accurate enough. Use the <c>LastModifiedTicks</c> instead. 
        /// </summary>
        /// <value>The last modified date/time.</value>
        public DateTime LastModified { get; private set; }

        /// <summary>
        /// Gets the last modified ticks which is much more precise that the <see cref="LastModified"/>. Use this value
        /// to compare last modification ticks on other validation contexts.
        /// <para />
        /// Because only full .NET provides a stopwatch, this property is only available in full .NET. All other target frameworks
        /// will return the <see cref="DateTime.Ticks"/> which is <c>not</c> reliable.
        /// </summary>
        /// <value>The last modified ticks.</value>
        public long LastModifiedTicks { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance contains warnings.
        /// </summary>
        public bool HasWarnings
        {
            get { return (GetFieldWarningCount() != 0) || (GetBusinessRuleWarningCount() != 0); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance contains errors.
        /// </summary>
        public bool HasErrors
        {
            get { return (GetFieldErrorCount() != 0) || (GetBusinessRuleErrorCount() != 0); }
        }

        /// <summary>
        /// Gets the total validation count of all fields and business rules.
        /// </summary>
        /// <returns>
        /// The number of validations available.
        /// </returns>
        public int GetValidationCount()
        {
            return GetValidations().Count;
        }

        /// <summary>
        /// Gets the total validation count of all fields and business rules with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>The number of validations available.</returns>
        public int GetValidationCount(object? tag)
        {
            return GetValidations(tag).Count;
        }

        /// <summary>
        /// Gets all the field and business rule validations.
        /// </summary>
        /// <returns>
        /// List of <see cref="IValidationResult"/> items.
        /// </returns>
        public List<IValidationResult> GetValidations()
        {
            var list = new List<IValidationResult>();

            lock (_fieldValidations)
            {
                list.AddRange(from validation in _fieldValidations
                              select validation as IValidationResult);
            }

            lock (_businessRuleValidations)
            {
                list.AddRange(from validation in _businessRuleValidations
                              select validation as IValidationResult);
            }

            return list;
        }

        /// <summary>
        /// Gets all the field and business rule validations with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// List of <see cref="IValidationResult"/> items.
        /// </returns>
        public List<IValidationResult> GetValidations(object? tag)
        {
            var list = new List<IValidationResult>();

            lock (_fieldValidations)
            {
                list.AddRange(from validation in _fieldValidations
                              where TagHelper.AreTagsEqual(validation.Tag, tag)
                              select validation as IValidationResult);
            }

            lock (_businessRuleValidations)
            {
                list.AddRange(from validation in _businessRuleValidations
                              where TagHelper.AreTagsEqual(validation.Tag, tag)
                              select validation as IValidationResult);
            }

            return list;
        }

        /// <summary>
        /// Gets the number of field and business rule warnings inside this context.
        /// </summary>
        /// <returns>The number of warnings available.</returns>
        public int GetWarningCount()
        {
            return GetWarnings().Count;
        }

        /// <summary>
        /// Gets the number of field and business rule warnings with the specified tag inside this context.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>The number of warnings available.</returns>
        public int GetWarningCount(object? tag)
        {
            return GetWarnings(tag).Count;
        }

        /// <summary>
        /// Gets all field and business rule warnings.
        /// </summary>
        /// <returns>List of <see cref="IValidationResult" /> items.</returns>
        public List<IValidationResult> GetWarnings()
        {
            var validationResults = new List<IValidationResult>();

            validationResults.AddRange(GetFieldWarnings().Cast<IValidationResult>());
            validationResults.AddRange(GetBusinessRuleWarnings().Cast<IValidationResult>());

            return validationResults;
        }

        /// <summary>
        /// Gets all field and business rule warnings with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>List of <see cref="IValidationResult" /> items.</returns>
        public List<IValidationResult> GetWarnings(object? tag)
        {
            var validationResults = new List<IValidationResult>();

            validationResults.AddRange(GetFieldWarnings(tag).Cast<IValidationResult>());
            validationResults.AddRange(GetBusinessRuleWarnings(tag).Cast<IValidationResult>());

            return validationResults;
        }

        /// <summary>
        /// Gets the number of field and business rule errors inside this context.
        /// </summary>
        /// <returns>The number of errors available.</returns>
        public int GetErrorCount()
        {
            return GetErrors().Count;
        }

        /// <summary>
        /// Gets the number of field and business rule errors with the specified tag inside this context.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>The number of errors available.</returns>
        public int GetErrorCount(object? tag)
        {
            return GetErrors(tag).Count;
        }

        /// <summary>
        /// Gets all field and business rule errors.
        /// </summary>
        /// <returns>List of <see cref="IValidationResult" /> items.</returns>
        public List<IValidationResult> GetErrors()
        {
            var validationResults = new List<IValidationResult>();

            // Note: casts cannot be removed
            validationResults.AddRange(GetFieldErrors().Cast<IValidationResult>());
            validationResults.AddRange(GetBusinessRuleErrors().Cast<IValidationResult>());

            return validationResults;
        }

        /// <summary>
        /// Gets all field and business rule errors with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>List of <see cref="IValidationResult" /> items.</returns>
        public List<IValidationResult> GetErrors(object? tag)
        {
            var validationResults = new List<IValidationResult>();

            // Note: casts cannot be removed
            validationResults.AddRange(GetFieldErrors(tag).Cast<IValidationResult>());
            validationResults.AddRange(GetBusinessRuleErrors(tag).Cast<IValidationResult>());

            return validationResults;
        }

        /// <summary>
        /// Gets the field validation count of all fields.
        /// </summary>
        /// <returns>The number of field validations available.</returns>
        public int GetFieldValidationCount()
        {
            return GetFieldValidations().Count;
        }

        /// <summary>
        /// Gets the field validation count of all fields with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>The number of field validations available.</returns>
        public int GetFieldValidationCount(object? tag)
        {
            return GetFieldValidations(tag).Count;
        }

        /// <summary>
        /// Gets all the field validations.
        /// </summary>
        /// <returns>List of <see cref="IFieldValidationResult" /> items.</returns>
        public List<IFieldValidationResult> GetFieldValidations()
        {
            lock (_fieldValidations)
            {
                var list = (from validation in _fieldValidations
                            select validation).ToList();

                return list;
            }
        }

        /// <summary>
        /// Gets all the field validations with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>List of <see cref="IFieldValidationResult" /> items.</returns>
        public List<IFieldValidationResult> GetFieldValidations(object? tag)
        {
            lock (_fieldValidations)
            {
                var list = (from validation in _fieldValidations
                            where TagHelper.AreTagsEqual(validation.Tag, tag)
                            select validation).ToList();

                return list;
            }
        }

        /// <summary>
        /// Gets all the field validations for the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>List of <see cref="IFieldValidationResult" /> items.</returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName" /> is <c>null</c> or whitespace.</exception>
        public List<IFieldValidationResult> GetFieldValidations(string propertyName)
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            lock (_fieldValidations)
            {
                var list = (from validation in _fieldValidations
                            where string.Equals(validation.PropertyName, propertyName, StringComparison.OrdinalIgnoreCase)
                            select validation).ToList();

                return list;
            }
        }

        /// <summary>
        /// Gets all the field validations for the specified property name with the specified tag.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>List of <see cref="IFieldValidationResult" /> items.</returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName" /> is <c>null</c> or whitespace.</exception>
        public List<IFieldValidationResult> GetFieldValidations(string propertyName, object? tag)
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            lock (_fieldValidations)
            {
                var list = (from validation in _fieldValidations
                            where string.Equals(validation.PropertyName, propertyName, StringComparison.OrdinalIgnoreCase) &&
                                  TagHelper.AreTagsEqual(validation.Tag, tag)
                            select validation).ToList();

                return list;
            }
        }

        /// <summary>
        /// Gets the field warning count of all fields.
        /// </summary>
        /// <returns>The number of field warnings available.</returns>
        public int GetFieldWarningCount()
        {
            return GetFieldWarnings().Count;
        }

        /// <summary>
        /// Gets the field warning count of all fields with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>The number of field warnings available.</returns>
        public int GetFieldWarningCount(object? tag)
        {
            return GetFieldWarnings(tag).Count;
        }

        /// <summary>
        /// Gets all the field warnings.
        /// </summary>
        /// <returns>List of <see cref="IFieldValidationResult" /> items.</returns>
        public List<IFieldValidationResult> GetFieldWarnings()
        {
            lock (_fieldValidations)
            {
                var list = (from validation in _fieldValidations
                            where validation.ValidationResultType == ValidationResultType.Warning
                            select validation).ToList();

                return list;
            }
        }

        /// <summary>
        /// Gets all the field warnings with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>List of <see cref="IFieldValidationResult" /> items.</returns>
        public List<IFieldValidationResult> GetFieldWarnings(object? tag)
        {
            lock (_fieldValidations)
            {
                var list = (from validation in _fieldValidations
                            where validation.ValidationResultType == ValidationResultType.Warning &&
                                  TagHelper.AreTagsEqual(validation.Tag, tag)
                            select validation).ToList();

                return list;
            }
        }

        /// <summary>
        /// Gets all the field warnings for the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// List of <see cref="IFieldValidationResult"/> items.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        public List<IFieldValidationResult> GetFieldWarnings(string propertyName)
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            lock (_fieldValidations)
            {
                var list = (from validation in _fieldValidations
                            where string.Equals(validation.PropertyName, propertyName, StringComparison.OrdinalIgnoreCase) &&
                                  validation.ValidationResultType == ValidationResultType.Warning
                            select validation).ToList();

                return list;
            }
        }

        /// <summary>
        /// Gets all the field warnings for the specified property name with the specified tag.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// List of <see cref="IFieldValidationResult"/> items.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        public List<IFieldValidationResult> GetFieldWarnings(string propertyName, object? tag)
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            lock (_fieldValidations)
            {
                var list = (from validation in _fieldValidations
                            where string.Equals(validation.PropertyName, propertyName, StringComparison.OrdinalIgnoreCase) &&
                                  validation.ValidationResultType == ValidationResultType.Warning &&
                                  TagHelper.AreTagsEqual(validation.Tag, tag)
                            select validation).ToList();

                return list;
            }
        }

        /// <summary>
        /// Gets the field error count of all fields.
        /// </summary>
        /// <returns>The number of field errors available.</returns>
        public int GetFieldErrorCount()
        {
            return GetFieldErrors().Count;
        }

        /// <summary>
        /// Gets the field error count of all fields with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>The number of field errors available.</returns>
        public int GetFieldErrorCount(object? tag)
        {
            return GetFieldErrors(tag).Count;
        }

        /// <summary>
        /// Gets all the field errors.
        /// </summary>
        /// <returns>
        /// List of <see cref="IFieldValidationResult"/> items.
        /// </returns>
        public List<IFieldValidationResult> GetFieldErrors()
        {
            lock (_fieldValidations)
            {
                var list = (from validation in _fieldValidations
                            where validation.ValidationResultType == ValidationResultType.Error
                            select validation).ToList();

                return list;
            }
        }

        /// <summary>
        /// Gets all the field errors with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// List of <see cref="IFieldValidationResult"/> items.
        /// </returns>
        public List<IFieldValidationResult> GetFieldErrors(object? tag)
        {
            lock (_fieldValidations)
            {
                var list = (from validation in _fieldValidations
                            where validation.ValidationResultType == ValidationResultType.Error &&
                                  TagHelper.AreTagsEqual(validation.Tag, tag)
                            select validation).ToList();

                return list;
            }
        }

        /// <summary>
        /// Gets all the field errors for the specified property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// List of <see cref="IFieldValidationResult"/> items.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        public List<IFieldValidationResult> GetFieldErrors(string propertyName)
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            lock (_fieldValidations)
            {
                var list = (from validation in _fieldValidations
                            where string.Equals(validation.PropertyName, propertyName, StringComparison.OrdinalIgnoreCase) &&
                                  validation.ValidationResultType == ValidationResultType.Error
                            select validation).ToList();

                return list;
            }
        }

        /// <summary>
        /// Gets all the field errors for the specified property name with the specified tag.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// List of <see cref="IFieldValidationResult"/> items.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        public List<IFieldValidationResult> GetFieldErrors(string propertyName, object? tag)
        {
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            lock (_fieldValidations)
            {
                var list = (from validation in _fieldValidations
                            where string.Equals(validation.PropertyName, propertyName, StringComparison.OrdinalIgnoreCase) &&
                                  validation.ValidationResultType == ValidationResultType.Error &&
                                  TagHelper.AreTagsEqual(validation.Tag, tag)
                            select validation).ToList();

                return list;
            }
        }

        /// <summary>
        /// Gets the business rule validation count.
        /// </summary>
        /// <returns>
        /// The number of business rule validations available.
        /// </returns>
        public int GetBusinessRuleValidationCount()
        {
            return GetBusinessRuleValidations().Count;
        }

        /// <summary>
        /// Gets the business rule validation count with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// The number of business rule validations available.
        /// </returns>
        public int GetBusinessRuleValidationCount(object? tag)
        {
            return GetBusinessRuleValidations(tag).Count;
        }

        /// <summary>
        /// Gets all the business rule validations.
        /// </summary>
        /// <returns>
        /// List of <see cref="IBusinessRuleValidationResult"/> items.
        /// </returns>
        public List<IBusinessRuleValidationResult> GetBusinessRuleValidations()
        {
            lock (_businessRuleValidations)
            {
                var list = (from validation in _businessRuleValidations
                            select validation).ToList();

                return list;
            }
        }

        /// <summary>
        /// Gets all the business rule validations with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// List of <see cref="IBusinessRuleValidationResult"/> items.
        /// </returns>
        public List<IBusinessRuleValidationResult> GetBusinessRuleValidations(object? tag)
        {
            lock (_businessRuleValidations)
            {
                var list = (from validation in _businessRuleValidations
                            where TagHelper.AreTagsEqual(validation.Tag, tag)
                            select validation).ToList();

                return list;
            }
        }

        /// <summary>
        /// Gets the business rule warning count.
        /// </summary>
        /// <returns>
        /// The number of business rule warnings available.
        /// </returns>
        public int GetBusinessRuleWarningCount()
        {
            return GetBusinessRuleWarnings().Count;
        }

        /// <summary>
        /// Gets the business rule warning count with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// The number of business rule warnings available.
        /// </returns>
        public int GetBusinessRuleWarningCount(object? tag)
        {
            return GetBusinessRuleWarnings(tag).Count;
        }

        /// <summary>
        /// Gets all the business rule warnings.
        /// </summary>
        /// <returns>
        /// List of <see cref="IBusinessRuleValidationResult"/> items.
        /// </returns>
        public List<IBusinessRuleValidationResult> GetBusinessRuleWarnings()
        {
            lock (_businessRuleValidations)
            {
                var list = (from validation in _businessRuleValidations
                            where validation.ValidationResultType == ValidationResultType.Warning
                            select validation).ToList();

                return list;
            }
        }

        /// <summary>
        /// Gets all the business rule warnings with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// List of <see cref="IBusinessRuleValidationResult"/> items.
        /// </returns>
        public List<IBusinessRuleValidationResult> GetBusinessRuleWarnings(object? tag)
        {
            lock (_businessRuleValidations)
            {
                var list = (from validation in _businessRuleValidations
                            where validation.ValidationResultType == ValidationResultType.Warning &&
                                  TagHelper.AreTagsEqual(validation.Tag, tag)
                            select validation).ToList();

                return list;
            }
        }

        /// <summary>
        /// Gets the business rule error count.
        /// </summary>
        /// <returns>
        /// The number of business rule errors available.
        /// </returns>
        public int GetBusinessRuleErrorCount()
        {
            return GetBusinessRuleErrors().Count;
        }

        /// <summary>
        /// Gets the business rule error count with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// The number of business rule errors available.
        /// </returns>
        public int GetBusinessRuleErrorCount(object? tag)
        {
            return GetBusinessRuleErrors(tag).Count;
        }

        /// <summary>
        /// Gets all the business rule errors.
        /// </summary>
        /// <returns>
        /// List of <see cref="IBusinessRuleValidationResult"/> items.
        /// </returns>
        public List<IBusinessRuleValidationResult> GetBusinessRuleErrors()
        {
            lock (_businessRuleValidations)
            {
                var list = (from validation in _businessRuleValidations
                            where validation.ValidationResultType == ValidationResultType.Error
                            select validation).ToList();

                return list;
            }
        }

        /// <summary>
        /// Gets all the business rule errors with the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// List of <see cref="IBusinessRuleValidationResult"/> items.
        /// </returns>
        public List<IBusinessRuleValidationResult> GetBusinessRuleErrors(object? tag)
        {
            lock (_businessRuleValidations)
            {
                var list = (from validation in _businessRuleValidations
                            where validation.ValidationResultType == ValidationResultType.Error &&
                                  TagHelper.AreTagsEqual(validation.Tag, tag)
                            select validation).ToList();

                return list;
            }
        }

        /// <summary>
        /// Adds the field validation result.
        /// </summary>
        /// <param name="fieldValidationResult">The field validation result.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="fieldValidationResult"/> is <c>null</c>.</exception>
        public void Add(IFieldValidationResult fieldValidationResult)
        {
            ArgumentNullException.ThrowIfNull(fieldValidationResult);

            lock (_fieldValidations)
            {
                _fieldValidations.Add(fieldValidationResult);

                UpdateLastModificationStamp(FastDateTime.Now);
            }
        }

        /// <summary>
        /// Removes the field validation result.
        /// </summary>
        /// <param name="fieldValidationResult">The field validation result.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="fieldValidationResult"/> is <c>null</c>.</exception>
        public void Remove(IFieldValidationResult fieldValidationResult)
        {
            ArgumentNullException.ThrowIfNull(fieldValidationResult);

            lock (_fieldValidations)
            {
                _fieldValidations.Remove(fieldValidationResult);

                UpdateLastModificationStamp(FastDateTime.Now);
            }
        }

        /// <summary>
        /// Adds the business rule validation result.
        /// </summary>
        /// <param name="businessRuleValidationResult">The business rule validation result.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="businessRuleValidationResult"/> is <c>null</c>.</exception>
        public void Add(IBusinessRuleValidationResult businessRuleValidationResult)
        {
            ArgumentNullException.ThrowIfNull(businessRuleValidationResult);

            lock (_businessRuleValidations)
            {
                _businessRuleValidations.Add(businessRuleValidationResult);

                UpdateLastModificationStamp(FastDateTime.Now);
            }
        }

        /// <summary>
        /// Removes the business rule validation result.
        /// </summary>
        /// <param name="businessRuleValidationResult">The business rule validation result.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="businessRuleValidationResult"/> is <c>null</c>.</exception>
        public void Remove(IBusinessRuleValidationResult businessRuleValidationResult)
        {
            ArgumentNullException.ThrowIfNull(businessRuleValidationResult);

            lock (_businessRuleValidations)
            {
                _businessRuleValidations.Remove(businessRuleValidationResult);

                UpdateLastModificationStamp(FastDateTime.Now);
            }
        }

        private void UpdateLastModificationStamp(DateTime dateTime, bool resetLastModifiedTicksToZero = false)
        {
            LastModified = dateTime;

            if (resetLastModifiedTicksToZero)
            {
                LastModifiedTicks = 0;
            }
            else
            {
                LastModifiedTicks = _stopWatch.ElapsedTicks;         
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("Errors");
            stringBuilder.AppendLine("===============================");

            var errors = GetErrors();
            if (errors.Count == 0)
            {
                stringBuilder.AppendLine("[no errors]");
            }
            else
            {
                foreach (var error in errors)
                {
                    stringBuilder.AppendLine("- {0}", error);
                }
            }

            stringBuilder.AppendLine();

            stringBuilder.AppendLine("Warnings");
            stringBuilder.AppendLine("===============================");

            var warnings = GetWarnings();
            if (warnings.Count == 0)
            {
                stringBuilder.AppendLine("[no warnings]");
            }
            else
            {
                foreach (var warning in warnings)
                {
                    stringBuilder.AppendLine("- {0}", warning);
                }
            }

            var finalString = stringBuilder.ToString();
            return finalString;
        }
    }
}
