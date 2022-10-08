namespace Catel.Tests.Data.Adapters
{
    using Catel.Data;
    using NUnit.Framework;

    public class ReflectionObjectAdapterFacts
    {
        public class TestClassWithRegularMembers
        {
            public string StringField;

            public string StringProperty { get; set; }
        }

        [TestFixture]
        public class TheTrySetValueMethod
        {
            [TestCase]
            public void SetsCatelModelPropertyValue()
            {
                var adapter = new ReflectionObjectAdapter();
                var model = new PersonTestModel();

                Assert.IsTrue(adapter.TrySetMemberValue(model, nameof(PersonTestModel.FirstName), "John"));
                Assert.AreEqual("John", model.FirstName);
            }

            [TestCase]
            public void SetsRegularPropertyValue()
            {
                var adapter = new ReflectionObjectAdapter();
                var model = new TestClassWithRegularMembers();

                Assert.IsTrue(adapter.TrySetMemberValue(model, nameof(TestClassWithRegularMembers.StringProperty), "John"));
                Assert.AreEqual("John", model.StringProperty);
            }

            [TestCase]
            public void SetsFieldValue()
            {
                var adapter = new ReflectionObjectAdapter();
                var model = new TestClassWithRegularMembers();

                Assert.IsTrue(adapter.TrySetMemberValue(model, nameof(TestClassWithRegularMembers.StringField), "John"));
                Assert.AreEqual("John", model.StringField);
            }

            [TestCase]
            public void ReturnsFalseForNonExistingMember()
            {
                var adapter = new ReflectionObjectAdapter();
                var model = new TestClassWithRegularMembers();

                Assert.IsFalse(adapter.TrySetMemberValue(model, "NotExistingMember", "John"));
            }
        }

        [TestFixture]
        public class TheTryGetValueMethod
        {
            [TestCase]
            public void GetsCatelModelPropertyValue()
            {
                var adapter = new ReflectionObjectAdapter();
                var model = new PersonTestModel
                {
                    FirstName = "John"
                };

                string value = string.Empty;

                Assert.IsTrue(adapter.TryGetMemberValue(model, nameof(PersonTestModel.FirstName), out value));
                Assert.AreEqual("John", model.FirstName);
            }

            [TestCase]
            public void GetsRegularPropertyValue()
            {
                var adapter = new ReflectionObjectAdapter();
                var model = new TestClassWithRegularMembers
                {
                    StringProperty = "John"
                };

                string value = string.Empty;

                Assert.IsTrue(adapter.TryGetMemberValue(model, nameof(TestClassWithRegularMembers.StringProperty), out value));
                Assert.AreEqual("John", value);
            }

            [TestCase]
            public void GetsFieldValue()
            {
                var adapter = new ReflectionObjectAdapter();
                var model = new TestClassWithRegularMembers
                {
                    StringField = "John"
                };

                string value = string.Empty;

                Assert.IsTrue(adapter.TryGetMemberValue(model, nameof(TestClassWithRegularMembers.StringField), out value));
                Assert.AreEqual("John", value);
            }

            [TestCase]
            public void ReturnsFalseForNonExistingMember()
            {
                var adapter = new ReflectionObjectAdapter();
                var model = new TestClassWithRegularMembers();
                string value = string.Empty;

                Assert.IsFalse(adapter.TryGetMemberValue(model, "NotExistingMember", out value));
            }
        }
    }
}
