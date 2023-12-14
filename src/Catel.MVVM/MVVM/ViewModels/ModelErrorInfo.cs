namespace Catel.MVVM
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

    /// <summary>
    /// Class containing all the errors and warnings retrieved via <see cref="INotifyDataErrorInfo"/> and
    /// <see cref="INotifyDataWarningInfo"/>.
    /// </summary>
    internal class ModelErrorInfo
    {
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

        /// <summary>
        /// List of field that were initialized with an error.
        /// </summary>
        private readonly HashSet<string> _initialErrorFields = new HashSet<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelErrorInfo"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        public ModelErrorInfo(object model)
        {
            ArgumentNullException.ThrowIfNull(model);

            _model = model;

            var modelAsINotifyPropertyChanged = _model as INotifyPropertyChanged;
            if (modelAsINotifyPropertyChanged is not null)
            {
                modelAsINotifyPropertyChanged.PropertyChanged += OnModelPropertyChanged;
            }

            var modelAsINotifyDataErrorInfo = _model as INotifyDataErrorInfo;
            if (modelAsINotifyDataErrorInfo is not null)
            {
                modelAsINotifyDataErrorInfo.ErrorsChanged += OnModelErrorsChanged;
            }

            var modelAsINotifyDataWarningInfo = _model as INotifyDataWarningInfo;
            if (modelAsINotifyDataWarningInfo is not null)
            {
                modelAsINotifyDataWarningInfo.WarningsChanged += OnModelWarningsChanged;
            }
        }

        /// <summary>
        /// Raised when the errors or warnings are updated.
        /// </summary>
        public event EventHandler? Updated;

        /// <summary>
        /// Synchronizes the validation state of the specified properties.
        /// </summary>
        /// <param name="propertyNames"></param>
        public void SynchronizeFieldValidation(IEnumerable<string> propertyNames)
        {
            var modelAsINotifyDataErrorInfo = _model as INotifyDataErrorInfo;
            var modelAsINotifyDataWarningInfo = _model as INotifyDataWarningInfo;

            foreach (var propertyName in propertyNames)
            {
                if (modelAsINotifyDataErrorInfo is not null)
                {
                    var errors = modelAsINotifyDataErrorInfo.GetErrors(propertyName);
                    HandleFieldErrors(propertyName, errors);
                }

                if (modelAsINotifyDataWarningInfo is not null)
                {
                    var warnings = modelAsINotifyDataWarningInfo.GetWarnings(propertyName);
                    HandleFieldWarnings(propertyName, warnings);
                }
            }
        }

        /// <summary>
        /// Called when a property on the model has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.PropertyName))
            {
                return;
            }

            var dataWarningInfo = _model as IDataWarningInfo;
            if (dataWarningInfo is not null)
            {
                HandleFieldWarnings(e.PropertyName, new[] { dataWarningInfo[e.PropertyName] });
            }

            var dataErrorInfo = _model as IDataErrorInfo;
            if (dataErrorInfo is not null)
            {
                HandleFieldErrors(e.PropertyName, new [] { dataErrorInfo[e.PropertyName] });
            }
        }

        /// <summary>
        /// Called when the errors on the model have changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DataErrorsChangedEventArgs"/> instance containing the event data.</param>
        private void OnModelErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
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

            Updated?.Invoke(this, EventArgs.Empty);
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
                if (_fieldErrors.TryGetValue(propertyName, out var fieldErrors))
                {
                    fieldErrors.Clear();
                }
                else
                {
                    fieldErrors = new List<string>();
                    _fieldErrors[propertyName] = fieldErrors;
                }

                foreach (object error in errors)
                {
                    var errorAsString = GetValidationString(error);
                    if (!string.IsNullOrEmpty(errorAsString))
                    {
                        fieldErrors.Add(errorAsString);
                    }
                }
            }
        }

        /// <summary>
        /// Called when the warnings on the model have changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DataErrorsChangedEventArgs"/> instance containing the event data.</param>
        private void OnModelWarningsChanged(object? sender, DataErrorsChangedEventArgs e)
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

            Updated?.Invoke(this, EventArgs.Empty);
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
                if (_fieldWarnings.TryGetValue(propertyName, out var fieldWarnings))
                {
                    fieldWarnings.Clear();
                }
                else
                {
                    fieldWarnings = new List<string>();
                    _fieldWarnings[propertyName] = fieldWarnings;
                }

                foreach (object warning in warnings)
                {
                    var warningAsString = GetValidationString(warning);
                    if (!string.IsNullOrEmpty(warningAsString))
                    {
                        fieldWarnings.Add(warningAsString);
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
                    if (_fieldErrors.TryGetValue(propertyName, out var fieldErrors))
                    {
                        errors.AddRange(fieldErrors);
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
                    if (_fieldWarnings.TryGetValue(propertyName, out var fieldWarnings))
                    {
                        errors.AddRange(fieldWarnings);
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
            var modelAsINotifyPropertyChanged = _model as INotifyPropertyChanged;
            if (modelAsINotifyPropertyChanged is not null)
            {
                modelAsINotifyPropertyChanged.PropertyChanged -= OnModelPropertyChanged;
            }

            var modelAsINotifyDataErrorInfo = _model as INotifyDataErrorInfo;
            if (modelAsINotifyDataErrorInfo is not null)
            {
                modelAsINotifyDataErrorInfo.ErrorsChanged -= OnModelErrorsChanged;
            }

            var modelAsINotifyDataWarningInfo = _model as INotifyDataWarningInfo;
            if (modelAsINotifyDataWarningInfo is not null)
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
        private string? GetValidationString(object? obj)
        {
            var objAsString = obj as string;
            if (objAsString is not null)
            {
                return objAsString;
            }

            var objAsValidationResult = obj as ValidationResult;
            if (objAsValidationResult is not null)
            {
                return objAsValidationResult.ErrorMessage;
            }

            return null;
        }

        /// <summary>
        /// Initializes the default errors.
        /// </summary>
        /// <param name="validationResults">The validation results.</param>
        public void InitializeDefaultErrors(IEnumerable<ValidationResult> validationResults)
        {
            foreach (var validationResult in validationResults)
            {
                if (!validationResult.MemberNames.Any())
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
                lock (_businessRuleErrors)
                {
                    _businessRuleErrors.Clear();
                }
            }
            else
            {
                lock (_initialErrorFields)
                {
                    if (_initialErrorFields.Contains(propertyName))
                    {
                        lock (_fieldErrors)
                        {
                            if (_fieldErrors.TryGetValue(propertyName, out var fieldErrors))
                            {
                                fieldErrors.Clear();
                            }
                        }

                        _initialErrorFields.Remove(propertyName);
                    }
                }
            }
        }
    }
}
