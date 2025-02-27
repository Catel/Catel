﻿namespace Catel.Tests.MVVM.ViewModels.TestClasses
{
    using System;
    using Catel.Data;
    using Catel.MVVM;
    using Catel.Services;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Test view model.
    /// </summary>
    public class TestViewModel : ViewModelBase
    {
        public TestViewModel(IServiceProvider serviceProvider)
            : this(null, null, serviceProvider)
        {
        }

        public TestViewModel(IPerson person, IServiceProvider serviceProvider)
            : this(person, serviceProvider)
        {
        }

        public TestViewModel(SpecialValidationModel specialValidationModel, IServiceProvider serviceProvider)
            : this(null, specialValidationModel, serviceProvider)
        {
        }

        private TestViewModel(IPerson person, SpecialValidationModel specialValidationModel, IServiceProvider serviceProvider)
            : base(serviceProvider, serviceProvider.GetRequiredService<IObjectAdapter>(), serviceProvider.GetRequiredService<Catel.Runtime.Serialization.ISerializer>(),
                  serviceProvider.GetRequiredService<IDispatcherService>(), serviceProvider.GetRequiredService<IViewModelManager>())
        {
            Person = person;
            SpecialValidationModel = specialValidationModel;

            GenerateData = new Command<object, object>(OnGenerateDataExecute, OnGenerateDataCanExecute);
            GenerateData.AutomaticallyDispatchEvents = false;

            DeferValidationUntilFirstSaveCall = false;
        }

        public bool ValidateModelsOnInitializationWrapper
        {
            get { return ValidateModelsOnInitialization; }
            set { ValidateModelsOnInitialization = value; }
        }

        public override string Title
        {
            get { return "View model title"; }
        }

        #region Models

        #region Constants
        /// <summary>
        /// Register the Person property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData PersonProperty = RegisterProperty<IPerson>("Person");

        /// <summary>
        /// Register the SpecialValidationModel property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData SpecialValidationModelProperty = RegisterProperty<SpecialValidationModel>("SpecialValidationModel");
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the person.
        /// </summary>
        [Model]
        public IPerson Person
        {
            get { return GetValue<IPerson>(PersonProperty); }
            private set { SetValue(PersonProperty, value); }
        }

        /// <summary>
        /// Gets or sets the special validation model.
        /// </summary>
        [Model]
        public SpecialValidationModel SpecialValidationModel
        {
            get { return GetValue<SpecialValidationModel>(SpecialValidationModelProperty); }
            set { SetValue(SpecialValidationModelProperty, value); }
        }
        #endregion

        #endregion

        #region View model

        #region Constants
        /// <summary>
        /// Register the FirstName property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData FirstNameProperty = RegisterProperty<string>("FirstName");

        /// <summary>
        /// Register the MiddleName property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData MiddleNameProperty = RegisterProperty<string>("MiddleName");

        /// <summary>
        /// Register the LastName property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData LastNameProperty = RegisterProperty<string>("LastName");

        /// <summary>
        /// Register the FieldErrorWhenEmpty property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData FieldErrorWhenEmptyProperty = RegisterProperty<string>("FieldErrorWhenEmpty");

        /// <summary>
        /// Register the FieldWarningWhenEmpty property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData FieldWarningWhenEmptyProperty = RegisterProperty<string>("FieldWarningWhenEmpty");

        /// <summary>
        /// Register the BusinessRuleErrorWhenEmpty property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData BusinessRuleErrorWhenEmptyProperty = RegisterProperty<string>("BusinessRuleErrorWhenEmpty");

        /// <summary>
        /// Register the BusinessRuleWarningWhenEmpty property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData BusinessRuleWarningWhenEmptyProperty = RegisterProperty<string>("BusinessRuleWarningWhenEmpty");

        /// <summary>Register the Age property so it is known in the class.</summary>
        public static readonly IPropertyData AgeProperty = RegisterProperty<TestViewModel, uint>(model => model.Age);
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [ViewModelToModel("Person")]
        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the middle name.
        /// </summary>
        [ViewModelToModel("Person")]
        public string MiddleName
        {
            get { return GetValue<string>(MiddleNameProperty); }
            set { SetValue(MiddleNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [ViewModelToModel("Person")]
        public string LastName
        {
            get { return GetValue<string>(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }

        [ViewModelToModel("Person")]
        public uint Age
        {
            get { return GetValue<uint>(AgeProperty); }
            set { SetValue(AgeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the field error when empty.
        /// </summary>
        [ViewModelToModel("SpecialValidationModel")]
        public string FieldErrorWhenEmpty
        {
            get { return GetValue<string>(FieldErrorWhenEmptyProperty); }
            set { SetValue(FieldErrorWhenEmptyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the field warning when empty.
        /// </summary>
        [ViewModelToModel("SpecialValidationModel")]
        public string FieldWarningWhenEmpty
        {
            get { return GetValue<string>(FieldWarningWhenEmptyProperty); }
            set { SetValue(FieldWarningWhenEmptyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the business rule error when empty.
        /// </summary>
        [ViewModelToModel("SpecialValidationModel")]
        public string BusinessRuleErrorWhenEmpty
        {
            get { return GetValue<string>(BusinessRuleErrorWhenEmptyProperty); }
            set { SetValue(BusinessRuleErrorWhenEmptyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the business rule warning when empty.
        /// </summary>
        [ViewModelToModel("SpecialValidationModel")]
        public string BusinessRuleWarningWhenEmpty
        {
            get { return GetValue<string>(BusinessRuleWarningWhenEmptyProperty); }
            set { SetValue(BusinessRuleWarningWhenEmptyProperty, value); }
        }
        #endregion

        #endregion

        #region Commands

        #region Properties
        /// <summary>
        /// Gets the GenerateData command.
        /// </summary>
        public Command<object, object> GenerateData { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Method to check whether the GenerateData command can be executed.
        /// </summary>
        /// <param name="parameter">The parameter of the command.</param>
        private bool OnGenerateDataCanExecute(object parameter)
        {
            if (!string.IsNullOrEmpty(FirstName))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(LastName))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Method to invoke when the GenerateData command is executed.
        /// </summary>
        /// <param name="parameter">The parameter of the command.</param>
        private void OnGenerateDataExecute(object parameter)
        {
            FirstName = "generated first name";
            LastName = "generated last name";
        }
        #endregion

        #endregion

        #region Methods
        /// <summary>
        /// Test wrapper for the protected <see cref="ViewModelBase.ParentViewModel"/> property.
        /// </summary>
        /// <returns></returns>
        public IViewModel GetParentViewModelForTest()
        {
            return ParentViewModel;
        }

        /// <summary>
        /// Test wrapper for the protected <see cref="ViewModelBase.GetAllModels"/> method.
        /// </summary>
        /// <returns></returns>
        public object[] GetAllModelsForTest()
        {
            return GetAllModels();
        }

        /// <summary>
        /// Test wrapper for the protected <see cref="ViewModelBase.IsModelRegistered"/> method.
        /// </summary>
        public bool IsModelRegisteredForTest(string name)
        {
            return IsModelRegistered(name);
        }

        /// <summary>
        /// Test wrapper to set the <see cref="ViewModelBase.InvalidateCommandsOnPropertyChanged"/> property.
        /// </summary>
        public void SetInvalidateCommandsOnPropertyChanged(bool value)
        {
            InvalidateCommandsOnPropertyChanged = value;
        }
        #endregion
    }
}
