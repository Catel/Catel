namespace Catel.Tests.MVVM.ViewModels.TestClasses
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
        public static readonly IPropertyData PersonProperty = RegisterProperty<IPerson>("Person");

        /// <summary>
        /// Register the ContactInfo property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData ContactInfoProperty = RegisterProperty<IContactInfo>("ContactInfo");

        /// <summary>
        /// Register the Email property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData EmailProperty = RegisterProperty<string>("Email");
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
