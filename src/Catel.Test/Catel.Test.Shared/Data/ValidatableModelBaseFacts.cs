// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBaseFacts.validation.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
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

                Assert.IsTrue(validationObject.IsValidated);

                var modelEditor = (IModelEditor) validationObject;
                modelEditor.SetValue(propertyName, propertyValue);

                Assert.IsTrue(validationObject.IsValidated);
            }

            #region Validation
            [TestCase]
            public void ValidationWithWarnings()
            {
                var validationObject = new ObjectWithValidation();
                var validation = (IValidatableModel)validationObject;

                // Check if the object now has warnings
                Assert.AreEqual(false, validation.HasWarnings);
                Assert.AreEqual(false, validation.HasErrors);

                // Now set a field warning and check it
                validationObject.ValueToValidate = ObjectWithValidation.ValueThatCausesFieldWarning;
                Assert.AreEqual(true, validation.HasWarnings);
                Assert.AreEqual(false, validation.HasErrors);

                // Now set a business warning and check it
                validationObject.ValueToValidate = ObjectWithValidation.ValueThatCausesBusinessWarning;
                Assert.AreEqual(true, validation.HasWarnings);
                Assert.AreEqual(false, validation.HasErrors);

                // Clear warning
                validationObject.ValueToValidate = ObjectWithValidation.ValueThatHasNoWarningsOrErrors;
                Assert.AreEqual(false, validation.HasWarnings);
                Assert.AreEqual(false, validation.HasErrors);
            }

            [TestCase]
            public void ValidationUsingAnnotationsForCatelProperties()
            {
                var validationObject = new ObjectWithValidation();
                var validation = (IValidatableModel)validationObject;

                Assert.IsFalse(validation.HasErrors);

                validationObject.ValueWithAnnotations = string.Empty;

                Assert.IsTrue(validation.HasErrors);

                validationObject.ValueWithAnnotations = "value";

                Assert.IsFalse(validation.HasErrors);
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

                Assert.IsFalse(validationObject.HasErrors);
            }
            #endregion

            #region IDataErrorInfo tests
            [TestCase]
            public void IDataErrorInfo_FieldWithError()
            {
                var obj = new ValidationTestModel();
                var validation = (IValidatableModel) obj;

                obj.ErrorWhenEmpty = string.Empty;

                Assert.IsTrue(validation.HasErrors);
                Assert.IsFalse(string.IsNullOrEmpty(((IDataErrorInfo)obj)["ErrorWhenEmpty"]));

                obj.ErrorWhenEmpty = "undo";

                Assert.IsFalse(validation.HasErrors);
                Assert.IsTrue(string.IsNullOrEmpty(((IDataErrorInfo)obj)["ErrorWhenEmpty"]));
            }
            #endregion

            #region INotifyDataErrorInfo tests
            [TestCase]
            public void INotifyDataErrorInfo_FieldWithError()
            {
                var obj = new ValidationTestModel();
                var validation = (IValidatableModel) obj;
                bool isInvoked = false;
                int count = 0;

                ((INotifyDataErrorInfo)obj).ErrorsChanged += (sender, e) =>
                {
                    Assert.AreEqual("ErrorWhenEmpty", e.PropertyName);
                    isInvoked = true;
                };

                isInvoked = false;
                obj.ErrorWhenEmpty = string.Empty;

                Assert.IsTrue(validation.HasErrors);
                Assert.IsTrue(isInvoked);
                count = 0;
                foreach (string fieldInfo in ((INotifyDataErrorInfo)obj).GetErrors("ErrorWhenEmpty"))
                {
                    count++;
                    Assert.IsFalse(string.IsNullOrEmpty(fieldInfo));
                }
                Assert.AreEqual(1, count);

                isInvoked = false;
                obj.ErrorWhenEmpty = "undo";

                Assert.IsFalse(validation.HasErrors);
                Assert.IsTrue(isInvoked);
                count = 0;
                foreach (string fieldInfo in ((INotifyDataErrorInfo)obj).GetErrors("ErrorWhenEmpty"))
                {
                    count++;
                    Assert.IsFalse(string.IsNullOrEmpty(fieldInfo));
                }
                Assert.AreEqual(0, count);
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
                    Assert.AreEqual(string.Empty, e.PropertyName);
                    isInvoked = true;
                };

                isInvoked = false;
                obj.BusinessRuleErrorWhenEmpty = string.Empty;

                Assert.IsTrue(validation.HasErrors);
                Assert.IsTrue(isInvoked);
                count = 0;
                foreach (string error in ((INotifyDataErrorInfo)obj).GetErrors(null))
                {
                    count++;
                    Assert.IsFalse(string.IsNullOrEmpty(error));
                }
                Assert.AreEqual(1, count);

                isInvoked = false;
                obj.BusinessRuleErrorWhenEmpty = "undo";

                Assert.IsFalse(validation.HasErrors);
                Assert.IsTrue(isInvoked);
                count = 0;
                foreach (string error in ((INotifyDataErrorInfo)obj).GetErrors(null))
                {
                    count++;
                    Assert.IsFalse(string.IsNullOrEmpty(error));
                }
                Assert.AreEqual(0, count);
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
                    Assert.AreEqual(string.Empty, e.PropertyName);
                    isInvoked = true;
                };

                isInvoked = false;
                obj.BusinessRuleErrorWhenEmpty = string.Empty;

                Assert.IsTrue(validation.HasErrors);
                Assert.IsTrue(isInvoked);
                count = 0;
                foreach (string error in ((INotifyDataErrorInfo)obj).GetErrors(string.Empty))
                {
                    count++;
                    Assert.IsFalse(string.IsNullOrEmpty(error));
                }
                Assert.AreEqual(1, count);

                isInvoked = false;
                obj.BusinessRuleErrorWhenEmpty = "undo";

                Assert.IsFalse(validation.HasErrors);
                Assert.IsTrue(isInvoked);
                count = 0;
                foreach (string error in ((INotifyDataErrorInfo)obj).GetErrors(string.Empty))
                {
                    count++;
                    Assert.IsFalse(string.IsNullOrEmpty(error));
                }
                Assert.AreEqual(0, count);
            }
            #endregion

            #region IDataWarningInfo tests
            [TestCase]
            public void IDataWarningInfo_FieldWithWarning()
            {
                var obj = new ValidationTestModel();
                var validation = (IValidatableModel)obj;

                obj.WarningWhenEmpty = string.Empty;

                Assert.IsTrue(validation.HasWarnings);
                Assert.IsFalse(string.IsNullOrEmpty(((IDataWarningInfo)obj)["WarningWhenEmpty"]));

                obj.WarningWhenEmpty = "undo";

                Assert.IsFalse(validation.HasWarnings);
                Assert.IsTrue(string.IsNullOrEmpty(((IDataWarningInfo)obj)["WarningWhenEmpty"]));
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
                    Assert.AreEqual("WarningWhenEmpty", e.PropertyName);
                    isInvoked = true;
                };

                isInvoked = false;
                obj.WarningWhenEmpty = string.Empty;

                Assert.IsTrue(validation.HasWarnings);
                Assert.IsTrue(isInvoked);
                count = 0;
                foreach (string fieldInfo in ((INotifyDataWarningInfo)obj).GetWarnings("WarningWhenEmpty"))
                {
                    count++;
                    Assert.IsFalse(string.IsNullOrEmpty(fieldInfo));
                }
                Assert.AreEqual(1, count);

                isInvoked = false;
                obj.WarningWhenEmpty = "undo";

                Assert.IsFalse(validation.HasWarnings);
                Assert.IsTrue(isInvoked);
                count = 0;
                foreach (string fieldInfo in ((INotifyDataWarningInfo)obj).GetWarnings("WarningWhenEmpty"))
                {
                    count++;
                    Assert.IsFalse(string.IsNullOrEmpty(fieldInfo));
                }
                Assert.AreEqual(0, count);
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
                    Assert.AreEqual(string.Empty, e.PropertyName);
                    isInvoked = true;
                };

                isInvoked = false;
                obj.BusinessRuleWarningWhenEmpty = string.Empty;

                Assert.IsTrue(validation.HasWarnings);
                Assert.IsTrue(isInvoked);
                count = 0;
                foreach (string warning in ((INotifyDataWarningInfo)obj).GetWarnings(null))
                {
                    count++;
                    Assert.IsFalse(string.IsNullOrEmpty(warning));
                }
                Assert.AreEqual(1, count);

                isInvoked = false;
                obj.BusinessRuleWarningWhenEmpty = "undo";

                Assert.IsFalse(validation.HasWarnings);
                Assert.IsTrue(isInvoked);
                count = 0;
                foreach (string warning in ((INotifyDataWarningInfo)obj).GetWarnings(null))
                {
                    count++;
                    Assert.IsFalse(string.IsNullOrEmpty(warning));
                }
                Assert.AreEqual(0, count);
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
                    Assert.AreEqual(string.Empty, e.PropertyName);
                    isInvoked = true;
                };

                isInvoked = false;
                obj.BusinessRuleWarningWhenEmpty = string.Empty;

                Assert.IsTrue(validation.HasWarnings);
                Assert.IsTrue(isInvoked);
                count = 0;
                foreach (string warning in ((INotifyDataWarningInfo)obj).GetWarnings(string.Empty))
                {
                    count++;
                    Assert.IsFalse(string.IsNullOrEmpty(warning));
                }
                Assert.AreEqual(1, count);

                isInvoked = false;
                obj.BusinessRuleWarningWhenEmpty = "undo";

                Assert.IsFalse(validation.HasWarnings);
                Assert.IsTrue(isInvoked);
                count = 0;
                foreach (string warning in ((INotifyDataWarningInfo)obj).GetWarnings(string.Empty))
                {
                    count++;
                    Assert.IsFalse(string.IsNullOrEmpty(warning));
                }
                Assert.AreEqual(0, count);
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

                Assert.AreEqual(1, validator.ValidateCount);

                Assert.AreEqual(1, validator.BeforeValidationCount);

                Assert.AreEqual(1, validator.BeforeValidateFieldsCount);
                Assert.AreEqual(1, validator.ValidateFieldsCount);
                Assert.AreEqual(1, validator.AfterValidateFieldsCount);

                Assert.AreEqual(1, validator.BeforeValidateBusinessRulesCount);
                Assert.AreEqual(1, validator.ValidateBusinessRulesCount);
                Assert.AreEqual(1, validator.AfterValidateBusinessRulesCount);

                Assert.AreEqual(1, validator.AfterValidationCount);
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

                Assert.AreEqual("Warning", dataWarningInfo[ClassWithValidator.WarningPropertyProperty.Name]);
                Assert.AreEqual("Error", dataErrorInfo[ClassWithValidator.ErrorPropertyProperty.Name]);
            }

            [TestCase]
            public void IValidator_AddBusinessRuleErrors()
            {
                var validator = new TestValidator();

                var classWithValidator = new ClassWithValidator() as IValidatable;
                classWithValidator.Validator = validator;

                classWithValidator.Validate(true);

                var dataErrorInfo = (IDataErrorInfo)classWithValidator;

                Assert.AreEqual("Error", dataErrorInfo.Error);
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
                public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), string.Empty);

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
                public static readonly PropertyData LastNameProperty = RegisterProperty("LastName", typeof(string), string.Empty);
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

                Assert.IsNotNull(testValidatorModel.Validator);

                testValidatorModel.Validate(true);

                Assert.IsTrue(testValidatorModel.HasErrors);
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

                Assert.IsFalse(validation.HasErrors);
                Assert.AreEqual(string.Empty, ((IDataErrorInfo)obj)["ErrorWhenEmpty"]);

                obj.HideValidationResults = false;

                Assert.AreNotEqual(string.Empty, ((IDataErrorInfo)obj)["ErrorWhenEmpty"]);
            }

            [TestCase]
            public void HidesTheBusinessRuleErrorsWhenTrue()
            {
                var obj = new ValidationTestModel();
                var validation = obj;
                obj.HideValidationResults = true;

                obj.BusinessRuleErrorWhenEmpty = string.Empty;

                Assert.IsFalse(validation.HasErrors);
                Assert.AreEqual(string.Empty, ((IDataErrorInfo)obj).Error);

                obj.HideValidationResults = false;

                Assert.AreNotEqual(string.Empty, ((IDataErrorInfo)obj).Error);
            }

            [TestCase]
            public void HidesTheFieldWarningsWhenTrue()
            {
                var obj = new ValidationTestModel();
                var validation = obj;
                obj.HideValidationResults = true;

                obj.WarningWhenEmpty = string.Empty;

                Assert.IsFalse(validation.HasWarnings);
                Assert.AreEqual(string.Empty, ((IDataWarningInfo)obj)["WarningWhenEmpty"]);

                obj.HideValidationResults = false;

                Assert.AreNotEqual(string.Empty, ((IDataWarningInfo)obj)["WarningWhenEmpty"]);
            }

            [TestCase]
            public void HidesTheBusinessRuleWarningsWhenTrue()
            {
                var obj = new ValidationTestModel();
                var validation = obj;
                obj.HideValidationResults = true;

                obj.BusinessRuleWarningWhenEmpty = string.Empty;

                Assert.IsFalse(validation.HasWarnings);
                Assert.AreEqual(string.Empty, ((IDataWarningInfo)obj).Warning);

                obj.HideValidationResults = false;

                Assert.AreNotEqual(string.Empty, ((IDataWarningInfo)obj).Warning);
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

                public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), string.Empty);
            }

            [TestCase]
            public void PreventsAttributeBasedValidation()
            {
                var model = new SuspendValidationModel();
                var validation = model as IValidatable;

                Assert.IsFalse(validation.HasErrors);

                using (model.SuspendValidations())
                {
                    model.FirstName = null;

                    Assert.IsFalse(validation.HasErrors);
                }
            }

            [TestCase]
            public void CorrectlyValidatesUnvalidatedPropertiesWhenSetToTrue()
            {
                var model = new SuspendValidationModel();
                var validation = model;

                Assert.IsFalse(validation.HasErrors);

                using (model.SuspendValidations())
                {
                    model.FirstName = null;

                    Assert.IsFalse(validation.HasErrors);
                }

                Assert.IsTrue(validation.HasErrors);
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

                Assert.IsTrue(validationContext.HasErrors);
                Assert.AreEqual("FirstName", ((FieldValidationResult)errors[0]).PropertyName);
                Assert.AreEqual("LastName", ((FieldValidationResult)errors[1]).PropertyName);
            }

            [TestCase]
            public void SuspendsValidationsAndValidatesOnResume()
            {
                var model = new SuspendableTestModel();

                using (model.SuspendValidations())
                {
                    model.Validate(true, true);

                    var innerValidationContext = model.GetValidationContext();

                    Assert.IsFalse(innerValidationContext.HasErrors);
                }

                //model.Validate(true, true);

                var validationContext = model.GetValidationContext();
                var errors = validationContext.GetErrors();

                Assert.IsTrue(validationContext.HasErrors);
                Assert.AreEqual("FirstName", ((FieldValidationResult)errors[0]).PropertyName);
                Assert.AreEqual("LastName", ((FieldValidationResult)errors[1]).PropertyName);
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

                        Assert.IsFalse(innerValidationContext1.HasErrors);
                    }

                    model.Validate(true, true);

                    var innerValidationContext2 = model.GetValidationContext();

                    Assert.IsFalse(innerValidationContext2.HasErrors);
                }

                //model.Validate(true, true);

                var validationContext = model.GetValidationContext();
                var errors = validationContext.GetErrors();

                Assert.IsTrue(validationContext.HasErrors);
                Assert.AreEqual("FirstName", ((FieldValidationResult)errors[0]).PropertyName);
                Assert.AreEqual("LastName", ((FieldValidationResult)errors[1]).PropertyName);
            }
        }

