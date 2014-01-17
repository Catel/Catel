// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelErrorInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

#if !WINDOWS_PHONE && !NET35
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;
#endif

    /// <summary>
    /// Class containing all the errors and warnings retrieved via <see cref="INotifyDataErrorInfo"/> and
    /// <see cref="INotifyDataWarningInfo"/>.
    /// </summary>
    internal class ModelErrorInfo
    {
        #region Fields
        private readonly object _model;

        /// <summary>
        /// Gets the field errors.
        /// </summary>
        private readonly Dictionary<string, List<string>> _fieldErrors = new Dictionary<string, List<string>>();

        /// <summary>
        /// Gets the field warnings.
        /// </summary>
        private readonly Dictionary<string, List<string>> _fieldWarnings = new Dictionary<string, List<string>>();

        /// <summary>
        /// Gets the business rule errors.
        /// </summary>
        private readonly List<string> _businessRuleErrors = new List<string>();

        /// <summary>
        /// Gets the business rule warnings.
        /// </summary>
        private readonly List<string> _businessRuleWarnings = new List<string>();

#if !WINDOWS_PHONE && !NET35
        /// <summary>
        /// List of field that were initialized with an error.
        /// </summary>
        private readonly List<string> _initialErrorFields = new List<string>();
#endif
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelErrorInfo"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        public ModelErrorInfo(object model)
        {
            Argument.IsNotNull("model", model);

            _model = model;

            // TODO: Find a way to read existing errors? It will need reflection to find out all properties
            // and a call to all the properties to gather the existing errors, maybe that's too much. Let's
            // see if users request this in the future

            var modelAsINotifyDataErrorInfo = _model as INotifyDataErrorInfo;
            if (modelAsINotifyDataErrorInfo != null)
            {
                modelAsINotifyDataErrorInfo.ErrorsChanged += OnModelErrorsChanged;
            }

            var modelAsINotifyDataWarningInfo = _model as INotifyDataWarningInfo;
            if (modelAsINotifyDataWarningInfo != null)
            {
                modelAsINotifyDataWarningInfo.WarningsChanged += OnModelWarningsChanged;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Raised when the errors or warnings are updated.
        /// </summary>
        public event EventHandler Updated;
        #endregion

        #region Methods
        /// <summary>
        /// Called when the errors on the model have changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DataErrorsChangedEventArgs"/> instance containing the event data.</param>
        private void OnModelErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            var notifyDataErrorInfo = ((INotifyDataErrorInfo)_model);
            var errors = notifyDataErrorInfo.GetErrors(e.PropertyName);

            if (string.IsNullOrEmpty(e.PropertyName))
            {
                HandleBusinessRuleErrors(errors);
            }
            else
            {
                HandleFieldErrors(e.PropertyName, errors);
            }

            Updated.SafeInvoke(this);
        }

        /// <summary>
        /// Handles the business rule errors.
        /// </summary>
        /// <param name="errors">The errors.</param>
        private void HandleBusinessRuleErrors(IEnumerable errors)
        {
            lock (_businessRuleErrors)
            {
                _businessRuleErrors.Clear();

                foreach (object error in errors)
                {
                    var errorAsString = GetValidationString(error);
                    if (!string.IsNullOrEmpty(errorAsString))
                    {
                        _businessRuleErrors.Add(errorAsString);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the field errors.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="errors">The errors.</param>
        private void HandleFieldErrors(string propertyName, IEnumerable errors)
        {
            lock (_fieldErrors)
            {
                if (_fieldErrors.ContainsKey(propertyName))
                {
                    _fieldErrors[propertyName].Clear();
                }
                else
                {
                    _fieldErrors.Add(propertyName, new List<string>());
                }

                foreach (object error in errors)
                {
                    var errorAsString = GetValidationString(error);
                    if (!string.IsNullOrEmpty(errorAsString))
                    {
                        _fieldErrors[propertyName].Add(errorAsString);
                    }
                }
            }
        }

        /// <summary>
        /// Called when the warnings on the model have changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DataErrorsChangedEventArgs"/> instance containing the event data.</param>
        private void OnModelWarningsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            var notifyDataWarningInfo = ((INotifyDataWarningInfo)_model);
            var warnings = notifyDataWarningInfo.GetWarnings(e.PropertyName);

            if (string.IsNullOrEmpty(e.PropertyName))
            {
                HandleBusinessRuleWarnings(warnings);
            }
            else
            {
                HandleFieldWarnings(e.PropertyName, warnings);
            }

            Updated.SafeInvoke(this);
        }

        /// <summary>
        /// Handles the business rule warnings.
        /// </summary>
        /// <param name="warnings">The warnings.</param>
        private void HandleBusinessRuleWarnings(IEnumerable warnings)
        {
            lock (_businessRuleWarnings)
            {
                _businessRuleWarnings.Clear();

                foreach (object warning in warnings)
                {
                    var warningAsString = GetValidationString(warning);
                    if (!string.IsNullOrEmpty(warningAsString))
                    {
                        _businessRuleWarnings.Add(warningAsString);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the field warnings.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="warnings">The warnings.</param>
        private void HandleFieldWarnings(string propertyName, IEnumerable warnings)
        {
            lock (_fieldWarnings)
            {
                if (_fieldWarnings.ContainsKey(propertyName))
                {
                    _fieldWarnings[propertyName].Clear();
                }
                else
                {
                    _fieldWarnings.Add(propertyName, new List<string>());
                }

                foreach (object warning in warnings)
                {
                    var warningAsString = GetValidationString(warning);
                    if (!string.IsNullOrEmpty(warningAsString))
                    {
                        _fieldWarnings[propertyName].Add(warningAsString);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the errors for the specificied <paramref name="propertyName"/>.
        /// <para />
        /// If the <paramref name="propertyName"/> is <c>null</c> or <see cref="string.Empty"/>,
        /// entity level errors will be returned.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><see cref="IEnumerable{String}"/> of errors.</returns>
        public IEnumerable<string> GetErrors(string propertyName)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(propertyName))
            {
                lock (_businessRuleErrors)
                {
                    errors.AddRange(_businessRuleErrors);
                }
            }
            else
            {
                lock (_fieldErrors)
                {
                    if (_fieldErrors.ContainsKey(propertyName))
                    {
                        errors.AddRange(_fieldErrors[propertyName]);
                    }
                }
            }

            return errors;
        }

        /// <summary>
        /// Gets the warnings for the specificied <paramref name="propertyName"/>.
        /// <para />
        /// If the <paramref name="propertyName"/> is <c>null</c> or <see cref="string.Empty"/>,
        /// entity level warnings will be returned.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><see cref="IEnumerable{String}"/> of warnings.</returns>
        public IEnumerable<string> GetWarnings(string propertyName)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(propertyName))
            {
                lock (_businessRuleWarnings)
                {
                    errors.AddRange(_businessRuleWarnings);
                }
            }
            else
            {
                lock (_fieldWarnings)
                {
                    if (_fieldWarnings.ContainsKey(propertyName))
                    {
                        errors.AddRange(_fieldWarnings[propertyName]);
                    }
                }
            }

            return errors;
        }

        /// <summary>
        /// Cleans up the information by unsubscribing from all events.
        /// </summary>
        public void CleanUp()
        {
            var modelAsINotifyDataErrorInfo = _model as INotifyDataErrorInfo;
            if (modelAsINotifyDataErrorInfo != null)
            {
                modelAsINotifyDataErrorInfo.ErrorsChanged -= OnModelErrorsChanged;
            }

            var modelAsINotifyDataWarningInfo = _model as INotifyDataWarningInfo;
            if (modelAsINotifyDataWarningInfo != null)
            {
                modelAsINotifyDataWarningInfo.WarningsChanged -= OnModelWarningsChanged;
            }
        }

        /// <summary>
        /// Gets the validation string from the object. This method supports the following types: <para />
        /// * string<para />
        /// * ValidationResult 
        /// </summary>
        /// <param name="obj">The object to convert to a string.</param>
        /// <returns>The string retrieved from the object or <c>null</c> if the object is not supported.</returns>
        private string GetValidationString(object obj)
        {
            var objAsString = obj as string;
            if (objAsString != null)
            {
                return objAsString;
            }

#if !WINDOWS_PHONE && !NET35
            var objAsValidationResult = obj as ValidationResult;
            if (objAsValidationResult != null)
            {
                return objAsValidationResult.ErrorMessage;
            }
#endif

            return null;
        }

#if !WINDOWS_PHONE && !NET35
        /// <summary>
        /// Initializes the default errors.
        /// </summary>
        /// <param name="validationResults">The validation results.</param>
        public void InitializeDefaultErrors(IEnumerable<ValidationResult> validationResults)
        {
            foreach (var validationResult in validationResults)
            {
                if (validationResult.MemberNames.Count() == 0)
                {
                    HandleBusinessRuleErrors(new object[] { validationResult });
                }
                else
                {
                    foreach (string propertyName in validationResult.MemberNames)
                    {
                        HandleFieldErrors(propertyName, new object[] { validationResult });

                        _initialErrorFields.Add(propertyName);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the default errors. This method is required when errors are initialize via <see cref="InitializeDefaultErrors"/>.
        /// This method checks whether default errors were added for a specific property (or at entity level if <paramref name="propertyName"/>
        /// is <see cref="string.Empty"/> or <c>null</c>).
        /// <para />
        /// Reason for this is that if the error is known on forehand, the entity implementation will not raise the
        /// <see cref="INotifyDataErrorInfo.ErrorsChanged"/> event.
        /// <para />
        /// If the default errors are cleared, the validation via <see cref="INotifyDataErrorInfo.ErrorsChanged"/> will take over from this point.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void ClearDefaultErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                _businessRuleErrors.Clear();
            }
            else
            {
                lock (_initialErrorFields)
                {
                    if (_initialErrorFields.Contains(propertyName))
                    {
                        lock (_fieldErrors)
                        {
                            if (_fieldErrors.ContainsKey(propertyName))
                            {
                                _fieldErrors[propertyName].Clear();
                            }
                        }

                        _initialErrorFields.Remove(propertyName);
                    }
                }
            }
        }
#endif
        #endregion
    }
}