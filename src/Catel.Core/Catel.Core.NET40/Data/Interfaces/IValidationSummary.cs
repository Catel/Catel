// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IValidationSummary.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
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
        /// 	<c>true</c> if this instance has warnings; otherwise, <c>false</c>.
        /// </value>
        bool HasWarnings { get; }

        /// <summary>
        /// Gets a value indicating whether the summary contains errors.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        bool HasErrors { get; }

        /// <summary>
        /// Gets a value indicating whether the summary contains field warnings.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has field warnings; otherwise, <c>false</c>.
        /// </value>
        bool HasFieldWarnings { get; }

        /// <summary>
        /// Gets a value indicating whether the summary contains field errors.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has field errors; otherwise, <c>false</c>.
        /// </value>
        bool HasFieldErrors { get; }

        /// <summary>
        /// Gets a value indicating whether the summary contains business rule warnings.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has business rule warnings; otherwise, <c>false</c>.
        /// </value>
        bool HasBusinessRuleWarnings { get; }

        /// <summary>
        /// Gets a value indicating whether the summary contains business rule errors.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has business rule errors; otherwise, <c>false</c>.
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
        ReadOnlyCollection<IBusinessRuleValidationResult> BusinessWarnings { get; }

        /// <summary>
        /// Gets a collection of business rule errors.
        /// </summary>
        /// <value>The business rule errors.</value>
        ReadOnlyCollection<IBusinessRuleValidationResult> BusinessRuleErrors { get; }
    }
}
