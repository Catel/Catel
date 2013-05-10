namespace Catel.Test.MVVM.ViewModels
{
    using Auditing;
    using Catel.Data;
    using Catel.IoC;
    using Catel.MVVM.Auditing;
    using System;
    using System.ComponentModel;
    using System.Threading;
    using Catel.MVVM;
    using Catel.MVVM.Services;
    using TestClasses;

    using TestViewModel = TestClasses.TestViewModel;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class ViewModelBaseTest
    {
        [TestClass]
        public class TheHasDirtyModelsProperty
        {
            [TestMethod]
            public void IsModelRegistered_ExistingModel()
            {
                var person = new Person();
                person.FirstName = "first name";
                person.LastName = "last name";

                var viewModel = new TestViewModel(person);

                Assert.IsFalse(viewModel.HasDirtyModel);

                person.FirstName = "new first name";

                Assert.IsTrue(viewModel.HasDirtyModel);
            }            
        }

        [TestMethod]
        public void Constructor_InjectedServiceLocator()
        {
            var serviceLocator = new ServiceLocator();
            var messageService = new MessageService();
            serviceLocator.RegisterInstance<IMessageService>(messageService);

            var navigationService = new NavigationService();
            serviceLocator.RegisterInstance<INavigationService>(navigationService);

            var viewModel = new TestViewModel(serviceLocator);
            Assert.AreEqual(messageService, viewModel.GetService<IMessageService>());
            Assert.IsTrue(ReferenceEquals(messageService, viewModel.GetService<IMessageService>()));
            Assert.AreEqual(navigationService, viewModel.GetService<INavigationService>());
            Assert.IsTrue(ReferenceEquals(navigationService, viewModel.GetService<INavigationService>()));
        }

        #region ViewModelToModel mappings
        [TestMethod]
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

            var viewModel = new MultipleModelMappingsViewModel(firstPerson);

            Assert.IsNotNull(viewModel.Person);
            Assert.IsNotNull(viewModel.ContactInfo);
            Assert.AreEqual("john@doe.com", viewModel.Email);

            viewModel.Person = secondPerson;

            Assert.IsNotNull(viewModel.Person);
            Assert.IsNotNull(viewModel.ContactInfo);
            Assert.AreEqual("Another email", viewModel.Email);
        }

        [TestMethod]
        public void ViewModelWithViewModelToModelMappings_PropertyChanges()
        {
            const string FirstName = "first name";
            const string LastName = "last name";

            var person = new Person();
            var viewModel = new TestViewModel(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(string.Empty, viewModel.FirstName);
            Assert.AreEqual(string.Empty, person.LastName);
            Assert.AreEqual(string.Empty, viewModel.LastName);

            // Model to view model mapping
            person.FirstName = FirstName;
            Assert.AreEqual(FirstName, person.FirstName);
            Assert.AreEqual(FirstName, viewModel.FirstName);

            // View model to model mapping
            viewModel.LastName = LastName;
            Assert.AreEqual(LastName, person.LastName);
            Assert.AreEqual(LastName, viewModel.LastName);
        }

#if !WINDOWS_PHONE
        [TestMethod]
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

        [TestMethod]
        public void ViewModelWithViewModelToModelMappings_FieldWarnings()
        {
            var person = new Person();
            var viewModel = new TestViewModel(person);

            var personAsWarning = (IDataWarningInfo)person;
            var viewModelAsWarning = (IDataWarningInfo)viewModel;

            person.FirstName = "first name";
            person.LastName = "last name";

            Assert.IsFalse(viewModel.HasErrors);
            Assert.IsTrue(viewModel.HasWarnings);
            Assert.AreNotEqual(string.Empty, personAsWarning[Person.MiddleNameProperty.Name]);
            Assert.AreNotEqual(string.Empty, viewModelAsWarning[TestViewModel.MiddleNameProperty.Name]);

            person.MiddleName = "middle name";

            Assert.IsFalse(viewModel.HasErrors);
            Assert.IsFalse(viewModel.HasWarnings);
            Assert.AreEqual(string.Empty, personAsWarning[Person.MiddleNameProperty.Name]);
            Assert.AreEqual(string.Empty, viewModelAsWarning[TestViewModel.MiddleNameProperty.Name]);
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void ViewModelWithViewModelToModelMappings_ValidateModelsOnInitialization()
        {
            var person = new PersonWithDataAnnotations();
            var viewModel = new TestViewModel(person, true);

            Assert.AreNotEqual(0, viewModel.ValidationContext.GetValidationCount());
        }

        [TestMethod]
        public void ViewModelWithViewModelToModelMappings_DoNotValidateModelsOnInitialization_UpdateViaViewModel()
        {
            var person = new PersonWithDataAnnotations();
            var viewModel = new TestViewModel(person, false);

            Assert.AreEqual(0, person.ValidationContext.GetValidationCount());
            Assert.AreEqual(0, viewModel.ValidationContext.GetValidationCount());

            viewModel.FirstName = null;

            Assert.AreNotEqual(0, person.ValidationContext.GetValidationCount());
            Assert.AreNotEqual(0, viewModel.ValidationContext.GetValidationCount());
        }

        [TestMethod]
        public void ViewModelWithViewModelToModelMappings_DoNotValidateModelsOnInitialization_UpdateViaModel()
        {
            var person = new PersonWithDataAnnotations();
            var viewModel = new TestViewModel(person, false);

            Assert.AreEqual(0, person.ValidationContext.GetValidationCount());
            Assert.AreEqual(0, viewModel.ValidationContext.GetValidationCount());

            person.FirstName = null;

            Assert.AreNotEqual(0, person.ValidationContext.GetValidationCount());
            Assert.AreNotEqual(0, viewModel.ValidationContext.GetValidationCount());
        }
#endif

        [TestMethod]
        public void ViewModelWithViewModelToModelMappings_DoNotMapWhenViewModelIsClosed()
        {
            var person = new Person();
            var viewModel = new TestViewModel(person, true);

            Assert.AreNotEqual("test1", person.FirstName);
            viewModel.FirstName = "test1";
            Assert.AreEqual("test1", person.FirstName);

            viewModel.CloseViewModel(true);
            viewModel.FirstName = "test2";

            Assert.AreEqual("test1", person.FirstName);
        }

        [TestMethod]
        public void ViewModelWithViewModelToModelMappings_TwoWay_InitiatedFromModel()
        {
            var person = new Person();
            var viewModel = new MappingViewModel(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsTwoWay);

            person.FirstName = "geert";

            // Both must have changed
            Assert.AreEqual("geert", person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsTwoWay);
        }

        [TestMethod]
        public void ViewModelWithViewModelToModelMappings_TwoWay_InitiatedFromViewModel()
        {
            var person = new Person();
            var viewModel = new MappingViewModel(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsTwoWay);

            viewModel.FirstNameAsTwoWay = "geert";

            // Both must have changed
            Assert.AreEqual("geert", viewModel.FirstNameAsTwoWay);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsTwoWay);
        }

        [TestMethod]
        public void ViewModelWithViewModelToModelMappings_OneWay_InitiatedFromModel()
        {
            var person = new Person();
            var viewModel = new MappingViewModel(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsOneWay);

            person.FirstName = "geert";

            // Both must have changed
            Assert.AreEqual("geert", person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsOneWay);
        }

        [TestMethod]
        public void ViewModelWithViewModelToModelMappings_OneWay_InitiatedFromViewModel()
        {
            var person = new Person();
            var viewModel = new MappingViewModel(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsOneWay);

            viewModel.FirstNameAsOneWay = "geert";

            // Only view model must have changed
            Assert.AreEqual("geert", viewModel.FirstNameAsOneWay);
            Assert.AreNotEqual(person.FirstName, viewModel.FirstNameAsOneWay);
        }

        [TestMethod]
        public void ViewModelWithViewModelToModelMappings_OneWayToSource_InitiatedFromModel()
        {
            var person = new Person();
            var viewModel = new MappingViewModel(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsOneWayToSource);

            person.FirstName = "geert";

            // Only model must have changed
            Assert.AreEqual("geert", person.FirstName);
            Assert.AreNotEqual(person.FirstName, viewModel.FirstNameAsOneWayToSource);
        }

        [TestMethod]
        public void ViewModelWithViewModelToModelMappings_OneWayToSource_InitiatedFromViewModel()
        {
            var person = new Person();
            var viewModel = new MappingViewModel(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsOneWayToSource);

            viewModel.FirstNameAsOneWayToSource = "geert";

            // Both must have changed
            Assert.AreEqual("geert", viewModel.FirstNameAsOneWayToSource);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsOneWayToSource);
        }

        [TestMethod]
        public void ViewModelWithViewModelToModelMappings_Explicit_InitiatedFromModel()
        {
            var person = new Person();
            var viewModel = new MappingViewModel(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsExplicit);

            person.FirstName = "geert";

            // Only model must have changed
            Assert.AreEqual("geert", person.FirstName);
            Assert.AreNotEqual(person.FirstName, viewModel.FirstNameAsExplicit);
        }

        [TestMethod]
        public void ViewModelWithViewModelToModelMappings_Explicit_InitiatedFromViewModel()
        {
            var person = new Person();
            var viewModel = new MappingViewModel(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsExplicit);

            viewModel.FirstNameAsExplicit = "geert";

            // Only view model model must have changed
            Assert.AreEqual("geert", viewModel.FirstNameAsExplicit);
            Assert.AreNotEqual(person.FirstName, viewModel.FirstNameAsExplicit);
        }

        [TestMethod]
        public void ViewModelWithViewModelToModelMappings_Explicit_InitiatedManually()
        {
            var person = new Person();
            var viewModel = new MappingViewModel(person);

            Assert.AreEqual(string.Empty, person.FirstName);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsExplicit);

            viewModel.FirstNameAsExplicit = "geert";
            viewModel.UpdateExplicitMappings();

            // Both must have changed
            Assert.AreEqual("geert", viewModel.FirstNameAsExplicit);
            Assert.AreEqual(person.FirstName, viewModel.FirstNameAsExplicit);
        }
        #endregion

        #region Child view models
        [TestMethod]
        public void SetParentviewModel()
        {
            var viewModel = new TestViewModel();
            var parentViewModel = new InterestedViewModel();

            Assert.IsNull(viewModel.GetParentViewModelForTest());
            ((IRelationalViewModel)viewModel).SetParentViewModel(parentViewModel);
            Assert.AreEqual(parentViewModel, viewModel.GetParentViewModelForTest());
        }

        [TestMethod]
        public void RegisterChildViewModel_Null()
        {
            var viewModel = new TestViewModel();

            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => ((IRelationalViewModel)viewModel).RegisterChildViewModel(null));
        }

        /// <summary>
        /// Checks whether a child view model is correctly subscribed by making sure the parent view model is also
        /// being validated. Then, it unsubscribes the child view model by closing it.
        /// </summary>
        [TestMethod]
        public void RegisterChildViewModel_RemovedViaClosingChildViewModel()
        {
            bool validationTriggered = false;
            ManualResetEvent validatedEvent = new ManualResetEvent(false);

            Person person = new Person();
            person.FirstName = "first name";
            person.LastName = "last name";

            var viewModel = new TestViewModel();
            var childViewModel = new TestViewModel(person);

            Assert.IsFalse(childViewModel.HasErrors);

            ((IRelationalViewModel)viewModel).RegisterChildViewModel(childViewModel);
            viewModel.Validating += delegate
                                        {
                                            validationTriggered = true;
                                            validatedEvent.Set();
                                        };

            childViewModel.FirstName = string.Empty;

#if NET
            validatedEvent.WaitOne(2000, false);
#else
            validatedEvent.WaitOne(2000);
#endif
            Assert.IsTrue(validationTriggered, "Validating event is not triggered");

            childViewModel.CloseViewModel(null);

            validationTriggered = false;
            validatedEvent.Reset();

#if NET
            validatedEvent.WaitOne(2000, false);
#else
            validatedEvent.WaitOne(2000);
#endif
            Assert.IsFalse(validationTriggered, "Validating event should not be triggered because child view model is removed");
        }

        /// <summary>
        /// Checks whether a child view model is correctly subscribed by making sure the parent view model is also
        /// being validated. Then, it unsubscribes the child view model by calling UnregisterChildViewModel.
        /// </summary>
        [TestMethod]
        public void RegisterChildViewModel_RemovedViaUnregisterChildViewModel()
        {
            bool validationTriggered = false;
            ManualResetEvent validatedEvent = new ManualResetEvent(false);

            Person person = new Person();
            person.FirstName = "first name";
            person.LastName = "last name";

            var viewModel = new TestViewModel();
            var childViewModel = new TestViewModel(person);

            Assert.IsFalse(childViewModel.HasErrors);

            ((IRelationalViewModel)viewModel).RegisterChildViewModel(childViewModel);
            viewModel.Validating += delegate
            {
                validationTriggered = true;
                validatedEvent.Set();
            };

            childViewModel.FirstName = string.Empty;

#if NET
            validatedEvent.WaitOne(2000, false);
#else
            validatedEvent.WaitOne(2000);
#endif
            Assert.IsTrue(validationTriggered, "Validating event is not triggered");

            ((IRelationalViewModel)viewModel).UnregisterChildViewModel(childViewModel);

            validationTriggered = false;
            validatedEvent.Reset();

#if NET
            validatedEvent.WaitOne(2000, false);
#else
            validatedEvent.WaitOne(2000);
#endif
            Assert.IsFalse(validationTriggered, "Validating event should not be triggered because child view model is removed");
        }

        [TestMethod]
        public void ChildViewModelUpdatesValidation()
        {
            Person person = new Person();
            person.LastName = "last name";

            var viewModel = new TestViewModel();
            var childViewModel = new TestViewModel(person);

            ((IRelationalViewModel)viewModel).RegisterChildViewModel(childViewModel);

            Assert.IsTrue(viewModel.HasErrors);
            Assert.IsTrue(childViewModel.HasErrors);

            person.FirstName = "first name";

            Assert.IsFalse(viewModel.HasErrors);
            Assert.IsFalse(childViewModel.HasErrors);

            person.FirstName = string.Empty;

            Assert.IsTrue(viewModel.HasErrors);
            Assert.IsTrue(childViewModel.HasErrors);
        }
        #endregion

        [TestMethod]
        public void GetAllModels()
        {
            Person person = new Person();
            person.FirstName = "first name";
            person.LastName = "last name";

            var viewModel = new TestViewModel(person);

            object[] models = viewModel.GetAllModelsForTest();
            Assert.AreEqual(2, models.Length);
            Assert.AreEqual(person, models[0]);
        }

        [TestMethod]
        public void ModelsSavedBySave()
        {
            Person person = new Person();
            person.FirstName = "first name";
            person.LastName = "last name";

            var viewModel = new TestViewModel(person);
            Assert.IsTrue(person.IsInEditSession);

            viewModel.FirstName = "new first name";

            viewModel.SaveAndCloseViewModel();

            Assert.IsFalse(person.IsInEditSession);
            Assert.AreEqual("new first name", person.FirstName);
        }

        [TestMethod]
        public void ModelsCanceledByCancel()
        {
            Person person = new Person();
            person.FirstName = "first name";
            person.LastName = "last name";

            var viewModel = new TestViewModel(person);
            Assert.IsTrue(person.IsInEditSession);

            viewModel.FirstName = "new first name";

            viewModel.CancelAndCloseViewModel();

            Assert.IsFalse(person.IsInEditSession);
            Assert.AreEqual("first name", person.FirstName);            
        }

        [TestMethod]
        public void IsModelRegistered_ExistingModel()
        {
            Person person = new Person();
            person.FirstName = "first name";
            person.LastName = "last name";

            var viewModel = new TestViewModel(person);

            Assert.IsTrue(viewModel.IsModelRegisteredForTest("Person"));
        }

        [TestMethod]
        public void IsModelRegistered_NonExistingModel()
        {
            Person person = new Person();
            person.FirstName = "first name";
            person.LastName = "last name";

            var viewModel = new TestViewModel(person);

            Assert.IsFalse(viewModel.IsModelRegisteredForTest("SecondPerson"));
        }

        [TestMethod]
        public void InvalidateCommands_Manual()
        {
            bool canExecuteChangedTriggered = false;
            ManualResetEvent canExecuteChangedEvent = new ManualResetEvent(false);

            var viewModel = new TestViewModel();
            viewModel.SetInvalidateCommandsOnPropertyChanged(false);

            ICatelCommand command = viewModel.GenerateData;
            command.CanExecuteChanged += delegate
            {
                canExecuteChangedTriggered = true;
                canExecuteChangedEvent.Set();
            };

            // By default, command can be executed
            Assert.IsTrue(viewModel.GenerateData.CanExecute(null));

            viewModel.FirstName = "first name";

            Assert.IsFalse(viewModel.GenerateData.CanExecute(null));
#if NET
            canExecuteChangedEvent.WaitOne(1000, false);
#else
            canExecuteChangedEvent.WaitOne(1000);
#endif
            Assert.IsFalse(canExecuteChangedTriggered);
        }

        [TestMethod]
        public void InvalidateCommands_AutomaticByPropertyChange()
        {
            bool canExecuteChangedTriggered = false;
            var canExecuteChangedEvent = new ManualResetEvent(false);

            var viewModel = new TestViewModel();
            viewModel.SetInvalidateCommandsOnPropertyChanged(true);

            ICatelCommand command = viewModel.GenerateData;
            command.CanExecuteChanged += delegate
            {
                canExecuteChangedTriggered = true;
                canExecuteChangedEvent.Set();
            };

            // By default, command can be executed
            Assert.IsTrue(viewModel.GenerateData.CanExecute(null));

            viewModel.FirstName = "first name";

            Assert.IsFalse(viewModel.GenerateData.CanExecute(null));
#if NET
            canExecuteChangedEvent.WaitOne(1000, false);
#else
            canExecuteChangedEvent.WaitOne(1000);
#endif
            Assert.IsTrue(canExecuteChangedTriggered);
        }

        #region Validation
        //[TestMethod]
        //public void DeferredValidation()
        //{
        //    var viewModel = new TestViewModelWithDeferredValidation();
        //    viewModel.FirstName = null;

        //    Assert.IsTrue(viewModel.HasErrors);
        //    Assert.AreEqual(string.Empty, ((IDataErrorInfo)viewModel)["FirstName"]);
        //    Assert.IsFalse(viewModel.SaveViewModel());
        //    Assert.IsTrue(viewModel.HasErrors);
        //    Assert.AreNotEqual(string.Empty, ((IDataErrorInfo)viewModel)["FirstName"]);
        //}

