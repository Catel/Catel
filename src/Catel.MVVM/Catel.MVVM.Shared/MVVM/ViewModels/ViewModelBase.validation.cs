// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBase.validation.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Catel.Data;
    using Catel.Reflection;
    using Logging;

#if NET40 || SILVERLIGHT && !WINDOWS_PHONE
    using System.ComponentModel.DataAnnotations;
#endif

    public partial class ViewModelBase
    {
        #region Fields
        /// <summary>
        /// Dictionary of properties that are decorated with the <see cref="ValidationToViewModelAttribute"/>. These properties should be
        /// updated after each validation sequence.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly Dictionary<string, ValidationToViewModelAttribute> _validationSummaries = new Dictionary<string, ValidationToViewModelAttribute>();

        /// <summary>
        /// A date/time with the latest update stamp of each validation summary.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly Dictionary<string, long> _validationSummariesUpdateStamps = new Dictionary<string, long>();
        #endregion

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
        /// Validates the specified notify changed properties only.
        /// </summary>
        /// <param name="force">If set to <c>true</c>, a validation is forced (even if the object knows it is already validated).</param>
        /// <param name="notifyChangedPropertiesOnly">if set to <c>true</c> only the properties for which the warnings or errors have been changed
        /// will be updated via <see cref="INotifyPropertyChanged.PropertyChanged"/>; otherwise all the properties that
        /// had warnings or errors but not anymore and properties still containing warnings or errors will be updated.</param>
        /// <returns>
        /// <c>true</c> if validation succeeds; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method is useful when the view model is initialized before the window, and therefore WPF does not update the errors and warnings.
        /// </remarks>
        public bool ValidateViewModel(bool force = false, bool notifyChangedPropertiesOnly = true)
        {
            if (IsClosed)
            {
                return true;
            }

            if (SuspendValidation)
            {
                return true;
            }

            Validate(force, notifyChangedPropertiesOnly);

            if (DeferValidationUntilFirstSaveCall)
            {
                return true;
            }

            return !HasErrors;
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

                    var modelValueAsModelBaseBase = model.Value as IModelValidation;
                    if (modelValueAsModelBaseBase != null)
                    {
                        modelValueAsModelBaseBase.Validate();
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
                    childViewModel.ValidateViewModel();
                    if (((INotifyDataErrorInfo)childViewModel).HasErrors)
                    {
                        _childViewModelsHaveErrors = true;
                        RaisePropertyChanged(() => HasErrors);
                    }
                }

                if (!_childViewModelsHaveErrors && (_childViewModelsHaveErrors != previousValue))
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
                var model = GetValue(mapping.ModelProperty);
                string[] modelProperties = mapping.ValueProperties;

                foreach (var modelProperty in modelProperties)
                {
                    bool hasSetFieldError = false;
                    bool hasSetFieldWarning = false;

                    // IDataErrorInfo
                    var dataErrorInfo = model as IDataErrorInfo;
                    if (dataErrorInfo != null)
                    {
                        if (!string.IsNullOrEmpty(dataErrorInfo[modelProperty]))
                        {
                        validationContext.AddFieldValidationResult(FieldValidationResult.CreateError(mapping.ViewModelProperty, dataErrorInfo[modelProperty]));

                            hasSetFieldError = true;
                        }
                    }

                    // IDataWarningInfo
                    var dataWarningInfo = model as IDataWarningInfo;
                    if (dataWarningInfo != null)
                    {
                        if (!string.IsNullOrEmpty(dataWarningInfo[modelProperty]))
                        {
                        validationContext.AddFieldValidationResult(FieldValidationResult.CreateWarning(mapping.ViewModelProperty, dataWarningInfo[modelProperty]));

                            hasSetFieldWarning = true;
                        }
                    }

                    // INotifyDataErrorInfo & INotifyDataWarningInfo
                    if (_modelErrorInfo.ContainsKey(mapping.ModelProperty))
                    {
                        var modelErrorInfo = _modelErrorInfo[mapping.ModelProperty];

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
                    // IDataErrorInfo
                    var dataErrorInfo = modelObject.Value as IDataErrorInfo;
                    if ((dataErrorInfo != null) && !string.IsNullOrEmpty(dataErrorInfo.Error))
                    {
                        validationContext.AddBusinessRuleValidationResult(BusinessRuleValidationResult.CreateError(dataErrorInfo.Error));
                    }

                    // IDataWarningInfo
                    var dataWarningInfo = modelObject.Value as IDataWarningInfo;
                    if ((dataWarningInfo != null) && !string.IsNullOrEmpty(dataWarningInfo.Warning))
                    {
                        validationContext.AddBusinessRuleValidationResult(BusinessRuleValidationResult.CreateWarning(dataWarningInfo.Warning));
                    }

                    // INotifyDataErrorInfo & INotifyDataWarningInfo
                    if (_modelErrorInfo.ContainsKey(modelObject.Key))
                    {
                        var modelErrorInfo = _modelErrorInfo[modelObject.Key];

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

        /// <summary>
        /// Called when the object is validated.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        protected override void OnValidated(IValidationContext validationContext)
        {
            bool updatedValidationSummaries = false;

            foreach (var validationSummaryInfo in _validationSummaries)
            {
                var isSummaryUpdateRequired = false;
                var lastUpdated = _validationSummariesUpdateStamps.ContainsKey(validationSummaryInfo.Key) ? _validationSummariesUpdateStamps[validationSummaryInfo.Key] : 0L;
                
                isSummaryUpdateRequired = this.IsValidationSummaryOutdated(lastUpdated, validationSummaryInfo.Value.IncludeChildViewModels);
                if (!isSummaryUpdateRequired)
                {
                    continue;
                }

                IValidationSummary validationSummary;
                if (validationSummaryInfo.Value.UseTagToFilter)
                {
                    validationSummary = this.GetValidationSummary(validationSummaryInfo.Value.IncludeChildViewModels, validationSummaryInfo.Value.Tag);
                }
                else
                {
                    validationSummary = this.GetValidationSummary(validationSummaryInfo.Value.IncludeChildViewModels);
                }

                PropertyHelper.SetPropertyValue(this, validationSummaryInfo.Key, validationSummary, false);
                _validationSummariesUpdateStamps[validationSummaryInfo.Key] = validationSummary.LastModifiedTicks;

                updatedValidationSummaries = true;
            }

            if (updatedValidationSummaries)
            {
                ViewModelCommandManager.InvalidateCommands();
            }

            base.OnValidated(validationContext);
        }
        #endregion
    }
}