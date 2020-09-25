// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonTestModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.Data
{
    using Catel.Data;

    public class PersonTestModel : ModelBase
    {
        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        public static readonly IPropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), null);


        public string LastName
        {
            get { return GetValue<string>(LastNameProperty); }
            set { SetValue(LastNameProperty, value); }
        }

        public static readonly IPropertyData LastNameProperty = RegisterProperty("LastName", typeof(string), null);


        public bool IsEnabled
        {
            get { return GetValue<bool>(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly IPropertyData IsEnabledProperty = RegisterProperty("IsEnabled", typeof(bool), null);
    }
}