#if !WINDOWS_PHONE
        [TestMethod]
        public void ModelValidation_NotifyDataErrorInfo_FieldErrors()
        {
            var testViewModel = new TestViewModel();

            Assert.IsFalse(testViewModel.HasErrors);

            testViewModel.SpecialValidationModel = new SpecialValidationModel();

            Assert.IsFalse(testViewModel.HasErrors);

            testViewModel.SpecialValidationModel.FieldErrorWhenEmpty = string.Empty;

            Assert.IsTrue(testViewModel.HasErrors);
            Assert.AreNotEqual(string.Empty, ((IDataErrorInfo)testViewModel)["FieldErrorWhenEmpty"]);

            testViewModel.SpecialValidationModel.FieldErrorWhenEmpty = "no error";

            Assert.IsFalse(testViewModel.HasErrors);
        }

        [TestMethod]
        public void ModelValidation_NotifyDataErrorInfo_BusinessErrors()
        {
            var testViewModel = new TestViewModel();

            Assert.IsFalse(testViewModel.HasErrors);

            testViewModel.SpecialValidationModel = new SpecialValidationModel();

            Assert.IsFalse(testViewModel.HasErrors);

            testViewModel.SpecialValidationModel.BusinessRuleErrorWhenEmpty = string.Empty;

            Assert.IsTrue(testViewModel.HasErrors);
            Assert.AreNotEqual(string.Empty, ((IDataErrorInfo)testViewModel).Error);

            testViewModel.SpecialValidationModel.BusinessRuleErrorWhenEmpty = "no error";

            Assert.IsFalse(testViewModel.HasErrors);
        }

        [TestMethod]
        public void ModelValidation_NotifyDataWarningInfo_FieldWarnings()
        {
            var testViewModel = new TestViewModel();

            Assert.IsFalse(testViewModel.HasWarnings);

            testViewModel.SpecialValidationModel = new SpecialValidationModel();

            Assert.IsFalse(testViewModel.HasWarnings);

            testViewModel.SpecialValidationModel.FieldWarningWhenEmpty = string.Empty;

            Assert.IsTrue(testViewModel.HasWarnings);
            Assert.AreNotEqual(string.Empty, ((IDataWarningInfo)testViewModel)["FieldWarningWhenEmpty"]);

            testViewModel.SpecialValidationModel.FieldWarningWhenEmpty = "no warning";

            Assert.IsFalse(testViewModel.HasWarnings);
        }

        [TestMethod]
        public void ModelValidation_NotifyDataWarningInfo_BusinessWarnings()
        {
            var testViewModel = new TestViewModel();

            Assert.IsFalse(testViewModel.HasWarnings);

            testViewModel.SpecialValidationModel = new SpecialValidationModel();

            Assert.IsFalse(testViewModel.HasWarnings);

            testViewModel.SpecialValidationModel.BusinessRuleWarningWhenEmpty = string.Empty;

            Assert.IsTrue(testViewModel.HasWarnings);
            Assert.AreNotEqual(string.Empty, ((IDataWarningInfo)testViewModel).Warning);

            testViewModel.SpecialValidationModel.BusinessRuleWarningWhenEmpty = "no warning";

            Assert.IsFalse(testViewModel.HasWarnings);
        }
