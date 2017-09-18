// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidatableParent.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Data.TestClasses
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Catel.Data;

    public class ValidatableParent : ChildAwareModelBase
    {
        public ValidatableChild Child
        {
            get => GetValue<ValidatableChild>(ChildProperty);
            set => SetValue(ChildProperty, value);
        }

        public static readonly PropertyData ChildProperty = RegisterProperty<ValidatableParent, ValidatableChild>(model => model.Child);

        public ObservableCollection<ValidatableChild> Collection
        {
            get => GetValue<ObservableCollection<ValidatableChild>>(CollectionProperty);
            set => SetValue(CollectionProperty, value);
        }

        public static readonly PropertyData CollectionProperty = RegisterProperty<ValidatableParent, ObservableCollection<ValidatableChild>>(model => model.Collection);

        protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
        {
            if (this.Child != null)
            {
                var errors = this.Child.GetErrorMessage();
                if (errors.Length != 0)
                {
                    validationResults.Add(BusinessRuleValidationResult.CreateError(errors));
                }
            }

            if (this.Collection != null && this.Collection.Count != 0)
            {
                var errors = this.Collection[0].GetErrorMessage();
                if (errors.Length != 0)
                {
                    validationResults.Add(BusinessRuleValidationResult.CreateError(errors));
                }
            }
        }
    }
}