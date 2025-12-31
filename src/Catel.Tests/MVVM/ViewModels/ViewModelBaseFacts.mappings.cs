namespace Catel.Tests.MVVM.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Catel.Data;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TestClasses;

    public partial class ViewModelBaseFacts
    {
        [TestCase]
        public void ViewModelWithViewModelToModelMappings_DoubleModels()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

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

            var viewModel = new TestViewModelWithMultipleModelMappings(firstPerson, serviceProvider);

            Assert.That(viewModel.Person, Is.Not.Null);
            Assert.That(viewModel.ContactInfo, Is.Not.Null);
            Assert.That(viewModel.Email, Is.EqualTo("john@doe.com"));

            viewModel.Person = secondPerson;

            Assert.That(viewModel.Person, Is.Not.Null);
            Assert.That(viewModel.ContactInfo, Is.Not.Null);
            Assert.That(viewModel.Email, Is.EqualTo("Another email"));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_MissingModelName_WorksWithSingleModel()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new Person();
            person.FirstName = "John";
            person.LastName = "Doe";
            person.ContactInfo.Street = "Unknown street";
            person.ContactInfo.City = "Unknown city";
            person.ContactInfo.Email = "john@doe.com";

            var viewModel = new TestViewModelWithImplicitModelMappings(person, serviceProvider);

            Assert.That(viewModel.Person, Is.Not.Null);
            Assert.That(viewModel.FirstName, Is.EqualTo("John"));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_Prevents_Duplicate_ObservableObject_Update()
        {
            // Written for https://github.com/Catel/Catel/issues/2164

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new PersonObservableObject();
            person.FirstName = "John";

            Assert.That(person.FirstNameChangedCounter, Is.EqualTo(1));

            var viewModel = new TestViewModelWithImplicitModelMappings(person, serviceProvider);

            Assert.That(person.FirstNameChangedCounter, Is.EqualTo(1));

            person.FirstName = "test";

            Assert.That(viewModel.FirstName, Is.EqualTo("test"));
            Assert.That(person.FirstNameChangedCounter, Is.EqualTo(2));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_MissingModelName_ThrowsExceptionWithMultipleModels()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new Person();
            person.FirstName = "John";
            person.LastName = "Doe";
            person.ContactInfo.Street = "Unknown street";
            person.ContactInfo.City = "Unknown city";
            person.ContactInfo.Email = "john@doe.com";

            Assert.Throws<InvalidOperationException>(() => new TestViewModelWithImplicitModelMappingsWithMultipleModels(person, serviceProvider));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_PropertyChanges()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            const string FirstName = "first name";
            const string LastName = "last name";
            const uint Age1 = 1;
            const uint Age2 = 2;

            var person = new Person();
            var viewModel = new TestFeaturedViewModel(person, serviceProvider);

            Assert.That(person.FirstName, Is.EqualTo(string.Empty));
            Assert.That(viewModel.FirstName, Is.EqualTo(string.Empty));
            Assert.That(person.LastName, Is.EqualTo(string.Empty));
            Assert.That(viewModel.LastName, Is.EqualTo(string.Empty));

            Assert.That(person.Age, Is.EqualTo(0));
            Assert.That(viewModel.Age, Is.EqualTo(0));

            // Model to view model mapping
            person.FirstName = FirstName;
            Assert.That(person.FirstName, Is.EqualTo(FirstName));
            Assert.That(viewModel.FirstName, Is.EqualTo(FirstName));

            // View model to model mapping
            viewModel.LastName = LastName;
            Assert.That(person.LastName, Is.EqualTo(LastName));
            Assert.That(viewModel.LastName, Is.EqualTo(LastName));

            person.Age = Age1;
            Assert.That(person.Age, Is.EqualTo(Age1));
            Assert.That(viewModel.Age, Is.EqualTo(Age1));

            viewModel.Age = Age2;
            Assert.That(person.Age, Is.EqualTo(Age2));
            Assert.That(viewModel.Age, Is.EqualTo(Age2));
        }

        [TestCase]
        public void ViewModelWithMappingConverters()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            const string FirstName = "first_name";
            const string LastName = "last_name";
            const uint Age1 = 1;
            const uint Age2 = 2;

            var person = new Person();
            var viewModel = new TestViewModelWithMappingConverters(person, serviceProvider);

            Assert.That(person.FirstName, Is.EqualTo(string.Empty));
            Assert.That(viewModel.FirstName, Is.EqualTo(string.Empty));
            Assert.That(person.LastName, Is.EqualTo(string.Empty));
            Assert.That(viewModel.LastName, Is.EqualTo(string.Empty));

            Assert.That(viewModel.FullName, Is.EqualTo(string.Empty));

            Assert.That(person.Age, Is.EqualTo(0));
            Assert.That(viewModel.Age, Is.EqualTo("0"));

            // Model to view model mapping
            person.FirstName = FirstName;
            Assert.That(person.FirstName, Is.EqualTo(FirstName));
            Assert.That(viewModel.FirstName, Is.EqualTo(FirstName));
            Assert.That(viewModel.FullName, Is.EqualTo(FirstName));

            // View model to model mapping
            viewModel.LastName = LastName;
            Assert.That(person.LastName, Is.EqualTo(LastName));
            Assert.That(viewModel.LastName, Is.EqualTo(LastName));
            Assert.That(viewModel.FullName, Is.EqualTo(FirstName + " " + LastName));
            Assert.That(viewModel.FullNameWithCustomSeparator, Is.EqualTo(FirstName + ";" + LastName));

            person.Age = Age1;
            Assert.That(person.Age, Is.EqualTo(Age1));
            Assert.That(viewModel.Age, Is.EqualTo(Age1.ToString()));

            viewModel.Age = Age2.ToString();
            Assert.That(person.Age, Is.EqualTo(Age2));
            Assert.That(viewModel.Age, Is.EqualTo(Age2.ToString()));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_FieldErrors()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new Person();
            var viewModel = new TestFeaturedViewModel(person, serviceProvider);

            var personAsError = (IDataErrorInfo)person;
            var viewModelAsError = (IDataErrorInfo)viewModel;

            person.FirstName = "first name";

            Assert.That(personAsError[Person.FirstNameProperty.Name], Is.EqualTo(string.Empty));
            Assert.That(viewModelAsError[TestFeaturedViewModel.FirstNameProperty.Name], Is.EqualTo(string.Empty));

            person.FirstName = string.Empty;

            Assert.That(personAsError[Person.FirstNameProperty.Name], Is.Not.EqualTo(string.Empty));
            Assert.That(viewModelAsError[TestFeaturedViewModel.FirstNameProperty.Name], Is.Not.EqualTo(string.Empty));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_FieldWarnings()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new Person();
            var viewModel = new TestFeaturedViewModel(person, serviceProvider);

            var personAsWarning = (IDataWarningInfo)person;
            var viewModelAsWarning = (IDataWarningInfo)viewModel;

            person.FirstName = "first name";
            person.LastName = "last name";

            var validation = (IValidatableModel)viewModel;

            Assert.That(validation.HasErrors, Is.False);
            Assert.That(validation.HasWarnings, Is.True);
            Assert.That(personAsWarning[Person.MiddleNameProperty.Name], Is.Not.EqualTo(string.Empty));
            Assert.That(viewModelAsWarning[TestFeaturedViewModel.MiddleNameProperty.Name], Is.Not.EqualTo(string.Empty));

            person.MiddleName = "middle name";

            Assert.That(validation.HasErrors, Is.False);
            Assert.That(validation.HasWarnings, Is.False);
            Assert.That(personAsWarning[Person.MiddleNameProperty.Name], Is.EqualTo(string.Empty));
            Assert.That(viewModelAsWarning[TestFeaturedViewModel.MiddleNameProperty.Name], Is.EqualTo(string.Empty));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_BusinessErrors()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new Person();
            var viewModel = new TestFeaturedViewModel(person, serviceProvider);

            var personAsError = (IDataErrorInfo)person;
            var viewModelAsError = (IDataErrorInfo)viewModel;

            person.FirstName = "first name";
            person.LastName = "last name";

            Assert.That(personAsError.Error, Is.EqualTo(string.Empty));
            Assert.That(viewModelAsError.Error, Is.EqualTo(string.Empty));

            person.FirstName = string.Empty;

            Assert.That(personAsError.Error, Is.Not.EqualTo(string.Empty));
            Assert.That(viewModelAsError.Error, Is.Not.EqualTo(string.Empty));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_BusinessWarnings()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new Person();
            var viewModel = new TestFeaturedViewModel(person, serviceProvider);

            var personAsWarning = (IDataWarningInfo)person;
            var viewModelAsWarning = (IDataWarningInfo)viewModel;

            person.FirstName = "first name";
            person.LastName = "last name";

            Assert.That(personAsWarning.Warning, Is.Not.EqualTo(string.Empty));
            Assert.That(viewModelAsWarning.Warning, Is.Not.EqualTo(string.Empty));

            person.MiddleName = "middle name";

            Assert.That(personAsWarning.Warning, Is.EqualTo(string.Empty));
            Assert.That(viewModelAsWarning.Warning, Is.EqualTo(string.Empty));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_ValidateModelsOnInitialization()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new PersonWithDataAnnotations();
            var viewModel = new TestFeaturedViewModel(person, serviceProvider)
            {
                ValidateModelsOnInitializationWrapper = true
            };

            ((IValidatableModel)person).Validate(true);

            Assert.That(viewModel.GetValidationContext().GetValidationCount(), Is.Not.EqualTo(0));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_DoNotValidateModelsOnInitialization_UpdateViaViewModel()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new PersonWithDataAnnotations();
            var viewModel = new TestFeaturedViewModel(person, serviceProvider)
            {
                ValidateModelsOnInitializationWrapper = false
            };

            Assert.That(person.GetValidationContext().GetValidationCount(), Is.EqualTo(0));
            Assert.That(viewModel.GetValidationContext().GetValidationCount(), Is.EqualTo(0));

            viewModel.FirstName = null;

            Assert.That(person.GetValidationContext().GetValidationCount(), Is.Not.EqualTo(0));
            Assert.That(viewModel.GetValidationContext().GetValidationCount(), Is.Not.EqualTo(0));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_DoNotValidateModelsOnInitialization_UpdateViaModel()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new PersonWithDataAnnotations();
            var viewModel = new TestFeaturedViewModel(person, serviceProvider)
            {
                ValidateModelsOnInitializationWrapper = false
            };

            Assert.That(person.GetValidationContext().GetValidationCount(), Is.EqualTo(0));
            Assert.That(viewModel.GetValidationContext().GetValidationCount(), Is.EqualTo(0));

            person.FirstName = null;

            Assert.That(person.GetValidationContext().GetValidationCount(), Is.Not.EqualTo(0));
            Assert.That(viewModel.GetValidationContext().GetValidationCount(), Is.Not.EqualTo(0));
        }

        [TestCase]
        public async Task ViewModelWithViewModelToModelMappings_DoNotMapWhenViewModelIsClosedAsync()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new Person();
            var viewModel = new TestFeaturedViewModel(person, serviceProvider)
            {
                ValidateModelsOnInitializationWrapper = false
            };

            Assert.That(person.FirstName, Is.Not.EqualTo("test1"));
            viewModel.FirstName = "test1";
            Assert.That(person.FirstName, Is.EqualTo("test1"));

            await viewModel.CloseViewModelAsync(true);
            viewModel.FirstName = "test2";

            Assert.That(person.FirstName, Is.EqualTo("test1"));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_TwoWay_InitiatedFromModel()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new Person();
            var viewModel = new TestViewModelWithMappings(person, serviceProvider);

            Assert.That(person.FirstName, Is.EqualTo(string.Empty));
            Assert.That(viewModel.FirstNameAsTwoWay, Is.EqualTo(person.FirstName));

            person.FirstName = "geert";

            // Both must have changed
            Assert.That(person.FirstName, Is.EqualTo("geert"));
            Assert.That(viewModel.FirstNameAsTwoWay, Is.EqualTo(person.FirstName));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_TwoWay_InitiatedFromViewModel()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new Person();
            var viewModel = new TestViewModelWithMappings(person, serviceProvider);

            Assert.That(person.FirstName, Is.EqualTo(string.Empty));
            Assert.That(viewModel.FirstNameAsTwoWay, Is.EqualTo(person.FirstName));

            viewModel.FirstNameAsTwoWay = "geert";

            // Both must have changed
            Assert.That(viewModel.FirstNameAsTwoWay, Is.EqualTo("geert"));
            Assert.That(viewModel.FirstNameAsTwoWay, Is.EqualTo(person.FirstName));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_OneWay_InitiatedFromModel()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new Person();
            var viewModel = new TestViewModelWithMappings(person, serviceProvider);

            Assert.That(person.FirstName, Is.EqualTo(string.Empty));
            Assert.That(viewModel.FirstNameAsOneWay, Is.EqualTo(person.FirstName));

            person.FirstName = "geert";

            // Both must have changed
            Assert.That(person.FirstName, Is.EqualTo("geert"));
            Assert.That(viewModel.FirstNameAsOneWay, Is.EqualTo(person.FirstName));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_OneWay_InitiatedFromViewModel()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new Person();
            var viewModel = new TestViewModelWithMappings(person, serviceProvider);

            Assert.That(person.FirstName, Is.EqualTo(string.Empty));
            Assert.That(viewModel.FirstNameAsOneWay, Is.EqualTo(person.FirstName));

            viewModel.FirstNameAsOneWay = "geert";

            // Only view model must have changed
            Assert.That(viewModel.FirstNameAsOneWay, Is.EqualTo("geert"));
            Assert.That(viewModel.FirstNameAsOneWay, Is.Not.EqualTo(person.FirstName));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_OneWayToSource_InitiatedFromModel()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new Person();
            var viewModel = new TestViewModelWithMappings(person, serviceProvider);

            Assert.That(person.FirstName, Is.EqualTo(string.Empty));
            Assert.That(viewModel.FirstNameAsOneWayToSource, Is.EqualTo(person.FirstName));

            person.FirstName = "geert";

            // Only model must have changed
            Assert.That(person.FirstName, Is.EqualTo("geert"));
            Assert.That(viewModel.FirstNameAsOneWayToSource, Is.Not.EqualTo(person.FirstName));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_OneWayToSource_InitiatedFromViewModel()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new Person();
            var viewModel = new TestViewModelWithMappings(person, serviceProvider);

            Assert.That(person.FirstName, Is.EqualTo(string.Empty));
            Assert.That(viewModel.FirstNameAsOneWayToSource, Is.EqualTo(person.FirstName));

            viewModel.FirstNameAsOneWayToSource = "geert";

            // Both must have changed
            Assert.That(viewModel.FirstNameAsOneWayToSource, Is.EqualTo("geert"));
            Assert.That(viewModel.FirstNameAsOneWayToSource, Is.EqualTo(person.FirstName));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_Explicit_InitiatedFromModel()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new Person();
            var viewModel = new TestViewModelWithMappings(person, serviceProvider);

            Assert.That(person.FirstName, Is.EqualTo(string.Empty));
            Assert.That(viewModel.FirstNameAsExplicit, Is.EqualTo(person.FirstName));

            person.FirstName = "geert";

            // When initiated from model => VM should change
            Assert.That(person.FirstName, Is.EqualTo("geert"));
            Assert.That(viewModel.FirstNameAsExplicit, Is.EqualTo(person.FirstName));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_Explicit_InitiatedFromViewModel()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new Person();
            var viewModel = new TestViewModelWithMappings(person, serviceProvider);

            Assert.That(person.FirstName, Is.EqualTo(string.Empty));
            Assert.That(viewModel.FirstNameAsExplicit, Is.EqualTo(person.FirstName));

            viewModel.FirstNameAsExplicit = "geert";

            // When initiated from VM => nothing should change
            Assert.That(viewModel.FirstNameAsExplicit, Is.EqualTo("geert"));
            Assert.That(viewModel.FirstNameAsExplicit, Is.Not.EqualTo(person.FirstName));
        }

        [TestCase]
        public void ViewModelWithViewModelToModelMappings_Explicit_InitiatedManually()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddCatelCore();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var person = new Person();
            var viewModel = new TestViewModelWithMappings(person, serviceProvider);

            Assert.That(person.FirstName, Is.EqualTo(string.Empty));
            Assert.That(viewModel.FirstNameAsExplicit, Is.EqualTo(person.FirstName));

            viewModel.FirstNameAsExplicit = "geert";
            viewModel.UpdateExplicitMappings();

            // Both must have changed
            Assert.That(viewModel.FirstNameAsExplicit, Is.EqualTo("geert"));
            Assert.That(viewModel.FirstNameAsExplicit, Is.EqualTo(person.FirstName));
        }
    }
}
