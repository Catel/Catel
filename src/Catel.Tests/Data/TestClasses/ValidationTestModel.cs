namespace Catel.Tests.Data;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Catel.Data;

/// <summary>
/// ValidationTest Data object class which fully supports serialization, property changed notifications,
/// backwards compatibility and error checking.
/// </summary>
public class ValidationTestModel : ValidatableModelBase
{
    public const string ValueThatHasNoWarningsOrErrors = "NoWarningsOrErrors";
    public const string ValueThatCausesFieldWarning = "FieldWarning";
    public const string ValueThatCausesBusinessWarning = "BusinessWarning";
    public const string ValueThatCausesFieldError = "FieldError";
    public const string ValueThatCausesBusinessError = "BusinessError";

    /// <summary>
    ///   Initializes a new object from scratch.
    /// </summary>
    public ValidationTestModel()
    {
        ErrorWhenEmpty = "noerror";
        WarningWhenEmpty = "nowarning";
        BusinessRuleErrorWhenEmpty = "noerror";
        BusinessRuleWarningWhenEmpty = "noerror";
    }

    public new bool AutomaticallyValidateOnPropertyChanged
    {
        get { return base.AutomaticallyValidateOnPropertyChanged; }
        set { base.AutomaticallyValidateOnPropertyChanged = value; }
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
    /// Gets or sets the object with annotation validation.
    /// </summary>
    public string ValueWithoutAnnotations
    {
        get { return GetValue<string>(ValueWithoutAnnotations); }
        set { SetValue(ValueWithoutAnnotations, value); }
    }

    /// <summary>
    /// Register the ValueWithAnnotations property so it is known in the class.
    /// </summary>
    public static readonly IPropertyData ValueWithoutAnnotationsProperty = RegisterProperty("ValueWithoutAnnotations", "value");

    /// <summary>
    ///   Gets or sets field that returns an error when empty.
    /// </summary>
    public string ErrorWhenEmpty
    {
        get { return GetValue<string>(ErrorWhenEmptyProperty); }
        set { SetValue(ErrorWhenEmptyProperty, value); }
    }

    /// <summary>
    ///   Register the ErrorWhenEmpty property so it is known in the class.
    /// </summary>
    public static readonly IPropertyData ErrorWhenEmptyProperty = RegisterProperty("ErrorWhenEmpty", string.Empty);

    /// <summary>
    ///   Gets or sets field that returns a warning when empty.
    /// </summary>
    public string WarningWhenEmpty
    {
        get { return GetValue<string>(WarningWhenEmptyProperty); }
        set { SetValue(WarningWhenEmptyProperty, value); }
    }

    /// <summary>
    ///   Register the WarningWhenEmpty property so it is known in the class.
    /// </summary>
    public static readonly IPropertyData WarningWhenEmptyProperty = RegisterProperty("WarningWhenEmpty", string.Empty);

    /// <summary>
    ///   Gets or sets field that returns a business rule error when empty.
    /// </summary>
    public string BusinessRuleErrorWhenEmpty
    {
        get { return GetValue<string>(BusinessRuleErrorWhenEmptyProperty); }
        set { SetValue(BusinessRuleErrorWhenEmptyProperty, value); }
    }

    /// <summary>
    ///   Register the BusinessRuleErrorWhenEmpty property so it is known in the class.
    /// </summary>
    public static readonly IPropertyData BusinessRuleErrorWhenEmptyProperty = RegisterProperty("BusinessRuleErrorWhenEmpty", string.Empty);

    /// <summary>
    ///   Gets or sets field that returns a business rule warning when empty.
    /// </summary>
    public string BusinessRuleWarningWhenEmpty
    {
        get { return GetValue<string>(BusinessRuleWarningWhenEmptyProperty); }
        set { SetValue(BusinessRuleWarningWhenEmptyProperty, value); }
    }

    /// <summary>
    ///   Register the BusinessRuleWarningWhenEmpty property so it is known in the class.
    /// </summary>
    public static readonly IPropertyData BusinessRuleWarningWhenEmptyProperty = RegisterProperty("BusinessRuleWarningWhenEmpty", string.Empty);

    public new bool HideValidationResults
    {
        get { return base.HideValidationResults; }
        set { base.HideValidationResults = value; }
    }

    protected override void ValidateFields(List<IFieldValidationResult> validationResults)
    {
        if (string.IsNullOrEmpty(ErrorWhenEmpty))
        {
            validationResults.Add(FieldValidationResult.CreateError(ErrorWhenEmptyProperty, "Cannot be empty"));
        }

        if (string.IsNullOrEmpty(WarningWhenEmpty))
        {
            validationResults.Add(FieldValidationResult.CreateWarning(WarningWhenEmptyProperty, "Should not be empty"));
        }

        if (ValueToValidate == ValueThatCausesFieldWarning)
        {
            validationResults.Add(FieldValidationResult.CreateWarning(ValueToValidateProperty, "Field warning"));
        }

        if (ValueToValidate == ValueThatCausesFieldError)
        {
            validationResults.Add(FieldValidationResult.CreateError(ValueToValidateProperty, "Field error"));
        }
    }

    protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
    {
        if (string.IsNullOrEmpty(BusinessRuleErrorWhenEmpty))
        {
            validationResults.Add(BusinessRuleValidationResult.CreateError("BusinessRuleErrorWhenEmpty should not be empty"));
        }

        if (string.IsNullOrEmpty(BusinessRuleWarningWhenEmpty))
        {
            validationResults.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarningWhenEmpty should not be empty"));
        }

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
