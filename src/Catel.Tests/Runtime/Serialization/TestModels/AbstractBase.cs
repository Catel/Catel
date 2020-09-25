namespace Catel.Tests.Runtime.Serialization.TestModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Catel.Data;

    public abstract class AbstractBase : ModelBase
    {
    }

    public class Derived1 : AbstractBase
    {
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly IPropertyData NameProperty = RegisterProperty("Name", typeof(string), "Test name");

    }

    public class Derived2 : AbstractBase
    {
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly IPropertyData NameProperty = RegisterProperty("Name", typeof(string), "Test name");

    }
}
