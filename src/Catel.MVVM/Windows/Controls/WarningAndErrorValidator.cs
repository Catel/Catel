// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WarningAndErrorValidator.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using Catel.Data;
    using Collections;
    using Reflection;

#if UWP
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;

    using UIEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;
#else
    using System.Windows.Controls;
    using UIEventArgs = System.EventArgs;
#endif

    #region Enum
    /// <summary>
    /// Business validation type.
    /// </summary>
    public enum ValidationType
    {
        /// <summary>
        /// Warning.
        /// </summary>
        Warning,

        /// <summary>
        /// Error.
        /// </summary>
        Error
    }

    /// <summary>
    /// Validation event action.
    /// </summary>
    public enum ValidationEventAction
    {
        /// <summary>
        /// Added.
        /// </summary>
        Added,

        /// <summary>
        /// Removed.
        /// </summary>
        Removed,

        /// <summary>
        /// Refresh the validation, don't add or remove.
        /// </summary>
        Refresh,

        /// <summary>
        /// All validation info of the specified object should be cleared.
        /// </summary>
        ClearAll
    }
    #endregion

    /// <summary>
    /// Control for adding business rule validation to the form. Assign a value or binding to source for the business object or 
    /// collection of business objects to validate.
    /// </summary>
    public class WarningAndErrorValidator : Control, IUniqueIdentifyable
    {
        #region Fields
        /// <summary>
        /// List of objects that are currently being validated. 
        /// </summary>
        private readonly Dictionary<object, ValidationData> _objectValidation = new Dictionary<object, ValidationData>();
        private readonly object _objectValidationLock = new object();

#if NET || NETCORE
        private InfoBarMessageControl _infoBarMessageControl;
#endif
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WarningAndErrorValidator"/> class.
        /// </summary>
        public WarningAndErrorValidator()
        {
            UniqueIdentifier = UniqueIdentifierHelper.GetUniqueIdentifier<WarningAndErrorValidator>();

#if NET || NETCORE
            Focusable = false;
#endif

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;

            this.HideValidationAdorner();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Source for validation. This can be an business object which implements <see cref="IDataErrorInfo"/> 
        /// and <see cref="INotifyPropertyChanged"/> or an <see cref="IEnumerable"/> containing bussiness objects.
        /// In case of a <see cref="IEnumerable"/> then the content should be static or the interface <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>.
        /// </summary>
        /// <remarks>
        /// Wrapper for the Source dependency property.
        /// </remarks>
        public object Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// DependencyProperty definition as the backing store for Source.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(object), typeof(WarningAndErrorValidator),
            new PropertyMetadata(null, (sender, e) => ((WarningAndErrorValidator)sender).UpdateSource(e.OldValue, e.NewValue)));

#if NET || NETCORE
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
#endif

        /// <summary>
        /// Gets the unique identifier.
        /// </summary>
        /// <value>The unique identifier.</value>
        public int UniqueIdentifier { get; private set; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when validation is triggered.
        /// </summary>
        public event EventHandler<ValidationEventArgs> Validation;
        #endregion

        #region Methods
        /// <summary>
        /// Called when the control is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object sender, UIEventArgs e)
        {
            Initialize();

#if !NET && !NETCORE
            RaiseEventsForAllErrorsAndWarnings();
#endif
        }

        /// <summary>
        /// Called when the control is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnUnloaded(object sender, UIEventArgs e)
        {
            CleanUp();
        }

        /// <summary>
        /// Initializes this instance. Loads all the errors and warnings that were added when the control was not yet loaded.
        /// </summary>
        private void Initialize()
        {
#if NET || NETCORE
            if (AutomaticallyRegisterToInfoBarMessageControl)
            {
                //_infoBarMessageControl = this.FindLogicalAncestorByType<InfoBarMessageControl>();
                _infoBarMessageControl = this.FindLogicalOrVisualAncestorByType<InfoBarMessageControl>();
                if (_infoBarMessageControl != null)
                {
                    _infoBarMessageControl.SubscribeWarningAndErrorValidator(this);
                }
            }
#endif

            var source = Source;
            if (source != null)
            {
                UpdateSource(null, source);
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

            foreach (object obj in objects)
            {
                if (obj is IEnumerable)
                {
                    RemoveObjectsFromWatchList(obj as IEnumerable);
                }
                else if (obj is INotifyPropertyChanged)
                {
                    RemoveObjectFromWatchList(obj);
                }
            }

            _objectValidation.Clear();

#if NET || NETCORE
            if (_infoBarMessageControl != null)
            {
                _infoBarMessageControl.UnsubscribeWarningAndErrorValidator(this);
                _infoBarMessageControl = null;
            }
#endif
        }

        /// <summary>
        /// Updates the source.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        private void UpdateSource(object oldValue, object newValue)
        {
            var oldValueAsIEnumerable = oldValue as IEnumerable;
            if (oldValueAsIEnumerable != null)
            {
                RemoveObjectsFromWatchList(oldValueAsIEnumerable);
            }
            else if (oldValue is INotifyPropertyChanged)
            {
                RemoveObjectFromWatchList(oldValue);
            }

#if NET || NETCORE
            if (!IsLoaded)
            {
                return;
            }
#endif

            var newValueAsIEnumerable = newValue as IEnumerable;
            if (newValueAsIEnumerable != null)
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
        private void AddObjectsToWatchList(IEnumerable values, IEnumerable parentEnumerable)
        {
            Argument.IsNotNull("values", values);

            foreach (object value in values)
            {
                AddObjectToWatchList(value, parentEnumerable);
            }

            // Supports IObservableCollection through INotifyCollectionChanged and support IEntityCollectionCore 
            var iNotifyCollectionChanged = values as INotifyCollectionChanged;
            if (iNotifyCollectionChanged != null)
            {
                iNotifyCollectionChanged.CollectionChanged += iNotifyCollectionChanged_CollectionChanged;

                AddObjectToWatchList(parentEnumerable);
            }
        }

        /// <summary>
        /// Adds the object to the watch list.
        /// </summary>
        /// <param name="value">The object to add to the watch list.</param>
        /// <param name="parentEnumerable">The parent enumerable. <c>Null</c> if the object does not belong to an enumerable.</param>
        private void AddObjectToWatchList(object value, IEnumerable parentEnumerable = null)
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
                    if (iNotifyPropertyChanged != null)
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
            foreach (object value in values)
            {
                RemoveObjectFromWatchList(value);
            }

            // Supports IObservableCollection through INotifyCollectionChanged and support IEntityCollectionCore 
            var iNotifyCollectionChanged = values as INotifyCollectionChanged;
            if (iNotifyCollectionChanged != null)
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
            if (iNotifyPropertyChanged != null)
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
        private void CheckObjectValidation(object value, string propertyChanged, IEnumerable parentEnumerable)
        {
            ValidationData currentValidationData;
            ValidationData oldValidationData;

            if (value is null)
            {
                return;
            }

            lock (_objectValidationLock)
            {
                if (!_objectValidation.TryGetValue(value, out currentValidationData))
                {
                    currentValidationData = new ValidationData(parentEnumerable);
                    _objectValidation[value] = currentValidationData;
                }

                oldValidationData = (ValidationData)currentValidationData.Clone();
            }

            var validatable = value as IValidatable;

            CheckObjectValidationForFields(value, propertyChanged, currentValidationData.FieldWarnings, ValidationType.Warning);

            #region Warnings - business
            currentValidationData.BusinessWarnings.Clear();

            if (validatable != null)
            {
                if (!validatable.IsHidingValidationResults)
                {
                    foreach (var warning in validatable.ValidationContext.GetBusinessRuleWarnings())
                    {
                        currentValidationData.BusinessWarnings.Add(new BusinessWarningOrErrorInfo(warning.Message));
                    }
                }
            }
            else
            {
                string businessWarning = GetWarningOrError(value, ValidationType.Warning);
                if (!string.IsNullOrEmpty(businessWarning))
                {
                    currentValidationData.BusinessWarnings.Add(new BusinessWarningOrErrorInfo(businessWarning));
                }
            }
            #endregion

            CheckObjectValidationForFields(value, propertyChanged, currentValidationData.FieldErrors, ValidationType.Error);

            #region Errors - business
            currentValidationData.BusinessErrors.Clear();

            if (validatable != null)
            {
                if (!validatable.IsHidingValidationResults)
                {
                    foreach (var error in validatable.ValidationContext.GetBusinessRuleErrors())
                    {
                        currentValidationData.BusinessErrors.Add(new BusinessWarningOrErrorInfo(error.Message));
                    }
                }
            }
            else
            {
                string businessError = GetWarningOrError(value, ValidationType.Error);
                if (!string.IsNullOrEmpty(businessError))
                {
                    currentValidationData.BusinessErrors.Add(new BusinessWarningOrErrorInfo(businessError));
                }
            }
            #endregion

            RaiseEventsForDifferences(value, oldValidationData, currentValidationData);
        }

        /// <summary>
        /// Checks the object validation for fields warnings or errors.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="propertyChanged">The property changed.</param>
        /// <param name="infoList">The info list containing the warning or error info.</param>
        /// <param name="validationType">Type of the validation.</param>
        private static void CheckObjectValidationForFields(object value, string propertyChanged, ObservableCollection<FieldWarningOrErrorInfo> infoList,
            ValidationType validationType)
        {
            if (string.IsNullOrEmpty(propertyChanged))
            {
                infoList.Clear();
            }
            else
            {
                for (int i = 0; i < infoList.Count; i++)
                {
                    if (string.Compare(infoList[i].Field, propertyChanged) == 0)
                    {
                        infoList.RemoveAt(i);
                    }
                }
            }

            Dictionary<string, string> fieldWarningsOrErrors = CheckFieldWarningsOrErrors(value, propertyChanged, validationType);
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
        private static Dictionary<string, string> CheckFieldWarningsOrErrors(object value, string propertyChanged, ValidationType validationType)
        {
            var warningsOrErrors = new Dictionary<string, string>();

            var validatable = value as IValidatable;
            if (validatable != null)
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
            if ((validationType == ValidationType.Warning) && (iDataWarningInfo is null))
            {
                return warningsOrErrors;
            }

            var iDataErrorInfo = value as IDataErrorInfo;
            if ((validationType == ValidationType.Error) && (iDataErrorInfo is null))
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

            foreach (string property in propertiesToCheck)
            {
                string warningOrError = string.Empty;
                switch (validationType)
                {
                    case ValidationType.Warning:
                        // ReSharper disable PossibleNullReferenceException
                        warningOrError = iDataWarningInfo[property];
                        // ReSharper restore PossibleNullReferenceException
                        break;

                    case ValidationType.Error:
                        // ReSharper disable PossibleNullReferenceException
                        warningOrError = iDataErrorInfo[property];
                        // ReSharper restore PossibleNullReferenceException
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
            string message = null;

            switch (type)
            {
                case ValidationType.Warning:
                    var valueAsIDataWarningInfo = value as IDataWarningInfo;
                    if (valueAsIDataWarningInfo != null)
                    {
                        message = valueAsIDataWarningInfo.Warning;
                    }
                    break;

                case ValidationType.Error:
                    var valueAsIDataErrorInfo = value as IDataErrorInfo;
                    if (valueAsIDataErrorInfo != null)
                    {
                        message = valueAsIDataErrorInfo.Error;
                    }
                    break;
            }

            return !string.IsNullOrEmpty(message) ? message : null;
        }

#if !NET && !NETCORE
        /// <summary>
        /// Raises the events for all errors and warnings.
        /// </summary>
        private void RaiseEventsForAllErrorsAndWarnings()
        {
            lock (_objectValidationLock)
            {
                foreach (var objectValidationKeyValue in _objectValidation)
                {
                    var obj = objectValidationKeyValue.Key;
                    var validation = objectValidationKeyValue.Value;

                    foreach (var fieldWarning in validation.FieldWarnings)
                    {
                        RaiseBusinessValidationWarningOrError(obj, fieldWarning.Message, ValidationEventAction.Refresh, ValidationType.Warning);
                    }

                    foreach (var businessWarning in validation.BusinessWarnings)
                    {
                        RaiseBusinessValidationWarningOrError(obj, businessWarning.Message, ValidationEventAction.Refresh, ValidationType.Warning);
                    }

                    foreach (var fieldError in validation.FieldErrors)
                    {
                        RaiseBusinessValidationWarningOrError(obj, fieldError.Message, ValidationEventAction.Refresh, ValidationType.Error);
                    }

                    foreach (var businessError in validation.BusinessErrors)
                    {
                        RaiseBusinessValidationWarningOrError(obj, businessError.Message, ValidationEventAction.Refresh, ValidationType.Error);
                    }
                }
            }
        }
#endif

        /// <summary>
        /// Raises the events for differences.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="oldValidationData">The old validation data.</param>
        /// <param name="newValidationData">The new validation data.</param>
        private void RaiseEventsForDifferences(object value, ValidationData oldValidationData, ValidationData newValidationData)
        {
            // Warnings - fields
            RaiseEventsForDifferencesInFields(value, oldValidationData.FieldWarnings, newValidationData.FieldWarnings, ValidationType.Warning);

            // Warnings - business
            RaiseEventsForDifferencesInBusiness(value, oldValidationData.BusinessWarnings, newValidationData.BusinessWarnings, ValidationType.Warning);

            // Errors - fields
            RaiseEventsForDifferencesInFields(value, oldValidationData.FieldErrors, newValidationData.FieldErrors, ValidationType.Error);

            // Errors - business
            RaiseEventsForDifferencesInBusiness(value, oldValidationData.BusinessErrors, newValidationData.BusinessErrors, ValidationType.Error);
        }

        /// <summary>
        /// Raises the events for differences in fields.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="oldFieldData">The old field data.</param>
        /// <param name="newFieldData">The new field data.</param>
        /// <param name="validationType">Type of the validation.</param>
        private void RaiseEventsForDifferencesInFields(object value, ICollection<FieldWarningOrErrorInfo> oldFieldData,
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
        private void RaiseEventsForDifferencesInBusiness(object value, ICollection<BusinessWarningOrErrorInfo> oldBusinessData,
            ICollection<BusinessWarningOrErrorInfo> newBusinessData, ValidationType validationType)
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
        private void iNotifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CheckObjectValidation(sender, e.PropertyName, null);
        }

        /// <summary>
        /// Handling change of collection updating connections and error messages.
        /// </summary>
        /// <param name="sender">A sender.</param>
        /// <param name="e">Event args.</param>
        private void iNotifyCollectionChanged_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // If the action is "reset", no OldItems will be available, so clear all items manually
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                var itemsToRemove = new Collection<object>();
                lock (_objectValidationLock)
                {
                    foreach (KeyValuePair<object, ValidationData> validationData in _objectValidation)
                    {
                        if (validationData.Value.ParentEnumerable == sender)
                        {
                            itemsToRemove.Add(validationData.Key);
                        }
                    }
                }

                // Remove the items that should be removed (outside the lock)
                foreach (object itemToRemove in itemsToRemove)
                {
                    RemoveObjectFromWatchList(itemToRemove);
                }

                return;
            }

            IEnumerable newItems = e.NewItems;
            IEnumerable oldItems = e.OldItems;

            if (oldItems != null)
            {
                RemoveObjectsFromWatchList(oldItems);
            }

            if (newItems != null)
            {
                AddObjectsToWatchList(newItems, sender as IEnumerable);
            }
        }
        #endregion
    }

    #region Data classes
    /// <summary>
    /// Class containing all validation info (thus warnings and errors) about a specific object.
    /// </summary>
    internal class ValidationData
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationData"/> class.
        /// </summary>
        /// <param name="parentEnumerable">The parent ParentEnumerable. <c>Null</c> if the object does not belong to an enumerable.</param>
        public ValidationData(IEnumerable parentEnumerable)
        {
            FieldWarnings = new ObservableCollection<FieldWarningOrErrorInfo>();
            BusinessWarnings = new ObservableCollection<BusinessWarningOrErrorInfo>();
            FieldErrors = new ObservableCollection<FieldWarningOrErrorInfo>();
            BusinessErrors = new ObservableCollection<BusinessWarningOrErrorInfo>();

            ParentEnumerable = parentEnumerable;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the parent enumerable.
        /// </summary>
        /// <value>The parent enumerable.</value>
        public IEnumerable ParentEnumerable { get; private set; }

        /// <summary>
        /// Gets the field warnings.
        /// </summary>
        /// <value>The field warnings.</value>
        public ObservableCollection<FieldWarningOrErrorInfo> FieldWarnings { get; private set; }

        /// <summary>
        /// Gets the business warnings.
        /// </summary>
        /// <value>The business warnings.</value>
        public ObservableCollection<BusinessWarningOrErrorInfo> BusinessWarnings { get; private set; }

        /// <summary>
        /// Gets the field errors.
        /// </summary>
        /// <value>The field errors.</value>
        public ObservableCollection<FieldWarningOrErrorInfo> FieldErrors { get; private set; }

        /// <summary>
        /// Gets the business errors.
        /// </summary>
        /// <value>The business errors.</value>
        public ObservableCollection<BusinessWarningOrErrorInfo> BusinessErrors { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Clears the warnings and errors.
        /// </summary>
        public void ClearWarningsAndErrors()
        {
            BusinessWarnings.Clear();
            FieldWarnings.Clear();
            BusinessErrors.Clear();
            FieldErrors.Clear();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            var validationData = new ValidationData(ParentEnumerable);

            validationData.FieldWarnings = new ObservableCollection<FieldWarningOrErrorInfo>();
            ((ICollection<FieldWarningOrErrorInfo>)validationData.FieldWarnings).AddRange(FieldWarnings);

            validationData.BusinessWarnings = new ObservableCollection<BusinessWarningOrErrorInfo>();
            ((ICollection<BusinessWarningOrErrorInfo>)validationData.BusinessWarnings).AddRange(BusinessWarnings);

            validationData.FieldErrors = new ObservableCollection<FieldWarningOrErrorInfo>();
            ((ICollection<FieldWarningOrErrorInfo>)validationData.FieldErrors).AddRange(FieldErrors);

            validationData.BusinessErrors = new ObservableCollection<BusinessWarningOrErrorInfo>();
            ((ICollection<BusinessWarningOrErrorInfo>)validationData.BusinessErrors).AddRange(BusinessErrors);

            return validationData;
        }
        #endregion
    }

    /// <summary>
    /// Information class about business warnings and errors.
    /// </summary>
    internal class BusinessWarningOrErrorInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BusinessWarningOrErrorInfo"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public BusinessWarningOrErrorInfo(string message)
        {
            Message = message;
        }

        #region Properties
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            if (obj.GetHashCode() != GetHashCode())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}", Message).GetHashCode();
        }
        #endregion
    }

    /// <summary>
    /// Information class about field warnings and errors.
    /// </summary>
    internal class FieldWarningOrErrorInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldWarningOrErrorInfo"/> class.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="message">The message.</param>
        public FieldWarningOrErrorInfo(string field, string message)
        {
            Field = field;
            Message = message;
        }

        #region Properties
        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <value>The field.</value>
        public string Field { get; private set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            if (obj.GetHashCode() != GetHashCode())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}|{1}", Field, Message).GetHashCode();
        }
        #endregion
    }
    #endregion

    #region Event args
    /// <summary>
    /// Event arguments for event <see cref="WarningAndErrorValidator"/> Validation.
    /// </summary>
    public class ValidationEventArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationEventArgs"/> class.
        /// </summary>
        /// <param name="value">The value that contains the warning or error.</param>
        /// <param name="message">The actual warning or error message.</param>
        /// <param name="action">The action of the validation event.</param>
        /// <param name="type">The type of validation.</param>
        internal ValidationEventArgs(object value, string message, ValidationEventAction action, ValidationType type)
        {
            Value = value;
            Message = message;
            Action = action;
            Type = type;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the value that contains the warning or error.
        /// </summary>
        /// <value>The value that contains the warning or error.</value>
        public object Value { get; private set; }

        /// <summary>
        /// Gets the actual warning or error message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; private set; }

        /// <summary>
        /// A action for handling event.
        /// </summary>
        public ValidationEventAction Action { get; private set; }

        /// <summary>
        /// Gets the type of the validation.
        /// </summary>
        /// <value>The type of the validation.</value>
        public ValidationType Type { get; private set; }
        #endregion
    }
    #endregion
}

#endif
