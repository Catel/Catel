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

        public static readonly IPropertyData NameProperty = RegisterProperty("Name", string.Empty);


        public KeyValuePair<string, string> KeyValuePair
        {
            get { return GetValue<KeyValuePair<string, string>>(SpecialValueProperty); }
            set { SetValue(SpecialValueProperty, value); }
        }

        public static readonly IPropertyData SpecialValueProperty = RegisterProperty<KeyValuePair<string, string>>("KeyValuePair");


        public object KeyValuePairAsObject
        {
            get { return GetValue<object>(KeyValuePairAsObjectProperty); }
            set { SetValue(KeyValuePairAsObjectProperty, value); }
        }

        public static readonly IPropertyData KeyValuePairAsObjectProperty = RegisterProperty<object>("KeyValuePairAsObject");
    }
}