#endif

        [TestMethod]
        public void ValidationToViewModel_WithoutTagFiltering()
        {
            var viewModel = new TestViewModelWithValidationTags();
            viewModel.ValidateViewModel();

            var summary = viewModel.ValidationSummaryWithoutTagFiltering;

            Assert.IsTrue(viewModel.HasErrors);
            Assert.IsNotNull(summary);
            Assert.AreEqual(2, summary.FieldErrors.Count);
        }

        [TestMethod]
        public void ValidationToViewModel_NullTag()
        {
            var viewModel = new TestViewModelWithValidationTags();
            viewModel.ValidateViewModel();

            var summary = viewModel.ValidationSummaryWithNullTag;

            Assert.IsTrue(viewModel.HasErrors);
            Assert.IsNotNull(summary);
            Assert.AreEqual(0, summary.FieldErrors.Count);  
        }

        [TestMethod]
        public void ValidationToViewModel_NonExistingTag()
        {
            var viewModel = new TestViewModelWithValidationTags();
            viewModel.ValidateViewModel();

            var summary = viewModel.ValidationSummaryWithNonExistingTag;

            Assert.IsTrue(viewModel.HasErrors);
            Assert.IsNotNull(summary);
            Assert.AreEqual(0, summary.FieldErrors.Count);  
        }

        [TestMethod]
        public void ValidationToViewModel_ExistingTag()
        {
            var viewModel = new TestViewModelWithValidationTags();
            viewModel.ValidateViewModel();

            var summary = viewModel.PersonValidationSummary;

            Assert.IsTrue(viewModel.HasErrors);
            Assert.IsNotNull(summary);
            Assert.AreEqual(2, summary.FieldErrors.Count);  
        }
        #endregion

