// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Person.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.MVVM.ViewModels.TestClasses
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using Catel.Data;

    public interface IPerson
    {
        string FirstName { get; set; }
        string MiddleName { get; set; }
        string LastName { get; set; }

        IContactInfo ContactInfo { get; set; }
    }

    public interface IContactInfo
    {
        string Street { get; set; }
        string City { get; set; }
        string Email { get; set; }
    }

    /// <summary>
    /// Person Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if NET
    [Serializable]
#endif
    public class Person : ModelBase, IPerson
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public Person()
        {
        }

#if NET
        /// <summary>
        /// Initializes a new object based on <see cref="SerializationInfo"/>.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        protected Person(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the FirstName.
        /// </summary>
        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        /// <summary>
        /// Register the FirstName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof (string), string.Empty);

        /// <summary>
        /// Gets or sets the middle name.
        /// </summary>
        public string MiddleName
        {
            get { return GetValue<string>(MiddleNameProperty); }
            set { SetValue(MiddleNameProperty, value); }
        }

        /// <summary>
        /// Register the MiddleName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData MiddleNameProperty = RegisterProperty("MiddleName", typeof (string), string.Empty);

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName
        {
            get { return GetValue<string>(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }

        /// <summary>
        /// Register the LastName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData LastNameProperty = RegisterProperty("LastName", typeof (string), string.Empty);

        /// <summary>
        /// Gets a readonly property.
        /// </summary>
        public object MyReadOnlyProperty
        {
            get { return GetValue<object>(MyReadOnlyPropertyProperty); }
            private set { SetValue(MyReadOnlyPropertyProperty, value); }
        }

        /// <summary>
        /// Register the MyReadOnlyProperty property so it is known in the class.
        /// </summary>
        public static readonly PropertyData MyReadOnlyPropertyProperty = RegisterProperty("MyReadOnlyProperty", typeof (object));

        /// <summary>
        /// Gets or sets the contact info.
        /// </summary>
        public IContactInfo ContactInfo
        {
            get { return GetValue<IContactInfo>(ContactInfoProperty); }
            set { SetValue(ContactInfoProperty, value); }
        }

        /// <summary>
        /// Register the ContactInfo property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ContactInfoProperty = RegisterProperty("ContactInfo", typeof (IContactInfo), () => new ContactInfo());
        #endregion

        #region Methods
        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (string.IsNullOrEmpty(FirstName))
            {
                validationResults.Add(FieldValidationResult.CreateError(FirstNameProperty, "First name is required"));
            }

            if (string.IsNullOrEmpty(MiddleName))
            {
                validationResults.Add(FieldValidationResult.CreateWarning(MiddleNameProperty, "No middle name, are you sure?"));
            }

            if (string.IsNullOrEmpty(LastName))
            {
                validationResults.Add(FieldValidationResult.CreateError(LastNameProperty, "Last name is required"));
            }
        }

        protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
        {
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName))
            {
                validationResults.Add(BusinessRuleValidationResult.CreateError("Both first and last name are required"));
            }

            if (string.IsNullOrEmpty(MiddleName))
            {
                validationResults.Add(BusinessRuleValidationResult.CreateWarning("No middle name"));
            }
        }
        #endregion
    }

    /// <summary>
    /// PersonWithDataAnnotations Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if NET
    [Serializable]
#endif
    public class PersonWithDataAnnotations : ModelBase, IPerson
    {
        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public PersonWithDataAnnotations()
        {
        }

#if NET
        /// <summary>
        /// Initializes a new object based on <see cref="SerializationInfo"/>.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        protected PersonWithDataAnnotations(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the FirstName.
        /// </summary>
        [Required]
        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        /// <summary>
        /// Register the FirstName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof (string), string.Empty);

        /// <summary>
        /// Gets or sets the middle name.
        /// </summary>
        public string MiddleName
        {
            get { return GetValue<string>(MiddleNameProperty); }
            set { SetValue(MiddleNameProperty, value); }
        }

        /// <summary>
        /// Register the MiddleName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData MiddleNameProperty = RegisterProperty("MiddleName", typeof (string), string.Empty);

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [Required]
        public string LastName
        {
            get { return GetValue<string>(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }

        /// <summary>
        /// Register the LastName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData LastNameProperty = RegisterProperty("LastName", typeof (string), string.Empty);

        /// <summary>
        /// Gets or sets the contact info.
        /// </summary>
        public IContactInfo ContactInfo
        {
            get { return GetValue<IContactInfo>(ContactInfoProperty); }
            set { SetValue(ContactInfoProperty, value); }
        }

        /// <summary>
        /// Register the ContactInfo property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ContactInfoProperty = RegisterProperty("ContactInfo", typeof (IContactInfo), () => new ContactInfo());
        #endregion
    }

    /// <summary>
    /// ContactInfo Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if NET
    [Serializable]
#endif
    public class ContactInfo : ModelBase, IContactInfo
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public ContactInfo()
        {
        }

#if NET
        /// <summary>
        /// Initializes a new object based on <see cref="SerializationInfo"/>.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        protected ContactInfo(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the street.
        /// </summary>
        public string Street
        {
            get { return GetValue<string>(StreetProperty); }
            set { SetValue(StreetProperty, value); }
        }

        /// <summary>
        /// Register the Street property so it is known in the class.
        /// </summary>
        public static readonly PropertyData StreetProperty = RegisterProperty("Street", typeof (string), string.Empty);

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public string City
        {
            get { return GetValue<string>(CityProperty); }
            set { SetValue(CityProperty, value); }
        }

        /// <summary>
        /// Register the City property so it is known in the class.
        /// </summary>
        public static readonly PropertyData CityProperty = RegisterProperty("City", typeof (string), string.Empty);

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email
        {
            get { return GetValue<string>(EmailProperty); }
            set { SetValue(EmailProperty, value); }
        }

        /// <summary>
        /// Register the Email property so it is known in the class.
        /// </summary>
        public static readonly PropertyData EmailProperty = RegisterProperty("Email", typeof (string), string.Empty);
        #endregion

        #region Methods
        #endregion
    }
}