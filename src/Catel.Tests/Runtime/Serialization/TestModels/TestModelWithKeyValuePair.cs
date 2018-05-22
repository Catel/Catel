// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestModelWithKeyValuePair.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.Runtime.Serialization.TestModels
{
    using System.Collections.Generic;
    using Catel.Data;
    using Catel.Runtime.Serialization;

    [SerializerModifier(typeof(KeyValuePairSerializerModifier))]
    public class TestModelWithKeyValuePair : ModelBase
    {
        public TestModelWithKeyValuePair()
        {
            KeyValuePair = new KeyValuePair<string, string>("somekey", "somevalue");
            KeyValuePairAsObject = new KeyValuePair<string, int>("somekey", 42);
        }

        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), null);


        public KeyValuePair<string, string> KeyValuePair
        {
            get { return GetValue<KeyValuePair<string, string>>(SpecialValueProperty); }
            set { SetValue(SpecialValueProperty, value); }
        }

        public static readonly PropertyData SpecialValueProperty = RegisterProperty("KeyValuePair", typeof(KeyValuePair<string, string>), null);


        public object KeyValuePairAsObject
        {
            get { return GetValue<object>(KeyValuePairAsObjectProperty); }
            set { SetValue(KeyValuePairAsObjectProperty, value); }
        }

        public static readonly PropertyData KeyValuePairAsObjectProperty = RegisterProperty("KeyValuePairAsObject", typeof(object), null);
    }
}