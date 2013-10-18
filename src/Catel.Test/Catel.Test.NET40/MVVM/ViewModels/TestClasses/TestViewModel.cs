// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.ViewModels.TestClasses
{
    using System.Collections.Generic;
    using Catel.Data;
    using Catel.IoC;
    using Catel.MVVM;

    /// <summary>
    /// Test view model.
    /// </summary>
    public class TestViewModel : ViewModelBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TestViewModel"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public TestViewModel(IServiceLocator serviceLocator)
            : this(serviceLocator, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestViewModel"/> class.
        /// </summary>
        public TestViewModel()
            : this(null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestViewModel" /> class.
        /// </summary>
        /// <param name="person">The person.</param>
        /// <param name="validateModelsOnInitialization">if set to <c>true</c>, the view model will validate on initialization.</param>
        public TestViewModel(IPerson person, bool validateModelsOnInitialization = true)
            : this(null, person, null, validateModelsOnInitialization)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestViewModel"/> class.
        /// </summary>
        /// <param name="specialValidationModel">The special validation model.</param>
        public TestViewModel(SpecialValidationModel specialValidationModel)
            : this(null, null, specialValidationModel)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestViewModel"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="person">The person.</param>
        /// <param name="specialValidationModel">The special validation model.</param>
        /// <param name="validateModelsOnInitialization">if set to <c>true</c>, the view model will validate on initialization.</param>
        private TestViewModel(IServiceLocator serviceLocator, IPerson person, SpecialValidationModel specialValidationModel,
                              bool validateModelsOnInitialization = true)
            : base(serviceLocator)
        {
            ValidateModelsOnInitialization = validateModelsOnInitialization;

            Person = person;
            SpecialValidationModel = specialValidationModel;

            GenerateData = new Command<object, object>(OnGenerateDataExecute, OnGenerateDataCanExecute);
            GenerateData.AutomaticallyDispatchEvents = false;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get { return "View model title"; }
        }
        #endregion

        #region Models

        #region Constants
        /// <summary>
        /// Register the Person property so it is known in the class.
        /// </summary>
        public static readonly PropertyData PersonProperty = RegisterProperty("Person", typeof (IPerson));

        /// <summary>
        /// Register the SpecialValidationModel property so it is known in the class.
        /// </summary>
        public static readonly PropertyData SpecialValidationModelProperty = RegisterProperty("SpecialValidationModel", typeof (SpecialValidationModel));
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
        public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof (string));

        /// <summary>
        /// Register the MiddleName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData MiddleNameProperty = RegisterProperty("MiddleName", typeof (string));

        /// <summary>
        /// Register the LastName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData LastNameProperty = RegisterProperty("LastName", typeof (string));

        /// <summary>
        /// Register the FieldErrorWhenEmpty property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FieldErrorWhenEmptyProperty = RegisterProperty("FieldErrorWhenEmpty", typeof (string));

        /// <summary>
        /// Register the FieldWarningWhenEmpty property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FieldWarningWhenEmptyProperty = RegisterProperty("FieldWarningWhenEmpty", typeof (string));

        /// <summary>
        /// Register the BusinessRuleErrorWhenEmpty property so it is known in the class.
        /// </summary>
        public static readonly PropertyData BusinessRuleErrorWhenEmptyProperty = RegisterProperty("BusinessRuleErrorWhenEmpty", typeof (string));

        /// <summary>
        /// Register the BusinessRuleWarningWhenEmpty property so it is known in the class.
        /// </summary>
        public static readonly PropertyData BusinessRuleWarningWhenEmptyProperty = RegisterProperty("BusinessRuleWarningWhenEmpty", typeof (string));
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

    public class TestViewModelWithDeferredValidation : TestViewModelWithValidationTags
    {
        #region Constructors
        public TestViewModelWithDeferredValidation()
        {
            DeferValidationUntilFirstSaveCall = true;
        }
        #endregion
    }

    /// <summary>
    /// Test view model with validation tags.
    /// </summary>
    public class TestViewModelWithValidationTags : ViewModelBase
    {
        #region Constants
        /// <summary>
        /// Register the FirstName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof (string));

        /// <summary>
        /// Register the MiddleName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData MiddleNameProperty = RegisterProperty("MiddleName", typeof (string));

        /// <summary>
        /// Register the LastName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData LastNameProperty = RegisterProperty("LastName", typeof (string));
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the middle name.
        /// </summary>
        public string MiddleName
        {
            get { return GetValue<string>(MiddleNameProperty); }
            set { SetValue(MiddleNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName
        {
            get { return GetValue<string>(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }

        /// <summary>
        /// Gets or sets the person validation summary.
        /// </summary>
        /// <value>The person validation summary.</value>
        [ValidationToViewModel(Tag = "PersonValidation")]
        public IValidationSummary PersonValidationSummary { get; set; }

        /// <summary>
        /// Gets or sets the validation summary with null tag.
        /// </summary>
        /// <value>The validation summary with null tag.</value>
        [ValidationToViewModel()]
        public IValidationSummary ValidationSummaryWithNullTag { get; set; }

        /// <summary>
        /// Gets or sets the validation summary with non existing tag.
        /// </summary>
        /// <value>The validation summary with non existing tag.</value>
        [ValidationToViewModel(Tag = "NoValidationExistsForThisTag")]
        public IValidationSummary ValidationSummaryWithNonExistingTag { get; set; }

        /// <summary>
        /// Gets or sets the validation summary without tag filtering.
        /// </summary>
        /// <value>The validation summary without tag filtering.</value>
        [ValidationToViewModel(UseTagToFilter = false)]
        public IValidationSummary ValidationSummaryWithoutTagFiltering { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Validates the field values of this object. Override this method to enable
        /// validation of field values.
        /// </summary>
        /// <param name="validationResults">The validation results, add additional results to this list.</param>
        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (string.IsNullOrEmpty(FirstName))
            {
                validationResults.Add(new FieldValidationResult(FirstNameProperty, ValidationResultType.Error, "First name is required") {Tag = "PersonValidation"});
            }

            if (string.IsNullOrEmpty(LastName))
            {
                validationResults.Add(new FieldValidationResult(LastNameProperty, ValidationResultType.Error, "Last name is required") {Tag = "PersonValidation"});
            }
        }
        #endregion
    }

    public class MappingViewModel : ViewModelBase
    {
        #region Constants
        /// <summary>
        /// Register the Person property so it is known in the class.
        /// </summary>
        public static readonly PropertyData PersonProperty = RegisterProperty("Person", typeof (IPerson));

        /// <summary>
        /// Register the FirstNameAsTwoWay property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FirstNameAsTwoWayProperty = RegisterProperty("FirstNameAsTwoWay", typeof (string), string.Empty);

        /// <summary>
        /// Register the FirstNameAsOneWay property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FirstNameAsOneWayProperty = RegisterProperty("FirstNameAsOneWay", typeof (string), string.Empty);

        /// <summary>
        /// Register the FirstNameAsOneWayToSource property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FirstNameAsOneWayToSourceProperty = RegisterProperty("FirstNameAsOneWayToSource", typeof (string), string.Empty);

        /// <summary>
        /// Register the FirstNameAsExplicit property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FirstNameAsExplicitProperty = RegisterProperty("FirstNameAsExplicit", typeof (string), string.Empty);
        #endregion

        #region Constructors
        public MappingViewModel(IPerson person)
        {
            Person = person;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        [Model]
        public IPerson Person
        {
            get { return GetValue<IPerson>(PersonProperty); }
            set { SetValue(PersonProperty, value); }
        }

        /// <summary>
        /// Gets or sets the the TwoWay mode.
        /// </summary>
        [ViewModelToModel("Person", "FirstName", Mode = ViewModelToModelMode.TwoWay)]
        public string FirstNameAsTwoWay
        {
            get { return GetValue<string>(FirstNameAsTwoWayProperty); }
            set { SetValue(FirstNameAsTwoWayProperty, value); }
        }

        /// <summary>
        /// Gets or sets the OneWay mode.
        /// </summary>
        [ViewModelToModel("Person", "FirstName", Mode = ViewModelToModelMode.OneWay)]
        public string FirstNameAsOneWay
        {
            get { return GetValue<string>(FirstNameAsOneWayProperty); }
            set { SetValue(FirstNameAsOneWayProperty, value); }
        }

        /// <summary>
        /// Gets or sets the OneWayToSource mode.
        /// </summary>
        [ViewModelToModel("Person", "FirstName", Mode = ViewModelToModelMode.OneWayToSource)]
        public string FirstNameAsOneWayToSource
        {
            get { return GetValue<string>(FirstNameAsOneWayToSourceProperty); }
            set { SetValue(FirstNameAsOneWayToSourceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Explicit model.
        /// </summary>
        [ViewModelToModel("Person", "FirstName", Mode = ViewModelToModelMode.Explicit)]
        public string FirstNameAsExplicit
        {
            get { return GetValue<string>(FirstNameAsExplicitProperty); }
            set { SetValue(FirstNameAsExplicitProperty, value); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Updates the explicit mappings.
        /// </summary>
        public void UpdateExplicitMappings()
        {
            UpdateExplicitViewModelToModelMappings();
        }
        #endregion
    }

    public class MultipleModelMappingsViewModel : ViewModelBase
    {
        #region Constants
        /// <summary>
        /// Register the Person property so it is known in the class.
        /// </summary>
        public static readonly PropertyData PersonProperty = RegisterProperty("Person", typeof (IPerson));

        /// <summary>
        /// Register the ContactInfo property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ContactInfoProperty = RegisterProperty("ContactInfo", typeof (IContactInfo));

        /// <summary>
        /// Register the Email property so it is known in the class.
        /// </summary>
        public static readonly PropertyData EmailProperty = RegisterProperty("Email", typeof (string));
        #endregion

        #region Constructors
        public MultipleModelMappingsViewModel(IPerson person)
        {
            Person = person;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the person.
        /// </summary>
        [Model]
        public IPerson Person
        {
            get { return GetValue<IPerson>(PersonProperty); }
            set { SetValue(PersonProperty, value); }
        }

        /// <summary>
        /// Gets or sets the contact info.
        /// </summary>
        [Model]
        [ViewModelToModel("Person")]
        public IContactInfo ContactInfo
        {
            get { return GetValue<IContactInfo>(ContactInfoProperty); }
            set { SetValue(ContactInfoProperty, value); }
        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [ViewModelToModel("ContactInfo")]
        public string Email
        {
            get { return GetValue<string>(EmailProperty); }
            set { SetValue(EmailProperty, value); }
        }
        #endregion
    }
}