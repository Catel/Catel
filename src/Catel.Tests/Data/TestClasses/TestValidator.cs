namespace Catel.Test.Data
{
    using System.Collections.Generic;
    using Catel.Data;

    public class TestValidator : ValidatorBase<ClassWithValidator>
    {
        public int ValidateCount { get; private set; }

        protected override void Validate(ClassWithValidator instance, Catel.Data.ValidationContext validationContext)
        {
            ValidateCount++;
        }

        public int BeforeValidationCount { get; private set; }

        protected override void BeforeValidation(ClassWithValidator instance, List<IFieldValidationResult> previousFieldValidationResults, List<IBusinessRuleValidationResult> previousBusinessRuleValidationResults)
        {
            BeforeValidationCount++;
        }

        public int BeforeValidateFieldsCount { get; private set; }

        protected override void BeforeValidateFields(ClassWithValidator instance, List<IFieldValidationResult> previousValidationResults)
        {
            BeforeValidateFieldsCount++;
        }

        public int ValidateFieldsCount { get; private set; }

        protected override void ValidateFields(ClassWithValidator instance, List<IFieldValidationResult> validationResults)
        {
            ValidateFieldsCount++;

            validationResults.Add(FieldValidationResult.CreateWarning(ClassWithValidator.WarningPropertyProperty, "Warning"));
            validationResults.Add(FieldValidationResult.CreateError(ClassWithValidator.ErrorPropertyProperty, "Error"));
        }

        public int AfterValidateFieldsCount { get; private set; }

        protected override void AfterValidateFields(ClassWithValidator instance, List<IFieldValidationResult> validationResults)
        {
            AfterValidateFieldsCount++;
        }

        public int BeforeValidateBusinessRulesCount { get; private set; }

        protected override void BeforeValidateBusinessRules(ClassWithValidator instance, List<IBusinessRuleValidationResult> previousValidationResults)
        {
            BeforeValidateBusinessRulesCount++;
        }

        public int ValidateBusinessRulesCount { get; private set; }

        protected override void ValidateBusinessRules(ClassWithValidator instance, List<IBusinessRuleValidationResult> validationResults)
        {
            ValidateBusinessRulesCount++;

            validationResults.Add(BusinessRuleValidationResult.CreateError("Error"));
        }

        public int AfterValidateBusinessRulesCount { get; private set; }

        protected override void AfterValidateBusinessRules(ClassWithValidator instance, List<IBusinessRuleValidationResult> validationResults)
        {
            AfterValidateBusinessRulesCount++;
        }

        public int AfterValidationCount { get; private set; }

        protected override void AfterValidation(ClassWithValidator instance, List<IFieldValidationResult> fieldValidationResults, List<IBusinessRuleValidationResult> businessRuleValidationResults)
        {
            AfterValidationCount++;
        }
    }
}