#if NET
        [TestMethod]
        public void ExposeAttribute_NoModelExposed()
        {
            ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => new ExposingViewModelWithNoModelDefined());
        }

        [TestMethod]
        public void ExposeAttribute_ExposedPropertyDoesNotExistOnModel()
        {
            ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => new ExposingViewModelWithInvalidProperty());
        }

        [TestMethod]
        public void ExposeAttribute_ExposedPropertyAlreadyExistsOnViewModel()
        {
            var vm = new ExposingViewModelWithExistingProperty();

            // The FirstName property is manually registered on the VM. Let's check if
            // the ExposeAttribute implicitly sets the ViewModelToModelAttribute for this
            // property
            vm.FirstName = "test";

            Assert.AreEqual(vm.FirstName, vm.Person.FirstName);
        }

        [TestMethod]
        public void ExposeAttribute_ReadOnlyPropertyOnModelDefinedAsNotReadOnly()
        {
            ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => new ExposingViewModelWithInvalidReadonlyMapping());
        }

        [TestMethod]
        public void ExposeAttribute_CheckIfPropertiesAreRegistered()
        {
            var vm = new ExposingViewModel() as IModel;

            Assert.IsNotNull(vm.GetPropertyType("FirstName"));
            Assert.IsNotNull(vm.GetPropertyType("MiddleName"));
            Assert.IsNotNull(vm.GetPropertyType("LastName"));
        }

        [TestMethod]
        public void ExposeAttribute_Validation()
        {
            var vm = new ExposingViewModel();

            var vmAsDataErrorInfo = (IDataErrorInfo)vm;

            Assert.AreNotEqual(string.Empty, vmAsDataErrorInfo["FirstName"]);
            Assert.AreNotEqual(string.Empty, vmAsDataErrorInfo["LastName"]);

            vm.Person.FirstName = "john";
            vm.Person.LastName = "doe";

            Assert.AreEqual(string.Empty, vmAsDataErrorInfo["FirstName"]);
            Assert.AreEqual(string.Empty, vmAsDataErrorInfo["LastName"]);
        }
