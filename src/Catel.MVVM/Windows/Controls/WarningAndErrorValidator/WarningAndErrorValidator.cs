namespace Catel.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using Catel.Data;
    using Reflection;
    using System.Windows.Controls;
    using UIEventArgs = System.EventArgs;
    using Catel;
    using Catel.Windows;

    /// <summary>
    /// Control for adding business rule validation to the form. Assign a value or binding to source for the business object or 
    /// collection of business objects to validate.
    /// </summary>
    public class WarningAndErrorValidator : Control, IUniqueIdentifyable
    {
        /// <summary>
        /// List of objects that are currently being validated. 
        /// </summary>
        private readonly Dictionary<object, ValidationData> _objectValidation = new Dictionary<object, ValidationData>();
        private readonly object _objectValidationLock = new object();

        private bool _isLoaded;
        private InfoBarMessageControl? _infoBarMessageControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="WarningAndErrorValidator"/> class.
        /// </summary>
        public WarningAndErrorValidator()
        {
            UniqueIdentifier = UniqueIdentifierHelper.GetUniqueIdentifier<WarningAndErrorValidator>();
            Focusable = false;

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;

            this.HideValidationAdorner();
        }

        /// <summary>
        /// Source for validation. This can be an business object which implements <see cref="IDataErrorInfo"/> 
        /// and <see cref="INotifyPropertyChanged"/> or an <see cref="IEnumerable"/> containing bussiness objects.
        /// In case of a <see cref="IEnumerable"/> then the content should be static or the interface <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <remarks>
        /// Wrapper for the Source dependency property.
        /// </remarks>
        public object? Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// DependencyProperty definition as the backing store for Source.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(object), typeof(WarningAndErrorValidator),
            new PropertyMetadata(null, (sender, e) => ((WarningAndErrorValidator)sender).UpdateSource(e.OldValue, e.NewValue)));

        /// <summary>
        /// Gets or sets a value indicating whether this warning and error validator should automatically register to the first <see cref="InfoBarMessageControl"/> it can find.
        /// </summary>
        /// <value><c>true</c> if this warning and error validator should automatically register to the first <see cref="InfoBarMessageControl"/> it can find; otherwise, <c>false</c>.</value>
        public bool AutomaticallyRegisterToInfoBarMessageControl
        {
            get { return (bool)GetValue(AutomaticallyRegisterToInfoBarMessageControlProperty); }
            set { SetValue(AutomaticallyRegisterToInfoBarMessageControlProperty, value); }
        }

        /// <summary>
        /// DependencyProperty definition as the backing store for AutomaticallyRegisterToInfoBarMessageControl.
        /// </summary>
        public static readonly DependencyProperty AutomaticallyRegisterToInfoBarMessageControlProperty = DependencyProperty.Register(nameof(AutomaticallyRegisterToInfoBarMessageControl),
            typeof(bool), typeof(WarningAndErrorValidator), new PropertyMetadata(true));

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>The unique identifier.</value>
        public int UniqueIdentifier { get; private set; }

        /// <summary>
        /// Occurs when validation is triggered.
        /// </summary>
        public event EventHandler<ValidationEventArgs>? Validation;

        /// <summary>
        /// Called when the control is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="UIEventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object? sender, UIEventArgs e)
        {
            if (_isLoaded)
            {
                return;
            }

            _isLoaded = true;

            Initialize();
        }

        /// <summary>
        /// Called when the control is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="UIEventArgs"/> instance containing the event data.</param>
        private void OnUnloaded(object? sender, UIEventArgs e)
        {
            _isLoaded = false;

            CleanUp();
        }

        /// <summary>
        /// Initializes this instance. Loads all the errors and warnings that were added when the control was not yet loaded.
        /// </summary>
        private void Initialize()
        {
            if (AutomaticallyRegisterToInfoBarMessageControl)
            {
                //_infoBarMessageControl = this.FindLogicalAncestorByType<InfoBarMessageControl>();
                _infoBarMessageControl = this.FindLogicalOrVisualAncestorByType<InfoBarMessageControl>();
                if (_infoBarMessageControl is not null)
                {
                    _infoBarMessageControl.SubscribeWarningAndErrorValidator(this);
                }
            }

            var source = Source;
            if (source is not null)
            {
                UpdateSource(null, source);

                // Since we are initializing, we *must* force raising changes, fixes https://github.com/Catel/Catel/issues/1670
                RaiseEventsForDifferences(source, null, CreateValidationData(source, null, null));
            }
        }

        /// <summary>
        /// Cleans up.
        /// </summary>
        private void CleanUp()
        {
            var objects = new List<object>();

            lock (_objectValidationLock)
            {
                objects.AddRange(_objectValidation.Keys);
            }

            foreach (var obj in objects)
            {
                if (obj is IEnumerable enumerable)
                {
                    RemoveObjectsFromWatchList(enumerable);
                }
                else if (obj is INotifyPropertyChanged)
                {
                    RemoveObjectFromWatchList(obj);
                }
            }

            _objectValidation.Clear();

            if (_infoBarMessageControl is not null)
            {
                _infoBarMessageControl.UnsubscribeWarningAndErrorValidator(this);
                _infoBarMessageControl = null;
            }
        }

        /// <summary>
        /// Updates the source.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void UpdateSource(object? oldValue, object? newValue)
        {
            var oldValueAsIEnumerable = oldValue as IEnumerable;
            if (oldValueAsIEnumerable is not null)
            {
                RemoveObjectsFromWatchList(oldValueAsIEnumerable);
            }
            else if (oldValue is INotifyPropertyChanged)
            {
                RemoveObjectFromWatchList(oldValue);
            }

            if (!IsLoaded)
            {
                return;
            }

            var newValueAsIEnumerable = newValue as IEnumerable;
            if (newValueAsIEnumerable is not null)
            {
                AddObjectsToWatchList(newValueAsIEnumerable, newValueAsIEnumerable);
            }
            else if (newValue is INotifyPropertyChanged)
            {
                AddObjectToWatchList(newValue);
            }
        }

        /// <summary>
        /// Adds an <see cref="IEnumerable"/> of objects to the watch list.
        /// </summary>
        /// <param name="values">The values to add to the watch list.</param>
        /// <param name="parentEnumerable">The parent enumerable. <c>Null</c> if the object does not belong to an enumerable.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="values"/> is <c>null</c>.</exception>
        private void AddObjectsToWatchList(IEnumerable values, IEnumerable? parentEnumerable)
        {
            ArgumentNullException.ThrowIfNull(values);

            foreach (var value in values)
            {
                AddObjectToWatchList(value, parentEnumerable);
            }

            // Supports IObservableCollection through INotifyCollectionChanged and support IEntityCollectionCore 
            var iNotifyCollectionChanged = values as INotifyCollectionChanged;
            if (iNotifyCollectionChanged is not null)
            {
                iNotifyCollectionChanged.CollectionChanged += iNotifyCollectionChanged_CollectionChanged;

                // Use to be AddObjectToWatchList(parentEnumerable);, but seems wrong so fixed, but in case there are issues,
                // this is probably the place to fix it
                AddObjectToWatchList(iNotifyCollectionChanged);
            }
        }

        /// <summary>
        /// Adds the object to the watch list.
        /// </summary>
        /// <param name="value">The object to add to the watch list.</param>
        /// <param name="parentEnumerable">The parent enumerable. <c>Null</c> if the object does not belong to an enumerable.</param>
        private void AddObjectToWatchList(object value, IEnumerable? parentEnumerable = null)
        {
            if (value is null)
            {
                return;
            }

            lock (_objectValidationLock)
            {
                if (!_objectValidation.ContainsKey(value))
                {
                    var iNotifyPropertyChanged = value as INotifyPropertyChanged;
                    if (iNotifyPropertyChanged is not null)
                    {
                        iNotifyPropertyChanged.PropertyChanged += iNotifyPropertyChanged_PropertyChanged;
                    }
                }
            }

            CheckObjectValidation(value, null, parentEnumerable);
        }

        /// <summary>
        /// Removes an <see cref="IEnumerable"/> of objects from the watch list.
        /// </summary>
        /// <param name="values">The values to remove from the watch list.</param>
        private void RemoveObjectsFromWatchList(IEnumerable values)
        {
            foreach (var value in values)
            {
                RemoveObjectFromWatchList(value);
            }

            // Supports IObservableCollection through INotifyCollectionChanged and support IEntityCollectionCore 
            var iNotifyCollectionChanged = values as INotifyCollectionChanged;
            if (iNotifyCollectionChanged is not null)
            {
                iNotifyCollectionChanged.CollectionChanged -= iNotifyCollectionChanged_CollectionChanged;

                RemoveObjectFromWatchList(values);
            }
        }

        /// <summary>
        /// Removes the object from watch list.
        /// </summary>
        /// <param name="value">The object to remove from the watch list.</param>
        private void RemoveObjectFromWatchList(object value)
        {
            if (value is null)
            {
                return;
            }

            var iNotifyPropertyChanged = value as INotifyPropertyChanged;
            if (iNotifyPropertyChanged is not null)
            {
                iNotifyPropertyChanged.PropertyChanged -= iNotifyPropertyChanged_PropertyChanged;
            }

            RaiseBusinessValidationWarningOrError(value, string.Empty, ValidationEventAction.ClearAll, ValidationType.Warning);
            RaiseBusinessValidationWarningOrError(value, string.Empty, ValidationEventAction.ClearAll, ValidationType.Error);

            lock (_objectValidationLock)
            {
                _objectValidation.Remove(value);
            }
        }

        /// <summary>
        /// Checks a entity that either implements the <see cref="IDataWarningInfo"/> or <see cref="IDataErrorInfo"/> on warnings and errors.
        /// </summary>
        /// <param name="value">The object to check.</param>
        /// <param name="propertyChanged">The propery that has been changed. <c>null</c> if no specific property has changed.</param>
        /// <param name="parentEnumerable">The parent enumerable. <c>Null</c> if the object does not belong to an enumerable.</param>
        /// <remarks>
        /// Internally calls the generic method with the same name.
        /// </remarks>
        private void CheckObjectValidation(object? value, string? propertyChanged, IEnumerable? parentEnumerable)
        {
            if (value is null)
            {
                return;
            }

            ValidationData? oldValidationData = null;
            ValidationData newValidationData;

            lock (_objectValidationLock)
            {
                if (!_objectValidation.TryGetValue(value, out oldValidationData))
                {
                    // We'll just use null
                }

                newValidationData = CreateValidationData(value, propertyChanged, parentEnumerable);

                _objectValidation[value] = newValidationData;
            }

            RaiseEventsForDifferences(value, oldValidationData, newValidationData);
        }

        private ValidationData CreateValidationData(object value, string? propertyChanged, IEnumerable? parentEnumerable)
        {
            var validationData = new ValidationData(parentEnumerable);

            if (value is null)
            {
                return validationData;
            }

            var validatable = value as IValidatable;

            CheckObjectValidationForFields(value, propertyChanged, validationData.FieldWarnings, ValidationType.Warning);

            #region Warnings - business
            validationData.BusinessWarnings.Clear();

            if (validatable is not null)
            {
                if (!validatable.IsHidingValidationResults)
                {
                    foreach (var warning in validatable.ValidationContext.GetBusinessRuleWarnings())
                    {
                        validationData.BusinessWarnings.Add(new BusinessWarningOrErrorInfo(warning.Message));
                    }
                }
            }
            else
            {
                var businessWarning = GetWarningOrError(value, ValidationType.Warning);
                if (!string.IsNullOrEmpty(businessWarning))
                {
                    validationData.BusinessWarnings.Add(new BusinessWarningOrErrorInfo(businessWarning));
                }
            }
            #endregion

            CheckObjectValidationForFields(value, propertyChanged, validationData.FieldErrors, ValidationType.Error);

            #region Errors - business
            validationData.BusinessErrors.Clear();

            if (validatable is not null)
            {
                if (!validatable.IsHidingValidationResults)
                {
                    foreach (var error in validatable.ValidationContext.GetBusinessRuleErrors())
                    {
                        validationData.BusinessErrors.Add(new BusinessWarningOrErrorInfo(error.Message));
                    }
                }
            }
            else
            {
                var businessError = GetWarningOrError(value, ValidationType.Error);
                if (!string.IsNullOrEmpty(businessError))
                {
                    validationData.BusinessErrors.Add(new BusinessWarningOrErrorInfo(businessError));
                }
            }
            #endregion

            return validationData;
        }

        /// <summary>
        /// Checks the object validation for fields warnings or errors.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="propertyChanged">The property changed.</param>
        /// <param name="infoList">The info list containing the warning or error info.</param>
        /// <param name="validationType">Type of the validation.</param>
        private static void CheckObjectValidationForFields(object value, string? propertyChanged, IList<FieldWarningOrErrorInfo> infoList,
            ValidationType validationType)
        {
            if (string.IsNullOrEmpty(propertyChanged))
            {
                infoList.Clear();
            }
            else
            {
                for (var i = 0; i < infoList.Count; i++)
                {
                    if (string.Compare(infoList[i].Field, propertyChanged) == 0)
                    {
                        infoList.RemoveAt(i);
                    }
                }
            }

            var fieldWarningsOrErrors = CheckFieldWarningsOrErrors(value, propertyChanged, validationType);
            foreach (var fieldWarningOrError in fieldWarningsOrErrors)
            {
                var fieldWarningOrErrorInfo = new FieldWarningOrErrorInfo(fieldWarningOrError.Key, fieldWarningOrError.Value);
                if (!infoList.Contains(fieldWarningOrErrorInfo))
                {
                    infoList.Add(new FieldWarningOrErrorInfo(fieldWarningOrError.Key, fieldWarningOrError.Value));
                }
            }
        }

        /// <summary>
        /// Checks the field warnings or errors.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="propertyChanged">The property changed.</param>
        /// <param name="validationType">Type of the validation.</param>
        /// <returns>
        /// List of warnings or errors returned by the object.
        /// </returns>
        private static Dictionary<string, string> CheckFieldWarningsOrErrors(object value, string? propertyChanged, ValidationType validationType)
        {
            var warningsOrErrors = new Dictionary<string, string>();

            var validatable = value as IValidatable;
            if (validatable is not null)
            {
                // Respect IsHidingValidationResults
                if (validatable.IsHidingValidationResults)
                {
                    return warningsOrErrors;
                }

                // Read all data from validation context
                var validationContext = validatable.ValidationContext;
                var fieldValidationResults = new List<IFieldValidationResult>();

                switch (validationType)
                {
                    case ValidationType.Warning:
                        fieldValidationResults = validationContext.GetFieldWarnings();
                        break;

                    case ValidationType.Error:
                        fieldValidationResults = validationContext.GetFieldErrors();
                        break;
                }

                foreach (var fieldValidationResult in fieldValidationResults)
                {
                    warningsOrErrors[fieldValidationResult.PropertyName] = fieldValidationResult.Message;
                }

                return warningsOrErrors;
            }

            var iDataWarningInfo = value as IDataWarningInfo;
            if (validationType == ValidationType.Warning && iDataWarningInfo is null)
            {
                return warningsOrErrors;
            }

            var iDataErrorInfo = value as IDataErrorInfo;
            if (validationType == ValidationType.Error && iDataErrorInfo is null)
            {
                return warningsOrErrors;
            }

            var propertiesToCheck = new List<string>();
            if (!string.IsNullOrEmpty(propertyChanged))
            {
                propertiesToCheck.Add(propertyChanged);
            }
            else
            {
                var type = value.GetType();
                var properties = type.GetPropertiesEx();
                foreach (var property in properties)
                {
                    propertiesToCheck.Add(property.Name);
                }
            }

            foreach (var property in propertiesToCheck)
            {
                var warningOrError = string.Empty;

                switch (validationType)
                {
                    case ValidationType.Warning:
                        if (iDataWarningInfo is not null)
                        {
                            warningOrError = iDataWarningInfo[property];
                        }
                        break;

                    case ValidationType.Error:
                        if (iDataErrorInfo is not null)
                        {
                            warningOrError = iDataErrorInfo[property];
                        }
                        break;
                }

                if (!string.IsNullOrEmpty(warningOrError))
                {
                    warningsOrErrors[property] = warningOrError;
                }
            }

            return warningsOrErrors;
        }

        /// <summary>
        /// Gets the warning or error message for the object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// Warning or error message formatted for the object.
        /// </returns>
        private static string GetWarningOrError(object value, ValidationType type)
        {
            var message = string.Empty;

            switch (type)
            {
                case ValidationType.Warning:
                    var valueAsIDataWarningInfo = value as IDataWarningInfo;
                    if (valueAsIDataWarningInfo is not null)
                    {
                        message = valueAsIDataWarningInfo.Warning;
                    }
                    break;

                case ValidationType.Error:
                    var valueAsIDataErrorInfo = value as IDataErrorInfo;
                    if (valueAsIDataErrorInfo is not null)
                    {
                        message = valueAsIDataErrorInfo.Error;
                    }
                    break;
            }

            return message;
        }

        /// <summary>
        /// Raises the events for differences.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="oldValidationData">The old validation data.</param>
        /// <param name="newValidationData">The new validation data.</param>
        private void RaiseEventsForDifferences(object value, ValidationData? oldValidationData, ValidationData newValidationData)
        {
            // Warnings - fields
            RaiseEventsForDifferencesInFields(value, oldValidationData?.FieldWarnings ?? (IEnumerable<FieldWarningOrErrorInfo>)Array.Empty<FieldWarningOrErrorInfo>(), newValidationData.FieldWarnings, ValidationType.Warning);

            // Warnings - business
            RaiseEventsForDifferencesInBusiness(value, oldValidationData?.BusinessWarnings ?? (IEnumerable<BusinessWarningOrErrorInfo>)Array.Empty<BusinessWarningOrErrorInfo>(), newValidationData.BusinessWarnings, ValidationType.Warning);

            // Errors - fields
            RaiseEventsForDifferencesInFields(value, oldValidationData?.FieldErrors ?? (IEnumerable<FieldWarningOrErrorInfo>)Array.Empty<FieldWarningOrErrorInfo>(), newValidationData.FieldErrors, ValidationType.Error);

            // Errors - business
            RaiseEventsForDifferencesInBusiness(value, oldValidationData?.BusinessErrors ?? (IEnumerable<BusinessWarningOrErrorInfo>)Array.Empty<BusinessWarningOrErrorInfo>(), newValidationData.BusinessErrors, ValidationType.Error);
        }

        /// <summary>
        /// Raises the events for differences in fields.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="oldFieldData">The old field data.</param>
        /// <param name="newFieldData">The new field data.</param>
        /// <param name="validationType">Type of the validation.</param>
        private void RaiseEventsForDifferencesInFields(object value, IEnumerable<FieldWarningOrErrorInfo> oldFieldData,
            ICollection<FieldWarningOrErrorInfo> newFieldData, ValidationType validationType)
        {
            foreach (var info in oldFieldData)
            {
                if (!newFieldData.Contains(info))
                {
                    RaiseBusinessValidationWarningOrError(value, info.Message, ValidationEventAction.Removed, validationType);
                }
            }

            foreach (var info in newFieldData)
            {
                if (!oldFieldData.Contains(info))
                {
                    RaiseBusinessValidationWarningOrError(value, info.Message, ValidationEventAction.Added, validationType);
                }
            }
        }

        /// <summary>
        /// Raises the events for differences in business.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="oldBusinessData">The old business data.</param>
        /// <param name="newBusinessData">The new business data.</param>
        /// <param name="validationType">Type of the validation.</param>
        private void RaiseEventsForDifferencesInBusiness(object value, IEnumerable<BusinessWarningOrErrorInfo> oldBusinessData,
            IEnumerable<BusinessWarningOrErrorInfo> newBusinessData, ValidationType validationType)
        {
            foreach (var info in oldBusinessData)
            {
                if (!newBusinessData.Contains(info))
                {
                    RaiseBusinessValidationWarningOrError(value, info.Message, ValidationEventAction.Removed, validationType);
                }
            }

            foreach (var info in newBusinessData)
            {
                if (!oldBusinessData.Contains(info))
                {
                    RaiseBusinessValidationWarningOrError(value, info.Message, ValidationEventAction.Added, validationType);
                }
            }
        }

        /// <summary>
        /// Raises an validation warning or error event.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">A message.</param>
        /// <param name="action">A action for handling the error event.</param>
        /// <param name="type">The type.</param>
        private void RaiseBusinessValidationWarningOrError(object value, string message, ValidationEventAction action, ValidationType type)
        {
            Validation?.Invoke(this, new ValidationEventArgs(value, message, action, type));
        }

        /// <summary>
        /// Handling changes of properties within entity.
        /// </summary>
        /// <param name="sender">A sender.</param>
        /// <param name="e">The event args.</param>
        private void iNotifyPropertyChanged_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            CheckObjectValidation(sender, e.PropertyName, null);
        }

        /// <summary>
        /// Handling change of collection updating connections and error messages.
        /// </summary>
        /// <param name="sender">A sender.</param>
        /// <param name="e">Event args.</param>
        private void iNotifyCollectionChanged_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            // If the action is "reset", no OldItems will be available, so clear all items manually
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                var itemsToRemove = new Collection<object>();
                lock (_objectValidationLock)
                {
                    foreach (var validationData in _objectValidation)
                    {
                        if (validationData.Value.ParentEnumerable == sender)
                        {
                            itemsToRemove.Add(validationData.Key);
                        }
                    }
                }

                // Remove the items that should be removed (outside the lock)
                foreach (var itemToRemove in itemsToRemove)
                {
                    RemoveObjectFromWatchList(itemToRemove);
                }

                return;
            }

            var newItems = e.NewItems;
            var oldItems = e.OldItems;

            if (oldItems is not null)
            {
                RemoveObjectsFromWatchList(oldItems);
            }

            if (newItems is not null)
            {
                AddObjectsToWatchList(newItems, sender as IEnumerable);
            }
        }
    }
}
