// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImplicitModelMappingsViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.MVVM.ViewModels.TestClasses
{
    using Catel.Data;
    using Catel.MVVM;

    public class TestViewModelWithImplicitModelMappings : ViewModelBase
    {
        public static readonly PropertyData PersonProperty = RegisterProperty("Person", typeof(IPerson));

        public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), null);

        public TestViewModelWithImplicitModelMappings(IPerson person)
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