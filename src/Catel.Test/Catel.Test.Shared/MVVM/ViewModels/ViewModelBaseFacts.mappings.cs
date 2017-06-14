// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBaseFacts.mappings.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.MVVM.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Catel.Data;
    using NUnit.Framework;
    using TestClasses;

    public partial class ViewModelBaseFacts
    {
        [TestCase]
        public void ViewModelWithViewModelToModelMappings_DoubleModels()
        {
            var firstPerson = new Person();
            firstPerson.FirstName = "John";
            firstPerson.LastName = "Doe";
            firstPerson.ContactInfo.Street = "Unknown street";
            firstPerson.ContactInfo.City = "Unknown city";
            firstPerson.ContactInfo.Email = "john@doe.com";

            var secondPerson = new Person();
            secondPerson.FirstName = "Second";
            secondPerson.LastName = "Person";
            secondPerson.ContactInfo.Street = "Another street";
            secondPerson.ContactInfo.City = "Another city";
            secondPerson.ContactInfo.Email = "Another email";

            var viewModel = new TestViewModelWithMultipleModelMappings(firstPerson);

            Assert.IsNotNull(viewModel.Person);
            Assert.IsNotNull(viewModel.ContactInfo);
            Assert.AreEqual("john@doe.com", viewModel.Email);

            viewModel.Person = secondPerson;

            Assert.IsNotNull(viewModel.Person);
            Assert.IsNotNull(viewModel.ContactInfo);
            Assert.AreEqual("Another email", viewModel.Email);
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_MissingModelName_WorksWithSingleModel()
        {
            var person = new Person();
            person.FirstName = "John";
            person.LastName = "Doe";
            person.ContactInfo.Street = "Unknown street";
            person.ContactInfo.City = "Unknown city";
            person.ContactInfo.Email = "john@doe.com";

            var viewModel = new TestViewModelWithImplicitModelMappings(person);

            Assert.IsNotNull(viewModel.Person);
            Assert.AreEqual("John", viewModel.FirstName);
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_MissingModelName_ThrowsExceptionWithMultipleModels()
        {
            var person = new Person();
            person.FirstName = "John";
            person.LastName = "Doe";
            person.ContactInfo.Street = "Unknown street";
            person.ContactInfo.City = "Unknown city";
            person.ContactInfo.Email = "john@doe.com";

            ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => new TestViewModelWithImplicitModelMappingsWithMultipleModels(person));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_PropertyChanges()
        {
            const string FirstName = "first name";
            const string LastName = "last name";
            const uint Age1 = 1;
            const uint Age2 = 2;

            var person = new Person();
            var viewModel = new TestViewModel(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(string.Empty, viewModel.FirstName);
            Assert.AreEqual(string.Empty, person.LastName);
            Assert.AreEqual(string.Empty, viewModel.LastName);

            Assert.AreEqual(0, person.Age);
            Assert.AreEqual(0, viewModel.Age);

            // Model to view model mapping
            person.FirstName = FirstName;
            Assert.AreEqual(FirstName, person.FirstName);
            Assert.AreEqual(FirstName, viewModel.FirstName);

            // View model to model mapping
            viewModel.LastName = LastName;
            Assert.AreEqual(LastName, person.LastName);
            Assert.AreEqual(LastName, viewModel.LastName);

            person.Age = Age1;
            Assert.AreEqual(Age1, person.Age);
            Assert.AreEqual(Age1, viewModel.Age);

            viewModel.Age = Age2;
            Assert.AreEqual(Age2, person.Age);
            Assert.AreEqual(Age2, viewModel.Age);
        }

        [TestCase]
        public void ViewModelWithMappingConverters()
        {
            const string FirstName = "first_name";
            const string LastName = "last_name";
            const uint Age1 = 1;
            const uint Age2 = 2;

            var person = new Person();
            var viewModel = new TestViewModelWithMappingConverters(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(string.Empty, viewModel.FirstName);
            Assert.AreEqual(string.Empty, person.LastName);
            Assert.AreEqual(string.Empty, viewModel.LastName);

            Assert.AreEqual(string.Empty, viewModel.FullName);

            Assert.AreEqual(0, person.Age);
            Assert.AreEqual("0", viewModel.Age);

            // Model to view model mapping
            person.FirstName = FirstName;
            Assert.AreEqual(FirstName, person.FirstName);
            Assert.AreEqual(FirstName, viewModel.FirstName);
            Assert.AreEqual(FirstName, viewModel.FullName);

            // View model to model mapping
            viewModel.LastName = LastName;
            Assert.AreEqual(LastName, person.LastName);
            Assert.AreEqual(LastName, viewModel.LastName);
            Assert.AreEqual(FirstName + " " + LastName, viewModel.FullName);
            Assert.AreEqual(FirstName + ";" + LastName, viewModel.FullNameWithCustomSeparator);

            person.Age = Age1;
            Assert.AreEqual(Age1, person.Age);
            Assert.AreEqual(Age1.ToString(), viewModel.Age);

            viewModel.Age = Age2.ToString();
            Assert.AreEqual(Age2, person.Age);
            Assert.AreEqual(Age2.ToString(), viewModel.Age);
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_FieldErrors()
        {
            var person = new Person();
            var viewModel = new TestViewModel(person);

            var personAsError = (IDataErrorInfo)person;
            var viewModelAsError = (IDataErrorInfo)viewModel;

            person.FirstName = "first name";

            Assert.AreEqual(string.Empty, personAsError[Person.FirstNameProperty.Name]);
            Assert.AreEqual(string.Empty, viewModelAsError[TestViewModel.FirstNameProperty.Name]);

            person.FirstName = string.Empty;

            Assert.AreNotEqual(string.Empty, personAsError[Person.FirstNameProperty.Name]);
            Assert.AreNotEqual(string.Empty, viewModelAsError[TestViewModel.FirstNameProperty.Name]);
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_FieldWarnings()
        {
            var person = new Person();
            var viewModel = new TestViewModel(person);

            var personAsWarning = (IDataWarningInfo)person;
            var viewModelAsWarning = (IDataWarningInfo)viewModel;

            person.FirstName = "first name";
            person.LastName = "last name";

            var validation = (IValidatableModel)viewModel;

            Assert.IsFalse(validation.HasErrors);
            Assert.IsTrue(validation.HasWarnings);
            Assert.AreNotEqual(string.Empty, personAsWarning[Person.MiddleNameProperty.Name]);
            Assert.AreNotEqual(string.Empty, viewModelAsWarning[TestViewModel.MiddleNameProperty.Name]);

            person.MiddleName = "middle name";

            Assert.IsFalse(validation.HasErrors);
            Assert.IsFalse(validation.HasWarnings);
            Assert.AreEqual(string.Empty, personAsWarning[Person.MiddleNameProperty.Name]);
            Assert.AreEqual(string.Empty, viewModelAsWarning[TestViewModel.MiddleNameProperty.Name]);
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_BusinessErrors()
        {
            var person = new Person();
            var viewModel = new TestViewModel(person);

            var personAsError = (IDataErrorInfo)person;
            var viewModelAsError = (IDataErrorInfo)viewModel;

            person.FirstName = "first name";
            person.LastName = "last name";

            Assert.AreEqual(string.Empty, personAsError.Error);
            Assert.AreEqual(string.Empty, viewModelAsError.Error);

            person.FirstName = string.Empty;

            Assert.AreNotEqual(string.Empty, personAsError.Error);
            Assert.AreNotEqual(string.Empty, viewModelAsError.Error);
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_BusinessWarnings()
        {
            var person = new Person();
            var viewModel = new TestViewModel(person);

            var personAsWarning = (IDataWarningInfo)person;
            var viewModelAsWarning = (IDataWarningInfo)viewModel;

            person.FirstName = "first name";
            person.LastName = "last name";

            Assert.AreNotEqual(string.Empty, personAsWarning.Warning);
            Assert.AreNotEqual(string.Empty, viewModelAsWarning.Warning);

            person.MiddleName = "middle name";

            Assert.AreEqual(string.Empty, personAsWarning.Warning);
            Assert.AreEqual(string.Empty, viewModelAsWarning.Warning);
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_ValidateModelsOnInitialization()
        {
            var person = new PersonWithDataAnnotations();
            var viewModel = new TestViewModel(person, true);

            ((IValidatableModel)person).Validate(true);

            Assert.AreNotEqual(0, viewModel.GetValidationContext().GetValidationCount());
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_DoNotValidateModelsOnInitialization_UpdateViaViewModel()
        {
            var person = new PersonWithDataAnnotations();
            var viewModel = new TestViewModel(person, false);

            Assert.AreEqual(0, person.GetValidationContext().GetValidationCount());
            Assert.AreEqual(0, viewModel.GetValidationContext().GetValidationCount());

            viewModel.FirstName = null;

            Assert.AreNotEqual(0, person.GetValidationContext().GetValidationCount());
            Assert.AreNotEqual(0, viewModel.GetValidationContext().GetValidationCount());
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_DoNotValidateModelsOnInitialization_UpdateViaModel()
        {
            var person = new PersonWithDataAnnotations();
            var viewModel = new TestViewModel(person, false);

            Assert.AreEqual(0, person.GetValidationContext().GetValidationCount());
            Assert.AreEqual(0, viewModel.GetValidationContext().GetValidationCount());

            person.FirstName = null;

            Assert.AreNotEqual(0, person.GetValidationContext().GetValidationCount());
            Assert.AreNotEqual(0, viewModel.GetValidationContext().GetValidationCount());
        }

        [TestCase]
        public async Task ViewModelWithViewModelToModelMappings_DoNotMapWhenViewModelIsClosed()
        {
            var person = new Person();
            var viewModel = new TestViewModel(person, true);

            Assert.AreNotEqual("test1", person.FirstName);
            viewModel.FirstName = "test1";
            Assert.AreEqual("test1", person.FirstName);

            await viewModel.CloseViewModelAsync(true);
            viewModel.FirstName = "test2";

            Assert.AreEqual("test1", person.FirstName);
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_TwoWay_InitiatedFromModel()
        {
            var person = new Person();
            var viewModel = new TestViewModelWithMappings(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsTwoWay);

            person.FirstName = "geert";

            // Both must have changed
            Assert.AreEqual("geert", person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsTwoWay);
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_TwoWay_InitiatedFromViewModel()
        {
            var person = new Person();
            var viewModel = new TestViewModelWithMappings(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsTwoWay);

            viewModel.FirstNameAsTwoWay = "geert";

            // Both must have changed
            Assert.AreEqual("geert", viewModel.FirstNameAsTwoWay);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsTwoWay);
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_OneWay_InitiatedFromModel()
        {
            var person = new Person();
            var viewModel = new TestViewModelWithMappings(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsOneWay);

            person.FirstName = "geert";

            // Both must have changed
            Assert.AreEqual("geert", person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsOneWay);
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_OneWay_InitiatedFromViewModel()
        {
            var person = new Person();
            var viewModel = new TestViewModelWithMappings(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsOneWay);

            viewModel.FirstNameAsOneWay = "geert";

            // Only view model must have changed
            Assert.AreEqual("geert", viewModel.FirstNameAsOneWay);
            Assert.AreNotEqual(person.FirstName, viewModel.FirstNameAsOneWay);
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_OneWayToSource_InitiatedFromModel()
        {
            var person = new Person();
            var viewModel = new TestViewModelWithMappings(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsOneWayToSource);

            person.FirstName = "geert";

            // Only model must have changed
            Assert.AreEqual("geert", person.FirstName);
            Assert.AreNotEqual(person.FirstName, viewModel.FirstNameAsOneWayToSource);
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_OneWayToSource_InitiatedFromViewModel()
        {
            var person = new Person();
            var viewModel = new TestViewModelWithMappings(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsOneWayToSource);

            viewModel.FirstNameAsOneWayToSource = "geert";

            // Both must have changed
            Assert.AreEqual("geert", viewModel.FirstNameAsOneWayToSource);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsOneWayToSource);
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_Explicit_InitiatedFromModel()
        {
            var person = new Person();
            var viewModel = new TestViewModelWithMappings(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsExplicit);

            person.FirstName = "geert";

            // When initiated from model => VM should change
            Assert.AreEqual("geert", person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsExplicit);
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_Explicit_InitiatedFromViewModel()
        {
            var person = new Person();
            var viewModel = new TestViewModelWithMappings(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsExplicit);

            viewModel.FirstNameAsExplicit = "geert";

            // When initiated from VM => nothing should change
            Assert.AreEqual("geert", viewModel.FirstNameAsExplicit);
            Assert.AreNotEqual(person.FirstName, viewModel.FirstNameAsExplicit);
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_Explicit_InitiatedManually()
        {
            var person = new Person();
            var viewModel = new TestViewModelWithMappings(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsExplicit);

            viewModel.FirstNameAsExplicit = "geert";
            viewModel.UpdateExplicitMappings();

            // Both must have changed
            Assert.AreEqual("geert", viewModel.FirstNameAsExplicit);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsExplicit);
        }
    }
}