#endif

        [TestMethod]
        public void CancelAfterCloseProtection()
        {
            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var vm = new TestViewModel();
            
            Assert.AreEqual(false, auditor.OnViewModelCanceledCalled);
            Assert.AreEqual(false, auditor.OnViewModelClosedCalled);

            vm.CancelAndCloseViewModel();

            Assert.AreEqual(true, auditor.OnViewModelCanceledCalled);
            Assert.AreEqual(true, auditor.OnViewModelClosedCalled);

            auditor.OnViewModelCanceledCalled = false;
            auditor.OnViewModelClosedCalled = false;

            vm.CancelAndCloseViewModel();

            Assert.AreEqual(false, auditor.OnViewModelCanceledCalled);
            Assert.AreEqual(false, auditor.OnViewModelClosedCalled);
        }

        [TestMethod]
        public void SaveAfterCloseProtection()
        {
            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var vm = new TestViewModel();

            Assert.AreEqual(false, auditor.OnViewModelSavedCalled);
            Assert.AreEqual(false, auditor.OnViewModelClosedCalled);

            vm.SaveAndCloseViewModel();

            Assert.AreEqual(true, auditor.OnViewModelSavedCalled);
            Assert.AreEqual(true, auditor.OnViewModelClosedCalled);

            auditor.OnViewModelSavedCalled = false;
            auditor.OnViewModelClosedCalled = false;

            vm.SaveAndCloseViewModel();

            Assert.AreEqual(false, auditor.OnViewModelSavedCalled);
            Assert.AreEqual(false, auditor.OnViewModelClosedCalled);
        }

        [TestMethod]
        public void CloseAfterCloseProtection()
        {
            var auditor = new TestAuditor();
            AuditingManager.RegisterAuditor(auditor);

            var vm = new TestViewModel();

            Assert.AreEqual(false, auditor.OnViewModelClosedCalled);

            vm.CloseViewModel(null);

            Assert.AreEqual(true, auditor.OnViewModelClosedCalled);

            auditor.OnViewModelClosedCalled = false;

            vm.CloseViewModel(null);

            Assert.AreEqual(false, auditor.OnViewModelClosedCalled);
        }

#if WINDOWS_PHONE
        [TestMethod]
        public void Tombstoning_AutomaticRecovery()
        {
            var vm = new TestViewModel();
            vm.FirstName = "John";
            vm.LastName = "Doe";

            var data = vm.SerializeForTombstoning();
            var recoveredVm = ViewModelBase.DeserializeFromTombstoning(typeof(TestViewModel), data);

            Assert.AreEqual(vm, recoveredVm);
        }
#endif
    }
}
