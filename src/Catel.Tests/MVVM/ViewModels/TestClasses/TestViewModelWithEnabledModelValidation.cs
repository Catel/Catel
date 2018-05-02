// <copyright file="TestViewModelWithEnabledModelValidation.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.MVVM.ViewModels.TestClasses
{
    using Catel.Data;
    using Catel.MVVM;

    public class TestViewModelWithEnabledModelValidation : ViewModelBase
    {
        /// <summary>
        /// Register the FirstName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string));

        /// <summary>
        /// Register the MiddleName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData MiddleNameProperty = RegisterProperty("MiddleName", typeof(string));

        /// <summary>
        /// Register the LastName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData LastNameProperty = RegisterProperty("LastName", typeof(string));

        /// <summary>
        /// Register the Person property so it is known in the class.
        /// </summary>
        public static readonly PropertyData PersonProperty = RegisterProperty("Person", typeof(IPerson));

        public TestViewModelWithEnabledModelValidation(Person person)
        {
            ValidateModelsOnInitialization = true;
            Person = person;
            DeferValidationUntilFirstSaveCall = false;
        }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [ViewModelToModel("Person")]
        public string FirstName
        {
            get => GetValue<string>(FirstNameProperty);
            set => SetValue(FirstNameProperty, value);
        }

        /// <summary>
        /// Gets or sets the middle name.
        /// </summary>
        [ViewModelToModel("Person")]
        public string MiddleName
        {
            get => GetValue<string>(MiddleNameProperty);
            set => SetValue(MiddleNameProperty, value);
        }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [ViewModelToModel("Person")]
        public string LastName
        {
            get => GetValue<string>(LastNameProperty);
            set => SetValue(LastNameProperty, value);
        }

        [Model]
        public IPerson Person
        {
            get => GetValue<IPerson>(PersonProperty);
            set => SetValue(PersonProperty, value);
        }
    }
}