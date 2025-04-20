namespace Catel.Tests.Data
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    using Catel.Data;
    using Catel.IoC;
    using Catel.Reflection;

    using NUnit.Framework;
    using TestClasses;

    public partial class ValidatableModelBaseFacts
    {
        [TestFixture]
        public class OldStyleUnitTests
        {
            [TestCase("IsDirty", true)]
            public void DoesNotCauseValidationWhenKnownModelBasePropertiesChange(string propertyName, object propertyValue)
            {
                var validationObject = (IValidatable)new ObjectWithValidation();

                validationObject.Validate();

                Assert.That(validationObject.IsValidated, Is.True);

                var modelEditor = (IModelEditor)validationObject;
                modelEditor.SetValue(propertyName, propertyValue);

                Assert.That(validationObject.IsValidated, Is.True);
            }

            [Test]
            public void PropertyData_Contains_True_For_Validation_Attributes_Value()
            {
                var validationObject = new ObjectWithValidation();

                var propertyDataManager = PropertyDataManager.Default.GetCatelTypeInfo(typeof(ObjectWithValidation));

                var propertyData = propertyDataManager.GetPropertyData(nameof(ObjectWithValidation.ValueWithAnnotations));

                Assert.That(propertyData.IsDecoratedWithValidationAttributes, Is.True);
            }

            [Test]
            public void PropertyData_Contains_False_For_Non_Validation_Attributes_Value()
            {
                var validationObject = new ObjectWithValidation();

                var propertyDataManager = PropertyDataManager.Default.GetCatelTypeInfo(typeof(ObjectWithValidation));

                var propertyData = propertyDataManager.GetPropertyData(nameof(ObjectWithValidation.ValueWithoutAnnotations));

                Assert.That(propertyData.IsDecoratedWithValidationAttributes, Is.False);
            }

            [Test, Explicit]
            public void Skips_Data_Annotation_For_Values_Not_Decorated_With_Attributes()
            {
                var validationObject = new ObjectWithValidation();

                validationObject.Validate(true);

                // Note: there is no good way to validate, so this test is set to explicit
                Assert.That(ValidatableModelBase.PropertiesNotCausingValidation[typeof(ObjectWithValidation)].Contains(nameof(ObjectWithValidation.ValueWithoutAnnotations)));
            }

            #region Validation
            [TestCase]
            public void ValidationWithWarnings()
            {
                var validationObject = new ObjectWithValidation();
                var validation = (IValidatableModel)validationObject;

                // Check if the object now has warnings
                Assert.That(validation.HasWarnings, Is.EqualTo(false));
                Assert.That(validation.HasErrors, Is.EqualTo(false));

                // Now set a field warning and check it
                validationObject.ValueToValidate = ObjectWithValidation.ValueThatCausesFieldWarning;
                Assert.That(validation.HasWarnings, Is.EqualTo(true));
                Assert.That(validation.HasErrors, Is.EqualTo(false));

                // Now set a business warning and check it
                validationObject.ValueToValidate = ObjectWithValidation.ValueThatCausesBusinessWarning;
                Assert.That(validation.HasWarnings, Is.EqualTo(true));
                Assert.That(validation.HasErrors, Is.EqualTo(false));

                // Clear warning
                validationObject.ValueToValidate = ObjectWithValidation.ValueThatHasNoWarningsOrErrors;
                Assert.That(validation.HasWarnings, Is.EqualTo(false));
                Assert.That(validation.HasErrors, Is.EqualTo(false));
            }

            [TestCase]
            public void ValidationUsingAnnotationsForCatelProperties()
            {
                var validationObject = new ObjectWithValidation();
                var validation = (IValidatableModel)validationObject;

                Assert.That(validation.HasErrors, Is.False);

                validationObject.ValueWithAnnotations = string.Empty;

                Assert.That(validation.HasErrors, Is.True);

                validationObject.ValueWithAnnotations = "value";

                Assert.That(validation.HasErrors, Is.False);
            }

            //[TestCase]
            //public void ValidationUsingAnnotationsForNonCatelProperties()
            //{
            //    var validationObject = new ObjectWithValidation();

            //    Assert.IsFalse(validationObject.HasErrors);

            //    validationObject.NonCatelPropertyWithAnnotations = string.Empty;
            //    validationObject.Validate(true);

            //    Assert.IsTrue(validationObject.HasErrors);

            //    validationObject.NonCatelPropertyWithAnnotations = "value";
            //    validationObject.Validate(true);

            //    Assert.IsFalse(validationObject.HasErrors);
            //}  

            [TestCase]
            public void ValidationUsingAnnotationsForNonCatelCalculatedProperties()
            {
                var validationObject = new ObjectWithValidation();

                Assert.That(validationObject.HasErrors, Is.False);
            }
            #endregion

            #region IDataErrorInfo tests
            [TestCase]
            public void IDataErrorInfo_FieldWithError()
            {
                var obj = new ValidationTestModel();
                var validation = (IValidatableModel)obj;

                obj.ErrorWhenEmpty = string.Empty;

                Assert.That(validation.HasErrors, Is.True);
                Assert.That(string.IsNullOrEmpty(((IDataErrorInfo)obj)["ErrorWhenEmpty"]), Is.False);

                obj.ErrorWhenEmpty = "undo";

                Assert.That(validation.HasErrors, Is.False);
                Assert.That(string.IsNullOrEmpty(((IDataErrorInfo)obj)["ErrorWhenEmpty"]), Is.True);
            }
            #endregion

            #region INotifyDataErrorInfo tests
            [TestCase]
            public void INotifyDataErrorInfo_FieldWithError()
            {
                var obj = new ValidationTestModel();
                var validation = (IValidatableModel)obj;
                bool isInvoked = false;
                int count = 0;

                ((INotifyDataErrorInfo)obj).ErrorsChanged += (sender, e) =>
                {
                    Assert.That(e.PropertyName, Is.EqualTo("ErrorWhenEmpty"));
                    isInvoked = true;
                };

                isInvoked = false;
                obj.ErrorWhenEmpty = string.Empty;

                Assert.That(validation.HasErrors, Is.True);
                Assert.That(isInvoked, Is.True);
                count = 0;
                foreach (string fieldInfo in ((INotifyDataErrorInfo)obj).GetErrors("ErrorWhenEmpty"))
                {
                    count++;
                    Assert.That(string.IsNullOrEmpty(fieldInfo), Is.False);
                }
                Assert.That(count, Is.EqualTo(1));

                isInvoked = false;
                obj.ErrorWhenEmpty = "undo";

                Assert.That(validation.HasErrors, Is.False);
                Assert.That(isInvoked, Is.True);
                count = 0;
                foreach (string fieldInfo in ((INotifyDataErrorInfo)obj).GetErrors("ErrorWhenEmpty"))
                {
                    count++;
                    Assert.That(string.IsNullOrEmpty(fieldInfo), Is.False);
                }
                Assert.That(count, Is.EqualTo(0));
            }

            [TestCase]
            public void INotifyDataErrorInfo_Null()
            {
                var obj = new ValidationTestModel();
                var validation = (IValidatableModel)obj;
                bool isInvoked = false;
                int count = 0;

                ((INotifyDataErrorInfo)obj).ErrorsChanged += (sender, e) =>
                {
                    Assert.That(e.PropertyName, Is.EqualTo(string.Empty));
                    isInvoked = true;
                };

                isInvoked = false;
                obj.BusinessRuleErrorWhenEmpty = string.Empty;

                Assert.That(validation.HasErrors, Is.True);
                Assert.That(isInvoked, Is.True);
                count = 0;
                foreach (string error in ((INotifyDataErrorInfo)obj).GetErrors(null))
                {
                    count++;
                    Assert.That(string.IsNullOrEmpty(error), Is.False);
                }
                Assert.That(count, Is.EqualTo(1));

                isInvoked = false;
                obj.BusinessRuleErrorWhenEmpty = "undo";

                Assert.That(validation.HasErrors, Is.False);
                Assert.That(isInvoked, Is.True);
                count = 0;
                foreach (string error in ((INotifyDataErrorInfo)obj).GetErrors(null))
                {
                    count++;
                    Assert.That(string.IsNullOrEmpty(error), Is.False);
                }
                Assert.That(count, Is.EqualTo(0));
            }

            [TestCase]
            public void INotifyDataErrorInfo_EmptyString()
            {
                var obj = new ValidationTestModel();
                var validation = (IValidatableModel)obj;
                bool isInvoked = false;
                int count = 0;

                ((INotifyDataErrorInfo)obj).ErrorsChanged += (sender, e) =>
                {
                    Assert.That(e.PropertyName, Is.EqualTo(string.Empty));
                    isInvoked = true;
                };

                isInvoked = false;
                obj.BusinessRuleErrorWhenEmpty = string.Empty;

                Assert.That(validation.HasErrors, Is.True);
                Assert.That(isInvoked, Is.True);
                count = 0;
                foreach (string error in ((INotifyDataErrorInfo)obj).GetErrors(string.Empty))
                {
                    count++;
                    Assert.That(string.IsNullOrEmpty(error), Is.False);
                }
                Assert.That(count, Is.EqualTo(1));

                isInvoked = false;
                obj.BusinessRuleErrorWhenEmpty = "undo";

                Assert.That(validation.HasErrors, Is.False);
                Assert.That(isInvoked, Is.True);
                count = 0;
                foreach (string error in ((INotifyDataErrorInfo)obj).GetErrors(string.Empty))
                {
                    count++;
                    Assert.That(string.IsNullOrEmpty(error), Is.False);
                }
                Assert.That(count, Is.EqualTo(0));
            }
            #endregion

            #region IDataWarningInfo tests
            [TestCase]
            public void IDataWarningInfo_FieldWithWarning()
            {
                var obj = new ValidationTestModel();
                var validation = (IValidatableModel)obj;

                obj.WarningWhenEmpty = string.Empty;

                Assert.That(validation.HasWarnings, Is.True);
                Assert.That(string.IsNullOrEmpty(((IDataWarningInfo)obj)["WarningWhenEmpty"]), Is.False);

                obj.WarningWhenEmpty = "undo";

                Assert.That(validation.HasWarnings, Is.False);
                Assert.That(string.IsNullOrEmpty(((IDataWarningInfo)obj)["WarningWhenEmpty"]), Is.True);
            }
            #endregion

            #region INotifyDataWarningInfo tests
            [TestCase]
            public void INotifyDataWarningInfo_FieldWithWarning()
            {
                var obj = new ValidationTestModel();
                var validation = (IValidatableModel)obj;
                bool isInvoked = false;
                int count = 0;

                ((INotifyDataWarningInfo)obj).WarningsChanged += (sender, e) =>
                {
                    Assert.That(e.PropertyName, Is.EqualTo("WarningWhenEmpty"));
                    isInvoked = true;
                };

                isInvoked = false;
                obj.WarningWhenEmpty = string.Empty;

                Assert.That(validation.HasWarnings, Is.True);
                Assert.That(isInvoked, Is.True);
                count = 0;
                foreach (string fieldInfo in ((INotifyDataWarningInfo)obj).GetWarnings("WarningWhenEmpty"))
                {
                    count++;
                    Assert.That(string.IsNullOrEmpty(fieldInfo), Is.False);
                }
                Assert.That(count, Is.EqualTo(1));

                isInvoked = false;
                obj.WarningWhenEmpty = "undo";

                Assert.That(validation.HasWarnings, Is.False);
                Assert.That(isInvoked, Is.True);
                count = 0;
                foreach (string fieldInfo in ((INotifyDataWarningInfo)obj).GetWarnings("WarningWhenEmpty"))
                {
                    count++;
                    Assert.That(string.IsNullOrEmpty(fieldInfo), Is.False);
                }
                Assert.That(count, Is.EqualTo(0));
            }

            [TestCase]
            public void INotifyDataWarningInfo_Null()
            {
                var obj = new ValidationTestModel();
                var validation = (IValidatableModel)obj;
                bool isInvoked = false;
                int count = 0;

                ((INotifyDataWarningInfo)obj).WarningsChanged += (sender, e) =>
                {
                    Assert.That(e.PropertyName, Is.EqualTo(string.Empty));
                    isInvoked = true;
                };

                isInvoked = false;
                obj.BusinessRuleWarningWhenEmpty = string.Empty;

                Assert.That(validation.HasWarnings, Is.True);
                Assert.That(isInvoked, Is.True);
                count = 0;
                foreach (string warning in ((INotifyDataWarningInfo)obj).GetWarnings(null))
                {
                    count++;
                    Assert.That(string.IsNullOrEmpty(warning), Is.False);
                }
                Assert.That(count, Is.EqualTo(1));

                isInvoked = false;
                obj.BusinessRuleWarningWhenEmpty = "undo";

                Assert.That(validation.HasWarnings, Is.False);
                Assert.That(isInvoked, Is.True);
                count = 0;
                foreach (string warning in ((INotifyDataWarningInfo)obj).GetWarnings(null))
                {
                    count++;
                    Assert.That(string.IsNullOrEmpty(warning), Is.False);
                }
                Assert.That(count, Is.EqualTo(0));
            }

            [TestCase]
            public void INotifyDataWarningInfo_EmptyString()
            {
                var obj = new ValidationTestModel();
                var validation = (IValidatableModel)obj;
                bool isInvoked = false;
                int count = 0;

                ((INotifyDataWarningInfo)obj).WarningsChanged += (sender, e) =>
                {
                    Assert.That(e.PropertyName, Is.EqualTo(string.Empty));
                    isInvoked = true;
                };

                isInvoked = false;
                obj.BusinessRuleWarningWhenEmpty = string.Empty;

                Assert.That(validation.HasWarnings, Is.True);
                Assert.That(isInvoked, Is.True);
                count = 0;
                foreach (string warning in ((INotifyDataWarningInfo)obj).GetWarnings(string.Empty))
                {
                    count++;
                    Assert.That(string.IsNullOrEmpty(warning), Is.False);
                }
                Assert.That(count, Is.EqualTo(1));

                isInvoked = false;
                obj.BusinessRuleWarningWhenEmpty = "undo";

                Assert.That(validation.HasWarnings, Is.False);
                Assert.That(isInvoked, Is.True);
                count = 0;
                foreach (string warning in ((INotifyDataWarningInfo)obj).GetWarnings(string.Empty))
                {
                    count++;
                    Assert.That(string.IsNullOrEmpty(warning), Is.False);
                }
                Assert.That(count, Is.EqualTo(0));
            }
            #endregion

            #region IValidator implementation
            [TestCase]
            public void IValidator_CheckIfEventsAreFired()
            {
                var validator = new TestValidator();

                var classWithValidator = new ClassWithValidator() as IValidatable;
                classWithValidator.Validator = validator;

                classWithValidator.Validate(true);

                Assert.That(validator.ValidateCount, Is.EqualTo(1));

                Assert.That(validator.BeforeValidationCount, Is.EqualTo(1));

                Assert.That(validator.BeforeValidateFieldsCount, Is.EqualTo(1));
                Assert.That(validator.ValidateFieldsCount, Is.EqualTo(1));
                Assert.That(validator.AfterValidateFieldsCount, Is.EqualTo(1));

                Assert.That(validator.BeforeValidateBusinessRulesCount, Is.EqualTo(1));
                Assert.That(validator.ValidateBusinessRulesCount, Is.EqualTo(1));
                Assert.That(validator.AfterValidateBusinessRulesCount, Is.EqualTo(1));

                Assert.That(validator.AfterValidationCount, Is.EqualTo(1));
            }

            [TestCase]
            public void IValidator_AddFieldErrors()
            {
                var validator = new TestValidator();

                var classWithValidator = new ClassWithValidator() as IValidatable;
                classWithValidator.Validator = validator;

                classWithValidator.Validate(true);

                var dataWarningInfo = (IDataWarningInfo)classWithValidator;
                var dataErrorInfo = (IDataErrorInfo)classWithValidator;

                Assert.That(dataWarningInfo[ClassWithValidator.WarningPropertyProperty.Name], Is.EqualTo("Warning"));
                Assert.That(dataErrorInfo[ClassWithValidator.ErrorPropertyProperty.Name], Is.EqualTo("Error"));
            }

            [TestCase]
            public void IValidator_AddBusinessRuleErrors()
            {
                var validator = new TestValidator();

                var classWithValidator = new ClassWithValidator() as IValidatable;
                classWithValidator.Validator = validator;

                classWithValidator.Validate(true);

                var dataErrorInfo = (IDataErrorInfo)classWithValidator;

                Assert.That(dataErrorInfo.Error, Is.EqualTo("Error"));
            }
            #endregion
        }

        [TestFixture]
        public class TheValidateModelAttribute
        {
            [ValidateModel(typeof(TestValidator))]
            public class TestValidatorModel : ValidatableModelBase
            {
                /// <summary>
                /// Gets or sets the property value.
                /// </summary>
                public string FirstName
                {
                    get { return GetValue<string>(FirstNameProperty); }
                    set { SetValue(FirstNameProperty, value); }
                }

                /// <summary>
                /// Register the FirstName property so it is known in the class.
                /// </summary>
                public static readonly IPropertyData FirstNameProperty = RegisterProperty("FirstName", string.Empty);

                /// <summary>
                /// Gets or sets the property value.
                /// </summary>
                public string LastName
                {
                    get { return GetValue<string>(LastNameProperty); }
                    set { SetValue(LastNameProperty, value); }
                }

                /// <summary>
                /// Register the LastName property so it is known in the class.
                /// </summary>
                public static readonly IPropertyData LastNameProperty = RegisterProperty("LastName", string.Empty);
            }

            public class TestValidator : ValidatorBase<TestValidatorModel>
            {
                protected override void ValidateFields(TestValidatorModel instance, List<IFieldValidationResult> validationResults)
                {
                    if (string.IsNullOrWhiteSpace(instance.FirstName))
                    {
                        validationResults.Add(FieldValidationResult.CreateError(TestValidatorModel.FirstNameProperty, "First name is required"));
                    }

                    if (string.IsNullOrWhiteSpace(instance.LastName))
                    {
                        validationResults.Add(FieldValidationResult.CreateError(TestValidatorModel.FirstNameProperty, "First name is required"));
                    }
                }
            }

            [TestCase]
            public void AutomaticallyCreatesValidator()
            {
                ServiceLocator.Default.RegisterType<IValidatorProvider, AttributeValidatorProvider>();
                var testValidatorModel = (IValidatable)new TestValidatorModel();

                Assert.That(testValidatorModel.Validator, Is.Not.Null);

                testValidatorModel.Validate(true);

                Assert.That(testValidatorModel.HasErrors, Is.True);
            }
        }

        [TestFixture]
        public class TheHideValidationResultsProperty
        {
            [TestCase]
            public void HidesTheFieldErrorsWhenTrue()
            {
                var obj = new ValidationTestModel();
                var validation = obj;
                obj.HideValidationResults = true;

                obj.ErrorWhenEmpty = string.Empty;

                Assert.That(validation.HasErrors, Is.False);
                Assert.That(((IDataErrorInfo)obj)["ErrorWhenEmpty"], Is.EqualTo(string.Empty));

                obj.HideValidationResults = false;

                Assert.That(((IDataErrorInfo)obj)["ErrorWhenEmpty"], Is.Not.EqualTo(string.Empty));
            }

            [TestCase]
            public void HidesTheBusinessRuleErrorsWhenTrue()
            {
                var obj = new ValidationTestModel();
                var validation = obj;
                obj.HideValidationResults = true;

                obj.BusinessRuleErrorWhenEmpty = string.Empty;

                Assert.That(validation.HasErrors, Is.False);
                Assert.That(((IDataErrorInfo)obj).Error, Is.EqualTo(string.Empty));

                obj.HideValidationResults = false;

                Assert.That(((IDataErrorInfo)obj).Error, Is.Not.EqualTo(string.Empty));
            }

            [TestCase]
            public void HidesTheFieldWarningsWhenTrue()
            {
                var obj = new ValidationTestModel();
                var validation = obj;
                obj.HideValidationResults = true;

                obj.WarningWhenEmpty = string.Empty;

                Assert.That(validation.HasWarnings, Is.False);
                Assert.That(((IDataWarningInfo)obj)["WarningWhenEmpty"], Is.EqualTo(string.Empty));

                obj.HideValidationResults = false;

                Assert.That(((IDataWarningInfo)obj)["WarningWhenEmpty"], Is.Not.EqualTo(string.Empty));
            }

            [TestCase]
            public void HidesTheBusinessRuleWarningsWhenTrue()
            {
                var obj = new ValidationTestModel();
                var validation = obj;
                obj.HideValidationResults = true;

                obj.BusinessRuleWarningWhenEmpty = string.Empty;

                Assert.That(validation.HasWarnings, Is.False);
                Assert.That(((IDataWarningInfo)obj).Warning, Is.EqualTo(string.Empty));

                obj.HideValidationResults = false;

                Assert.That(((IDataWarningInfo)obj).Warning, Is.Not.EqualTo(string.Empty));
            }
        }

        [TestFixture]
        public class TheSuspendValidationProperty
        {
            public class SuspendValidationModel : ValidatableModelBase
            {
                [Required]
                public string FirstName
                {
                    get { return GetValue<string>(FirstNameProperty); }
                    set { SetValue(FirstNameProperty, value); }
                }

                public static readonly IPropertyData FirstNameProperty = RegisterProperty("FirstName", string.Empty);
            }

            [TestCase]
            public void PreventsAttributeBasedValidation()
            {
                var model = new SuspendValidationModel();
                var validation = model as IValidatable;

                Assert.That(validation.HasErrors, Is.False);

                using (model.SuspendValidations())
                {
                    model.FirstName = null;

                    Assert.That(validation.HasErrors, Is.False);
                }
            }

            [TestCase]
            public void CorrectlyValidatesUnvalidatedPropertiesWhenSetToTrue()
            {
                var model = new SuspendValidationModel();
                var validation = model;

                Assert.That(validation.HasErrors, Is.False);

                using (model.SuspendValidations())
                {
                    model.FirstName = null;

                    Assert.That(validation.HasErrors, Is.False);
                }

                Assert.That(validation.HasErrors, Is.True);
            }
        }

        [TestFixture]
        public class TheSuspendValidationsFacts
        {
            [TestCase]
            public void CorrectlyValidates()
            {
                var model = new SuspendableTestModel();

                model.Validate(true, true);

                var validationContext = model.GetValidationContext();
                var errors = validationContext.GetErrors();

                Assert.That(validationContext.HasErrors, Is.True);
                Assert.That(((FieldValidationResult)errors[0]).PropertyName, Is.EqualTo("FirstName"));
                Assert.That(((FieldValidationResult)errors[1]).PropertyName, Is.EqualTo("LastName"));
            }

            [TestCase]
            public void SuspendsValidationsAndValidatesOnResume()
            {
                var model = new SuspendableTestModel();

                using (model.SuspendValidations())
                {
                    model.Validate(true, true);

                    var innerValidationContext = model.GetValidationContext();

                    Assert.That(innerValidationContext.HasErrors, Is.False);
                }

                var validationContext = model.GetValidationContext();
                var errors = validationContext.GetErrors();

                Assert.That(validationContext.HasErrors, Is.True);
                Assert.That(((FieldValidationResult)errors[0]).PropertyName, Is.EqualTo("FirstName"));
                Assert.That(((FieldValidationResult)errors[1]).PropertyName, Is.EqualTo("LastName"));
            }

            [TestCase]
            public void SuspendsValidationsAndValidatesOnResumeWithScopes()
            {
                var model = new SuspendableTestModel();

                using (model.SuspendValidations())
                {
                    using (model.SuspendValidations())
                    {
                        model.Validate(true, true);

                        var innerValidationContext1 = model.GetValidationContext();

                        Assert.That(innerValidationContext1.HasErrors, Is.False);
                    }

                    var innerValidationContext2 = model.GetValidationContext();

                    Assert.That(innerValidationContext2.HasErrors, Is.False);
                }

                var validationContext = model.GetValidationContext();
                var errors = validationContext.GetErrors();

                Assert.That(validationContext.HasErrors, Is.True);
                Assert.That(((FieldValidationResult)errors[0]).PropertyName, Is.EqualTo("FirstName"));
                Assert.That(((FieldValidationResult)errors[1]).PropertyName, Is.EqualTo("LastName"));
            }
        }

        // Test case for https://catelproject.atlassian.net/browse/CTL-246
        [TestFixture]
        public class ValidationOfNonCatelProperties
        {
            public class ModelWithCalculatedPropertiesValidation : ValidatableModelBase
            {
                [System.ComponentModel.DataAnnotations.Range(1, 10)]
                public int Weight
                {
                    get { return 2 * 6; }
                }
            }

            [TestCase]
            public void CorrectlyValidatesNonCatelProperties()
            {
                var model = new ModelWithCalculatedPropertiesValidation();
                model.Validate(true);

                Assert.That(model.HasErrors, Is.True);

                var validationContext = model.GetValidationContext();
                var errors = validationContext.GetErrors();

                Assert.That(errors.Count, Is.EqualTo(1));
                Assert.That(((FieldValidationResult)errors[0]).PropertyName, Is.EqualTo("Weight"));
            }
        }

        [TestFixture]
        public class IgnoreDataAnnotationValidation
        {
            /// <summary>
            /// On each property access increment counter - way to check 
            /// how many times property was requested to check annotation validation,
            /// because every annotation validation get property value.
            /// </summary>
            public class ModelWithoutAnnotation : ValidatableModelBase
            {
                private int _counter;

                public int Counter { get { return _counter++; } }

                public void SetValidateUsingDataAnnotations(bool value)
                {
                    ValidateUsingDataAnnotations = value;
                }

                public new void Validate(bool force, bool useAnnotations)
                {
                    base.Validate(force, useAnnotations);
                }

                public new void Validate(bool force)
                {
                    base.Validate(force);
                }
            }

            [TestCase]
            public void OnMethodParamIgnoreDataAnnotationSkipAnnotationValidation()
            {
                var model = new ModelWithoutAnnotation();

                // validate model without data annotations
                model.Validate(true, false);

                Assert.That(model.Counter, Is.EqualTo(0));
            }

            [TestCase]
            public void OnInstancePropertyIgnoreDataAnnotationSkipAnnotationValidation()
            {
                // Set intance property to skip data annotations validation
                var model = new ModelWithoutAnnotation();
                model.SetValidateUsingDataAnnotations(false);

                model.Validate(true);

                Assert.That(model.Counter, Is.EqualTo(0));
            }

            [TestCase]
            public void OnStaticPropertyIgnoreDataAnnotationSkipAnnotationValidation()
            {
                // store original value
                var oldValue = ValidatableModelBase.DefaultValidateUsingDataAnnotationsValue;

                ValidatableModelBase.DefaultValidateUsingDataAnnotationsValue = false;
                var model = new ModelWithoutAnnotation();

                model.Validate(true);

                // store original value
                ValidatableModelBase.DefaultValidateUsingDataAnnotationsValue = oldValue;

                Assert.That(model.Counter, Is.EqualTo(0));
            }

            [TestCase]
            public void ByDefaultValidateDataAnnotation()
            {
                // By default instance property set to check annotation validation
                var model = new ModelWithoutAnnotation();

                model.Validate(true);

                Assert.That(model.Counter, Is.EqualTo(1));
            }
        }

        [TestFixture]
        public class IgnoreDataAnnotationValidationOnSetValue
        {
            public class ModelWithoutAnnotation : ValidatableModelBase
            {
                public static readonly IPropertyData CounterProperty = RegisterProperty<ModelWithoutAnnotation, int>(model => model.Counter);

                public int Counter
                {
                    get { return GetValue<int>(CounterProperty); }

                    set { SetValue(CounterProperty, value); }
                }

                public void SetValidateUsingDataAnnotations(bool value)
                {
                    ValidateUsingDataAnnotations = value;
                }

                public bool HasNotValidatedProperties()
                {
                    var t = typeof(ValidatableModelBase);
                    var vscf = t.GetFieldEx("_validationSuspensionContext", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                    var vsc = vscf.GetValue(this) as SuspensionContext;

                    return vsc.Properties.Count() != 0;
                }
            }

            [TestCase]
            public void OnInstancePropertyIgnoreDataAnnotationSkipAnnotationValidation()
            {
                // Set intance property to skip data annotations validation
                var model = new ModelWithoutAnnotation();

                using (model.SuspendValidations())
                {
                    model.SetValidateUsingDataAnnotations(false);

                    model.Counter = 1;

                    Assert.That(model.HasNotValidatedProperties(), Is.EqualTo(false));
                }
            }

            [TestCase]
            public void ByDefaultValidateDataAnnotationOnSetValue()
            {
                // By default instance property set to check annotation validation
                var model = new ModelWithoutAnnotation();

                using (model.SuspendValidations())
                {
                    model.Counter = 1;

                    Assert.That(model.HasNotValidatedProperties(), Is.EqualTo(true));
                }
            }
        }
    }
}
