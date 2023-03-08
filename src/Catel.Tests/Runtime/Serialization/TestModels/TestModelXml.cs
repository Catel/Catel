namespace Catel.Tests.Runtime.Serialization.TestModels
{
    using Catel.Data;
    using Catel.Tests.Data;
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;
    using System;

    [Serializable]
    public class XmlModelWithAttributesOnly : ModelBase
    {
        [XmlAttribute]
        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        public static readonly IPropertyData FirstNameProperty = RegisterProperty("FirstName", string.Empty);
    }

    [Serializable]
    public class XmlFamily : ModelBase
    {
        public XmlFamily()
        {
            Persons = new ObservableCollection<XmlPerson>();
            ModelsWithAttributesOnly = new ObservableCollection<XmlModelWithAttributesOnly>();
        }

        public string LastName
        {
            get { return GetValue<string>(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }

        public static readonly IPropertyData LastNameProperty = RegisterProperty("LastName", string.Empty);


        public ObservableCollection<XmlPerson> Persons
        {
            get { return GetValue<ObservableCollection<XmlPerson>>(PersonsProperty); }
            private set { SetValue(PersonsProperty, value); }
        }

        public static readonly IPropertyData PersonsProperty = RegisterProperty("Persons", () => new ObservableCollection<XmlPerson>());


        public ObservableCollection<XmlModelWithAttributesOnly> ModelsWithAttributesOnly
        {
            get { return GetValue<ObservableCollection<XmlModelWithAttributesOnly>>(ModelsWithAttributesOnlyProperty); }
            set { SetValue(ModelsWithAttributesOnlyProperty, value); }
        }

        public static readonly IPropertyData ModelsWithAttributesOnlyProperty = RegisterProperty("ModelsWithAttributesOnly", () => new ObservableCollection<XmlModelWithAttributesOnly>());
    }

    [Serializable]
    public class XmlPerson : ModelBase
    {
        public Gender Gender
        {
            get { return GetValue<Gender>(GenderProperty); }
            set { SetValue(GenderProperty, value); }
        }

        public static readonly IPropertyData GenderProperty = RegisterProperty("Gender", Data.Gender.Female);


        [XmlAttribute]
        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        public static readonly IPropertyData FirstNameProperty = RegisterProperty("FirstName", string.Empty);


        [XmlAttribute]
        public string LastName
        {
            get { return GetValue<string>(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }

        public static readonly IPropertyData LastNameProperty = RegisterProperty("LastName", string.Empty);
    }
}
