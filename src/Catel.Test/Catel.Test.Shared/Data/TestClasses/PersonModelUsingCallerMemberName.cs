// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonTestModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Data
{
    using Catel.Data;

    public class PersonModelUsingCallerMemberName : ModelBase
    {
        public static PropertyData NameProperty = RegisterProperty("Name", typeof(string), string.Empty);

        public string Name
        {
            get => Get<string>();
            set => Set(value);
        }
    }
}