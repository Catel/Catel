namespace Catel.Tests.Runtime.Serialization.TestModels
{
    using Catel.Data;
    using Catel.Runtime.Serialization;

    public class CustomSerializationModelBase : ModelBase
    {
        public CustomSerializationModelBase(ISerializer serializer) 
            : base(serializer)
        {
            
        }

        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        public static readonly IPropertyData FirstNameProperty = RegisterProperty("FirstName", string.Empty);

        public bool IsCustomSerialized { get; protected set; }

        public bool IsCustomDeserialized { get; protected set; }
    }
}
