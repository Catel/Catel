// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidatableParent.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Data.TestClasses
{
    using System.Collections.Generic;
    using Catel.Data;

    public class ValidatableParent : ChildAwareModelBase
    {
        public ValidatableChild Child
        {
            get => GetValue<ValidatableChild>(ChildProperty);
            set => SetValue(ChildProperty, value);
        }

        public static readonly PropertyData ChildProperty = RegisterProperty<ValidatableParent, ValidatableChild>(model => model.Child);

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
        }

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (this.Child == null)
            {
                validationResults.Add(FieldValidationResult.CreateError(nameof(Child), "F: There is no child"));
            }
        }
    }
}