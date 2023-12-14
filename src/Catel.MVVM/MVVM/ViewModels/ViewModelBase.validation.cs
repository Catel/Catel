namespace Catel.MVVM
{
    using System.ComponentModel;
    using System.Linq;
    using Data;

    public partial class ViewModelBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether all validation should be deferred until the first call to <see cref="SaveViewModelAsync"/>.
        /// <para />
        /// If this value is <c>true</c>, all validation will be suspended. As soon as the first call is made to the <see cref="SaveViewModelAsync"/>,
        /// the validation will no longer be suspended and activated.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the validation should be deferred; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// If this value is used, it must be set as first property in the view model because the validation kicks in immediately
        /// when properties change.
        /// </remarks>
        [ExcludeFromValidation]
        protected bool DeferValidationUntilFirstSaveCall
        {
            get
            {
                return HideValidationResults;
            }
            set
            {
                HideValidationResults = value;

                foreach (var childViewModel in ChildViewModels.ToList())
                {
                    var childVm = childViewModel as ViewModelBase;
                    if (childVm is not null)
                    {
                        childVm.DeferValidationUntilFirstSaveCall = DeferValidationUntilFirstSaveCall;
                    }
                }

                RaisePropertyChanged(string.Empty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the models as soon as they are initialized. This means that
        /// as soon as a model value is set, the view model checks whether the entity already contains errors.
        /// <para />
        /// If this value is <c>true</c>, the errors will immediately be returned for mappings on the model. Otherwise, the errors
        /// will only become available when a value is entered and then being undone.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the models should be validated on initialization; otherwise, <c>false</c>.
        /// </value>
        [ExcludeFromValidation]
        protected bool ValidateModelsOnInitialization { get; set; }

        /// <summary>
        /// Validates the current object for field and business rule errors.
        /// </summary>
        /// <param name="force">If set to <c>true</c>, a validation is forced. When the validation is not forced, it means 
        /// that when the object is already validated, and no properties have been changed, no validation actually occurs 
        /// since there is no reason for any values to have changed.
        /// </param>
        /// <remarks>
        /// To check whether this object contains any errors, use the ValidationContext property.
        /// </remarks>
        public override void Validate(bool force = false)
        {
            if (IsClosed)
            {
                return;
            }

            base.Validate(force);
        }

        /// <summary>
        /// Called when the object is validating.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        protected override void OnValidating(IValidationContext validationContext)
        {
            base.OnValidating(validationContext);

            lock (_modelLock)
            {
                foreach (var modelKeyValuePair in _modelObjects)
                {
                    var model = modelKeyValuePair.Value;
                    if (model is null)
                    {
                        continue;
                    }

                    if (!_modelObjectsInfo[modelKeyValuePair.Key].SupportValidation)
                    {
                        continue;
                    }

                    var validatable = model as IValidatable;
                    if (validatable is not null)
                    {
                        validatable.Validate();
                    }
                }
            }

            lock (ChildViewModels)
            {
                var previousValue = _childViewModelsHaveErrors;

                _childViewModelsHaveErrors = false;

                var childViewModels = ChildViewModels.ToArray();
                foreach (var childViewModel in childViewModels)
                {
                    childViewModel.Validate();
                    if (childViewModel.HasErrors)
                    {
                        _childViewModelsHaveErrors = true;
                        RaisePropertyChanged(nameof(HasErrors));
                    }
                }

                if (!_childViewModelsHaveErrors && _childViewModelsHaveErrors != previousValue)
                {
                    RaisePropertyChanged(nameof(HasErrors));
                }
            }
        }

        /// <summary>
        /// Called when the object is validating the fields.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        protected override void OnValidatingFields(IValidationContext validationContext)
        {
            base.OnValidatingFields(validationContext);

            // Map all field errors and warnings from the model to this viewmodel
            foreach (var viewModelToModelMap in _viewModelToModelMap)
            {
                var mapping = viewModelToModelMap.Value;

                lock (_modelLock)
                {
                    if (!_modelObjectsInfo[mapping.ModelProperty].SupportValidation)
                    {
                        continue;
                    }
                }

                var model = GetValue<object>(mapping.ModelProperty);
                string[] modelProperties = mapping.ValueProperties;

                foreach (var modelProperty in modelProperties)
                {
                    bool hasSetFieldError = false;
                    bool hasSetFieldWarning = false;

                    // IDataErrorInfo
                    var dataErrorInfo = model as IDataErrorInfo;
                    if (dataErrorInfo is not null)
                    {
                        var error = dataErrorInfo[modelProperty];
                        if (!string.IsNullOrWhiteSpace(error))
                        {
                            validationContext.Add(FieldValidationResult.CreateError(mapping.ViewModelProperty, error));

                            hasSetFieldError = true;
                        }
                    }

                    // IDataWarningInfo
                    var dataWarningInfo = model as IDataWarningInfo;
                    if (dataWarningInfo is not null)
                    {
                        var warning = dataWarningInfo[modelProperty];
                        if (!string.IsNullOrWhiteSpace(warning))
                        {
                            validationContext.Add(FieldValidationResult.CreateWarning(mapping.ViewModelProperty, warning));

                            hasSetFieldWarning = true;
                        }
                    }

                    // INotifyDataErrorInfo & INotifyDataWarningInfo

                    if (_modelErrorInfo.TryGetValue(mapping.ModelProperty, out var modelErrorInfo))
                    {
                        if (!hasSetFieldError)
                        {
                            foreach (var error in modelErrorInfo.GetErrors(modelProperty))
                            {
                                if (!string.IsNullOrEmpty(error))
                                {
                                    validationContext.Add(FieldValidationResult.CreateError(mapping.ViewModelProperty, error));
                                    break;
                                }
                            }
                        }

                        if (!hasSetFieldWarning)
                        {
                            foreach (var warning in modelErrorInfo.GetWarnings(modelProperty))
                            {
                                if (!string.IsNullOrEmpty(warning))
                                {
                                    validationContext.Add(FieldValidationResult.CreateWarning(mapping.ViewModelProperty, warning));
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when the object is validating the business rules.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        protected override void OnValidatingBusinessRules(IValidationContext validationContext)
        {
            base.OnValidatingBusinessRules(validationContext);

            lock (_modelLock)
            {
                foreach (var modelObject in _modelObjects)
                {
                    if (!_modelObjectsInfo[modelObject.Key].SupportValidation)
                    {
                        continue;
                    }

                    // IDataErrorInfo
                    var dataErrorInfo = modelObject.Value as IDataErrorInfo;
                    if (dataErrorInfo is not null && !string.IsNullOrEmpty(dataErrorInfo.Error))
                    {
                        validationContext.Add(BusinessRuleValidationResult.CreateError(dataErrorInfo.Error));
                    }

                    // IDataWarningInfo
                    var dataWarningInfo = modelObject.Value as IDataWarningInfo;
                    if (dataWarningInfo is not null && !string.IsNullOrEmpty(dataWarningInfo.Warning))
                    {
                        validationContext.Add(BusinessRuleValidationResult.CreateWarning(dataWarningInfo.Warning));
                    }

                    // INotifyDataErrorInfo & INotifyDataWarningInfo
                    if (_modelErrorInfo.TryGetValue(modelObject.Key, out var modelErrorInfo))
                    {
                        foreach (var error in modelErrorInfo.GetErrors(string.Empty))
                        {
                            validationContext.Add(BusinessRuleValidationResult.CreateError(error));
                        }

                        foreach (var warning in modelErrorInfo.GetWarnings(string.Empty))
                        {
                            validationContext.Add(BusinessRuleValidationResult.CreateWarning(warning));
                        }
                    }
                }
            }
        }
    }
}
