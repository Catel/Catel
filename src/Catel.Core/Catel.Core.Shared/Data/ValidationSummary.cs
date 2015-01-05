// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationSummary.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

#if NETFX_CORE
    using Catel.Collections;
#endif

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
            LastModifiedTicks = validationContext.LastModifiedTicks;
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
            LastModifiedTicks = validationContext.LastModifiedTicks;
        }
        #endregion

        #region Properties
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
            get
            {
                return new ReadOnlyCollection<IFieldValidationResult>(_fieldWarnings);
            }
        }

        /// <summary>
        /// Gets a collection of field errors.
        /// </summary>
        /// <value>The field errors.</value>
        public ReadOnlyCollection<IFieldValidationResult> FieldErrors
        {
            get
            {
                return new ReadOnlyCollection<IFieldValidationResult>(_fieldErrors);
            }
        }

        /// <summary>
        /// Gets a collection of business rule warnings.
        /// </summary>
        /// <value>The business warnings.</value>
        public ReadOnlyCollection<IBusinessRuleValidationResult> BusinessWarnings
        {
            get
            {
               return new ReadOnlyCollection<IBusinessRuleValidationResult>(_businessRuleWarnings);
            }
        }

        /// <summary>
        /// Gets a collection of business rule errors.
        /// </summary>
        /// <value>The business rule errors.</value>
        public ReadOnlyCollection<IBusinessRuleValidationResult> BusinessRuleErrors
        {
            get
            {
                return new ReadOnlyCollection<IBusinessRuleValidationResult>(_businessRuleErrors);
            }
        }
        #endregion
    }
}
