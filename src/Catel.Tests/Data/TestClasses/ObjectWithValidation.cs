namespace Catel.Test.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using Catel.Data;

#if NET
    [Serializable]
#endif
    public class ObjectWithValidation : ValidatableModelBase
    {
        #region Constants
        public const string ValueThatHasNoWarningsOrErrors = "NoWarningsOrErrors";
        public const string ValueThatCausesFieldWarning = "FieldWarning";
        public const string ValueThatCausesBusinessWarning = "BusinessWarning";
        public const string ValueThatCausesFieldError = "FieldError";
        public const string ValueThatCausesBusinessError = "BusinessError";
        #endregion

        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ObjectWithValidation()
        {
            NonCatelPropertyWithAnnotations = "default value";
        }

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected ObjectWithValidation(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
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
        public static readonly PropertyData ValueToValidateProperty = RegisterProperty("ValueToValidate", typeof(string), ValueThatHasNoWarningsOrErrors);

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
        public static readonly PropertyData ValueWithAnnotationsProperty = RegisterProperty("ValueWithAnnotations", typeof(string), "value");
        #endregion

        #region Methods
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
        #endregion
    }
}