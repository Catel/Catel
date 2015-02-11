﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBaseFacts.validation.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Catel.Data;
    using Catel.IoC;

    using NUnit.Framework;

    public partial class ModelBaseFacts
    {
        [TestFixture]
        public class OldStyleUnitTests
        {
            #region Validation
            [TestCase]
            public void ValidationWithWarnings()
            {
                var validationObject = new ObjectWithValidation();
                var validation = validationObject as IModelValidation;

                // Check if the object now has warnings
                Assert.AreEqual(false, validation.HasWarnings);
                Assert.AreEqual(false, validation.HasErrors);
                Assert.AreEqual(0, validation.FieldWarningCount);
                Assert.AreEqual(0, validation.FieldErrorCount);
                Assert.AreEqual(0, validation.BusinessRuleWarningCount);
                Assert.AreEqual(0, validation.BusinessRuleErrorCount);

                // Now set a field warning and check it
                validationObject.ValueToValidate = ObjectWithValidation.ValueThatCausesFieldWarning;
                Assert.AreEqual(true, validation.HasWarnings);
                Assert.AreEqual(false, validation.HasErrors);
                Assert.AreEqual(1, validation.FieldWarningCount);
                Assert.AreEqual(0, validation.FieldErrorCount);
                Assert.AreEqual(0, validation.BusinessRuleWarningCount);
                Assert.AreEqual(0, validation.BusinessRuleErrorCount);

                // Now set a business warning and check it
                validationObject.ValueToValidate = ObjectWithValidation.ValueThatCausesBusinessWarning;
                Assert.AreEqual(true, validation.HasWarnings);
                Assert.AreEqual(false, validation.HasErrors);
                Assert.AreEqual(0, validation.FieldWarningCount);
                Assert.AreEqual(0, validation.FieldErrorCount);
                Assert.AreEqual(1, validation.BusinessRuleWarningCount);
                Assert.AreEqual(0, validation.BusinessRuleErrorCount);

                // Clear warning
                validationObject.ValueToValidate = ObjectWithValidation.ValueThatHasNoWarningsOrErrors;
                Assert.AreEqual(false, validation.HasWarnings);
                Assert.AreEqual(false, validation.HasErrors);
                Assert.AreEqual(0, validation.FieldWarningCount);
                Assert.AreEqual(0, validation.FieldErrorCount);
                Assert.AreEqual(0, validation.BusinessRuleWarningCount);
                Assert.AreEqual(0, validation.BusinessRuleErrorCount);
            }

#if !WINDOWS_PHONE
            [TestCase]
            public void ValidationUsingAnnotationsForCatelProperties()
            {
                var validationObject = new ObjectWithValidation();
                var validation = validationObject as IModelValidation;

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
                var validationObject = new ObjectWithValidation() as IModelValidation;

                Assert.IsFalse(validationObject.HasErrors);
            }
#endif
            #endregion

            #region IDataErrorInfo tests
#if !WINDOWS_PHONE
            [TestCase]
            public void IDataErrorInfo_FieldWithError()
            {
                var obj = new ValidationTest();
                var validation = obj as IModelValidation;

                obj.ErrorWhenEmpty = string.Empty;

                Assert.IsTrue(validation.HasErrors);
                Assert.IsFalse(string.IsNullOrEmpty(((IDataErrorInfo)obj)["ErrorWhenEmpty"]));

                obj.ErrorWhenEmpty = "undo";

                Assert.IsFalse(validation.HasErrors);
                Assert.IsTrue(string.IsNullOrEmpty(((IDataErrorInfo)obj)["ErrorWhenEmpty"]));
            }
#endif
            #endregion

            #region INotifyDataErrorInfo tests
#if !WINDOWS_PHONE
            [TestCase]
            public void INotifyDataErrorInfo_FieldWithError()
            {
                var obj = new ValidationTest();
                var validation = obj as IModelValidation;
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
                var obj = new ValidationTest();
                var validation = obj as IModelValidation;
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
                var obj = new ValidationTest();
                var validation = obj as IModelValidation;
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
#endif
            #endregion

            #region IDataWarningInfo tests
            [TestCase]
            public void IDataWarningInfo_FieldWithWarning()
            {
                var obj = new ValidationTest();
                var validation = obj as IModelValidation;

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
                var obj = new ValidationTest();
                var validation = obj as IModelValidation;
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
                var obj = new ValidationTest();
                var validation = obj as IModelValidation;
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
                var obj = new ValidationTest();
                var validation = obj as IModelValidation;
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

                var classWithValidator = new ClassWithValidator() as IModelValidation;
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

#if !WINDOWS_PHONE
            [TestCase]
            public void IValidator_AddFieldErrors()
            {
                var validator = new TestValidator();

                var classWithValidator = new ClassWithValidator() as IModelValidation;
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

                var classWithValidator = new ClassWithValidator() as IModelValidation;
                classWithValidator.Validator = validator;

                classWithValidator.Validate(true);

                var dataErrorInfo = (IDataErrorInfo)classWithValidator;

                Assert.AreEqual("Error", dataErrorInfo.Error);
            }
#endif
            #endregion
        }

        [TestFixture]
        public class TheValidateModelAttribute
        {
            [ValidateModel(typeof(TestValidator))]
            public class TestValidatorModel : ModelBase
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
                var testValidatorModel = new TestValidatorModel() as IModelValidation;

                Assert.IsNotNull(testValidatorModel.Validator);

                testValidatorModel.Validate(true);

                Assert.IsTrue(testValidatorModel.HasErrors);
            }
        }

        [TestFixture]
        public class TheSuspendValidationProperty
        {
            public class SuspendValidationModel : ModelBase
            {
                public bool SuspendValidationWrapper
                {
                    get { return SuspendValidation; }
                    set { SuspendValidation = value; }
                }

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
                var validation = model as IModelValidation;

                Assert.IsFalse(validation.HasErrors);

                model.SuspendValidationWrapper = true;
                model.FirstName = null;

                Assert.IsFalse(validation.HasErrors);
            }

            [TestCase]
            public void CorrectlyValidatesUnvalidatedPropertiesWhenSetToTrue()
            {
                var model = new SuspendValidationModel();
                var validation = model as IModelValidation;

                Assert.IsFalse(validation.HasErrors);

                model.SuspendValidationWrapper = true;
                model.FirstName = null;

                Assert.IsFalse(validation.HasErrors);

                model.SuspendValidationWrapper = false;

                Assert.IsTrue(validation.HasErrors);
            }
        }

#if NET
        // Test case for https://catelproject.atlassian.net/browse/CTL-246
        [TestFixture]
        public class ValidationOfNonCatelProperties
        {
            public class ModelWithCalculatedPropertiesValidation : ModelBase
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
                var model = new ModelWithCalculatedPropertiesValidation() as IModelValidation;
                model.Validate(true);

                Assert.IsTrue(model.HasErrors);

                var validationContext = model.GetValidationContext();
                var errors = validationContext.GetErrors();

                Assert.AreEqual(1, errors.Count);
                Assert.AreEqual("Weight", ((FieldValidationResult)errors[0]).PropertyName);
            }
        }
#endif
    }
}