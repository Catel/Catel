// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestViewModelWithMultipleModelMappings.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.MVVM.ViewModels.TestClasses
{
    using Catel.Data;
    using Catel.MVVM;

    public class TestViewModelWithMultipleModelMappings : ViewModelBase
    {
        #region Constructors
        public TestViewModelWithMultipleModelMappings(IPerson person)
        {
            Person = person;
        }
        #endregion

        #region Constants
        /// <summary>
        /// Register the Person property so it is known in the class.
        /// </summary>
        public static readonly PropertyData PersonProperty = RegisterProperty("Person", typeof(IPerson));

        /// <summary>
        /// Register the ContactInfo property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ContactInfoProperty = RegisterProperty("ContactInfo", typeof(IContactInfo));

        /// <summary>
        /// Register the Email property so it is known in the class.
        /// </summary>
        public static readonly PropertyData EmailProperty = RegisterProperty("Email", typeof(string));
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