namespace Catel.Tests.MVVM.ViewModels.TestClasses
{
    using System;
    using Catel.Data;
    using Catel.MVVM;

    public class TestViewModelWithImplicitModelMappings : FeaturedViewModelBase
    {
        public static readonly IPropertyData PersonProperty = RegisterProperty<IPerson>("Person");

        public static readonly IPropertyData FirstNameProperty = RegisterProperty<string>("FirstName");

        public TestViewModelWithImplicitModelMappings(IPerson person, IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            Person = person;
        }

        [Model]
        public IPerson Person
        {
            get { return GetValue<IPerson>(PersonProperty); }
            private set { SetValue(PersonProperty, value); }
        }

        [ViewModelToModel]
        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }
    }
}
