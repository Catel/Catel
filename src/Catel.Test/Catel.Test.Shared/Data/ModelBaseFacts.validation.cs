// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBaseFacts.validation.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Catel.Data;
    using Catel.IoC;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public partial class ModelBaseFacts
    {
        [TestClass]
        public class OldStyleUnitTests
        {
            #region Validation
            [TestMethod]
            public void ValidationWithWarnings()
            {
                var validationObject = new ObjectWithValidation();

                // Check if the object now has warnings
                Assert.AreEqual(false, validationObject.HasWarnings);
                Assert.AreEqual(false, validationObject.HasErrors);
                Assert.AreEqual(0, validationObject.FieldWarningCount);
                Assert.AreEqual(0, validationObject.FieldErrorCount);
                Assert.AreEqual(0, validationObject.BusinessRuleWarningCount);
                Assert.AreEqual(0, validationObject.BusinessRuleErrorCount);

                // Now set a field warning and check it
                validationObject.ValueToValidate = ObjectWithValidation.ValueThatCausesFieldWarning;
                Assert.AreEqual(true, validationObject.HasWarnings);
                Assert.AreEqual(false, validationObject.HasErrors);
                Assert.AreEqual(1, validationObject.FieldWarningCount);
                Assert.AreEqual(0, validationObject.FieldErrorCount);
                Assert.AreEqual(0, validationObject.BusinessRuleWarningCount);
                Assert.AreEqual(0, validationObject.BusinessRuleErrorCount);

                // Now set a business warning and check it
                validationObject.ValueToValidate = ObjectWithValidation.ValueThatCausesBusinessWarning;
                Assert.AreEqual(true, validationObject.HasWarnings);
                Assert.AreEqual(false, validationObject.HasErrors);
                Assert.AreEqual(0, validationObject.FieldWarningCount);
                Assert.AreEqual(0, validationObject.FieldErrorCount);
                Assert.AreEqual(1, validationObject.BusinessRuleWarningCount);
                Assert.AreEqual(0, validationObject.BusinessRuleErrorCount);

                // Clear warning
                validationObject.ValueToValidate = ObjectWithValidation.ValueThatHasNoWarningsOrErrors;
                Assert.AreEqual(false, validationObject.HasWarnings);
                Assert.AreEqual(false, validationObject.HasErrors);
                Assert.AreEqual(0, validationObject.FieldWarningCount);
                Assert.AreEqual(0, validationObject.FieldErrorCount);
                Assert.AreEqual(0, validationObject.BusinessRuleWarningCount);
                Assert.AreEqual(0, validationObject.BusinessRuleErrorCount);
            }

#if !WINDOWS_PHONE
            [TestMethod]
            public void ValidationUsingAnnotationsForCatelProperties()
            {
                var validationObject = new ObjectWithValidation();

                Assert.IsFalse(validationObject.HasErrors);

                validationObject.ValueWithAnnotations = string.Empty;

                Assert.IsTrue(validationObject.HasErrors);

                validationObject.ValueWithAnnotations = "value";

                Assert.IsFalse(validationObject.HasErrors);
            }

            //[TestMethod]
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

            [TestMethod]
            public void ValidationUsingAnnotationsForNonCatelCalculatedProperties()
            {
                var validationObject = new ObjectWithValidation();

                Assert.IsFalse(validationObject.HasErrors);
            }
#endif
            #endregion

            #region IDataErrorInfo tests
#if !WINDOWS_PHONE
            [TestMethod]
            public void IDataErrorInfo_FieldWithError()
            {
                var obj = new ValidationTest();

                obj.ErrorWhenEmpty = string.Empty;

                Assert.IsTrue(obj.HasErrors);
                Assert.IsFalse(string.IsNullOrEmpty(((IDataErrorInfo)obj)["ErrorWhenEmpty"]));

                obj.ErrorWhenEmpty = "undo";

                Assert.IsFalse(obj.HasErrors);
                Assert.IsTrue(string.IsNullOrEmpty(((IDataErrorInfo)obj)["ErrorWhenEmpty"]));
            }

            [TestMethod]
            public void IDataErrorInfo_SetFieldErrorOutsideValidation()
            {
                var obj = new ValidationTest();
                obj.AutomaticallyValidateOnPropertyChanged = false;

                Assert.IsFalse(obj.HasErrors);
                Assert.IsTrue(string.IsNullOrEmpty(((IDataErrorInfo)obj)["ErrorWhenEmpty"]));

                obj.ErrorWhenEmpty = string.Empty;
                obj.SetFieldErrorOutsideValidation();

                Assert.IsTrue(obj.HasErrors);
                Assert.IsFalse(string.IsNullOrEmpty(((IDataErrorInfo)obj)["ErrorWhenEmpty"]));
            }

            [TestMethod]
            public void IDataErrorInfo_SetBusinessErrorOutsideValidation()
            {
                var obj = new ValidationTest();
                obj.AutomaticallyValidateOnPropertyChanged = false;

                Assert.IsFalse(obj.HasErrors);
                Assert.IsTrue(string.IsNullOrEmpty(((IDataErrorInfo)obj).Error));

                obj.BusinessRuleErrorWhenEmpty = string.Empty;
                obj.SetBusinessRuleErrorOutsideValidation();

                Assert.IsTrue(obj.HasErrors);
                Assert.IsFalse(string.IsNullOrEmpty(((IDataErrorInfo)obj).Error));
            }
#endif
            #endregion

            #region INotifyDataErrorInfo tests
#if !WINDOWS_PHONE
            [TestMethod]
            public void INotifyDataErrorInfo_FieldWithError()
            {
                var obj = new ValidationTest();
                bool isInvoked = false;
                int count = 0;

                ((INotifyDataErrorInfo)obj).ErrorsChanged += (sender, e) =>
                {
                    Assert.AreEqual("ErrorWhenEmpty", e.PropertyName);
                    isInvoked = true;
                };

                isInvoked = false;
                obj.ErrorWhenEmpty = string.Empty;

                Assert.IsTrue(obj.HasErrors);
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

                Assert.IsFalse(obj.HasErrors);
                Assert.IsTrue(isInvoked);
                count = 0;
                foreach (string fieldInfo in ((INotifyDataErrorInfo)obj).GetErrors("ErrorWhenEmpty"))
                {
                    count++;
                    Assert.IsFalse(string.IsNullOrEmpty(fieldInfo));
                }
                Assert.AreEqual(0, count);
            }

            [TestMethod]
            public void INotifyDataErrorInfo_Null()
            {
                var obj = new ValidationTest();
                bool isInvoked = false;
                int count = 0;

                ((INotifyDataErrorInfo)obj).ErrorsChanged += (sender, e) =>
                {
                    Assert.AreEqual(string.Empty, e.PropertyName);
                    isInvoked = true;
                };

                isInvoked = false;
                obj.BusinessRuleErrorWhenEmpty = string.Empty;

                Assert.IsTrue(obj.HasErrors);
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

                Assert.IsFalse(obj.HasErrors);
                Assert.IsTrue(isInvoked);
                count = 0;
                foreach (string error in ((INotifyDataErrorInfo)obj).GetErrors(null))
                {
                    count++;
                    Assert.IsFalse(string.IsNullOrEmpty(error));
                }
                Assert.AreEqual(0, count);
            }

            [TestMethod]
            public void INotifyDataErrorInfo_EmptyString()
            {
                var obj = new ValidationTest();
                bool isInvoked = false;
                int count = 0;

                ((INotifyDataErrorInfo)obj).ErrorsChanged += (sender, e) =>
                {
                    Assert.AreEqual(string.Empty, e.PropertyName);
                    isInvoked = true;
                };

                isInvoked = false;
                obj.BusinessRuleErrorWhenEmpty = string.Empty;

                Assert.IsTrue(obj.HasErrors);
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

                Assert.IsFalse(obj.HasErrors);
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
            [TestMethod]
            public void IDataWarningInfo_FieldWithWarning()
            {
                var obj = new ValidationTest();

                obj.WarningWhenEmpty = string.Empty;

                Assert.IsTrue(obj.HasWarnings);
                Assert.IsFalse(string.IsNullOrEmpty(((IDataWarningInfo)obj)["WarningWhenEmpty"]));

                obj.WarningWhenEmpty = "undo";

                Assert.IsFalse(obj.HasWarnings);
                Assert.IsTrue(string.IsNullOrEmpty(((IDataWarningInfo)obj)["WarningWhenEmpty"]));
            }

            [TestMethod]
            public void IDataErrorInfo_SetFieldWarningOutsideValidation()
            {
                var obj = new ValidationTest();
                obj.AutomaticallyValidateOnPropertyChanged = false;

                Assert.IsFalse(obj.HasWarnings);
                Assert.IsTrue(string.IsNullOrEmpty(((IDataWarningInfo)obj)["WarningWhenEmpty"]));

                obj.WarningWhenEmpty = string.Empty;
                obj.SetFieldWarningOutsideValidation();

                Assert.IsTrue(obj.HasWarnings);
                Assert.IsFalse(string.IsNullOrEmpty(((IDataWarningInfo)obj)["WarningWhenEmpty"]));
            }

            [TestMethod]
            public void IDataErrorInfo_SetBusinessWarningOutsideValidation()
            {
                var obj = new ValidationTest();
                obj.AutomaticallyValidateOnPropertyChanged = false;

                Assert.IsFalse(obj.HasWarnings);
                Assert.IsTrue(string.IsNullOrEmpty(((IDataWarningInfo)obj).Warning));

                obj.BusinessRuleWarningWhenEmpty = string.Empty;
                obj.SetBusinessRuleWarningOutsideValidation();

                Assert.IsTrue(obj.HasWarnings);
                Assert.IsFalse(string.IsNullOrEmpty(((IDataWarningInfo)obj).Warning));
            }
            #endregion

            #region INotifyDataWarningInfo tests
            [TestMethod]
            public void INotifyDataWarningInfo_FieldWithWarning()
            {
                var obj = new ValidationTest();
                bool isInvoked = false;
                int count = 0;

                ((INotifyDataWarningInfo)obj).WarningsChanged += (sender, e) =>
                {
                    Assert.AreEqual("WarningWhenEmpty", e.PropertyName);
                    isInvoked = true;
                };

                isInvoked = false;
                obj.WarningWhenEmpty = string.Empty;

                Assert.IsTrue(obj.HasWarnings);
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

                Assert.IsFalse(obj.HasWarnings);
                Assert.IsTrue(isInvoked);
                count = 0;
                foreach (string fieldInfo in ((INotifyDataWarningInfo)obj).GetWarnings("WarningWhenEmpty"))
                {
                    count++;
                    Assert.IsFalse(string.IsNullOrEmpty(fieldInfo));
                }
                Assert.AreEqual(0, count);
            }

            [TestMethod]
            public void INotifyDataWarningInfo_Null()
            {
                var obj = new ValidationTest();
                bool isInvoked = false;
                int count = 0;

                ((INotifyDataWarningInfo)obj).WarningsChanged += (sender, e) =>
                {
                    Assert.AreEqual(string.Empty, e.PropertyName);
                    isInvoked = true;
                };

                isInvoked = false;
                obj.BusinessRuleWarningWhenEmpty = string.Empty;

                Assert.IsTrue(obj.HasWarnings);
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

                Assert.IsFalse(obj.HasWarnings);
                Assert.IsTrue(isInvoked);
                count = 0;
                foreach (string warning in ((INotifyDataWarningInfo)obj).GetWarnings(null))
                {
                    count++;
                    Assert.IsFalse(string.IsNullOrEmpty(warning));
                }
                Assert.AreEqual(0, count);
            }

            [TestMethod]
            public void INotifyDataWarningInfo_EmptyString()
            {
                var obj = new ValidationTest();
                bool isInvoked = false;
                int count = 0;

                ((INotifyDataWarningInfo)obj).WarningsChanged += (sender, e) =>
                {
                    Assert.AreEqual(string.Empty, e.PropertyName);
                    isInvoked = true;
                };

                isInvoked = false;
                obj.BusinessRuleWarningWhenEmpty = string.Empty;

                Assert.IsTrue(obj.HasWarnings);
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

                Assert.IsFalse(obj.HasWarnings);
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
            [TestMethod]
            public void IValidator_CheckIfEventsAreFired()
            {
                var validator = new TestValidator();

                var classWithValidator = new ClassWithValidator();
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
            [TestMethod]
            public void IValidator_AddFieldErrors()
            {
                var validator = new TestValidator();

                var classWithValidator = new ClassWithValidator();
                classWithValidator.Validator = validator;

                classWithValidator.Validate(true);

                var dataWarningInfo = (IDataWarningInfo)classWithValidator;
                var dataErrorInfo = (IDataErrorInfo)classWithValidator;

                Assert.AreEqual("Warning", dataWarningInfo[ClassWithValidator.WarningPropertyProperty.Name]);
                Assert.AreEqual("Error", dataErrorInfo[ClassWithValidator.ErrorPropertyProperty.Name]);
            }

            [TestMethod]
            public void IValidator_AddBusinessRuleErrors()
            {
                var validator = new TestValidator();

                var classWithValidator = new ClassWithValidator();
                classWithValidator.Validator = validator;

                classWithValidator.Validate(true);

                var dataErrorInfo = (IDataErrorInfo)classWithValidator;

                Assert.AreEqual("Error", dataErrorInfo.Error);
            }
#endif
            #endregion
        }

        [TestClass]
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

            [TestMethod]
            public void AutomaticallyCreatesValidator()
            {
                ServiceLocator.Default.RegisterType<IValidatorProvider, AttributeValidatorProvider>();
                var testValidatorModel = new TestValidatorModel();

                Assert.IsNotNull(testValidatorModel.Validator);

                testValidatorModel.Validate(true);

                Assert.IsTrue(testValidatorModel.HasErrors);
            }
        }

        [TestClass]
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

            [TestMethod]
            public void PreventsAttributeBasedValidation()
            {
                var model = new SuspendValidationModel();

                Assert.IsFalse(model.HasErrors);

                model.SuspendValidationWrapper = true;
                model.FirstName = null;

                Assert.IsFalse(model.HasErrors);
            }

            [TestMethod]
            public void CorrectlyValidatesUnvalidatedPropertiesWhenSetToTrue()
            {
                var model = new SuspendValidationModel();

                Assert.IsFalse(model.HasErrors);

                model.SuspendValidationWrapper = true;
                model.FirstName = null;

                Assert.IsFalse(model.HasErrors);

                model.SuspendValidationWrapper = false;

                Assert.IsTrue(model.HasErrors);
            }
        }

#if NET
        // Test case for https://catelproject.atlassian.net/browse/CTL-246
        [TestClass]
        public class ValidationOfNonCatelProperties
        {
            public class ModelWithCalculatedPropertiesValidation : ModelBase
            {
                [Range(1, 10)]
                public int Weight
                {
                    get { return 2 * 6; }
                }
            }

            [TestMethod]
            public void CorrectlyValidatesNonCatelProperties()
            {
                var model = new ModelWithCalculatedPropertiesValidation();
                model.Validate(true);

                Assert.IsTrue(model.HasErrors);

                var validationContext = model.ValidationContext;
                var errors = validationContext.GetErrors();

                Assert.AreEqual(1, errors.Count);
                Assert.AreEqual("Weight", ((FieldValidationResult)errors[0]).PropertyName);
            }
        }
#endif
    }
}