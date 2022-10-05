// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MappingViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.MVVM.ViewModels.TestClasses
{
    using Catel.Data;
    using Catel.MVVM;
    using Catel.MVVM.MVVM.ViewModels.Mappings;

    public class TestViewModelWithMappings : ViewModelBase
    {
        #region Constructors
        public TestViewModelWithMappings(IPerson person)
        {
            Person = person;
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

        #region Constants
        /// <summary>
        /// Register the Person property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData PersonProperty = RegisterProperty<IPerson>("Person");

        /// <summary>
        /// Register the FirstNameAsTwoWay property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData FirstNameAsTwoWayProperty = RegisterProperty("FirstNameAsTwoWay", string.Empty);

        /// <summary>
        /// Register the FirstNameAsOneWay property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData FirstNameAsOneWayProperty = RegisterProperty("FirstNameAsOneWay", string.Empty);

        /// <summary>
        /// Register the FirstNameAsOneWayToSource property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData FirstNameAsOneWayToSourceProperty = RegisterProperty("FirstNameAsOneWayToSource", string.Empty);

        /// <summary>
        /// Register the FirstNameAsExplicit property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData FirstNameAsExplicitProperty = RegisterProperty("FirstNameAsExplicit", string.Empty);
        #endregion

        #region Properties
        public bool DeferValidationUntilFirstSaveCallWrapper
        {
            get { return DeferValidationUntilFirstSaveCall; }
            set { DeferValidationUntilFirstSaveCall = value; }
        }

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

        [ViewModelToModel("Person", "LastName")]
        public string LastName
        {
            get { return GetValue<string>(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }

        public static readonly IPropertyData LastNameProperty = RegisterProperty(nameof(LastName), string.Empty);
        #endregion
    }
}
