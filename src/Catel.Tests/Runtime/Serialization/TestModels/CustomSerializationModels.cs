// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomSerializationModels.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.Runtime.Serialization.TestModels
{
    using Catel.Data;

    public class CustomSerializationModelBase : ModelBase
    {
        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        public static readonly IPropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), null);

        public bool IsCustomSerialized { get; protected set; }

        public bool IsCustomDeserialized { get; protected set; }
    }
}