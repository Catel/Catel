// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestViewModelWithMappingConverters.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.MVVM.ViewModels.TestClasses
{
    using Catel.Data;
    using Catel.MVVM;

    public class TestViewModelWithMappingConverters : ViewModelBase
    {
        public TestViewModelWithMappingConverters(Person person = null)
        {
            Person = person;
        }

        [Model]
        public Person Person
        {
            get { return GetValue<Person>(PersonProperty); }
            set { SetValue(PersonProperty, value); }
        }

        /// <summary>
        /// Register the Person property so it is known in the class.
        /// </summary>
        public static readonly PropertyData PersonProperty = RegisterProperty("Person", typeof(Person), null);

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
        /// Register the FirstName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string));


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
        /// Register the MiddleName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData MiddleNameProperty = RegisterProperty("MiddleName", typeof(string));


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
        /// Register the LastName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData LastNameProperty = RegisterProperty("LastName", typeof(string));

  
        [ViewModelToModel("Person", ConverterType = typeof(UIntToStringMapping))]
        public string Age
        {
            get { return GetValue<string>(AgeProperty); }
            set { SetValue(AgeProperty, value); }
        }

        /// <summary>Register the Age property so it is known in the class.</summary>
        public static readonly PropertyData AgeProperty = RegisterProperty<TestViewModelWithMappingConverters, string>(model => model.Age);

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        [ViewModelToModel("Person", "FirstName", AdditionalPropertiesToWatch = new[] { "LastName" }, ConverterType = typeof(CollapsMapping))]
        public string FullName
        {
            get { return GetValue<string>(FullNameProperty); }
            set { SetValue(FullNameProperty, value); }
        }

        /// <summary>Register the FullName property so it is known in the class.</summary>
        public static readonly PropertyData FullNameProperty = RegisterProperty<TestViewModelWithMappingConverters, string>(model => model.FullName);

        /// <summary>
        /// Gets or sets the full name with separated names with ';'.
        /// </summary>
        [ViewModelToModel("Person", "FirstName", AdditionalPropertiesToWatch = new[] { "LastName" }, ConverterType = typeof(CollapsMapping),
            AdditionalConstructorArgs = new object[] { ';' })]
        public string FullNameWithCustomSeparator
        {
            get { return GetValue<string>(FullNameWithCustomSeparatorProperty); }
            set { SetValue(FullNameWithCustomSeparatorProperty, value); }
        }

        /// <summary>Register the FullNameWithCustomSeparator property so it is known in the class.</summary>
        public static readonly PropertyData FullNameWithCustomSeparatorProperty = RegisterProperty<TestViewModelWithMappingConverters, string>(model => model.FullNameWithCustomSeparator);
    }
}