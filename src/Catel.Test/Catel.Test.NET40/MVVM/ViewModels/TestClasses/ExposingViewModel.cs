// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExposingViewModel.cs" company="Catel development team">
//   Copyright (c) 2011 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.ViewModels.TestClasses
{
    using Catel.Data;
    using Catel.MVVM;

    /// <summary>
    ///   Exposing view model.
    /// </summary>
    public class ExposingViewModel : ViewModelBase
    {
        public ExposingViewModel()
        {
            Person = new Person();
        }

        /// <summary>
        ///   Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get { return "Exposing view model"; }
        }

        /// <summary>
        /// Gets or sets the person.
        /// </summary>
        [Model]
        [Expose("FirstName")]
        [Expose("MiddleName")]
        [Expose("LastName")]
        [Expose("MyReadOnlyProperty", Mode = ViewModelToModelMode.OneWay)]
        public Person Person
        {
            get { return GetValue<Person>(PersonProperty); }
            private set { SetValue(PersonProperty, value); }
        }

        /// <summary>
        /// Register the Person property so it is known in the class.
        /// </summary>
        public static readonly PropertyData PersonProperty = RegisterProperty("Person", typeof(Person));
    }

    /// <summary>
    ///   Exposing view model.
    /// </summary>
    public class ExposingViewModelWithInvalidProperty : ViewModelBase
    {
        /// <summary>
        ///   Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get { return "Exposing view model"; }
        }

        /// <summary>
        /// Gets or sets the person.
        /// </summary>
        [Model]
        [Expose("NonExistingProperty")]
        [Expose("MiddleName")]
        [Expose("LastName")]
        public Person Person
        {
            get { return GetValue<Person>(PersonProperty); }
            private set { SetValue(PersonProperty, value); }
        }

        /// <summary>
        /// Register the Person property so it is known in the class.
        /// </summary>
        public static readonly PropertyData PersonProperty = RegisterProperty("Person", typeof(Person));
    }

    /// <summary>
    ///   Exposing view model.
    /// </summary>
    public class ExposingViewModelWithNoModelDefined : ViewModelBase
    {
        /// <summary>
        ///   Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get { return "Exposing view model"; }
        }

        /// <summary>
        /// Gets or sets the person.
        /// </summary>
        [Expose("FirstName")]
        [Expose("MiddleName")]
        [Expose("LastName")]
        public Person Person
        {
            get { return GetValue<Person>(PersonProperty); }
            private set { SetValue(PersonProperty, value); }
        }

        /// <summary>
        /// Register the Person property so it is known in the class.
        /// </summary>
        public static readonly PropertyData PersonProperty = RegisterProperty("Person", typeof(Person));
    }

    /// <summary>
    ///   Exposing view model.
    /// </summary>
    public class ExposingViewModelWithExistingProperty : ViewModelBase
    {
        public ExposingViewModelWithExistingProperty()
        {
            Person = new Person();
        }

        /// <summary>
        ///   Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get { return "Exposing view model"; }
        }

        /// <summary>
        /// Gets or sets the person.
        /// </summary>
        [Model]
        [Expose("FirstName")]
        [Expose("MiddleName")]
        [Expose("LastName")]
        public Person Person
        {
            get { return GetValue<Person>(PersonProperty); }
            private set { SetValue(PersonProperty, value); }
        }

        /// <summary>
        /// Register the Person property so it is known in the class.
        /// </summary>
        public static readonly PropertyData PersonProperty = RegisterProperty("Person", typeof(Person));

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
    }

    /// <summary>
    ///   Exposing view model.
    /// </summary>
    public class ExposingViewModelWithInvalidReadonlyMapping : ViewModelBase
    {
        /// <summary>
        ///   Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get { return "Exposing view model"; }
        }

        /// <summary>
        /// Gets or sets the person.
        /// </summary>
        [Model]
        [Expose("FirstName")]
        [Expose("MiddleName")]
        [Expose("LastName")]
        [Expose("MyReadOnlyProperty")]
        public Person Person
        {
            get { return GetValue<Person>(PersonProperty); }
            private set { SetValue(PersonProperty, value); }
        }

        /// <summary>
        /// Register the Person property so it is known in the class.
        /// </summary>
        public static readonly PropertyData PersonProperty = RegisterProperty("Person", typeof(Person));
    }
}