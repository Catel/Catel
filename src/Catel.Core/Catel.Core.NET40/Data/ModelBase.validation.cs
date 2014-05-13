// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.validation.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Xml.Serialization;
    using Catel.IoC;

    using Logging;
    using Reflection;
    using Text;

    public partial class ModelBase
    {
        #region Constants
        /// <summary>
        /// The name of the <see cref="IDataWarningInfo.Warning"/> property.
        /// </summary>
        private const string WarningMessageProperty = "IDataWarningInfo.Warning";

        /// <summary>
        /// The name of the <see cref="HasWarnings"/> property.
        /// </summary>
        private const string HasWarningsMessageProperty = "HasWarnings";

        /// <summary>
        /// The name of the <see cref="IDataErrorInfo.Error"/> property.
        /// </summary>
        private const string ErrorMessageProperty = "IDataErrorInfo.Error";

        /// <summary>
        /// The name of the <see cref="HasErrors"/> property.
        /// </summary>
        private const string HasErrorsMessageProperty = "HasErrors";
        #endregion

        #region Fields
        /// <summary>
        /// Backing field for the <see cref="SuspendValidation"/> property. Because it has custom logic, it needs a backing field.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private bool _suspendValidation;

        /// <summary>
        /// Field that determines whether a validator has been retrieved yet.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private bool _hasRetrievedValidatorOnce;

        /// <summary>
        /// The backing field for the <see cref="Validator"/> property.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private IValidator _validator;

        /// <summary>
        /// Lock object to make sure that multiple validations at the same time are not allowed.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly object _validationLock = new object();

        /// <summary>
        /// The internal validation context, which can contain in-between validation info.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private ValidationContext _internalValidationContext = new ValidationContext();

        /// <summary>
        /// List of property names that were changed, but not checked for validation because validation was suspended at that
        /// time.
        /// <para />
        /// As soon as validation is activated again, these properties should be validated.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly HashSet<string> _propertiesNotCheckedDuringDisabledValidation = new HashSet<string>();

        /// <summary>
        /// The property names that failed to validate and should be skipped next time for NET 4.0 
        /// attribute validation.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private static readonly Dictionary<Type, HashSet<string>> _propertyValuesIgnoredOrFailedForValidation = new Dictionary<Type, HashSet<string>>();

#if !WINDOWS_PHONE && !NETFX_CORE && !PCL && !NET35

#if NET
        [field: NonSerialized]
#endif
        private bool _firstAnnotationValidation = true;

        /// <summary>
        /// A dictionary containing the annotation (attribute) validation results of properties of this class.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly Dictionary<string, string> _dataAnnotationValidationResults = new Dictionary<string, string>();

#if NET
        [field: NonSerialized]
#endif
        private readonly Dictionary<string, System.ComponentModel.DataAnnotations.ValidationContext> _dataAnnotationsValidationContext = new Dictionary<string, System.ComponentModel.DataAnnotations.ValidationContext>();
#endif
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the object is currently validating. During validation, no validation will be invoked.
        /// </summary>
        /// <value>
        /// <c>true</c> if the object is validating; otherwise, <c>false</c>.
        /// </value>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        protected bool IsValidating { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this object is validated or not.
        /// </summary>
#if !WINDOWS_PHONE && !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        [XmlIgnore]
        private bool IsValidated { get; set; }

        /// <summary>
        /// Gets or sets the validator to use.
        /// <para />
        /// By default, this value retrieves the default validator from them <see cref="IValidatorProvider"/> if it is
        /// registered in the <see cref="Catel.IoC.ServiceLocator"/>.
        /// </summary>
#if !WINDOWS_PHONE && !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        [XmlIgnore]
        public IValidator Validator
        {
            get
            {
                if (_validator == null)
                {
                    if (!_hasRetrievedValidatorOnce)
                    {
                        var dependencyResolver = this.GetDependencyResolver();
                        var validatorProvider = dependencyResolver.TryResolve<IValidatorProvider>();
                        if (validatorProvider != null)
                        {
                            _validator = validatorProvider.GetValidator(GetType());
                            if (_validator != null)
                            {
                                Log.Debug("Found validator '{0}' for view model '{1}' via the registered IValidatorProvider", _validator.GetType().FullName, GetType().FullName);
                            }
                        }

                        _hasRetrievedValidatorOnce = true;
                    }
                }

                return _validator;
            }
            set
            {
                _validator = value;
            }
        }

        /// <summary>
        /// Gets the validation context which contains all information about the validation.
        /// </summary>
        /// <value>The validation context.</value>
#if !WINDOWS_PHONE && !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        [XmlIgnore]
        public IValidationContext ValidationContext { get; private set; }

        /// <summary>
        /// Gets the number of field warnings.
        /// </summary>
        /// <value>The field warning count.</value>
#if !WINDOWS_PHONE && !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        [XmlIgnore]
        public int FieldWarningCount
        {
            get
            {
                if (!IsValidated)
                {
                    Validate();
                }

                return ValidationContext.GetFieldWarningCount();
            }
        }

        /// <summary>
        /// Gets the number of business rule warnings.
        /// </summary>
        /// <value>The business rule warning count.</value>
#if !WINDOWS_PHONE && !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        [XmlIgnore]
        public int BusinessRuleWarningCount
        {
            get
            {
                if (!IsValidated)
                {
                    Validate();
                }

                return ValidationContext.GetBusinessRuleWarningCount();
            }
        }

        /// <summary>
        /// Gets the number of field errors.
        /// </summary>
        /// <value>The field error count.</value>
#if !WINDOWS_PHONE && !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        [XmlIgnore]
        public int FieldErrorCount
        {
            get
            {
                if (!IsValidated)
                {
                    Validate();
                }

                return ValidationContext.GetFieldErrorCount();
            }
        }

        /// <summary>
        /// Gets the number of business rule errors.
        /// </summary>
        /// <value>The business rule error count.</value>
#if !WINDOWS_PHONE && !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        [XmlIgnore]
        public int BusinessRuleErrorCount
        {
            get
            {
                if (!IsValidated)
                {
                    Validate();
                }

                return ValidationContext.GetBusinessRuleErrorCount();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the validation should be suspended. A call to <see cref="Validate(bool)"/> will be returned immediately.
        /// </summary>
        /// <value><c>true</c> if validation should be suspended; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Unlike the <see cref="HideValidationResults"/> property, this property will prevent validation. If you want validation
        /// and the ability to query results, but simply hide the validation results, use the <see cref="HideValidationResults"/> property.
        /// </remarks>
#if !WINDOWS_PHONE && !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        protected bool SuspendValidation
        {
            get { return _suspendValidation || SuspendValidationForAllModels || LeanAndMeanModel; }
            set
            {
                _suspendValidation = value;

                if (!_suspendValidation)
                {
                    CatchUpWithSuspendedAnnotationsValidation();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value for the <see cref="SuspendValidation"/> for each model.
        /// <para />
        /// By default, this value is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if the validation must be suspended by default; otherwise, <c>false</c>.</value>
        public static bool DefaultSuspendValidationValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the validation for all classes deriving from <see cref="ModelBase"/> should be suspended.
        /// <para />
        /// This is a good way to improve performance for a specific operation where validation only causes overhead.
        /// </summary>
        /// <value><c>true</c> if validation should be suspended for all models; otherwise, <c>false</c>.</value>
        public static bool SuspendValidationForAllModels { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the validation results should be hidden. This means that 
        /// the <see cref="ValidationContext"/> should be filled, but the <see cref="IDataErrorInfo"/> and 
        /// <see cref="INotifyDataErrorInfo"/> should not expose any of the validation ressults.
        /// <para />
        /// This is very useful when the validation in the UI should be delayed to a specific point. However, the
        /// validation is still available for retrieval.
        /// <para />
        /// By default, this value is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if the validation should be hidden; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Unlike the <see cref="SuspendValidation"/> property, this property will not prevent validation. It will only
        /// prevent the error interfaces to not expose them.
        /// </remarks>
#if !WINDOWS_PHONE && !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        protected bool HideValidationResults { get; set; }

        /// <summary>
        /// Gets a value indicating whether the object is currently hiding its validation results. If the object
        /// hides its validation results, it is still possible to retrieve the validation results using the
        /// <see cref="ValidationContext"/>.
        /// </summary>
        bool IModel.IsHidingValidationResults { get { return HideValidationResults; } }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the object is validating.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        public event EventHandler Validating;

        /// <summary>
        /// Occurs when the object is about the validate the fields.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        protected event EventHandler ValidatingFields;

        /// <summary>
        /// Occurs when the object has validated the fields.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        protected event EventHandler ValidatedFields;

        /// <summary>
        /// Occurs when the object is about the validate the business rules.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        protected event EventHandler ValidatingBusinessRules;

        /// <summary>
        /// Occurs when the object has validated the business rules.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        protected event EventHandler ValidatedBusinessRules;

        /// <summary>
        /// Occurs when the object is validated.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        public event EventHandler Validated;
        #endregion

        #region Methods
        /// <summary>
        /// Catches up with suspended annotations validation.
        /// <para />
        /// This method will take care of unvalidated properties that have been changed during
        /// the suspended validation state of this model.
        /// </summary>
        private void CatchUpWithSuspendedAnnotationsValidation()
        {
            if (_propertiesNotCheckedDuringDisabledValidation.Count > 0)
            {
                Log.Debug("Validation was suspended, and properties were not checked during this suspended time, checking them now");

                foreach (string property in _propertiesNotCheckedDuringDisabledValidation)
                {
                    var propertyData = GetPropertyData(property);
                    var propertyValue = GetValueFast(propertyData.Name);
                    ValidatePropertyUsingAnnotations(property, propertyValue, propertyData);
                }

                _propertiesNotCheckedDuringDisabledValidation.Clear();
            }
        }

        /// <summary>
        /// Validates the property using data annotations.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value to validate.</param>
        /// <param name="catelPropertyData">The catel property data. Can be <c>null</c> for non-Catel properties.</param>
        /// <returns><c>true</c> if no errors using data annotations are found; otherwise <c>false</c>.</returns>
        private bool ValidatePropertyUsingAnnotations(string propertyName, object value, PropertyData catelPropertyData)
        {
            if (SuspendValidation)
            {
                _propertiesNotCheckedDuringDisabledValidation.Add(propertyName);
                return true;
            }

#if !WINDOWS_PHONE && !NETFX_CORE && !PCL && !NET35
            var type = GetType();

            try
            {
                if (!_propertyValuesIgnoredOrFailedForValidation[type].Contains(propertyName))
                {
                    if (catelPropertyData != null)
                    {
                        var propertyInfo = catelPropertyData.GetPropertyInfo(type);
                        if (propertyInfo == null || !propertyInfo.HasPublicGetter)
                        {
                            _propertyValuesIgnoredOrFailedForValidation[type].Add(propertyName);
                            return false;
                        }
                    }
                    else
                    {
#if NET
                        if (type.GetPropertyEx(propertyName) == null)
                        {
                            Log.Debug("Property '{0}' cannot be found via reflection, ignoring this property for type '{1}'", propertyName, type.FullName);

                            _propertyValuesIgnoredOrFailedForValidation[type].Add(propertyName);
                            return false;
                        }
#else
                        // Checking via reflection is faster than catching the exception
                        if (!Reflection.PropertyHelper.IsPublicProperty(this, propertyName))
                        {
                            Log.Debug("Property '{0}' is not a public property, cannot validate non-public properties in silverlight", propertyName);

                            _propertyValuesIgnoredOrFailedForValidation[type].Add(propertyName);
                            return false;
                        }
#endif
                    }

                    if (!_dataAnnotationsValidationContext.ContainsKey(propertyName))
                    {
                        _dataAnnotationsValidationContext[propertyName] = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null) { MemberName = propertyName };
                    }

                    System.ComponentModel.DataAnnotations.Validator.ValidateProperty(value, _dataAnnotationsValidationContext[propertyName]);

                    // If succeeded, clear any previous error
                    if (_dataAnnotationValidationResults.ContainsKey(propertyName))
                    {
                        _dataAnnotationValidationResults[propertyName] = null;
                    }
                }
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException validationException)
            {
                _dataAnnotationValidationResults[propertyName] = validationException.Message;
                return false;
            }
            catch (Exception ex)
            {
                _propertyValuesIgnoredOrFailedForValidation[type].Add(propertyName);

                Log.Warning(ex, "Failed to validate property '{0}' via Validator (property does not exists?)", propertyName);
            }
#endif

            return true;
        }

        /// <summary>
        /// Validates the field values of this object. Override this method to enable
        /// validation of field values.
        /// </summary>
        /// <param name="validationResults">The validation results, add additional results to this list.</param>
        protected virtual void ValidateFields(List<IFieldValidationResult> validationResults)
        {
        }

        /// <summary>
        /// Sets the field validation result.
        /// </summary>
        /// <param name="validationResult">The field validation result.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResult"/> is <c>null</c>.</exception>
        protected void SetFieldValidationResult(IFieldValidationResult validationResult)
        {
            Argument.IsNotNull("validationResult", validationResult);

            if (string.IsNullOrEmpty(validationResult.Message))
            {
                return;
            }

            var previousValidations = _internalValidationContext.GetFieldValidations(validationResult.PropertyName);

            // First, check if the same error already exists
            bool alreadyExists = (from previousValidation in previousValidations
                                  where string.Compare(previousValidation.Message, validationResult.Message, StringComparison.Ordinal) == 0
                                  select previousValidation).Any();
            if (alreadyExists)
            {
                return;
            }

            _internalValidationContext.AddFieldValidationResult(validationResult);

            if (!IsValidating)
            {
                switch (validationResult.ValidationResultType)
                {
                    case ValidationResultType.Warning:
                        NotifyWarningsChanged(validationResult.PropertyName, true);
                        break;

                    case ValidationResultType.Error:
                        NotifyErrorsChanged(validationResult.PropertyName, true);
                        break;
                }
            }
        }

        /// <summary>
        /// Validates the business rules of this object. Override this method to enable
        /// validation of business rules.
        /// </summary>
        /// <param name="validationResults">The validation results, add additional results to this list.</param>
        protected virtual void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
        {
        }

        /// <summary>
        /// Sets the business rule validation result.
        /// </summary>
        /// <param name="validationResult">The business rule validation result.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResult"/> is <c>null</c>.</exception>
        protected void SetBusinessRuleValidationResult(IBusinessRuleValidationResult validationResult)
        {
            Argument.IsNotNull("validationResult", validationResult);

            if (string.IsNullOrEmpty(validationResult.Message))
            {
                return;
            }

            var previousValidations = _internalValidationContext.GetBusinessRuleValidations();

            // First, check if the same error already exists
            bool alreadyExists = (from previousFieldValidation in previousValidations
                                  where string.Compare(previousFieldValidation.Message, validationResult.Message) == 0
                                  select previousFieldValidation).Any();
            if (alreadyExists)
            {
                return;
            }

            _internalValidationContext.AddBusinessRuleValidationResult(validationResult);

            if (!IsValidating)
            {
                switch (validationResult.ValidationResultType)
                {
                    case ValidationResultType.Warning:
                        NotifyWarningsChanged(string.Empty, true);
                        break;

                    case ValidationResultType.Error:
                        NotifyErrorsChanged(string.Empty, true);
                        break;
                }
            }
        }

        /// <summary>
        /// Called when the object is validating.
        /// </summary>
        protected virtual void OnValidating()
        {
            Validating.SafeInvoke(this);
        }

        /// <summary>
        /// Called when the object is validating the fields.
        /// </summary>
        protected virtual void OnValidatingFields()
        {
            ValidatingFields.SafeInvoke(this);
        }

        /// <summary>
        /// Called when the object has validated the fields.
        /// </summary>
        protected virtual void OnValidatedFields()
        {
            ValidatedFields.SafeInvoke(this);
        }

        /// <summary>
        /// Called when the object is validating the business rules.
        /// </summary>
        protected virtual void OnValidatingBusinessRules()
        {
            ValidatingBusinessRules.SafeInvoke(this);
        }

        /// <summary>
        /// Called when the object has validated the business rules.
        /// </summary>
        protected virtual void OnValidatedBusinessRules()
        {
            ValidatedBusinessRules.SafeInvoke(this);
        }

        /// <summary>
        /// Called when the object is validated.
        /// </summary>
        protected virtual void OnValidated()
        {
            Validated.SafeInvoke(this);
        }

        /// <summary>
        /// Validates the current object for field and business rule errors.
        /// </summary>
        /// <param name="force">If set to <c>true</c>, a validation is forced. When the validation is not forced, it means 
        /// that when the object is already validated, and no properties have been changed, no validation actually occurs 
        /// since there is no reason for any values to have changed.
        /// </param>
        /// <remarks>
        /// To check whether this object contains any errors, use the <see cref="HasErrors"/> property.
        /// </remarks>
        public void Validate(bool force = false)
        {
            Validate(force, true);
        }

        /// <summary>
        /// Validates the current object for field and business rule errors.
        /// </summary>
        /// <param name="force">If set to <c>true</c>, a validation is forced (even if the object knows it is already validated).</param>
        /// <param name="validateDataAnnotations">If set to <c>true</c>, the data annotations will be checked. This value is only used if <paramref name="force"/> is set to <c>true</c>.</param>
        /// <remarks>
        /// To check whether this object contains any errors, use the <see cref="HasErrors"/> property.
        /// </remarks>
        internal void Validate(bool force, bool validateDataAnnotations)
        {
            if (SuspendValidation)
            {
                return;
            }

            if (IsValidating)
            {
                return;
            }

            IsValidating = true;

            var validationContext = (ValidationContext)ValidationContext;
            var changes = new List<ValidationContextChange>();
            bool hasErrors = HasErrors;
            bool hasWarnings = HasWarnings;

            var validator = Validator;
            if (validator != null)
            {
                validator.BeforeValidation(this, validationContext.GetFieldValidations(), validationContext.GetBusinessRuleValidations());
            }

            OnValidating();

            CatchUpWithSuspendedAnnotationsValidation();

            if (force && validateDataAnnotations)
            {
                var type = GetType();

                var ignoredOrFailedPropertyValidations = _propertyValuesIgnoredOrFailedForValidation[type];

                // In forced mode, validate all registered properties for annotations
                var catelTypeInfo = PropertyDataManager.GetCatelTypeInfo(type);
                foreach (var propertyData in catelTypeInfo.GetCatelProperties())
                {
                    var propertyInfo = propertyData.Value.GetPropertyInfo(type);
                    if (propertyInfo == null || !propertyInfo.HasPublicGetter)
                    {
                        // Note: non-public getter, do not validate
                        ignoredOrFailedPropertyValidations.Add(propertyData.Key);
                        continue;
                    }

                    var propertyValue = GetValue(propertyData.Value);
                    ValidatePropertyUsingAnnotations(propertyData.Key, propertyValue, propertyData.Value);
                }

#if !WINDOWS_PHONE && !NETFX_CORE && !PCL && !NET35
                // Validate non-catel properties as well for attribute validation
                foreach (var propertyInfo in catelTypeInfo.GetNonCatelProperties())
                {
                    

                    if (_firstAnnotationValidation)
                    {
                        if (propertyInfo.Value.IsDecoratedWithAttribute(typeof (ExcludeFromValidationAttribute)))
                        {
                            ignoredOrFailedPropertyValidations.Add(propertyInfo.Key);
                        }
                    }

                    if (!propertyInfo.Value.HasPublicGetter)
                    {
                        // Note: non-public getter, do not validate
                        ignoredOrFailedPropertyValidations.Add(propertyInfo.Key);
                        continue;
                    }

                    // TODO: Should we check for annotations attributes?
                    if (ignoredOrFailedPropertyValidations.Contains(propertyInfo.Key))
                    {
                        continue;
                    }

                    try
                    {
                        var propertyValue = propertyInfo.Value.PropertyInfo.GetValue(this, null);
                        ValidatePropertyUsingAnnotations(propertyInfo.Key, propertyValue, null);
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "Failed to validate property '{0}.{1}', adding it to the ignore list", type.Name, propertyInfo.Key);
                        ignoredOrFailedPropertyValidations.Add(propertyInfo.Key);
                    }
                }

                _firstAnnotationValidation = false;
#endif
            }

            if (!IsValidated || force)
            {
                lock (_validationLock)
                {
                    var fieldValidationResults = new List<IFieldValidationResult>();
                    var businessRuleValidationResults = new List<IBusinessRuleValidationResult>();

                    #region Fields
                    if (validator != null)
                    {
                        validator.BeforeValidateFields(this, validationContext.GetFieldValidations());
                    }

                    OnValidatingFields();

                    if (validator != null)
                    {
                        validator.ValidateFields(this, fieldValidationResults);
                    }

#if !WINDOWS_PHONE && !NETFX_CORE && !PCL && !NET35
                    // Support annotation validation
                    fieldValidationResults.AddRange(from fieldAnnotationValidation in _dataAnnotationValidationResults
                                                    where !string.IsNullOrEmpty(fieldAnnotationValidation.Value)
                                                    select (IFieldValidationResult)FieldValidationResult.CreateError(fieldAnnotationValidation.Key, fieldAnnotationValidation.Value));
#endif

                    ValidateFields(fieldValidationResults);

                    // In-between validations, it might be possible that users used the SetFieldValidationResult
                    if (_internalValidationContext != null)
                    {
                        fieldValidationResults.AddRange(_internalValidationContext.GetFieldValidations());
                    }

                    OnValidatedFields();

                    if (validator != null)
                    {
                        validator.AfterValidateFields(this, fieldValidationResults);
                    }
                    #endregion

                    #region Business rules
                    if (validator != null)
                    {
                        validator.BeforeValidateBusinessRules(this, validationContext.GetBusinessRuleValidations());
                    }

                    OnValidatingBusinessRules();

                    if (validator != null)
                    {
                        validator.ValidateBusinessRules(this, businessRuleValidationResults);
                    }

                    ValidateBusinessRules(businessRuleValidationResults);

                    // In-between validations, it might be possible that users used the SetBusinessRuleValidationResult
                    if (_internalValidationContext != null)
                    {
                        businessRuleValidationResults.AddRange(_internalValidationContext.GetBusinessRuleValidations());
                    }

                    OnValidatedBusinessRules();

                    if (validator != null)
                    {
                        validator.AfterValidateBusinessRules(this, businessRuleValidationResults);
                    }
                    #endregion

                    if (validator != null)
                    {
                        validator.Validate(this, validationContext);
                    }

                    IsValidated = true;

                    // Clear internal validation
                    _internalValidationContext = new ValidationContext();

                    changes = validationContext.SynchronizeWithContext(new ValidationContext(fieldValidationResults, businessRuleValidationResults));
                }
            }

            OnValidated();

            if (validator != null)
            {
                validator.AfterValidation(this, validationContext.GetFieldValidations(), validationContext.GetBusinessRuleValidations());
            }

            #region Notify changes
            bool hasNotifiedBusinessWarningsChanged = false;
            bool hasNotifiedBusinessErrorsChanged = false;
            foreach (var change in changes)
            {
                var changeAsFieldValidationResult = change.ValidationResult as IFieldValidationResult;
                var changeAsBusinessRuleValidationResult = change.ValidationResult as IBusinessRuleValidationResult;

                if (changeAsFieldValidationResult != null)
                {
                    switch (change.ValidationResult.ValidationResultType)
                    {
                        case ValidationResultType.Warning:
                            NotifyWarningsChanged(changeAsFieldValidationResult.PropertyName, false);
                            break;

                        case ValidationResultType.Error:
                            NotifyErrorsChanged(changeAsFieldValidationResult.PropertyName, false);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else if (changeAsBusinessRuleValidationResult != null)
                {
                    switch (change.ValidationResult.ValidationResultType)
                    {
                        case ValidationResultType.Warning:
                            if (!hasNotifiedBusinessWarningsChanged)
                            {
                                hasNotifiedBusinessWarningsChanged = true;
                                NotifyWarningsChanged(string.Empty, false);
                            }
                            break;

                        case ValidationResultType.Error:
                            if (!hasNotifiedBusinessErrorsChanged)
                            {
                                hasNotifiedBusinessErrorsChanged = true;
                                NotifyErrorsChanged(string.Empty, false);
                            }
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }


            if (HasWarnings != hasWarnings)
            {
                RaisePropertyChanged("HasWarnings");
            }

            if (HasErrors != hasErrors)
            {
                RaisePropertyChanged("HasErrors");
            }
            #endregion

            IsValidating = false;
        }

        /// <summary>
        /// Notifies all listeners that the errors for the specified property have changed. If the
        /// <paramref name="propertyName"/> is <c>null</c> or <see cref="string.Empty"/>, the business
        /// errors will be updated.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="notifyHasErrors">if set to <c>true</c>, the <see cref="HasErrors"/> property will be notified as well.</param>
        private void NotifyErrorsChanged(string propertyName, bool notifyHasErrors)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                RaisePropertyChanged(ErrorMessageProperty);

                ErrorsChanged.SafeInvoke(this, new DataErrorsChangedEventArgs(string.Empty));
            }
            else
            {
                RaisePropertyChanged(this, new PropertyChangedEventArgs(propertyName), false, true);

                ErrorsChanged.SafeInvoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            if (notifyHasErrors)
            {
                RaisePropertyChanged("HasErrors");
            }
        }

        /// <summary>
        /// Notifies all listeners that the warnings for the specified property have changed. If the
        /// <paramref name="propertyName"/> is <c>null</c> or <see cref="string.Empty"/>, the business
        /// errors will be updated.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="notifyHasWarnings">if set to <c>true</c>, the <see cref="HasWarnings"/> property will be notified as well.</param>
        private void NotifyWarningsChanged(string propertyName, bool notifyHasWarnings)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                RaisePropertyChanged(WarningMessageProperty);

                WarningsChanged.SafeInvoke(this, new DataErrorsChangedEventArgs(string.Empty));
            }
            else
            {
                RaisePropertyChanged(this, new PropertyChangedEventArgs(propertyName), false, true);

                WarningsChanged.SafeInvoke(this, new DataErrorsChangedEventArgs(propertyName));
            }

            if (notifyHasWarnings)
            {
                RaisePropertyChanged("HasWarnings");
            }
        }

        /// <summary>
        /// Gets the list messages.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <param name="validationResult">The validation result.</param>
        /// <returns>
        /// String representing the output of all items in the fields an business object.
        /// </returns>
        /// <remarks>
        /// This method is used to create a message string for field warnings or errors and business warnings
        /// or errors. Just pass the right dictionary and list to this method.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="validationContext"/> is <c>null</c>.</exception>
        private static string GetListMessages(IValidationContext validationContext, ValidationResultType validationResult)
        {
            Argument.IsNotNull("validationContext", validationContext);

            var messageBuilder = new StringBuilder();

            switch (validationResult)
            {
                case ValidationResultType.Warning:
                    foreach (var field in validationContext.GetFieldWarnings())
                    {
                        messageBuilder.AppendLine("* {0}", field.Message);
                    }

                    foreach (var businessItem in validationContext.GetBusinessRuleWarnings())
                    {
                        messageBuilder.AppendLine("* {0}", businessItem.Message);
                    }
                    break;

                case ValidationResultType.Error:
                    foreach (var field in validationContext.GetFieldErrors())
                    {
                        messageBuilder.AppendLine("* {0}", field.Message);
                    }

                    foreach (var businessItem in validationContext.GetBusinessRuleErrors())
                    {
                        messageBuilder.AppendLine("* {0}", businessItem.Message);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException("validationResult");
            }

            return messageBuilder.ToString();
        }
        #endregion

        #region IDataWarningInfo Members
        /// <summary>
        /// Gets the current warning.
        /// </summary>
        string IDataWarningInfo.Warning
        {
            get
            {
                if (HideValidationResults)
                {
                    return string.Empty;
                }

                if (!IsValidated && AutomaticallyValidateOnPropertyChanged)
                {
                    Validate();
                }

                var warning = (from businessRuleWarning in ValidationContext.GetBusinessRuleWarnings()
                               select businessRuleWarning.Message).FirstOrDefault();

                return warning ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets a warning for a specific column.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <returns>The warning or <see cref="string.Empty"/> if no warning is available.</returns>
        string IDataWarningInfo.this[string columnName]
        {
            get
            {
                if (string.IsNullOrEmpty(columnName))
                {
                    return string.Empty;
                }

                if (HideValidationResults)
                {
                    return string.Empty;
                }

                if (!IsValidated && AutomaticallyValidateOnPropertyChanged)
                {
                    Validate();
                }

                var warning = (from fieldWarning in ValidationContext.GetFieldWarnings(columnName)
                               select fieldWarning.Message).FirstOrDefault();

                return warning ?? string.Empty;
            }
        }

        /// <summary>
        /// Returns a message that contains all the current warnings.
        /// </summary>
        /// <param name="userFriendlyObjectName">Name of the user friendly object.</param>
        /// <returns>
        /// Warning string or empty in case of no warnings.
        /// </returns>
        public string GetWarningMessage(string userFriendlyObjectName = null)
        {
            if (!HasWarnings)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(userFriendlyObjectName))
            {
                // Use the real entity name (stupid developer that passes a useless value)
                userFriendlyObjectName = GetType().Name;
            }

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(ResourceHelper.GetString("WarningsFound"), userFriendlyObjectName);
            messageBuilder.Append(GetListMessages(ValidationContext, ValidationResultType.Warning));

            return messageBuilder.ToString();
        }
        #endregion

        #region IDataErrorInfo Members
        /// <summary>
        /// Gets the current error.
        /// </summary>
        string IDataErrorInfo.Error
        {
            get
            {
                if (HideValidationResults)
                {
                    return string.Empty;
                }

                if (!IsValidated && AutomaticallyValidateOnPropertyChanged)
                {
                    Validate();
                }

                var error = (from businessRuleError in ValidationContext.GetBusinessRuleErrors()
                             select businessRuleError.Message).FirstOrDefault();

                return error ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets an error for a specific column.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <returns>The error or <see cref="string.Empty"/> if no error is available.</returns>
        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                if (string.IsNullOrEmpty(columnName))
                {
                    return string.Empty;
                }

                if (HideValidationResults)
                {
                    return string.Empty;
                }

                if (!IsValidated && AutomaticallyValidateOnPropertyChanged)
                {
                    Validate();
                }

                var error = (from fieldError in ValidationContext.GetFieldErrors(columnName)
                             select fieldError.Message).FirstOrDefault();

                return error ?? string.Empty;
            }
        }

        /// <summary>
        /// Returns a message that contains all the current errors.
        /// </summary>
        /// <param name="userFriendlyObjectName">Name of the user friendly object.</param>
        /// <returns>
        /// Error string or empty in case of no errors.
        /// </returns>
        public string GetErrorMessage(string userFriendlyObjectName = null)
        {
            if (!HasErrors)
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(userFriendlyObjectName))
            {
                // Use the real entity name (stupid developer that passes a useless value)
                userFriendlyObjectName = GetType().Name;
            }

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(ResourceHelper.GetString("ErrorsFound"), userFriendlyObjectName);
            messageBuilder.Append(GetListMessages(ValidationContext, ValidationResultType.Error));

            return messageBuilder.ToString();
        }
        #endregion

        #region INotifyDataErrorInfo Members
        /// <summary>
        /// Gets a value indicating whether this object contains any field or business errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
#if !WINDOWS_PHONE && !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        [XmlIgnore]
        public bool HasErrors
        {
            get
            {
                if (HideValidationResults)
                {
                    return false;
                }

                return (FieldErrorCount + BusinessRuleErrorCount) > 0;
            }
        }

        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire object.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire object.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve validation errors for, or null or <see cref="F:System.String.Empty"/> to retrieve errors for the entire object.</param>
        /// <returns>
        /// The validation errors for the property or object.
        /// </returns>
        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            if (HideValidationResults)
            {
                yield return null;
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                lock (ValidationContext)
                {
                    foreach (var error in ValidationContext.GetBusinessRuleErrors())
                    {
                        yield return error.Message;
                    }

                }
            }
            else
            {
                lock (ValidationContext)
                {
                    foreach (var error in ValidationContext.GetFieldErrors(propertyName))
                    {
                        yield return error.Message;
                    }
                }
            }
        }
        #endregion

        #region INotifyDataWarningInfo Members
        /// <summary>
        /// Gets a value indicating whether this object contains any field or business warnings.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has warnings; otherwise, <c>false</c>.
        /// </value>
#if !WINDOWS_PHONE && !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        [XmlIgnore]
        public bool HasWarnings
        {
            get
            {
                if (HideValidationResults)
                {
                    return false;
                }

                return (FieldWarningCount + BusinessRuleWarningCount) > 0;
            }
        }

        /// <summary>
        /// Occurs when the warnings have changed.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        public event EventHandler<DataErrorsChangedEventArgs> WarningsChanged;

        /// <summary>
        /// Gets the warnings for the specific property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><see cref="IEnumerable"/> of warnings.</returns>
        IEnumerable INotifyDataWarningInfo.GetWarnings(string propertyName)
        {
            if (HideValidationResults)
            {
                yield return null;
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                lock (ValidationContext)
                {
                    foreach (var warning in ValidationContext.GetBusinessRuleWarnings())
                    {
                        yield return warning.Message;
                    }

                }
            }
            else
            {
                lock (ValidationContext)
                {
                    foreach (var warning in ValidationContext.GetFieldWarnings(propertyName))
                    {
                        yield return warning.Message;
                    }
                }
            }
        }
        #endregion

        #region Notifications
        /// <summary>
        /// Raises the right events based on the validation result.
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        /// <param name="notifyGlobal">If set to <c>true</c>, the global properties such as <see cref="HasErrors" /> and <see cref="HasWarnings" /> are also raised.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="validationResult"/> is <c>null</c>.</exception>
        protected void NotifyValidationResult(IValidationResult validationResult, bool notifyGlobal)
        {
            Argument.IsNotNull("validationResult", validationResult);

            var propertyName = string.Empty;

            var fieldValidationResult = validationResult as IFieldValidationResult;
            if (fieldValidationResult != null)
            {
                propertyName = fieldValidationResult.PropertyName;
            }

            switch (validationResult.ValidationResultType)
            {
                case ValidationResultType.Warning:
                    NotifyWarningsChanged(propertyName, notifyGlobal);
                    break;

                case ValidationResultType.Error:
                    NotifyErrorsChanged(propertyName, notifyGlobal);
                    break;
            }
        }
        #endregion
    }
}