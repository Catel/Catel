// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationSummary.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Catel.Collections;

    /// <summary>
    /// The validation summary that contains a momentum of the <see cref="IValidationContext"/>.
    /// </summary>
    public class ValidationSummary : IValidationSummary
    {
        #region Fields
        /// <summary>
        /// The field warnings.
        /// </summary>
        private readonly List<IFieldValidationResult> _fieldWarnings;

        /// <summary>
        /// The field errors.
        /// </summary>
        private readonly List<IFieldValidationResult> _fieldErrors;

        /// <summary>
        /// The business rule warnings.
        /// </summary>
        private readonly List<IBusinessRuleValidationResult> _businessRuleWarnings;

        /// <summary>
        /// The business rule errors.
        /// </summary>
        private readonly List<IBusinessRuleValidationResult> _businessRuleErrors;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationSummary"/> class.
        /// </summary>
        /// <param name="validationContext">The validation context to base the summary on.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="validationContext"/> is <c>null</c>.</exception>
        public ValidationSummary(IValidationContext validationContext)
        {
            Argument.IsNotNull("validationContext", validationContext);

            _fieldWarnings = validationContext.GetFieldWarnings();
            _fieldErrors = validationContext.GetFieldErrors();
            _businessRuleWarnings = validationContext.GetBusinessRuleWarnings();
            _businessRuleErrors = validationContext.GetBusinessRuleErrors();

            LastModified = validationContext.LastModified;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationSummary"/> class and filters all the validations on the specified tag.
        /// </summary>
        /// <param name="validationContext">The validation context to base the summary on.</param>
        /// <param name="tag">The tag.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="validationContext"/> is <c>null</c>.</exception>
        public ValidationSummary(IValidationContext validationContext, object tag)
        {
            Argument.IsNotNull("validationContext", validationContext);

            _fieldWarnings = validationContext.GetFieldWarnings(tag);
            _fieldErrors = validationContext.GetFieldErrors(tag);
            _businessRuleWarnings = validationContext.GetBusinessRuleWarnings(tag);
            _businessRuleErrors = validationContext.GetBusinessRuleErrors(tag);

            LastModified = validationContext.LastModified;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the last modified date/time.
        /// </summary>
        /// <value>The last modified date/time.</value>
        public DateTime LastModified { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the summary contains warnings.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has warnings; otherwise, <c>false</c>.
        /// </value>
        public bool HasWarnings
        {
            get { return HasFieldWarnings || HasBusinessRuleWarnings; }
        }

        /// <summary>
        /// Gets a value indicating whether the summary contains errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrors
        {
            get { return HasFieldErrors || HasBusinessRuleErrors; }
        }

        /// <summary>
        /// Gets a value indicating whether the summary contains field warnings.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has field warnings; otherwise, <c>false</c>.
        /// </value>
        public bool HasFieldWarnings
        {
            get { return _fieldWarnings.Count > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the summary contains field errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has field errors; otherwise, <c>false</c>.
        /// </value>
        public bool HasFieldErrors
        {
            get { return _fieldErrors.Count > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the summary contains business rule warnings.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has business rule warnings; otherwise, <c>false</c>.
        /// </value>
        public bool HasBusinessRuleWarnings
        {
            get { return _businessRuleWarnings.Count > 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the summary contains business rule errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has business rule errors; otherwise, <c>false</c>.
        /// </value>
        public bool HasBusinessRuleErrors
        {
            get { return _businessRuleErrors.Count > 0; }
        }

        /// <summary>
        /// Gets a collection of field warnings.
        /// </summary>
        /// <value>The field warnings.</value>
        public ReadOnlyCollection<IFieldValidationResult> FieldWarnings
        {
            get { return _fieldWarnings.AsReadOnly(); }
        }

        /// <summary>
        /// Gets a collection of field errors.
        /// </summary>
        /// <value>The field errors.</value>
        public ReadOnlyCollection<IFieldValidationResult> FieldErrors
        {
            get { return _fieldErrors.AsReadOnly(); }
        }

        /// <summary>
        /// Gets a collection of business rule warnings.
        /// </summary>
        /// <value>The business warnings.</value>
        public ReadOnlyCollection<IBusinessRuleValidationResult> BusinessWarnings
        {
            get { return _businessRuleWarnings.AsReadOnly(); }
        }

        /// <summary>
        /// Gets a collection of business rule errors.
        /// </summary>
        /// <value>The business rule errors.</value>
        public ReadOnlyCollection<IBusinessRuleValidationResult> BusinessRuleErrors
        {
            get { return _businessRuleErrors.AsReadOnly(); }
        }
        #endregion
    }
}
