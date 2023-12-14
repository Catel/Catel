namespace Catel.Data
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The validation summary interface.
    /// </summary>
    public interface IValidationSummary
    {
        /// <summary>
        /// Gets a value indicating whether the summary contains warnings.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has warnings; otherwise, <c>false</c>.
        /// </value>
        bool HasWarnings { get; }

        /// <summary>
        /// Gets a value indicating whether the summary contains errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        bool HasErrors { get; }

        /// <summary>
        /// Gets a value indicating whether the summary contains field warnings.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has field warnings; otherwise, <c>false</c>.
        /// </value>
        bool HasFieldWarnings { get; }

        /// <summary>
        /// Gets a value indicating whether the summary contains field errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has field errors; otherwise, <c>false</c>.
        /// </value>
        bool HasFieldErrors { get; }

        /// <summary>
        /// Gets a value indicating whether the summary contains business rule warnings.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has business rule warnings; otherwise, <c>false</c>.
        /// </value>
        bool HasBusinessRuleWarnings { get; }

        /// <summary>
        /// Gets a value indicating whether the summary contains business rule errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has business rule errors; otherwise, <c>false</c>.
        /// </value>
        bool HasBusinessRuleErrors { get; }

        /// <summary>
        /// Gets a collection of field warnings.
        /// </summary>
        /// <value>The field warnings.</value>
        ReadOnlyCollection<IFieldValidationResult> FieldWarnings { get; }

        /// <summary>
        /// Gets a collection of field errors.
        /// </summary>
        /// <value>The field errors.</value>
        ReadOnlyCollection<IFieldValidationResult> FieldErrors { get; }

        /// <summary>
        /// Gets a collection of business rule warnings.
        /// </summary>
        /// <value>The business warnings.</value>
        ReadOnlyCollection<IBusinessRuleValidationResult> BusinessRuleWarnings { get; }

        /// <summary>
        /// Gets a collection of business rule errors.
        /// </summary>
        /// <value>The business rule errors.</value>
        ReadOnlyCollection<IBusinessRuleValidationResult> BusinessRuleErrors { get; }

        /// <summary>
        /// Gets the last modified date/time.
        /// <para />
        /// Note that this is just an informational value and should not be used for comparisons. The <see cref="DateTime"/> 
        /// is not accurate enough. Use the <c>LastModifiedTicks</c> instead. 
        /// </summary>
        /// <value>The last modified date/time.</value>
        DateTime LastModified { get; }

        /// <summary>
        /// Gets the last modified ticks which is much more precise that the <see cref="LastModified"/>. Use this value
        /// to compare last modification ticks on other validation contexts.
        /// <para />
        /// Because only full .NET provides a stopwatch, this property is only available in full .NET. All other target frameworks
        /// will return the <see cref="DateTime.Ticks"/> which is <c>not</c> reliable.
        /// </summary>
        /// <value>The last modified ticks.</value>
        long LastModifiedTicks { get; }
    }
}
