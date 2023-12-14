namespace Catel.Tests.Runtime.Serialization.TestModels
{
    using System.Collections.Generic;
    using Catel.Data;

    public class TestModelWithNestedListMembers : ModelBase
    {
        public TestModelWithNestedListMembers()
        {
            Children = new List<TestModelWithNestedListMembers_Level1>();
        }

        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly IPropertyData NameProperty = RegisterProperty(nameof(Name), string.Empty);

        public List<TestModelWithNestedListMembers_Level1> Children
        {
            get { return GetValue<List<TestModelWithNestedListMembers_Level1>>(ChildrenProperty); }
            set { SetValue(ChildrenProperty, value); }
        }

        public static readonly IPropertyData ChildrenProperty = RegisterProperty(nameof(Children), () => new List<TestModelWithNestedListMembers_Level1>());
    }

    public class TestModelWithNestedListMembers_Level1 : ModelBase
    {
        public TestModelWithNestedListMembers_Level1()
        {
            Children = new List<TestModelWithNestedListMembers_Level2>();
        }

        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly IPropertyData NameProperty = RegisterProperty(nameof(Name), string.Empty);

        public List<TestModelWithNestedListMembers_Level2> Children
        {
            get { return GetValue<List<TestModelWithNestedListMembers_Level2>>(ChildrenProperty); }
            set { SetValue(ChildrenProperty, value); }
        }

        public static readonly IPropertyData ChildrenProperty = RegisterProperty(nameof(Children), () => new List<TestModelWithNestedListMembers_Level2>());
    }

    public class TestModelWithNestedListMembers_Level2 : ModelBase
    {
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly IPropertyData NameProperty = RegisterProperty(nameof(Name), string.Empty);
    }
}
