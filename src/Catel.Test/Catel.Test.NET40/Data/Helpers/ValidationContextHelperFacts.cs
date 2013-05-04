// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationContextHelperTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using System;
    using System.Collections.Generic;
    using Catel.Data;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class ValidationContextChangeFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullValidationResult()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new ValidationContextChange(null, ValidationContextChangeType.Added));
            }

            [TestMethod]
            public void SetsValuesCorrectlyForAddedError()
            {
                var change = new ValidationContextChange(FieldValidationResult.CreateError("Property", "Error"), ValidationContextChangeType.Added);

                Assert.AreEqual("Property", ((FieldValidationResult) change.ValidationResult).PropertyName);
                Assert.AreEqual("Error", change.ValidationResult.Message);
                Assert.AreEqual(ValidationContextChangeType.Added, change.ChangeType);
            }

            [TestMethod]
            public void SetsValuesCorrectlyForRemovedError()
            {
                var change = new ValidationContextChange(FieldValidationResult.CreateError("Property", "Error"), ValidationContextChangeType.Removed);

                Assert.AreEqual("Property", ((FieldValidationResult) change.ValidationResult).PropertyName);
                Assert.AreEqual("Error", change.ValidationResult.Message);
                Assert.AreEqual(ValidationContextChangeType.Removed, change.ChangeType);
            }
        }
    }

    public class ValidationContextHelperFacts
    {
        [TestClass]
        public class TheGetChangesMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullFirstArgument()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ValidationContextHelper.GetChanges(null, new ValidationContext()));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullSecondArgument()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ValidationContextHelper.GetChanges(new ValidationContext(), null));
            }

            [TestMethod]
            public void ReturnsEmptyCollectionForEqualValidationContexts()
            {
                var fieldValidationResults1 = new List<IFieldValidationResult>();
                fieldValidationResults1.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults1.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults1 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context1 = new ValidationContext(fieldValidationResults1, businessRuleValidationResults1);

                var fieldValidationResults2 = new List<IFieldValidationResult>();
                fieldValidationResults2.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults2.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults2 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context2 = new ValidationContext(fieldValidationResults2, businessRuleValidationResults2);

                var changes = ValidationContextHelper.GetChanges(context1, context2);

                Assert.AreEqual(0, changes.Count);
            }

            [TestMethod]
            public void ReturnsChangesForAddedFieldWarning()
            {
                var fieldValidationResults1 = new List<IFieldValidationResult>();
                fieldValidationResults1.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults1 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context1 = new ValidationContext(fieldValidationResults1, businessRuleValidationResults1);

                var fieldValidationResults2 = new List<IFieldValidationResult>();
                fieldValidationResults2.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults2.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults2 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context2 = new ValidationContext(fieldValidationResults2, businessRuleValidationResults2);

                var changes = ValidationContextHelper.GetChanges(context1, context2);

                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual("MyProperty", ((IFieldValidationResult) changes[0].ValidationResult).PropertyName);
                Assert.AreEqual("FieldWarning", changes[0].ValidationResult.Message);
                Assert.AreEqual(ValidationResultType.Warning, changes[0].ValidationResult.ValidationResultType);
                Assert.AreEqual(ValidationContextChangeType.Added, changes[0].ChangeType);
            }

            [TestMethod]
            public void ReturnsChangesForRemovedFieldWarning()
            {
                var fieldValidationResults1 = new List<IFieldValidationResult>();
                fieldValidationResults1.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults1.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults1 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context1 = new ValidationContext(fieldValidationResults1, businessRuleValidationResults1);

                var fieldValidationResults2 = new List<IFieldValidationResult>();
                fieldValidationResults2.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults2 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context2 = new ValidationContext(fieldValidationResults2, businessRuleValidationResults2);

                var changes = ValidationContextHelper.GetChanges(context1, context2);

                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual("MyProperty", ((IFieldValidationResult) changes[0].ValidationResult).PropertyName);
                Assert.AreEqual("FieldWarning", changes[0].ValidationResult.Message);
                Assert.AreEqual(ValidationResultType.Warning, changes[0].ValidationResult.ValidationResultType);
                Assert.AreEqual(ValidationContextChangeType.Removed, changes[0].ChangeType);
            }

            [TestMethod]
            public void ReturnsChangesForAddedFieldError()
            {
                var fieldValidationResults1 = new List<IFieldValidationResult>();
                fieldValidationResults1.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));

                var businessRuleValidationResults1 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context1 = new ValidationContext(fieldValidationResults1, businessRuleValidationResults1);

                var fieldValidationResults2 = new List<IFieldValidationResult>();
                fieldValidationResults2.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults2.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults2 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context2 = new ValidationContext(fieldValidationResults2, businessRuleValidationResults2);

                var changes = ValidationContextHelper.GetChanges(context1, context2);

                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual("MyProperty", ((IFieldValidationResult) changes[0].ValidationResult).PropertyName);
                Assert.AreEqual("FieldError", changes[0].ValidationResult.Message);
                Assert.AreEqual(ValidationResultType.Error, changes[0].ValidationResult.ValidationResultType);
                Assert.AreEqual(ValidationContextChangeType.Added, changes[0].ChangeType);
            }

            [TestMethod]
            public void ReturnsChangesForRemovedFieldError()
            {
                var fieldValidationResults1 = new List<IFieldValidationResult>();
                fieldValidationResults1.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults1.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults1 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context1 = new ValidationContext(fieldValidationResults1, businessRuleValidationResults1);

                var fieldValidationResults2 = new List<IFieldValidationResult>();
                fieldValidationResults2.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));

                var businessRuleValidationResults2 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context2 = new ValidationContext(fieldValidationResults2, businessRuleValidationResults2);

                var changes = ValidationContextHelper.GetChanges(context1, context2);

                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual("MyProperty", ((IFieldValidationResult) changes[0].ValidationResult).PropertyName);
                Assert.AreEqual("FieldError", changes[0].ValidationResult.Message);
                Assert.AreEqual(ValidationResultType.Error, changes[0].ValidationResult.ValidationResultType);
                Assert.AreEqual(ValidationContextChangeType.Removed, changes[0].ChangeType);
            }

            [TestMethod]
            public void ReturnsChangesForAddedBusinessRuleWarning()
            {
                var fieldValidationResults1 = new List<IFieldValidationResult>();
                fieldValidationResults1.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults1.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults1 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context1 = new ValidationContext(fieldValidationResults1, businessRuleValidationResults1);

                var fieldValidationResults2 = new List<IFieldValidationResult>();
                fieldValidationResults2.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults2.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults2 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context2 = new ValidationContext(fieldValidationResults2, businessRuleValidationResults2);

                var changes = ValidationContextHelper.GetChanges(context1, context2);

                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual("BusinessRuleWarning", changes[0].ValidationResult.Message);
                Assert.AreEqual(ValidationResultType.Warning, changes[0].ValidationResult.ValidationResultType);
                Assert.AreEqual(ValidationContextChangeType.Added, changes[0].ChangeType);
            }

            [TestMethod]
            public void ReturnsChangesForRemovedBusinessRuleWarning()
            {
                var fieldValidationResults1 = new List<IFieldValidationResult>();
                fieldValidationResults1.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults1.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults1 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context1 = new ValidationContext(fieldValidationResults1, businessRuleValidationResults1);

                var fieldValidationResults2 = new List<IFieldValidationResult>();
                fieldValidationResults2.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults2.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults2 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context2 = new ValidationContext(fieldValidationResults2, businessRuleValidationResults2);

                var changes = ValidationContextHelper.GetChanges(context1, context2);

                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual("BusinessRuleWarning", changes[0].ValidationResult.Message);
                Assert.AreEqual(ValidationResultType.Warning, changes[0].ValidationResult.ValidationResultType);
                Assert.AreEqual(ValidationContextChangeType.Removed, changes[0].ChangeType);
            }

            [TestMethod]
            public void ReturnsChangesForAddedBusinessRuleError()
            {
                var fieldValidationResults1 = new List<IFieldValidationResult>();
                fieldValidationResults1.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults1.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults1 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                var context1 = new ValidationContext(fieldValidationResults1, businessRuleValidationResults1);

                var fieldValidationResults2 = new List<IFieldValidationResult>();
                fieldValidationResults2.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults2.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults2 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context2 = new ValidationContext(fieldValidationResults2, businessRuleValidationResults2);

                var changes = ValidationContextHelper.GetChanges(context1, context2);

                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual("BusinessRuleError", changes[0].ValidationResult.Message);
                Assert.AreEqual(ValidationResultType.Error, changes[0].ValidationResult.ValidationResultType);
                Assert.AreEqual(ValidationContextChangeType.Added, changes[0].ChangeType);
            }

            [TestMethod]
            public void ReturnsChangesForRemovedBusinessRuleError()
            {
                var fieldValidationResults1 = new List<IFieldValidationResult>();
                fieldValidationResults1.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults1.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults1 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context1 = new ValidationContext(fieldValidationResults1, businessRuleValidationResults1);

                var fieldValidationResults2 = new List<IFieldValidationResult>();
                fieldValidationResults2.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults2.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults2 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                var context2 = new ValidationContext(fieldValidationResults2, businessRuleValidationResults2);

                var changes = ValidationContextHelper.GetChanges(context1, context2);

                Assert.AreEqual(1, changes.Count);
                Assert.AreEqual("BusinessRuleError", changes[0].ValidationResult.Message);
                Assert.AreEqual(ValidationResultType.Error, changes[0].ValidationResult.ValidationResultType);
                Assert.AreEqual(ValidationContextChangeType.Removed, changes[0].ChangeType);
            }

            [TestMethod]
            public void ReturnsChangesForComplexScenario()
            {
                var fieldValidationResults1 = new List<IFieldValidationResult>();
                fieldValidationResults1.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarning"));
                fieldValidationResults1.Add(FieldValidationResult.CreateError("MyProperty", "FieldError"));

                var businessRuleValidationResults1 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarning"));
                businessRuleValidationResults1.Add(BusinessRuleValidationResult.CreateError("BusinessRuleError"));
                var context1 = new ValidationContext(fieldValidationResults1, businessRuleValidationResults1);

                var fieldValidationResults2 = new List<IFieldValidationResult>();
                fieldValidationResults2.Add(FieldValidationResult.CreateWarning("MyProperty", "FieldWarningTextHasChanged"));
                fieldValidationResults2.Add(FieldValidationResult.CreateError("NewProperty", "FieldErrorForNewProperty"));

                var businessRuleValidationResults2 = new List<IBusinessRuleValidationResult>();
                businessRuleValidationResults2.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarningTextHasChanged"));
                var context2 = new ValidationContext(fieldValidationResults2, businessRuleValidationResults2);

                var changes = ValidationContextHelper.GetChanges(context1, context2);

                Assert.AreEqual(7, changes.Count);
                int counter;

                // Field warning text has changed, thus removed
                counter = 0;
                Assert.AreEqual("MyProperty", ((IFieldValidationResult) changes[counter].ValidationResult).PropertyName);
                Assert.AreEqual("FieldWarning", changes[counter].ValidationResult.Message);
                Assert.AreEqual(ValidationResultType.Warning, changes[counter].ValidationResult.ValidationResultType);
                Assert.AreEqual(ValidationContextChangeType.Removed, changes[counter].ChangeType);

                // Field error has been removed
                counter++;
                Assert.AreEqual("MyProperty", ((IFieldValidationResult) changes[counter].ValidationResult).PropertyName);
                Assert.AreEqual("FieldError", changes[counter].ValidationResult.Message);
                Assert.AreEqual(ValidationResultType.Error, changes[counter].ValidationResult.ValidationResultType);
                Assert.AreEqual(ValidationContextChangeType.Removed, changes[counter].ChangeType);

                // Field warning text has changed, thus added
                counter++;
                Assert.AreEqual("MyProperty", ((IFieldValidationResult) changes[counter].ValidationResult).PropertyName);
                Assert.AreEqual("FieldWarningTextHasChanged", changes[counter].ValidationResult.Message);
                Assert.AreEqual(ValidationResultType.Warning, changes[counter].ValidationResult.ValidationResultType);
                Assert.AreEqual(ValidationContextChangeType.Added, changes[counter].ChangeType);

                // Field error added
                counter++;
                Assert.AreEqual("NewProperty", ((IFieldValidationResult) changes[counter].ValidationResult).PropertyName);
                Assert.AreEqual("FieldErrorForNewProperty", changes[counter].ValidationResult.Message);
                Assert.AreEqual(ValidationResultType.Error, changes[counter].ValidationResult.ValidationResultType);
                Assert.AreEqual(ValidationContextChangeType.Added, changes[counter].ChangeType);

                // Business rule text has changed, thus removed
                counter++;
                Assert.AreEqual("BusinessRuleWarning", changes[counter].ValidationResult.Message);
                Assert.AreEqual(ValidationResultType.Warning, changes[counter].ValidationResult.ValidationResultType);
                Assert.AreEqual(ValidationContextChangeType.Removed, changes[counter].ChangeType);

                // Business rule error has been removed
                counter++;
                Assert.AreEqual("BusinessRuleError", changes[counter].ValidationResult.Message);
                Assert.AreEqual(ValidationResultType.Error, changes[counter].ValidationResult.ValidationResultType);
                Assert.AreEqual(ValidationContextChangeType.Removed, changes[counter].ChangeType);

                // Business rule text has changed, thus added
                counter++;
                Assert.AreEqual("BusinessRuleWarningTextHasChanged", changes[counter].ValidationResult.Message);
                Assert.AreEqual(ValidationResultType.Warning, changes[counter].ValidationResult.ValidationResultType);
                Assert.AreEqual(ValidationContextChangeType.Added, changes[counter].ChangeType);
            }
        }
    }
}