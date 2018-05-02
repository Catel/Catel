// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestViewModelWithImplicitModelMappingsWithMultipleModels.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.MVVM.ViewModels.TestClasses
{
    using Catel.Data;
    using Catel.MVVM;

    public class TestViewModelWithImplicitModelMappingsWithMultipleModels : ViewModelBase
    {
        public TestViewModelWithImplicitModelMappingsWithMultipleModels(IPerson person)
        {
        }

        [Model]
        public IPerson Person1
        {
            get { return GetValue<IPerson>(Person1Property); }
            private set { SetValue(Person1Property, value); }
        }

        public static readonly PropertyData Person1Property = RegisterProperty("Person1", typeof(IPerson));

        [Model]
        public IPerson Person2
        {
            get { return GetValue<IPerson>(Person2Property); }
            private set { SetValue(Person2Property, value); }
        }

        public static readonly PropertyData Person2Property = RegisterProperty("Person2", typeof(IPerson));


        [ViewModelToModel]
        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), null);
    }
}