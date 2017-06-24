// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBase.validation.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.MVVM
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Data;

    public partial class ViewModelBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether all validation should be deferred until the first call to <see cref="SaveViewModelAsync"/>.
        /// <para />
        /// If this value is <c>true</c>, all validation will be suspended. As soon as the first call is made to the <see cref="SaveViewModelAsync"/>,
        /// the validation will no longer be suspended and activated.
        /// <para />
        /// The default value is <c>false</c>.
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
                    if (childVm != null)
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
        #endregion

        #region Methods
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
                foreach (KeyValuePair<string, object> model in _modelObjects)
                {
                    if (model.Value == null)
                    {
                        continue;
                    }

                    if (!_modelObjectsInfo[model.Key].SupportValidation)
                    {
                        continue;
                    }

                    var validatable = model.Value as IValidatable;
                    if (validatable != null)
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
                        RaisePropertyChanged(() => HasErrors);
                    }
                }

                if (!_childViewModelsHaveErrors && _childViewModelsHaveErrors != previousValue)
                {
                    RaisePropertyChanged(() => HasErrors);
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

                var model = GetValue(mapping.ModelProperty);
                string[] modelProperties = mapping.ValueProperties;

                foreach (var modelProperty in modelProperties)
                {
                    bool hasSetFieldError = false;
                    bool hasSetFieldWarning = false;

                    // IDataErrorInfo
                    var dataErrorInfo = model as IDataErrorInfo;
                    if (dataErrorInfo != null && !string.IsNullOrEmpty(dataErrorInfo[modelProperty]))
                    {
                        validationContext.AddFieldValidationResult(FieldValidationResult.CreateError(mapping.ViewModelProperty, dataErrorInfo[modelProperty]));

                        hasSetFieldError = true;
                    }

                    // IDataWarningInfo
                    var dataWarningInfo = model as IDataWarningInfo;
                    if (dataWarningInfo != null && !string.IsNullOrEmpty(dataWarningInfo[modelProperty]))
                    {
                        validationContext.AddFieldValidationResult(FieldValidationResult.CreateWarning(mapping.ViewModelProperty, dataWarningInfo[modelProperty]));

                        hasSetFieldWarning = true;
                    }

                    // INotifyDataErrorInfo & INotifyDataWarningInfo

                    ModelErrorInfo modelErrorInfo;
                    if (_modelErrorInfo.TryGetValue(mapping.ModelProperty, out modelErrorInfo))
                    {
                        if (!hasSetFieldError)
                        {
                            foreach (string error in modelErrorInfo.GetErrors(modelProperty))
                            {
                                if (!string.IsNullOrEmpty(error))
                                {
                                    validationContext.AddFieldValidationResult(FieldValidationResult.CreateError(mapping.ViewModelProperty, error));
                                    break;
                                }
                            }
                        }

                        if (!hasSetFieldWarning)
                        {
                            foreach (string warning in modelErrorInfo.GetWarnings(modelProperty))
                            {
                                if (!string.IsNullOrEmpty(warning))
                                {
                                    validationContext.AddFieldValidationResult(FieldValidationResult.CreateWarning(mapping.ViewModelProperty, warning));
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
                    if (dataErrorInfo != null && !string.IsNullOrEmpty(dataErrorInfo.Error))
                    {
                        validationContext.AddBusinessRuleValidationResult(BusinessRuleValidationResult.CreateError(dataErrorInfo.Error));
                    }

                    // IDataWarningInfo
                    var dataWarningInfo = modelObject.Value as IDataWarningInfo;
                    if (dataWarningInfo != null && !string.IsNullOrEmpty(dataWarningInfo.Warning))
                    {
                        validationContext.AddBusinessRuleValidationResult(BusinessRuleValidationResult.CreateWarning(dataWarningInfo.Warning));
                    }

                    // INotifyDataErrorInfo & INotifyDataWarningInfo
                    ModelErrorInfo modelErrorInfo;
                    if (_modelErrorInfo.TryGetValue(modelObject.Key, out modelErrorInfo))
                    {
                        foreach (var error in modelErrorInfo.GetErrors(string.Empty))
                        {
                            validationContext.AddBusinessRuleValidationResult(BusinessRuleValidationResult.CreateError(error));
                        }

                        foreach (var warning in modelErrorInfo.GetWarnings(string.Empty))
                        {
                            validationContext.AddBusinessRuleValidationResult(BusinessRuleValidationResult.CreateWarning(warning));
                        }
                    }
                }
            }
        }

        #endregion
    }
}