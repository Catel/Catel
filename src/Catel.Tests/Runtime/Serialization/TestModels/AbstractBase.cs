namespace Catel.Tests.Runtime.Serialization.TestModels
{
    using Catel.Data;
    using Catel.Runtime.Serialization;

    public abstract class AbstractBase : ModelBase
    {
        protected AbstractBase(ISerializer serializer)
            : base(serializer)
        {
            
        }
    }

    public class Derived1 : AbstractBase
    {
        public Derived1(ISerializer serializer)
            : base(serializer)
        {
            
        }

        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly IPropertyData NameProperty = RegisterProperty("Name", "Test name");

    }

    public class Derived2 : AbstractBase
    {
        public Derived2(ISerializer serializer)
            : base(serializer)
        {
            
        }

        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly IPropertyData NameProperty = RegisterProperty("Name", "Test name");

    }
}
