// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.validation.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using Runtime.Serialization;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;
    using IoC;

    using Logging;

#if NET
    using Reflection;
#endif

    public partial class ModelBase
    {
        #region Constants
        /// <summary>
        /// The name of the <see cref="IDataWarningInfo.Warning"/> property.
        /// </summary>
        private const string WarningMessageProperty = "IDataWarningInfo.Warning";

        /// <summary>
        /// The name of the <see cref="INotifyDataWarningInfo.HasWarnings"/> property.
        /// </summary>
        private const string HasWarningsMessageProperty = "INotifyDataWarningInfo.HasWarnings";

        /// <summary>
        /// The name of the <see cref="IDataErrorInfo.Error"/> property.
        /// </summary>
        private const string ErrorMessageProperty = "IDataErrorInfo.Error";

        /// <summary>
        /// The name of the <see cref="INotifyDataErrorInfo.HasErrors"/> property.
        /// </summary>
        private const string HasErrorsMessageProperty = "INotifyDataErrorInfo.HasErrors";
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
        /// The backing field for the <see cref="IModelValidation.Validator"/> property.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private IValidator _validator;

        /// <summary>
        /// The validation context, which can contain in-between validation info.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly ValidationContext _validationContext = new ValidationContext();

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

#if NET
        [field: NonSerialized]
#endif
        private SuspensionContext _validationSuspensionContext;

        /// <summary>
        /// The property names that failed to validate and should be skipped next time for NET 4.0 
        /// attribute validation.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private static readonly Dictionary<Type, HashSet<string>> _propertyValuesIgnoredOrFailedForValidation = new Dictionary<Type, HashSet<string>>();

#if !NETFX_CORE && !PCL

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

#if NET
        [field: NonSerialized]
#endif
        private event EventHandler<DataErrorsChangedEventArgs> _errorsChanged;

#if NET
        [field: NonSerialized]
#endif
        private event EventHandler<DataErrorsChangedEventArgs> _warningsChanged;

#if NET
        [field: NonSerialized]
#endif
        private event EventHandler<ValidationEventArgs> _validating;

#if NET
        [field: NonSerialized]
#endif
        private event EventHandler<ValidationEventArgs> _validated;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the object is currently validating. During validation, no validation will be invoked.
        /// </summary>
        /// <value>
        /// <c>true</c> if the object is validating; otherwise, <c>false</c>.
        /// </value>
#if NET
        [Browsable(false)]
#endif
        [XmlIgnore]
        protected bool IsValidating { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this object is validated or not.
        /// </summary>
#if !NETFX_CORE && !PCL
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
#if !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        [XmlIgnore]
        IValidator IModelValidation.Validator
        {
            get { return GetValidator(); }
            set { _validator = value; }
        }

        /// <summary>
        /// Gets the validation context which contains all information about the validation.
        /// </summary>
        /// <value>The validation context.</value>
#if !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        [XmlIgnore]
        IValidationContext IModelValidation.ValidationContext
        {
            get { return _validationContext; }
        }

        /// <summary>
        /// Gets the number of field warnings.
        /// </summary>
        /// <value>The field warning count.</value>
#if !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        [XmlIgnore]
        int IModelValidation.FieldWarningCount
        {
            get
            {
                EnsureValidationIsUpToDate();

                return _validationContext.GetFieldWarningCount();
            }
        }

        /// <summary>
        /// Gets the number of business rule warnings.
        /// </summary>
        /// <value>The business rule warning count.</value>
#if !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        [XmlIgnore]
        int IModelValidation.BusinessRuleWarningCount
        {
            get
            {
                EnsureValidationIsUpToDate();

                return _validationContext.GetBusinessRuleWarningCount();
            }
        }

        /// <summary>
        /// Gets the number of field errors.
        /// </summary>
        /// <value>The field error count.</value>
#if !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        [XmlIgnore]
        int IModelValidation.FieldErrorCount
        {
            get
            {
                EnsureValidationIsUpToDate();

                return _validationContext.GetFieldErrorCount();
            }
        }

        /// <summary>
        /// Gets the number of business rule errors.
        /// </summary>
        /// <value>The business rule error count.</value>
#if !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        [XmlIgnore]
        int IModelValidation.BusinessRuleErrorCount
        {
            get
            {
                EnsureValidationIsUpToDate();

                return _validationContext.GetBusinessRuleErrorCount();
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
#if !NETFX_CORE && !PCL
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
        /// Gets or sets a value indicating whether the validation should not try to process data annotations.
        /// </summary>
        protected bool ValidateUsingDataAnnotations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the validation should not try to process data annotations.
        /// </summary>
        public static bool DefaultValidateUsingDataAnnotationsValue { get; set; }

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
#if !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        protected bool HideValidationResults { get; set; }

        /// <summary>
        /// Gets a value indicating whether the object is currently hiding its validation results. If the object
        /// hides its validation results, it is still possible to retrieve the validation results using the
        /// <see cref="ValidationContext"/>.
        /// </summary>
        bool IModelValidation.IsHidingValidationResults { get { return HideValidationResults; } }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the object is validating.
        /// </summary>
        event EventHandler<ValidationEventArgs> IModelValidation.Validating
        {
            add { _validating += value; }
            remove { _validating -= value; }
        }

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
        event EventHandler<ValidationEventArgs> IModelValidation.Validated
        {
            add { _validated += value; }
            remove { _validated -= value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Suspends the validation until the disposable token has been disposed.
        /// </summary>
        /// <returns></returns>
        public IDisposable SuspendValidations(bool validateOnResume = true)
        {
            var token = new DisposableToken<ModelBase>(this, x =>
            {
                lock (_lock)
                {
                    if (_validationSuspensionContext == null)
                    {
                        _validationSuspensionContext = new SuspensionContext();
                    }

                    _validationSuspensionContext.Increment();
                }
            },
            x =>
            {
                SuspensionContext suspensionContext;

                lock (_lock)
                {
                    suspensionContext = _validationSuspensionContext;
                    if (suspensionContext != null)
                    {
                        suspensionContext.Decrement();

                        if (suspensionContext.Counter == 0)
                        {
                            _validationSuspensionContext = null;
                        }
                    }
                }

                if (validateOnResume)
                {
                    if (suspensionContext != null && suspensionContext.Counter == 0)
                    {
                        //var properties = suspensionContext.Properties;

                        // TODO: In v5, replace with context and smart validation
                        Validate(true, true);

                        //foreach (var property in properties)
                        //{
                        //    Validate(property);
                        //}
                    }
                }
            });

            return token;
        }

        /// <summary>
        /// Ensures the validation is up to date.
        /// </summary>
        /// <param name="constraint">if set to <c>true</c>, the validation will be updated if not up to date.</param>
        private void EnsureValidationIsUpToDate(bool constraint = true)
        {
            if (constraint && !IsValidated)
            {
                Validate();
            }
        }

        /// <summary>
        /// Gets the validator. If the field is <c>null</c>, it will query the service locator.
        /// </summary>
        /// <returns>IValidator.</returns>
        private IValidator GetValidator()
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

                foreach (var property in _propertiesNotCheckedDuringDisabledValidation)
                {
                    var propertyData = GetPropertyData(property);
                    var propertyValue = GetValueFast<object>(propertyData.Name);
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
            if (!ValidateUsingDataAnnotations)
            {
                return true;
            }

            if (SuspendValidation)
            {
                _propertiesNotCheckedDuringDisabledValidation.Add(propertyName);
                return true;
            }

#if !NETFX_CORE && !PCL
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
                            Log.Debug("Property '{0}' is not a public property, cannot validate non-public properties in the current platform", propertyName);

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
        /// Validates the business rules of this object. Override this method to enable
        /// validation of business rules.
        /// </summary>
        /// <param name="validationResults">The validation results, add additional results to this list.</param>
        protected virtual void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
        {
        }

        /// <summary>
        /// Called when the object is validating.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        protected virtual void OnValidating(IValidationContext validationContext)
        {
            var handler = _validating;
            if (handler != null)
            {
                handler(this, new ValidationEventArgs(validationContext));
            }
        }

        /// <summary>
        /// Called when the object is validating the fields.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        protected virtual void OnValidatingFields(IValidationContext validationContext)
        {
            ValidatingFields.SafeInvoke(this);
        }

        /// <summary>
        /// Called when the object has validated the fields.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        protected virtual void OnValidatedFields(IValidationContext validationContext)
        {
            ValidatedFields.SafeInvoke(this);
        }

        /// <summary>
        /// Called when the object is validating the business rules.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        protected virtual void OnValidatingBusinessRules(IValidationContext validationContext)
        {
            ValidatingBusinessRules.SafeInvoke(this);
        }

        /// <summary>
        /// Called when the object has validated the business rules.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        protected virtual void OnValidatedBusinessRules(IValidationContext validationContext)
        {
            ValidatedBusinessRules.SafeInvoke(this);
        }

        /// <summary>
        /// Called when the object is validated.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        protected virtual void OnValidated(IValidationContext validationContext)
        {
            var handler = _validated;
            if (handler != null)
            {
                handler(this, new ValidationEventArgs(validationContext));
            }
        }

        /// <summary>
        /// Validates the current object for field and business rule errors.
        /// </summary>
        /// <param name="force">If set to <c>true</c>, a validation is forced. When the validation is not forced, it means 
        /// that when the object is already validated, and no properties have been changed, no validation actually occurs 
        /// since there is no reason for any values to have changed.
        /// </param>
        /// <remarks>
        /// To check whether this object contains any errors, use the <see cref="INotifyDataErrorInfo.HasErrors"/> property.
        /// </remarks>
        void IModelValidation.Validate(bool force)
        {
            Validate(force);
        }

        /// <summary>
        /// Validates the current object for field and business rule errors.
        /// </summary>
        /// <param name="force">If set to <c>true</c>, a validation is forced. When the validation is not forced, it means 
        /// that when the object is already validated, and no properties have been changed, no validation actually occurs 
        /// since there is no reason for any values to have changed.
        /// </param>
        /// <remarks>
        /// To check whether this object contains any errors, use the <see cref="INotifyDataErrorInfo.HasErrors"/> property.
        /// </remarks>
        protected void Validate(bool force = false)
        {
            Validate(force, ValidateUsingDataAnnotations);
        }

        /// <summary>
        /// Validates the current object for field and business rule errors.
        /// </summary>
        /// <param name="force">If set to <c>true</c>, a validation is forced (even if the object knows it is already validated).</param>
        /// <param name="validateDataAnnotations">If set to <c>true</c>, the data annotations will be checked. This value is only used if <paramref name="force"/> is set to <c>true</c>.</param>
        /// <remarks>
        /// To check whether this object contains any errors, use the <see cref="INotifyDataErrorInfo.HasErrors"/> property.
        /// </remarks>
        /// TODO: Try to revert to internal but is required by XAMARIN_FORMS
        public void Validate(bool force, bool validateDataAnnotations)
        {
            lock (_lock)
            {
                // Note: in v5 we should check the actual properties we missed
                var validationSuspensionContext = _validationSuspensionContext;
                if (validationSuspensionContext != null)
                {
                    return;
                }
            }

            if (SuspendValidation)
            {
                return;
            }

            if (IsValidating)
            {
                return;
            }

            IsValidating = true;

            var existingValidationContext = _validationContext;
            if (existingValidationContext == null)
            {
                existingValidationContext = new ValidationContext();
            }

            var hasErrors = existingValidationContext.HasErrors;
            var hasWarnings = existingValidationContext.HasWarnings;

            var validationContext = new ValidationContext();
            var changes = new List<ValidationContextChange>();

            var validator = GetValidator();
            if (validator != null)
            {
                validator.BeforeValidation(this, existingValidationContext.GetFieldValidations(), existingValidationContext.GetBusinessRuleValidations());
            }

            OnValidating(validationContext);

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

#if !NETFX_CORE && !PCL
                // Validate non-catel properties as well for attribute validation
                foreach (var propertyInfo in catelTypeInfo.GetNonCatelProperties())
                {
                    if (_firstAnnotationValidation)
                    {
                        if (propertyInfo.Value.IsDecoratedWithAttribute<ExcludeFromValidationAttribute>())
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
                lock (_validationContext)
                {
                    var fieldValidationResults = new List<IFieldValidationResult>();
                    var businessRuleValidationResults = new List<IBusinessRuleValidationResult>();

                    #region Fields
                    if (validator != null)
                    {
                        validator.BeforeValidateFields(this, validationContext.GetFieldValidations());
                    }

                    OnValidatingFields(validationContext);

                    if (validator != null)
                    {
                        validator.ValidateFields(this, fieldValidationResults);
                    }

#if !NETFX_CORE && !PCL
                    // Support annotation validation
                    fieldValidationResults.AddRange(from fieldAnnotationValidation in _dataAnnotationValidationResults
                                                    where !string.IsNullOrEmpty(fieldAnnotationValidation.Value)
                                                    select (IFieldValidationResult)FieldValidationResult.CreateError(fieldAnnotationValidation.Key, fieldAnnotationValidation.Value));
#endif

                    ValidateFields(fieldValidationResults);

                    OnValidatedFields(validationContext);

                    if (validator != null)
                    {
                        validator.AfterValidateFields(this, fieldValidationResults);
                    }

                    // As the last step, sync the field validation results with the context
                    foreach (var fieldValidationResult in fieldValidationResults)
                    {
                        validationContext.AddFieldValidationResult(fieldValidationResult);
                    }
                    #endregion

                    #region Business rules
                    if (validator != null)
                    {
                        validator.BeforeValidateBusinessRules(this, validationContext.GetBusinessRuleValidations());
                    }

                    OnValidatingBusinessRules(validationContext);

                    if (validator != null)
                    {
                        validator.ValidateBusinessRules(this, businessRuleValidationResults);
                    }

                    ValidateBusinessRules(businessRuleValidationResults);

                    OnValidatedBusinessRules(validationContext);

                    if (validator != null)
                    {
                        validator.AfterValidateBusinessRules(this, businessRuleValidationResults);
                    }

                    // As the last step, sync the field validation results with the context
                    foreach (var businessRuleValidationResult in businessRuleValidationResults)
                    {
                        validationContext.AddBusinessRuleValidationResult(businessRuleValidationResult);
                    }
                    #endregion

                    if (validator != null)
                    {
                        validator.Validate(this, validationContext);
                    }

                    IsValidated = true;

                    // Manual sync to get the changes
                    changes = existingValidationContext.SynchronizeWithContext(validationContext);
                }
            }

            OnValidated(validationContext);

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

            if (_validationContext.HasWarnings != hasWarnings)
            {
                RaisePropertyChanged(HasWarningsMessageProperty);
            }

            if (_validationContext.HasErrors != hasErrors)
            {
                RaisePropertyChanged(HasErrorsMessageProperty);
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
        /// <param name="notifyHasErrors">if set to <c>true</c>, the <see cref="INotifyDataErrorInfo.HasErrors"/> property will be notified as well.</param>
        private void NotifyErrorsChanged(string propertyName, bool notifyHasErrors)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                RaisePropertyChanged(ErrorMessageProperty);

                _errorsChanged.SafeInvoke(this, () => new DataErrorsChangedEventArgs(string.Empty));
            }
            else
            {
                RaisePropertyChanged(this, new PropertyChangedEventArgs(propertyName), false, true);

                _errorsChanged.SafeInvoke(this, () => new DataErrorsChangedEventArgs(propertyName));
            }

            if (notifyHasErrors)
            {
                RaisePropertyChanged(HasErrorsMessageProperty);
            }
        }

        /// <summary>
        /// Notifies all listeners that the warnings for the specified property have changed. If the
        /// <paramref name="propertyName"/> is <c>null</c> or <see cref="string.Empty"/>, the business
        /// errors will be updated.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="notifyHasWarnings">if set to <c>true</c>, the <see cref="INotifyDataWarningInfo.HasWarnings"/> property will be notified as well.</param>
        private void NotifyWarningsChanged(string propertyName, bool notifyHasWarnings)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                RaisePropertyChanged(WarningMessageProperty);

                _warningsChanged.SafeInvoke(this, () => new DataErrorsChangedEventArgs(string.Empty));
            }
            else
            {
                RaisePropertyChanged(this, new PropertyChangedEventArgs(propertyName), false, true);

                _warningsChanged.SafeInvoke(this, () => new DataErrorsChangedEventArgs(propertyName));
            }

            if (notifyHasWarnings)
            {
                RaisePropertyChanged(HasWarningsMessageProperty);
            }
        }

        /// <summary>
        /// Gets the current business warnings.
        /// </summary>
        /// <returns>The warnings or <see cref="string.Empty"/> if no warning is available.</returns>
        protected virtual string GetBusinessRuleWarnings()
        {
            var warning = (from businessRuleWarning in _validationContext.GetBusinessRuleWarnings()
                           select businessRuleWarning.Message).FirstOrDefault();

            return warning ?? string.Empty;
        }

        /// <summary>
        /// Gets the warnings for a specific column.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <returns>The warnings or <see cref="string.Empty"/> if no warning is available.</returns>
        protected virtual string GetFieldWarnings(string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
            {
                return string.Empty;
            }

            var warning = (from fieldWarning in _validationContext.GetFieldWarnings(columnName)
                           select fieldWarning.Message).FirstOrDefault();

            return warning ?? string.Empty;
        }

        /// <summary>
        /// Gets the current errors errors.
        /// </summary>
        /// <returns>The errors or <see cref="string.Empty"/> if no error is available.</returns>
        protected virtual string GetBusinessRuleErrors()
        {
            var error = (from businessRuleError in _validationContext.GetBusinessRuleErrors()
                         select businessRuleError.Message).FirstOrDefault();

            return error ?? string.Empty;
        }

        /// <summary>
        /// Gets the errors for a specific column.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <returns>The errors or <see cref="string.Empty"/> if no error is available.</returns>
        protected virtual string GetFieldErrors(string columnName)
        {
            if (string.IsNullOrEmpty(columnName))
            {
                return string.Empty;
            }

            var error = (from fieldError in _validationContext.GetFieldErrors(columnName)
                         select fieldError.Message).FirstOrDefault();

            return error ?? string.Empty;
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

                EnsureValidationIsUpToDate(AutomaticallyValidateOnPropertyChanged);

                return GetBusinessRuleWarnings() ?? string.Empty;
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

                EnsureValidationIsUpToDate(AutomaticallyValidateOnPropertyChanged);

                return GetFieldWarnings(columnName) ?? string.Empty;
            }
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

                EnsureValidationIsUpToDate(AutomaticallyValidateOnPropertyChanged);

                return GetBusinessRuleErrors() ?? string.Empty;
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

                EnsureValidationIsUpToDate(AutomaticallyValidateOnPropertyChanged);

                return GetFieldErrors(columnName) ?? string.Empty;
            }
        }
        #endregion

        #region INotifyDataErrorInfo Members
        /// <summary>
        /// Gets a value indicating whether this object contains any field or business errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
#if !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        [XmlIgnore]
        bool INotifyDataErrorInfo.HasErrors
        {
            get
            {
                if (HideValidationResults)
                {
                    return false;
                }

                EnsureValidationIsUpToDate();

                return _validationContext.HasErrors;
            }
        }

        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire object.
        /// </summary>
        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
        {
            add { _errorsChanged += value; }
            remove { _errorsChanged -= value; }
        }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire object.
        /// </summary>
        /// <param name="propertyName">The name of the property to retrieve validation errors for, or null or <see cref="F:System.String.Empty"/> to retrieve errors for the entire object.</param>
        /// <returns>
        /// The validation errors for the property or object.
        /// </returns>
        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            var elements = new List<string>();

            if (HideValidationResults)
            {
                return elements;
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                lock (_validationContext)
                {
                    foreach (var error in _validationContext.GetBusinessRuleErrors())
                    {
                        elements.Add(error.Message);
                    }

                }
            }
            else
            {
                lock (_validationContext)
                {
                    foreach (var error in _validationContext.GetFieldErrors(propertyName))
                    {
                        elements.Add(error.Message);
                    }
                }
            }

            return elements;
        }
        #endregion

        #region INotifyDataWarningInfo Members
        /// <summary>
        /// Gets a value indicating whether this object contains any field or business warnings.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has warnings; otherwise, <c>false</c>.
        /// </value>
#if !NETFX_CORE && !PCL
        [Browsable(false)]
#endif
        [XmlIgnore]
        bool INotifyDataWarningInfo.HasWarnings
        {
            get
            {
                if (HideValidationResults)
                {
                    return false;
                }

                EnsureValidationIsUpToDate();

                return _validationContext.HasWarnings;
            }
        }

        /// <summary>
        /// Occurs when the warnings have changed.
        /// </summary>
        event EventHandler<DataErrorsChangedEventArgs> INotifyDataWarningInfo.WarningsChanged
        {
            add { _warningsChanged += value; }
            remove { _warningsChanged -= value; }
        }

        /// <summary>
        /// Gets the warnings for the specific property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><see cref="IEnumerable"/> of warnings.</returns>
        IEnumerable INotifyDataWarningInfo.GetWarnings(string propertyName)
        {
            var elements = new List<string>();

            if (HideValidationResults)
            {
                return elements;
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                lock (_validationContext)
                {
                    foreach (var warning in _validationContext.GetBusinessRuleWarnings())
                    {
                        elements.Add(warning.Message);
                    }

                }
            }
            else
            {
                lock (_validationContext)
                {
                    foreach (var warning in _validationContext.GetFieldWarnings(propertyName))
                    {
                        elements.Add(warning.Message);
                    }
                }
            }

            return elements;
        }
        #endregion

        #region Notifications
        /// <summary>
        /// Raises the right events based on the validation result.
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        /// <param name="notifyGlobal">If set to <c>true</c>, the global properties such as <see cref="INotifyDataErrorInfo.HasErrors" /> and <see cref="INotifyDataWarningInfo.HasWarnings" /> are also raised.</param>
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