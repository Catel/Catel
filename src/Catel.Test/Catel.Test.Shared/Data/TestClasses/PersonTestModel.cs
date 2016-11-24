// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonTestModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Data
{
    using Catel.Data;

    public class PersonTestModel : ModelBase
    {
        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), null);


        public string LastName
        {
            get { return GetValue<string>(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }

        public static readonly PropertyData LastNameProperty = RegisterProperty("LastName", typeof(string), null);
    }
}