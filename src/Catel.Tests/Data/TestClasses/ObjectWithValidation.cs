﻿namespace Catel.Tests.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Catel.Data;
    using Catel.Runtime.Serialization;

    [Serializable]
    public class ObjectWithValidation : ValidatableModelBase
    {
        public const string ValueThatHasNoWarningsOrErrors = "NoWarningsOrErrors";
        public const string ValueThatCausesFieldWarning = "FieldWarning";
        public const string ValueThatCausesBusinessWarning = "BusinessWarning";
        public const string ValueThatCausesFieldError = "FieldError";
        public const string ValueThatCausesBusinessError = "BusinessError";

        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ObjectWithValidation(IServiceProvider serviceProvider, Catel.Data.IObjectAdapter objectAdapter, ISerializer serializer)
            : base(serviceProvider, objectAdapter, serializer)
        {
            NonCatelPropertyWithAnnotations = "default value";
        }

        /// <summary>
        ///   Gets or sets the value to validate.
        /// </summary>
        public string ValueToValidate
        {
            get { return GetValue<string>(ValueToValidateProperty); }
            set { SetValue(ValueToValidateProperty, value); }
        }

        /// <summary>
        ///   Register the ValueToValidate property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData ValueToValidateProperty = RegisterProperty("ValueToValidate", ValueThatHasNoWarningsOrErrors);

        [Required(ErrorMessage = "Non-catel is required")]
        public string NonCatelPropertyWithAnnotations { get; set; }

        [Required(ErrorMessage = "Non-catel is required")]
        public string NonCatelCalculatedPropertyWithAnnotations { get { return "default value"; } }

        /// <summary>
        /// Gets or sets the object with annotation validation.
        /// </summary>
        [Required(ErrorMessage = "Field is required")]
        public string ValueWithAnnotations
        {
            get { return GetValue<string>(ValueWithAnnotationsProperty); }
            set { SetValue(ValueWithAnnotationsProperty, value); }
        }

        /// <summary>
        /// Register the ValueWithAnnotations property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData ValueWithAnnotationsProperty = RegisterProperty("ValueWithAnnotations", "value");

        /// <summary>
        ///   Validates the fields.
        /// </summary>
        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (ValueToValidate == ValueThatCausesFieldWarning)
            {
                validationResults.Add(FieldValidationResult.CreateWarning(ValueToValidateProperty, "Field warning"));
            }

            if (ValueToValidate == ValueThatCausesFieldError)
            {
                validationResults.Add(FieldValidationResult.CreateError(ValueToValidateProperty, "Field error"));
            }
        }

        /// <summary>
        ///   Validates the business rules.
        /// </summary>
        protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
        {
            if (ValueToValidate == ValueThatCausesBusinessWarning)
            {
                validationResults.Add(BusinessRuleValidationResult.CreateWarning("Business rule warning"));
            }

            if (ValueToValidate == ValueThatCausesBusinessError)
            {
                validationResults.Add(BusinessRuleValidationResult.CreateError("Business rule error"));
            }
        }
    }
}
