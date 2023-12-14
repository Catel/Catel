namespace Catel.Tests.Data
{
    using System;
    using System.Collections.Generic;
    using Catel.Data;

    using NUnit.Framework;

    public class ValidationContextChangeFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullValidationResult()
            {
                Assert.Throws<ArgumentNullException>(() => new ValidationContextChange(null, ValidationContextChangeType.Added));
            }

            [TestCase]
            public void SetsValuesCorrectlyForAddedError()
            {
                var change = new ValidationContextChange(FieldValidationResult.CreateError("Property", "Error"), ValidationContextChangeType.Added);

                Assert.That(((FieldValidationResult)change.ValidationResult).PropertyName, Is.EqualTo("Property"));
                Assert.That(change.ValidationResult.Message, Is.EqualTo("Error"));
                Assert.That(change.ChangeType, Is.EqualTo(ValidationContextChangeType.Added));
            }

            [TestCase]
            public void SetsValuesCorrectlyForRemovedError()
            {
                var change = new ValidationContextChange(FieldValidationResult.CreateError("Property", "Error"), ValidationContextChangeType.Removed);

                Assert.That(((FieldValidationResult)change.ValidationResult).PropertyName, Is.EqualTo("Property"));
                Assert.That(change.ValidationResult.Message, Is.EqualTo("Error"));
                Assert.That(change.ChangeType, Is.EqualTo(ValidationContextChangeType.Removed));
            }
        }
    }

    public class ValidationContextHelperFacts
    {
        [TestFixture]
        public class TheGetChangesMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullFirstArgument()
            {
                Assert.Throws<ArgumentNullException>(() => ValidationContextHelper.GetChanges(null, new ValidationContext()));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullSecondArgument()
            {
                Assert.Throws<ArgumentNullException>(() => ValidationContextHelper.GetChanges(new ValidationContext(), null));
            }

            [TestCase]
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

                Assert.That(changes.Count, Is.EqualTo(0));
            }

            [TestCase]
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

                Assert.That(changes.Count, Is.EqualTo(1));
                Assert.That(((IFieldValidationResult)changes[0].ValidationResult).PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(changes[0].ValidationResult.Message, Is.EqualTo("FieldWarning"));
                Assert.That(changes[0].ValidationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Warning));
                Assert.That(changes[0].ChangeType, Is.EqualTo(ValidationContextChangeType.Added));
            }

            [TestCase]
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

                Assert.That(changes.Count, Is.EqualTo(1));
                Assert.That(((IFieldValidationResult)changes[0].ValidationResult).PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(changes[0].ValidationResult.Message, Is.EqualTo("FieldWarning"));
                Assert.That(changes[0].ValidationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Warning));
                Assert.That(changes[0].ChangeType, Is.EqualTo(ValidationContextChangeType.Removed));
            }

            [TestCase]
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

                Assert.That(changes.Count, Is.EqualTo(1));
                Assert.That(((IFieldValidationResult)changes[0].ValidationResult).PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(changes[0].ValidationResult.Message, Is.EqualTo("FieldError"));
                Assert.That(changes[0].ValidationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Error));
                Assert.That(changes[0].ChangeType, Is.EqualTo(ValidationContextChangeType.Added));
            }

            [TestCase]
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

                Assert.That(changes.Count, Is.EqualTo(1));
                Assert.That(((IFieldValidationResult)changes[0].ValidationResult).PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(changes[0].ValidationResult.Message, Is.EqualTo("FieldError"));
                Assert.That(changes[0].ValidationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Error));
                Assert.That(changes[0].ChangeType, Is.EqualTo(ValidationContextChangeType.Removed));
            }

            [TestCase]
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

                Assert.That(changes.Count, Is.EqualTo(1));
                Assert.That(changes[0].ValidationResult.Message, Is.EqualTo("BusinessRuleWarning"));
                Assert.That(changes[0].ValidationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Warning));
                Assert.That(changes[0].ChangeType, Is.EqualTo(ValidationContextChangeType.Added));
            }

            [TestCase]
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

                Assert.That(changes.Count, Is.EqualTo(1));
                Assert.That(changes[0].ValidationResult.Message, Is.EqualTo("BusinessRuleWarning"));
                Assert.That(changes[0].ValidationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Warning));
                Assert.That(changes[0].ChangeType, Is.EqualTo(ValidationContextChangeType.Removed));
            }

            [TestCase]
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

                Assert.That(changes.Count, Is.EqualTo(1));
                Assert.That(changes[0].ValidationResult.Message, Is.EqualTo("BusinessRuleError"));
                Assert.That(changes[0].ValidationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Error));
                Assert.That(changes[0].ChangeType, Is.EqualTo(ValidationContextChangeType.Added));
            }

            [TestCase]
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

                Assert.That(changes.Count, Is.EqualTo(1));
                Assert.That(changes[0].ValidationResult.Message, Is.EqualTo("BusinessRuleError"));
                Assert.That(changes[0].ValidationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Error));
                Assert.That(changes[0].ChangeType, Is.EqualTo(ValidationContextChangeType.Removed));
            }

            [TestCase]
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

                Assert.That(changes.Count, Is.EqualTo(7));
                int counter;

                // Field warning text has changed, thus removed
                counter = 0;
                Assert.That(((IFieldValidationResult)changes[counter].ValidationResult).PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(changes[counter].ValidationResult.Message, Is.EqualTo("FieldWarning"));
                Assert.That(changes[counter].ValidationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Warning));
                Assert.That(changes[counter].ChangeType, Is.EqualTo(ValidationContextChangeType.Removed));

                // Field error has been removed
                counter++;
                Assert.That(((IFieldValidationResult)changes[counter].ValidationResult).PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(changes[counter].ValidationResult.Message, Is.EqualTo("FieldError"));
                Assert.That(changes[counter].ValidationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Error));
                Assert.That(changes[counter].ChangeType, Is.EqualTo(ValidationContextChangeType.Removed));

                // Field warning text has changed, thus added
                counter++;
                Assert.That(((IFieldValidationResult)changes[counter].ValidationResult).PropertyName, Is.EqualTo("MyProperty"));
                Assert.That(changes[counter].ValidationResult.Message, Is.EqualTo("FieldWarningTextHasChanged"));
                Assert.That(changes[counter].ValidationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Warning));
                Assert.That(changes[counter].ChangeType, Is.EqualTo(ValidationContextChangeType.Added));

                // Field error added
                counter++;
                Assert.That(((IFieldValidationResult)changes[counter].ValidationResult).PropertyName, Is.EqualTo("NewProperty"));
                Assert.That(changes[counter].ValidationResult.Message, Is.EqualTo("FieldErrorForNewProperty"));
                Assert.That(changes[counter].ValidationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Error));
                Assert.That(changes[counter].ChangeType, Is.EqualTo(ValidationContextChangeType.Added));

                // Business rule text has changed, thus removed
                counter++;
                Assert.That(changes[counter].ValidationResult.Message, Is.EqualTo("BusinessRuleWarning"));
                Assert.That(changes[counter].ValidationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Warning));
                Assert.That(changes[counter].ChangeType, Is.EqualTo(ValidationContextChangeType.Removed));

                // Business rule error has been removed
                counter++;
                Assert.That(changes[counter].ValidationResult.Message, Is.EqualTo("BusinessRuleError"));
                Assert.That(changes[counter].ValidationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Error));
                Assert.That(changes[counter].ChangeType, Is.EqualTo(ValidationContextChangeType.Removed));

                // Business rule text has changed, thus added
                counter++;
                Assert.That(changes[counter].ValidationResult.Message, Is.EqualTo("BusinessRuleWarningTextHasChanged"));
                Assert.That(changes[counter].ValidationResult.ValidationResultType, Is.EqualTo(ValidationResultType.Warning));
                Assert.That(changes[counter].ChangeType, Is.EqualTo(ValidationContextChangeType.Added));
            }
        }
    }
}