#if NET
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

                Assert.IsTrue(model.HasErrors);

                var validationContext = model.GetValidationContext();
                var errors = validationContext.GetErrors();

                Assert.AreEqual(1, errors.Count);
                Assert.AreEqual("Weight", ((FieldValidationResult)errors[0]).PropertyName);
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
                int _counter;

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

                Assert.AreEqual(0, model.Counter);
            }

            [TestCase]
            public void OnInstancePropertyIgnoreDataAnnotationSkipAnnotationValidation()
            {
                // Set intance property to skip data annotations validation
                var model = new ModelWithoutAnnotation();
                model.SetValidateUsingDataAnnotations(false);

                model.Validate(true);

                Assert.AreEqual(0, model.Counter);
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

                Assert.AreEqual(0, model.Counter);
            }

            [TestCase]
            public void ByDefaultValidateDataAnnotation()
            {
                // By default instance property set to check annotation validation
                var model = new ModelWithoutAnnotation();

                model.Validate(true);

                Assert.AreEqual(1, model.Counter);
            }
        }

        [TestFixture]
        public class IgnoreDataAnnotationValidationOnSetValue
        {
            public class ModelWithoutAnnotation : ValidatableModelBase
            {
                public static readonly PropertyData CounterProperty = RegisterProperty<ModelWithoutAnnotation, int>(model => model.Counter);

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
                    var f = t.GetFieldEx("_propertiesNotCheckedDuringDisabledValidation", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                    var v = f.GetValue(this) as HashSet<string>;

                    return v.Count != 0;
                }
            }

            [TestCase]
            public void OnInstancePropertyIgnoreDataAnnotationSkipAnnotationValidation()
            {
                var oldSuspension = ValidatableModelBase.SuspendValidationForAllModels;
                ValidatableModelBase.SuspendValidationForAllModels = true;

                // Set intance property to skip data annotations validation
                var model = new ModelWithoutAnnotation();
                model.SetValidateUsingDataAnnotations(false);

                model.Counter = 1;

                Assert.AreEqual(false, model.HasNotValidatedProperties());

                ValidatableModelBase.SuspendValidationForAllModels = oldSuspension;
            }


            [TestCase]
            public void ByDefaultValidateDataAnnotationOnSetValue()
            {
                var oldSuspension = ValidatableModelBase.SuspendValidationForAllModels;
                ValidatableModelBase.SuspendValidationForAllModels = true;

                // By default instance property set to check annotation validation
                var model = new ModelWithoutAnnotation();

                model.Counter = 1;

                Assert.AreEqual(true, model.HasNotValidatedProperties());

                ValidatableModelBase.SuspendValidationForAllModels = oldSuspension;
            }
        }
#endif
    }
}